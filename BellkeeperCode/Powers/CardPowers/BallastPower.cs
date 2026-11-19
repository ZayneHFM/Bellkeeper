using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;

namespace Bellkeeper.BellkeeperCode.Powers.CardPowers;

/// <summary>
/// 压舱状态（U24）：本回合你不会失去生命。回合开始时移除。
/// </summary>
public class BallastPower : BellkeeperPower
{
    public override PowerType Type => PowerType.Buff;

    public override PowerStackType StackType => PowerStackType.Single;

    public override decimal ModifyHpLostAfterOstyLate(
        Creature target,
        decimal amount,
        ValueProp props,
        Creature dealer,
        CardModel? cardSource)
    {
        return Owner is not null && target == Owner ? 0m : amount;
    }

    public override Task AfterPlayerTurnStart(PlayerChoiceContext ctx, Player player)
    {
        if (Owner == player.Creature)
        {
            RemoveInternal();
        }
        return Task.CompletedTask;
    }
}
