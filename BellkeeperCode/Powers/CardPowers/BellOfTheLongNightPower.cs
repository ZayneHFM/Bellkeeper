using System.Collections.Generic;
using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;

namespace Bellkeeper.BellkeeperCode.Powers.CardPowers;

/// <summary>
/// 长夜古钟状态（A01）：在你的回合结束时，使你的手牌费用在本场战斗中降低 Amount。
/// </summary>
public class BellOfTheLongNightPower : BellkeeperPower
{
    public override PowerType Type => PowerType.Buff;

    public override PowerStackType StackType => PowerStackType.Counter;

    public override Task BeforeSideTurnEnd(
        PlayerChoiceContext choiceContext,
        CombatSide side,
        IEnumerable<Creature> participants)
    {
        if (Owner is null || side != Owner.Side || Amount <= 0)
            return Task.CompletedTask;

        var hand = Owner.Player?.PlayerCombatState?.Hand;
        if (hand is null)
            return Task.CompletedTask;

        foreach (var card in hand.Cards)
        {
            if (card.EnergyCost.CostsX || card.EnergyCost.Canonical < 0)
                continue;
            if (card.EnergyCost.GetResolved() <= 0)
                continue;

            card.EnergyCost.AddThisCombat(-Amount, true);
        }

        return Task.CompletedTask;
    }
}
