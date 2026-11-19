using System.Threading.Tasks;
using Bellkeeper.BellkeeperCode.Mechanics;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;

namespace Bellkeeper.BellkeeperCode.Powers;

/// <summary>
/// 【先行 X】：正面余响。在你的回合开始或【叩钟】时，每层抽 1 张牌，然后移除。
/// 挂在 AfterPlayerTurnStart：此时手牌已经抽完，额外抽牌不会和常规抽牌抢时序。
/// </summary>
public class VanguardPower : ChimePower
{
    public override ChimeAxis Axis => ChimeAxis.Vanguard;

    public override Task AfterPlayerTurnStart(PlayerChoiceContext choiceContext, Player player)
    {
        if (player.Creature != Owner) return Task.CompletedTask;
        return Toll.TriggerNatural(choiceContext, this);
    }

    public override async Task ExecuteEffect(
        PlayerChoiceContext choiceContext,
        Creature owner,
        int effectiveLayers,
        CardModel? cardSource = null)
    {
        if (effectiveLayers <= 0 || owner.IsDead || owner.Player is null) return;
        await CardPileCmd.Draw(choiceContext, effectiveLayers, owner.Player, false);
    }
}
