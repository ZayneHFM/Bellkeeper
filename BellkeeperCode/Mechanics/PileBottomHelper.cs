using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BaseLib.Utils;
using Bellkeeper.BellkeeperCode.Cards;
using MegaCrit.Sts2.Core.CardSelection;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Models;

namespace Bellkeeper.BellkeeperCode.Mechanics;

/// <summary>
/// 【沉底 (n)】与【探底 (n)】的底层封装。
///
/// ─── 三条硬事实（全部从本机 dll 反汇编确认，不是猜） ───
///
/// 1) LocString.KeyPathToLocString 的实现是 String.Split('/')：
///      ldc.i4.s 47 / callvirt String::Split / ldelem.ref[0] / ldelem.ref[1]
///      newobj LocString..ctor(locTable, locEntryKey)
///    所以键路径必须写 "表名/键名"。写 "BELLKEEPER-XXX" 会 Split 出 1 段，
///    取 [1] 直接 IndexOutOfRange，被 catch 掉之后就变成「静默不显示」。
///
/// 2) DynamicVar.ToString() 走 get_IntValue() → get_BaseValue()，
///    所以卡面裸 {Damage} 永远显示基础值，不吃力量/敏捷。
///    要吃修正必须写成 {Damage:diff()}（HighlightDifferencesFormatter）。
///
/// 3) 跨牌堆移动必须走 CardPileCmd.Add(...)，不能直接 card.RemoveFromCurrentPile()
///    + pile.AddInternal(...)。后者只改 C# 列表，不通知 NPlayerHand 释放 NCard 节点，
///    紧接着抽牌就会死锁。「沉底卡死」就是这个原因。
/// </summary>
public static class PileBottomHelper
{
    // ---------------------------------------------------------------
    // 本地化助手
    // ---------------------------------------------------------------

    /// <summary>
    /// 取选牌提示。cards.json 里没配就回落到预设提示，绝不抛异常。
    /// </summary>
    public static LocString GetPrompt(string key, LocString fallback)
    {
        try
        {
            var loc = LocString.KeyPathToLocString($"cards/{key}");
            if (loc.Exists()) return loc;
        }
        catch (Exception e)
        {
            MainFile.Logger.Error($"选牌提示 {key} 取用失败: {e.Message}");
        }
        return fallback;
    }

    private static LocString Prompt(string key, LocString fallback)
        => GetPrompt(key, fallback);

    /// <summary>
    /// 过滤掉正在打出的这张牌自身（此时它可能还在手牌列表里）。
    /// </summary>
    private static Func<CardModel, bool> NotSelf(CardModel source)
        => c => !ReferenceEquals(c, source);

    // ---------------------------------------------------------------
    // 【沉底】
    // ---------------------------------------------------------------

    /// <summary>
    /// 兼容旧接口：沉底 1 张。
    /// </summary>
    public static async Task<CardModel?> SelectAndSinkOneFromHand(
        PlayerChoiceContext ctx, CardModel source, Creature owner)
    {
        var sunk = await SelectAndSinkFromHand(ctx, source, owner, 1, 1);
        return sunk.Count > 0 ? sunk[0] : null;
    }

    /// <summary>
    /// 【沉底 n】：从手牌自选 minCount~maxCount 张，按玩家点选顺序压入抽牌堆底。
    ///
    /// 「定序」怎么来的：CommonActions.SelectCards 返回的 IEnumerable 顺序
    /// 就是玩家点击的先后顺序。我们按这个顺序逐张 Add 到堆底，
    /// 先点的先入底、后点的压在更下面 —— 抽牌时后点的先被抽到。
    /// 玩家因此完全掌控 n 张牌的出场次序。
    /// </summary>
    public static async Task<List<CardModel>> SelectAndSinkFromHand(
        PlayerChoiceContext ctx,
        CardModel source,
        Creature owner,
        int minCount,
        int maxCount)
    {
        var result = new List<CardModel>();

        var player = owner.Player;
        if (player is null) return result;

        var hand = player.PlayerCombatState?.Hand;
        if (hand is null) return result;

        var filter = NotSelf(source);
        int available = hand.Cards.Count(filter);
        if (available == 0 || maxCount <= 0) return result;

        int actualMax = Math.Min(maxCount, available);
        int actualMin = Math.Min(Math.Max(minCount, 1), actualMax);

        // 这个提示要告诉玩家「按顺序选」
        var prompt = Prompt("BELLKEEPER-SELECT_TO_SINK", CardSelectorPrefs.DiscardSelectionPrompt);

        try
        {
            if (actualMin == 1 && actualMax == 1)
            {
                var single = await CommonActions.SelectSingleCard(
                    source, prompt, ctx, PileType.Hand, filter);
                if (single is not null)
                {
                    await SinkCard(ctx, single, owner);
                    result.Add(single);
                }
            }
            else
            {
                var many = await CommonActions.SelectCards(
                    source, prompt, ctx, PileType.Hand, filter, actualMin, actualMax);
                if (many is not null)
                {
                    foreach (var card in many.ToList())
                    {
                        if (card is null) continue;
                        await SinkCard(ctx, card, owner);
                        result.Add(card);
                    }
                }
            }
        }
        catch (OperationCanceledException)
        {
            return result;
        }
        catch (Exception e)
        {
            MainFile.Logger.Error("沉底选牌失败: " + e);
        }

        return result;
    }

