using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Bellkeeper.BellkeeperCode.Cards;
using HarmonyLib;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Relics;
using MegaCrit.Sts2.Core.Nodes.CommonUi;

namespace Bellkeeper.BellkeeperCode.Mechanics;

/// <summary>
/// 先古遗物《古老牙齿》(ArchaicTooth) 拾取与文案补丁：
/// 1) 负钟人同时持有《震击》与《守夜》时，拾取《古老牙齿》将两张牌二合一合成为《子夜》，
///    只要任一源卡升级过即直接得到升级版《子夜+》，并继承任意一张源卡上的兼容附魔；
/// 2) 修正 Orobas 预览与《古老牙齿》悬浮提示，对负钟人显示“合成为【子夜】”并在预览阶段正确显示《子夜+》与《守夜》。
/// </summary>
[HarmonyPatch(typeof(ArchaicTooth), nameof(ArchaicTooth.AfterObtained))]
public static class ArchaicToothFusionPatch
{
    private static readonly MethodInfo? GetStarterCardMethod =
        AccessTools.Method(typeof(ArchaicTooth), "GetTranscendenceStarterCard");

    private static readonly ConditionalWeakTable<ArchaicTooth, FusionPreviewState> PreviewStates = new();

    private sealed class FusionPreviewState
    {
        public bool HasBoth { get; init; }
        public CardModel? VigilCard { get; init; }
    }

    public sealed class FusionState
    {
        public required Player Owner { get; init; }
        public required CardModel Partner { get; init; }
        public required bool PartnerUpgraded { get; init; }
        public required EnchantmentModel? PartnerEnchantment { get; init; }
        public required HashSet<Midnight> PreExistingMidnights { get; init; }
    }

    internal static bool IsBellkeeperFusionTooth(ArchaicTooth tooth)
    {
        if (!tooth.IsMutable)
        {
            return false;
        }

        if (PreviewStates.TryGetValue(tooth, out var preview))
        {
            return preview.HasBoth;
        }

        if (tooth.Owner?.Character is Bellkeeper.BellkeeperCode.Character.Bellkeeper)
        {
            var ringingId = ModelDb.Card<RingingBlow>().Id;
            var vigilId = ModelDb.Card<VigilBell>().Id;
            var starterId = tooth.StarterCard?.Id;
            return starterId is null || starterId == ringingId || starterId == vigilId;
        }

        return false;
    }

    internal static bool TryGetVigilPreviewCard(ArchaicTooth tooth, out CardModel? vigil)
    {
        if (PreviewStates.TryGetValue(tooth, out var preview) && preview.HasBoth)
        {
            vigil = preview.VigilCard;
            return vigil is not null;
        }
        vigil = null;
        return false;
    }

    public static void Prefix(ArchaicTooth __instance, out FusionState? __state)
    {
        __state = null;
        if (!__instance.IsMutable) return;
        var owner = __instance.Owner;
        if (owner is null) return;

        var cards = owner.Deck.Cards;
        var primary = GetStarterCardMethod?.Invoke(__instance, [owner]) as CardModel
            ?? cards.FirstOrDefault(c => c is RingingBlow or VigilBell);

        CardModel? partner = primary switch
        {
            RingingBlow => cards.FirstOrDefault(c => c is VigilBell),
            VigilBell => cards.FirstOrDefault(c => c is RingingBlow),
            _ => null,
        };

        if (partner is null) return;

        __state = new FusionState
        {
            Owner = owner,
            Partner = partner,
            PartnerUpgraded = partner.IsUpgraded,
            PartnerEnchantment = partner.Enchantment,
            PreExistingMidnights = cards.OfType<Midnight>().ToHashSet(),
        };
    }

    public static void Postfix(ref Task __result, FusionState? __state)
    {
        if (__state is null) return;
        __result = CompleteFusionAsync(__result, __state);
    }

    private static async Task CompleteFusionAsync(Task originalTask, FusionState state)
    {
        await originalTask;

        var deckCards = state.Owner.Deck.Cards;
        var createdMidnight = deckCards.OfType<Midnight>()
            .FirstOrDefault(m => !state.PreExistingMidnights.Contains(m));

        if (createdMidnight is not null)
        {
            if (state.PartnerUpgraded && !createdMidnight.IsUpgraded)
            {
                CardCmd.Upgrade(createdMidnight, CardPreviewStyle.None);
            }

            if (createdMidnight.Enchantment is null && state.PartnerEnchantment is not null)
            {
                var cloned = (EnchantmentModel)state.PartnerEnchantment.MutableClone();
                if (cloned.CanEnchant(createdMidnight))
                {
                    CardCmd.Enchant(cloned, createdMidnight, state.PartnerEnchantment.Amount);
                }
            }
        }

        if (deckCards.Contains(state.Partner))
        {
            await CardPileCmd.RemoveFromDeck(state.Partner, showPreview: false);
        }
    }

