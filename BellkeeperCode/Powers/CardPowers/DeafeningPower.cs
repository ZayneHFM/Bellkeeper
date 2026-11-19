using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Powers;

namespace Bellkeeper.BellkeeperCode.Powers.CardPowers;

/// <summary>
/// 震耳欲聋（R04）：每当【钟鸣】造成伤害，施加 1 层【易伤】（升级 2）。
///
/// 挂载点：AfterDamageGiven
///   判据：伤害由自己造成，且没有卡牌来源——那是【钟鸣】的 AOE。
///   卡牌攻击有 cardSource，会被排除掉。
/// </summary>
public class DeafeningPower : BellkeeperPower
{
    public override PowerType Type => PowerType.Buff;

    public override PowerStackType StackType => PowerStackType.Counter;

    public override async Task AfterDamageGiven(
        PlayerChoiceContext choiceContext,
        Creature? target,
        DamageResult result,
        MegaCrit.Sts2.Core.ValueProps.ValueProp props,
        Creature? dealer,
        CardModel? cardSource)
    {
        if (Owner is null || Amount <= 0) return;
        if (dealer != Owner) return;
        if (target is null || target.IsDead) return;
        if (result.TotalDamage <= 0) return;
        if (cardSource is not null) return;

        await PowerCmd.Apply<VulnerablePower>(choiceContext, target, Amount, Owner, null, false);
    }
}
