using System.Linq;
using System.Threading.Tasks;
using Bellkeeper.BellkeeperCode.Mechanics;
using Bellkeeper.BellkeeperCode.Powers.CardPowers;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;

namespace Bellkeeper.BellkeeperCode.Powers;

/// <summary>
/// 【守御 X】：在你的下个回合开始时或【叩钟】时，获得 X 点格挡。
/// 若处于《时间停滞》效果下，则改为在回合结束时触发。
/// </summary>
public class GuardPower : ChimePower
{
    public override ChimeAxis Axis => ChimeAxis.Guard;

    public override Task AfterPlayerTurnStart(PlayerChoiceContext choiceContext, Player player)
    {
        if (player.Creature != Owner) return Task.CompletedTask;
        if (Owner.Powers.OfType<TimeStandstillPower>().Any(p => p.Amount > 0))
            return Task.CompletedTask;
        return Toll.TriggerNatural(choiceContext, this);
    }

    public override async Task ExecuteEffect(
        PlayerChoiceContext choiceContext,
        Creature owner,
        int effectiveLayers,
        CardModel? cardSource = null)
    {
        if (effectiveLayers <= 0 || owner.IsDead) return;
        await CreatureCmd.GainBlock(owner, effectiveLayers, BlockProps.nonCardUnpowered, null, false);
    }
}
