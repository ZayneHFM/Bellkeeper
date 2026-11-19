using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Bellkeeper.BellkeeperCode.Cards;
using Bellkeeper.BellkeeperCode.Powers;
using Bellkeeper.BellkeeperCode.Powers.CardPowers;
using Bellkeeper.BellkeeperCode.Relics;
using Godot;
using MegaCrit.Sts2.Core.CardSelection;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Helpers;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Nodes.Rooms;
using MegaCrit.Sts2.Core.Nodes.Vfx;

namespace Bellkeeper.BellkeeperCode.Mechanics;

/// <summary>
/// 【叩钟】与回合开始自然触发的统一结算中心。
///
/// 关键纪律：
///   1. 快照隔离：先固定当前 ChimePower 列表再结算，结算中新增的余响不进入本次清算。
///   2. 10 条余响/失声统一走 ChimePower.ExecuteEffect，避免两套触发逻辑分叉。
///   3. 【延音】、绕梁三日、先知先觉、回音长廊、回声室、寂静撞针、追槌全部在此收口。
/// </summary>
public static class Toll
{
    public const string SfxToll = "res://Bellkeeper/audio/bell_toll.ogg";
    public const string SfxNatural = "res://Bellkeeper/audio/bell_natural.ogg";

    /// <summary>本次触发读取的层数入口。</summary>
    public static int TriggeredAmount(ChimePower power) => power.Amount;

    /// <summary>
    /// 单个余响/失声在回合开始时的自然触发：执行效果（含回声室额外结算），再处理延音/移除。
    /// </summary>
    public static async Task TriggerNatural(PlayerChoiceContext choiceContext, ChimePower chime)
    {
        var owner = chime.Owner;
        if (owner is null || owner.IsDead || chime.Amount <= 0) return;

        int layers = TriggeredAmount(chime);
        await chime.ExecuteEffect(choiceContext, owner, layers, null);

        if (chime.IsPositive && !owner.IsDead)
        {
            var echoChamber = owner.Powers.OfType<EchoChamberPower>().FirstOrDefault();
            if (echoChamber is not null && echoChamber.TriggersRemainingThisTurn > 0)
            {
                echoChamber.TriggersRemainingThisTurn--;
                await chime.ExecuteEffect(choiceContext, owner, layers, null);
            }
        }

        await ResolveNaturalRetentionAsync(choiceContext, owner, chime);
    }

    /// <summary>兼容旧接口：同步结算自然触发后的保留/移除。</summary>
    public static void AfterNaturalTrigger(ChimePower power)
    {
        var owner = power.Owner;
        if (owner is null) return;

        if (!power.IsPositive)
        {
            power.RemoveInternal();
            return;
        }

        var sustain = owner.GetPower<SustainPower>();
        if (sustain is not null && sustain.Amount > 0)
        {
            _ = PowerCmd.Decrement(sustain);

            bool keepFull = owner.Powers.OfType<ThreeDaysEchoPower>().Any(p => p.Amount > 0);
            int retained = keepFull ? power.Amount : power.Amount / 2;
            power.SetAmount(retained, true);
            if (power.Amount <= 0) power.RemoveInternal();
            return;
        }

        power.RemoveInternal();
    }

    private static async Task ResolveNaturalRetentionAsync(
        PlayerChoiceContext choiceContext,
        Creature owner,
        ChimePower power)
    {
        if (!power.IsPositive)
        {
            power.RemoveInternal();
            return;
        }

        var sustain = owner.GetPower<SustainPower>();
        if (sustain is not null && sustain.Amount > 0)
        {
            await PowerCmd.Decrement(sustain);

            bool keepFull = owner.Powers.OfType<ThreeDaysEchoPower>().Any(p => p.Amount > 0);
            int retained = keepFull ? power.Amount : power.Amount / 2;
            power.SetAmount(retained, true);
            if (power.Amount <= 0) power.RemoveInternal();
            return;
        }

        power.RemoveInternal();
    }