    /// <summary>
    /// 把一张牌放到抽牌堆底（走官方异步命令，释放 UI 节点），再广播监听。
    /// </summary>
    public static async Task SinkCard(PlayerChoiceContext ctx, CardModel card, Creature owner)
    {
        var player = owner.Player;
        if (player is null) return;

        // 官方跨牌堆移动。不要换回 RemoveFromCurrentPile + AddInternal。
        await CardPileCmd.Add(card, PileType.Draw, CardPilePosition.Bottom, null, false);

        // 兜底：确认真的落在末尾（有些路径会把它插到中间）
        var drawPile = player.PlayerCombatState?.DrawPile;
        if (drawPile is not null && drawPile.Cards.Count > 0 && drawPile.Cards[^1] != card)
        {
            drawPile.MoveToBottomInternal(card);
            drawPile.InvokeContentsChanged();
        }

        // 《沉蓄》：被沉底时本场战斗永久增伤
        if (card is Cards.SunkenMomentum sm)
        {
            sm.OnSunk();
        }

        await NotifyCardSunk(ctx, owner, card);
    }

    // ---------------------------------------------------------------
    // 【探底】
    // ---------------------------------------------------------------

    /// <summary>
    /// 【探底 n】：使抽牌堆底的 n 张牌在它下一次被打出前费用减 1。
    /// </summary>
    public static Task<List<CardModel>> DredgeBottom(
        PlayerChoiceContext ctx,
        Creature owner,
        int count)
    {
        var affected = new List<CardModel>();
        if (count <= 0) return Task.FromResult(affected);

        var player = owner.Player;
        if (player is null) return Task.FromResult(affected);

        var drawPile = player.PlayerCombatState?.DrawPile;
        if (drawPile is null || drawPile.Cards.Count == 0) return Task.FromResult(affected);

        int take = Math.Min(count, drawPile.Cards.Count);
        for (int i = 0; i < take; i++)
        {
            var bottomCard = drawPile.Cards[drawPile.Cards.Count - 1 - i];
            ReduceCostUntilPlayed(bottomCard, 1);
            if (bottomCard is SunkenMomentum sm)
            {
                sm.OnSunk();
            }
            affected.Add(bottomCard);
        }

        return Task.FromResult(affected);
    }

    /// <summary>
    /// 旧版【探底 n】接口保留：看抽牌堆底 n 张并置顶 1 张。
    /// </summary>
    public static async Task<CardModel?> ScryBottomToTop(
        PlayerChoiceContext ctx,
        CardModel source,
        Creature owner,
        int lookCount = 3)
    {
        var player = owner.Player;
        if (player is null) return null;

        var drawPile = player.PlayerCombatState?.DrawPile;
        if (drawPile is null || drawPile.Cards.Count == 0 || lookCount <= 0) return null;

        int take = Math.Min(lookCount, drawPile.Cards.Count);
        var bottom = drawPile.Cards.Skip(drawPile.Cards.Count - take).ToHashSet();

        var prompt = Prompt("BELLKEEPER-SELECT_TO_DREDGE", CardSelectorPrefs.TransformSelectionPrompt);

        CardModel? chosen;
        try
        {
            chosen = await CommonActions.SelectSingleCard(
                source, prompt, ctx, PileType.Draw, c => bottom.Contains(c));
        }
        catch (Exception e)
        {
            MainFile.Logger.Error("探底选牌失败: " + e);
            return null;
        }

        if (chosen is null) return null;

        drawPile.MoveToTopInternal(chosen);
        drawPile.InvokeContentsChanged();

        return chosen;
    }

    // ---------------------------------------------------------------
    // 从堆底抽牌
    // ---------------------------------------------------------------

