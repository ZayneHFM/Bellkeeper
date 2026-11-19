using System.Threading.Tasks;
using Bellkeeper.BellkeeperCode.Mechanics;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;

namespace Bellkeeper.BellkeeperCode.Powers;

/// <summary>
/// 【聚气 X】：正面余响。在你的回合开始或【叩钟】时，每层获得 1 点能量，然后移除。
/// 挂在 AfterEnergyReset：能量已经回满再给，给的能量不会被回气覆盖。
/// </summary>
public class GatherQiPower : ChimePower
{
    public override ChimeAxis Axis => ChimeAxis.GatherQi;

    public override Task AfterEnergyReset(Player player)
    {
        if (player.Creature != Owner) return Task.CompletedTask;
        return Toll.TriggerNatural(new BlockingPlayerChoiceContext(), this);
    }

    public override async Task ExecuteEffect(
        PlayerChoiceContext choiceContext,
        Creature owner,
        int effectiveLayers,
        CardModel? cardSource = null)
    {
        if (effectiveLayers <= 0 || owner.IsDead || owner.Player is null) return;
        await PlayerCmd.GainEnergy(effectiveLayers, owner.Player);
    }
}