    /// <summary>
    /// 【叩钟】：清算 owner 身上所有余响/失声。
    /// 返回本次移除的正/负余响种类数。
    /// </summary>
    public static async Task<TollResult> Ring(
        PlayerChoiceContext choiceContext,
        Creature owner,
        CardModel? cardSource = null,
        bool keepPositiveChimes = false)
    {
        if (owner is null || owner.IsDead || owner.CombatState is null)
            return new TollResult(0, 0);

        // 1. 快照隔离：结算中新增的余响不进本次清算
        var snapshot = owner.GetPowerInstances<ChimePower>().Where(p => p.Amount > 0).ToList();
        if (snapshot.Count == 0)
        {
            await ChasingMallet.OnTollRing(choiceContext, owner);
            return new TollResult(0, 0);
        }

        PlayTollVisualsAndAudio(owner, SfxToll);

        // 2. R05《回音长廊》：每回合第一次【叩钟】时，正面【余响】额外触发 X% 层数
        int galleryBonusPct = 0;
        var gallery = owner.Powers.OfType<CarillonGalleryPower>().FirstOrDefault();
        if (gallery is not null && !gallery.TriggeredThisTurn && gallery.Amount > 0)
        {
            gallery.TriggeredThisTurn = true;
            galleryBonusPct = gallery.Amount;
        }

        // 3. R08《回声室》：每回合前 X 次触发正面【余响】时，额外结算 1 次
        var echoChamber = owner.Powers.OfType<EchoChamberPower>().FirstOrDefault();

        int positive = 0, negative = 0;
        foreach (var chime in snapshot)
        {
            if (chime.Amount <= 0 || owner.IsDead) continue;

            if (chime.IsPositive) positive++; else negative++;

            int effectiveLayers = chime.Amount;
            if (chime.IsPositive && galleryBonusPct > 0)
            {
                effectiveLayers += (chime.Amount * galleryBonusPct) / 100;
            }

            await chime.ExecuteEffect(choiceContext, owner, effectiveLayers, cardSource);

            if (chime.IsPositive && echoChamber is not null && echoChamber.TriggersRemainingThisTurn > 0)
            {
                echoChamber.TriggersRemainingThisTurn--;
                await chime.ExecuteEffect(choiceContext, owner, effectiveLayers, cardSource);
            }
        }

        // 4. 【延音】与移除结算（一次叩钟有正面余响且非不移除时消耗 1 层延音）
        var sustain = owner.GetPower<SustainPower>();
        bool hasSustain = positive > 0 && !keepPositiveChimes && sustain is not null && sustain.Amount > 0;
        if (hasSustain)
        {
            await PowerCmd.Decrement(sustain!);
        }

        bool keepFull = hasSustain && owner.Powers.OfType<ThreeDaysEchoPower>().Any(p => p.Amount > 0);
        int removedDistinctTypes = 0;
        foreach (var chime in snapshot)
        {
            if (chime.IsPositive && keepPositiveChimes)
            {
                continue;
            }

            if (chime.IsPositive && hasSustain)
            {
                int retained = keepFull ? chime.Amount : chime.Amount / 2;
                if (retained > 0)
                {
                    chime.SetAmount(retained, true);
                }
                else
                {
                    chime.RemoveInternal();
                    removedDistinctTypes++;
                }
            }
            else
            {
                chime.RemoveInternal();
                removedDistinctTypes++;
            }
        }

        // 5. U06《追槌》：每当你【叩钟】，将弃牌堆中的《追槌》放回手牌
        await ChasingMallet.OnTollRing(choiceContext, owner);

        // 6. 遗物【寂静撞针】：每当你【叩钟】时，每移除 1 种【余响】，获得 3 点格挡
        if (owner.Player is not null && removedDistinctTypes > 0)
        {
            foreach (var striker in owner.Player.Relics.OfType<SilentStriker>().ToList())
            {
                striker.Flash();
                await striker.OnTollRemovedTypes(owner, removedDistinctTypes);
            }
        }

        return new TollResult(positive, negative);
    }