    /// <summary>
    /// 从抽牌堆底抽 count 张。
    ///
    /// 索引方向已确认：Cards[0] 是堆顶，Cards[^1] 是堆底。
    /// 把堆底那张搬到堆顶再走标准 Draw(1)，这样手牌上限、动画、洗牌全都复用原版。
    /// </summary>
    public static async Task<List<CardModel>> DrawFromBottom(
        PlayerChoiceContext ctx, Creature owner, int count)
    {
        var drawn = new List<CardModel>();
        if (count <= 0) return drawn;

        var player = owner.Player;
        if (player is null) return drawn;

        IEnumerable<CardModel> result;
        bool prev = BlindSeerDrawPatch.ForceBottomDraw.Value;
        BlindSeerDrawPatch.ForceBottomDraw.Value = true;
        try
        {
            result = await CardPileCmd.Draw(ctx, count, player, false);
        }
        finally
        {
            BlindSeerDrawPatch.ForceBottomDraw.Value = prev;
        }

        foreach (var card in result)
        {
            BlindSeerDrawPatch.ConsumePendingBottomDraw(card);
            if (card is SunkenMomentum sm)
            {
                sm.OnSunk();
            }
            drawn.Add(card);
            await NotifyCardDrawnFromBottom(ctx, owner, card);
        }

        return drawn;
    }

    /// <summary>
    /// 把抽牌堆底的 1 张牌移到堆顶（不抽）。供《翻找》使用。
    /// </summary>
    public static Task<CardModel?> MoveBottomToTop(PlayerChoiceContext ctx, Creature owner)
    {
        var drawPile = owner.Player?.PlayerCombatState?.DrawPile;
        if (drawPile is null || drawPile.Cards.Count == 0) return Task.FromResult<CardModel?>(null);

        var bottomCard = drawPile.Cards[^1];
        drawPile.MoveToTopInternal(bottomCard);
        drawPile.InvokeContentsChanged();

        return Task.FromResult<CardModel?>(bottomCard);
    }

    /// <summary>
    /// 把抽牌堆顺序整个颠倒。供《翻箱倒柜》使用。
    /// </summary>
    public static Task ReverseDrawPile(PlayerChoiceContext ctx, Creature owner)
    {
        var drawPile = owner.Player?.PlayerCombatState?.DrawPile;
        if (drawPile is null || drawPile.Cards.Count < 2) return Task.CompletedTask;

        var snapshot = drawPile.Cards.Reverse().ToList();
        foreach (var c in drawPile.Cards.ToList())
        {
            drawPile.RemoveInternal(c, true);
        }
        foreach (var c in snapshot)
        {
            drawPile.AddInternal(c, drawPile.Cards.Count, true);
        }
        drawPile.InvokeContentsChanged();

        return Task.CompletedTask;
    }

    // ---------------------------------------------------------------
    // 费用
    // ---------------------------------------------------------------

    /// <summary>本回合降低单张卡的费用。</summary>
    public static void ReduceCostForTurn(CardModel card, int reduction)
    {
        if (reduction <= 0) return;
        card.EnergyCost.AddThisTurn(-reduction, true);
    }

    public static void ReduceCostUntilPlayed(CardModel card, int reduction)
    {
        if (reduction <= 0) return;
        card.EnergyCost.AddUntilPlayed(-reduction, true);
    }

    /// <summary>本回合把这张卡设为免费。</summary>
    public static void SetCostZeroForTurn(CardModel card)
    {
        card.SetToFreeThisTurn();
    }

    /// <summary>取这张卡的基础费用。</summary>
    public static int GetCardEnergyCost(CardModel card)
    {
        return card.EnergyCost.Canonical;
    }

    // ---------------------------------------------------------------
    // 监听广播
    // ---------------------------------------------------------------

    private static async Task NotifyCardSunk(
        PlayerChoiceContext ctx, Creature owner, CardModel card)
    {
        foreach (var p in owner.Powers.OfType<ICardSunkListener>().ToList())
        {
            await p.OnCardSunk(ctx, owner, card);
        }

        if (owner.Player is not null)
        {
            foreach (var cane in owner.Player.Relics.OfType<Relics.WornBlindCane>().ToList())
            {
                await cane.OnFirstSinkInCombat(ctx, owner.Player);
            }
        }
    }

    public static async Task NotifyCardDrawnFromBottom(
        PlayerChoiceContext ctx, Creature owner, CardModel card)
    {
        foreach (var p in owner.Powers.OfType<IDrawnFromBottomListener>().ToList())
        {
            await p.OnCardDrawnFromBottom(ctx, owner, card);
        }
    }
}

/// <summary>【沉底】发生时收到通知的能力牌实现这个接口。</summary>
public interface ICardSunkListener
{
    Task OnCardSunk(PlayerChoiceContext ctx, Creature owner, CardModel card);
}

/// <summary>从抽牌堆底抽牌时收到通知的能力牌实现这个接口。</summary>
public interface IDrawnFromBottomListener
{
    Task OnCardDrawnFromBottom(PlayerChoiceContext ctx, Creature owner, CardModel card);
}
