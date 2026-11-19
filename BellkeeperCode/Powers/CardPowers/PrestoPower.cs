using System;
using System.Threading.Tasks;
using Bellkeeper.BellkeeperCode.Cards;
using Bellkeeper.BellkeeperCode.Mechanics;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;

namespace Bellkeeper.BellkeeperCode.Powers.CardPowers;

/// <summary>
/// 急板状态（R19）：本回合你的牌费用减 Amount。每打出 1 张牌，【负重 Amount】。
/// </summary>
public class PrestoPower : BellkeeperPower
{
    private CardModel? _justAppliedSource;

    public override PowerType Type => PowerType.Buff;

    public override PowerStackType StackType => PowerStackType.Counter;

    public override Task AfterPowerAmountChanged(
        PlayerChoiceContext choiceContext,
        PowerModel power,
        decimal amount,
        Creature applier,
        CardModel? cardSource)
    {
        if (power == this && amount > 0 && cardSource is not null)
        {
            _justAppliedSource = cardSource;
        }
        return Task.CompletedTask;
    }

    public override bool TryModifyEnergyCostInCombat(CardModel card, decimal originalCost, out decimal modifiedCost)
    {
        if (Owner is not null &&
            Amount > 0 &&
            card.Owner.Creature == Owner &&
            originalCost > 0m)
        {
            modifiedCost = Math.Max(0m, originalCost - Amount);
            return true;
        }

        modifiedCost = originalCost;
        return false;
    }

    public override async Task AfterCardPlayed(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        if (Owner is null || Amount <= 0 || cardPlay.Card.Owner.Creature != Owner) return;
        if (ReferenceEquals(cardPlay.Card, _justAppliedSource))
        {
            _justAppliedSource = null;
            return;
        }

        await ChimeGrants.GrantFromEffect<BurdenPower>(choiceContext, Owner, Amount);
    }

    public override Task AfterPlayerTurnStart(PlayerChoiceContext ctx, Player player)
    {
        if (Owner == player.Creature)
        {
            _justAppliedSource = null;
            RemoveInternal();
        }
        return Task.CompletedTask;
    }
}