    /// <summary>
    /// 自选 X 张手牌置于抽牌堆顶。【乱拍】用。
    /// 走 CardSelectCmd.FromHand + CardPileCmd.Add(..., PileType.Draw, CardPilePosition.Top, ...) 保证自选且 UI 节点正常释放。
    /// </summary>
    public static async Task PutHandCardsOnTop(
        PlayerChoiceContext choiceContext, Creature owner, int count, AbstractModel? source = null)
    {
        if (count <= 0) return;
        var player = owner.Player;
        if (player is null) return;

        var hand = player.PlayerCombatState?.Hand;
        if (hand is null || hand.Cards.Count == 0) return;

        int actualCount = Math.Min(count, hand.Cards.Count);
        if (hand.Cards.Count == 1)
        {
            await CardPileCmd.Add(hand.Cards[0], PileType.Draw, CardPilePosition.Top, null, false);
            return;
        }

        List<CardModel> targets;
        try
        {
            var prompt = PileBottomHelper.GetPrompt("BELLKEEPER-SELECT_FOR_OFFBEAT", CardSelectorPrefs.DiscardSelectionPrompt);
            var prefs = new CardSelectorPrefs(prompt, actualCount, actualCount);
            var chosen = await CardSelectCmd.FromHand(choiceContext, player, prefs, null, source!);
            targets = chosen?.ToList() ?? [];
        }
        catch (OperationCanceledException)
        {
            return;
        }
        catch (Exception e)
        {
            MainFile.Logger.Error("乱拍选牌置顶失败，回退为末尾手牌: " + e.Message);
            targets = hand.Cards.Skip(hand.Cards.Count - actualCount).ToList();
        }

        foreach (var card in targets.AsEnumerable().Reverse())
        {
            await CardPileCmd.Add(card, PileType.Draw, CardPilePosition.Top, null, false);
        }
    }

    /// <summary>自选弃置 X 张手牌。走 CardSelectCmd.FromHandForDiscard + CardCmd.Discard。</summary>
    public static async Task DiscardFromHand(
        PlayerChoiceContext choiceContext, Creature owner, int count, AbstractModel? source = null)
    {
        if (count <= 0) return;
        var player = owner.Player;
        if (player is null) return;

        var hand = player.PlayerCombatState?.Hand;
        if (hand is null || hand.Cards.Count == 0) return;

        if (hand.Cards.Count <= count)
        {
            await CardCmd.Discard(choiceContext, hand.Cards.ToList());
            return;
        }

        try
        {
            var prefs = new CardSelectorPrefs(CardSelectorPrefs.DiscardSelectionPrompt, count);
            var chosen = await CardSelectCmd.FromHandForDiscard(choiceContext, player, prefs, null, source!);
            if (chosen is not null)
            {
                await CardCmd.Discard(choiceContext, chosen.ToList());
                return;
            }
        }
        catch (OperationCanceledException)
        {
            return;
        }
        catch (Exception e)
        {
            MainFile.Logger.Error("哑音选牌弃牌失败，回退为弃置末尾手牌: " + e.Message);
        }

        var fallback = hand.Cards.Skip(hand.Cards.Count - count).ToList();
        await CardCmd.Discard(choiceContext, fallback);
    }

    public static void PlayTollVisualsAndAudio(Creature owner, string sfxPath = SfxToll)
    {
        try
        {
            if (ResourceLoader.Exists(sfxPath) && NCombatRoom.Instance is not null)
            {
                var stream = ResourceLoader.Load<AudioStream>(sfxPath);
                if (stream is not null)
                {
                    var player = new AudioStreamPlayer { Stream = stream, VolumeDb = -2.0f };
                    NCombatRoom.Instance.AddChild(player);
                    player.Finished += () => player.QueueFree();
                    player.Play();
                }
            }

            var vfxContainer = NCombatRoom.Instance?.CombatVfxContainer;
            if (vfxContainer is not null)
            {
                var wave = NScratchVfx.Create(owner, true);
                if (wave is not null)
                    vfxContainer.AddChildSafely(wave);
            }
        }
        catch
        {
            // Ignore in headless/non-room tests
        }
    }
}

/// <summary>一次叩钟清算的结果。</summary>
public readonly record struct TollResult(int PositiveKinds, int NegativeKinds)
{
    public int TotalKinds => PositiveKinds + NegativeKinds;
}
