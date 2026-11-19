using System.Collections.Generic;
using System.Reflection;
using System.Threading.Tasks;
using Bellkeeper.BellkeeperCode.Cards;
using HarmonyLib;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Context;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Rewards;
using MegaCrit.Sts2.Core.Runs;
using MegaCrit.Sts2.Core.Models.Relics;

namespace Bellkeeper.BellkeeperCode.Mechanics;

/// <summary>
/// 先古遗物《召唤铃铛》(CallingBell) 联动补丁：
/// 当持有/查看者为负钟人时，将拾取获得的《铃铛的诅咒》替换为专属诅咒《铃铛的认可》，
/// 并同步更新遗物界面的悬浮预览卡牌。
/// </summary>
[HarmonyPatch(typeof(CallingBell), nameof(CallingBell.AfterObtained))]
public static class CallingBellPatch
{
    private static readonly MethodInfo? GenerateRewardsMethod =
        AccessTools.Method(typeof(CallingBell), "GenerateRewards");

    private static bool IsBellkeeperContext(CallingBell relic)
    {
        if (!relic.IsMutable)
        {
            return false;
        }

        if (relic.Owner is not null)
        {
            return relic.Owner.Character is Bellkeeper.BellkeeperCode.Character.Bellkeeper;
        }

        if (RunManager.Instance?.IsInProgress == true)
        {
            var state = RunManager.Instance.DebugOnlyGetState();
            var localPlayer = state is null ? null : LocalContext.GetMe(state);
            return localPlayer?.Character is Bellkeeper.BellkeeperCode.Character.Bellkeeper;
        }

        return false;
    }

    [HarmonyPrefix]
    public static bool Prefix(CallingBell __instance, ref Task __result)
    {
        if (!__instance.IsMutable)
        {
            return true;
        }

        var owner = __instance.Owner;
        if (owner?.Character is not Bellkeeper.BellkeeperCode.Character.Bellkeeper)
        {
            return true;
        }

        __result = ObtainWithRecognitionAsync(__instance, owner);
        return false;
    }

    private static async Task ObtainWithRecognitionAsync(CallingBell relic, Player owner)
    {
        await CardPileCmd.AddCurseToDeck<BellsRecognition>(owner);
        await Cmd.Wait(0.75f);

        if (GenerateRewardsMethod?.Invoke(relic, null) is List<Reward> rewards)
        {
            await RewardsCmd.OfferCustom(owner, rewards);
        }
    }

    [HarmonyPatch(typeof(CallingBell), "ExtraHoverTips", MethodType.Getter)]
    private static class ExtraHoverTipsPatch
    {
        public static void Postfix(CallingBell __instance, ref IEnumerable<IHoverTip> __result)
        {
            if (!IsBellkeeperContext(__instance)) return;
            __result = HoverTipFactory.FromCardWithCardHoverTips<BellsRecognition>();
        }
    }
}
