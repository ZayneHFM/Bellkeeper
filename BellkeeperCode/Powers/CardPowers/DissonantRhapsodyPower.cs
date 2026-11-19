using System.Linq;
using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;

namespace Bellkeeper.BellkeeperCode.Powers.CardPowers;

/// <summary>
/// 走音狂想（U21）：每回合前 2 次获得【失声】时，抽 1 张牌并对所有敌人造成 Amount 点伤害。
/// </summary>
public class DissonantRhapsodyPower : BellkeeperPower
{
    public override PowerType Type => PowerType.Buff;

    public override PowerStackType StackType => PowerStackType.Counter;

    public override PowerInstanceType InstanceType => PowerInstanceType.Instanced;

    public int TriggersThisTurn { get; set; }

    public override Task AfterPlayerTurnStart(PlayerChoiceContext choiceContext, Player player)
    {
        if (Owner == player.Creature)
        {
            TriggersThisTurn = 0;
        }
        return Task.CompletedTask;
    }

    public override async Task AfterPowerAmountChanged(
        PlayerChoiceContext choiceContext,
        PowerModel power,
        decimal amount,
        Creature applier,
        CardModel? cardSource)
    {
        if (Owner is null || Amount <= 0 || amount <= 0) return;
        if (power.Owner != Owner) return;
        if (power is not ChimePower { IsPositive: false }) return;
        if (TriggersThisTurn >= 2) return;

        var player = Owner.Player;
        if (player is null) return;

        TriggersThisTurn++;
        await CardPileCmd.Draw(choiceContext, 1, player, false);

        var state = Owner.CombatState;
        if (state is null) return;
        foreach (var enemy in state.Enemies.ToList())
        {
            if (enemy.IsDead) continue;
            await CreatureCmd.Damage(
                choiceContext, enemy, Amount, DamageProps.nonCardUnpowered, Owner, null, null);
        }
    }
}
