using System;
using System.Linq;
using System.Threading.Tasks;
using Bellkeeper.BellkeeperCode.Mechanics;
using Bellkeeper.BellkeeperCode.Relics;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;

namespace Bellkeeper.BellkeeperCode.Powers;

/// <summary>
/// 【负重 X】：在你的下个回合开始时，失去 X 点能量。
/// 挂在 AfterEnergyReset：能量已回满再去扣，扣不穿 0；空蓝叩钟时扣不动 = 赖账。
/// </summary>
public class BurdenPower : ChimePower
{
    public override ChimeAxis Axis => ChimeAxis.Burden;

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

        int loseAmount = effectiveLayers;
        var pad = owner.Player.Relics.OfType<FrayedShoulderPad>().FirstOrDefault();
        if (pad is not null && !pad.UsedThisCombat)
        {
            pad.UsedThisCombat = true;
            pad.Flash();
            loseAmount = Math.Max(0, loseAmount - 1);
            await CardPileCmd.Draw(choiceContext, 1, owner.Player, false);
        }

        int currentEnergy = owner.Player.PlayerCombatState?.Energy ?? 0;
        int actualLose = Math.Min(currentEnergy, loseAmount);
        if (actualLose > 0)
        {
            await PlayerCmd.LoseEnergy(actualLose, owner.Player);
        }
    }
}
