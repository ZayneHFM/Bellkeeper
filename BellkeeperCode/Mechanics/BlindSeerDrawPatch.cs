using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Threading;
using Bellkeeper.BellkeeperCode.Powers.CardPowers;
using HarmonyLib;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;

namespace Bellkeeper.BellkeeperCode.Mechanics;

/// <summary>
/// R22《盲眼先知》与【摸底】共用的底层抽牌重定向补丁：
/// 当玩家持有 BlindSeerPower（或 ForceBottomDraw 上下文开启）时，CardPileCmd.Draw 改为抽取 drawPile.Cards[^1]。
/// </summary>
[HarmonyPatch]
public static class BlindSeerDrawPatch
{
    public static readonly AsyncLocal<bool> ForceBottomDraw = new();

    private static readonly HashSet<CardModel> PendingBlindSeerBottomDraws = new();
    private static readonly object Sync = new();

    internal static MethodBase TargetMethod()
    {
        var method = AccessTools.Method(
                typeof(CardPileCmd),
                "DrawInternal",
                new[] { typeof(PlayerChoiceContext), typeof(decimal), typeof(Player), typeof(bool) })
            ?? AccessTools.Method(
                typeof(CardPileCmd),
                "Draw",
                new[] { typeof(PlayerChoiceContext), typeof(decimal), typeof(Player), typeof(bool) });
        return AccessTools.AsyncMoveNext(method)!;
    }

    internal static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
    {
        var stateMachineType = TargetMethod().DeclaringType
            ?? throw new InvalidOperationException("Cannot locate DrawInternal state machine type.");
        var playerField = AccessTools.Field(stateMachineType, "player")
            ?? throw new InvalidOperationException("Cannot locate DrawInternal.player field.");
        var selectNextMethod = AccessTools.Method(typeof(BlindSeerDrawPatch), nameof(SelectNextCardToDraw))
            ?? throw new InvalidOperationException("Cannot locate SelectNextCardToDraw.");

        bool patched = false;
        foreach (var ins in instructions)
        {
            if (!patched &&
                ins.opcode == OpCodes.Call &&
                ins.operand is MethodInfo mi &&
                mi.IsGenericMethod &&
                mi.DeclaringType == typeof(Enumerable) &&
                mi.Name == nameof(Enumerable.FirstOrDefault) &&
                mi.GetParameters().Length == 1)
            {
                patched = true;
                var loadSelf = new CodeInstruction(OpCodes.Ldarg_0);
                loadSelf.labels.AddRange(ins.labels);
                yield return loadSelf;
                yield return new CodeInstruction(OpCodes.Ldfld, playerField);
                yield return new CodeInstruction(OpCodes.Call, selectNextMethod);
                continue;
            }

            yield return ins;
        }

        if (!patched)
        {
            MainFile.Logger.Error("BlindSeerDrawPatch failed to find Enumerable.FirstOrDefault in DrawInternal.");
        }
    }

    public static CardModel? SelectNextCardToDraw(IReadOnlyList<CardModel> cards, Player? player)
    {
        if (cards.Count == 0) return null;

        if (ForceBottomDraw.Value)
        {
            return cards[^1];
        }

        if (HasBlindSeer(player))
        {
            var bottom = cards[^1];
            lock (Sync)
            {
                PendingBlindSeerBottomDraws.Add(bottom);
            }
            return bottom;
        }

        return cards[0];
    }

    public static bool ConsumePendingBottomDraw(CardModel card)
    {
        lock (Sync)
        {
            return PendingBlindSeerBottomDraws.Remove(card);
        }
    }

    private static bool HasBlindSeer(Player? player)
    {
        var creature = player?.Creature;
        if (creature is null) return false;
        return creature.Powers.Any(p => p is BlindSeerPower or BlindSeerUpgradedPower);
    }
}
