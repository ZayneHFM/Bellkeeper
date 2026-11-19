using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Bellkeeper.BellkeeperCode.Extensions;
using Bellkeeper.BellkeeperCode.Mechanics;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;

namespace Bellkeeper.BellkeeperCode.Powers.CardPowers;

/// <summary>
/// 时间停滞（U05）：【钟鸣】与【守御】改为在你的回合结束时触发。
/// </summary>
public class TimeStandstillPower : BellkeeperPower
{
    public override string CustomPackedIconPath => "slowing_skip_turn.png".PowerImagePath();
    public override string CustomBigIconPath => "slowing_skip_turn.png".BigPowerImagePath();

    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Single;

    public override async Task BeforeSideTurnEnd(
        PlayerChoiceContext choiceContext,
        CombatSide side,
        IEnumerable<Creature> participants)
    {
        if (Owner is null || side != Owner.Side || Amount <= 0 || Owner.IsDead) return;

        var chimes = Owner.GetPowerInstances<ChimePower>()
            .Where(p => (p is GuardPower || p is PealPower) && p.Amount > 0)
            .ToList();

        foreach (var chime in chimes)
        {
            if (Owner.IsDead) break;
            if (chime.Amount <= 0) continue;
            await Toll.TriggerNatural(choiceContext, chime);
        }
    }
}