    [HarmonyPatch(typeof(ArchaicTooth), nameof(ArchaicTooth.SetupForPlayer))]
    private static class SetupForPlayerPatch
    {
        public static void Postfix(ArchaicTooth __instance, Player player, ref bool __result)
        {
            if (!__result || player is null) return;

            var cards = player.Deck.Cards;
            var ringing = cards.FirstOrDefault(c => c is RingingBlow);
            var vigil = cards.FirstOrDefault(c => c is VigilBell);
            if (ringing is null && vigil is null) return;

            bool hasBoth = ringing is not null && vigil is not null;
            PreviewStates.AddOrUpdate(__instance, new FusionPreviewState
            {
                HasBoth = hasBoth,
                VigilCard = vigil,
            });

            if (!hasBoth || __instance.StarterCard is null) return;

            bool shouldUpgrade = ringing!.IsUpgraded || vigil!.IsUpgraded;
            bool ancientAlreadyUpgraded = (__instance.AncientCard?.CurrentUpgradeLevel ?? 0) > 0;
            var fallbackEnchantment = ringing.Enchantment ?? vigil.Enchantment;
            bool needEnchantmentPreview = ringing.Enchantment is null && vigil.Enchantment is not null;

            if ((shouldUpgrade && !ancientAlreadyUpgraded) || needEnchantmentPreview)
            {
                var previewMidnight = player.RunState.CreateCard<Midnight>(player);
                if (shouldUpgrade)
                {
                    CardCmd.Upgrade(previewMidnight, CardPreviewStyle.None);
                }
                if (fallbackEnchantment is not null)
                {
                    var cloned = (EnchantmentModel)fallbackEnchantment.MutableClone();
                    if (cloned.CanEnchant(previewMidnight))
                    {
                        CardCmd.Enchant(cloned, previewMidnight, fallbackEnchantment.Amount);
                    }
                }
                __instance.SetupForTests(__instance.StarterCard, previewMidnight.ToSerializable());
            }
        }
    }

    [HarmonyPatch(typeof(RelicModel), nameof(RelicModel.DynamicDescription), MethodType.Getter)]
    private static class DynamicDescriptionPatch
    {
        public static void Postfix(RelicModel __instance, ref LocString __result)
        {
            if (__instance is ArchaicTooth tooth && IsBellkeeperFusionTooth(tooth))
            {
                ReplaceFusionLoc(__result, "BELLKEEPER-ARCHAIC_TOOTH_FUSION.description", ref __result);
            }
        }
    }

    [HarmonyPatch(typeof(RelicModel), nameof(RelicModel.DynamicEventDescription), MethodType.Getter)]
    private static class DynamicEventDescriptionPatch
    {
        public static void Postfix(RelicModel __instance, ref LocString __result)
        {
            if (__instance is ArchaicTooth tooth && IsBellkeeperFusionTooth(tooth))
            {
                ReplaceFusionLoc(__result, "BELLKEEPER-ARCHAIC_TOOTH_FUSION.eventDescription", ref __result);
            }
        }
    }

    [HarmonyPatch(typeof(ArchaicTooth), "ExtraHoverTips", MethodType.Getter)]
    private static class ExtraHoverTipsPatch
    {
        public static void Postfix(ArchaicTooth __instance, ref IEnumerable<IHoverTip> __result)
        {
            if (!__instance.IsMutable) return;
            if (TryGetVigilPreviewCard(__instance, out var vigil)
                && vigil is not null
                && __instance.StarterCard?.Id != vigil.Id)
            {
                __result = __result.Append(HoverTipFactory.FromCard(vigil, vigil.IsUpgraded));
            }
        }
    }

    private static void ReplaceFusionLoc(LocString sourceVars, string key, ref LocString target)
    {
        var loc = LocString.KeyPathToLocString($"relics/{key}");
        if (loc.Exists())
        {
            loc.AddVariablesFrom(sourceVars);
            target = loc;
        }
    }
}
