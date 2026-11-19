using Bellkeeper.BellkeeperCode.Mechanics;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;

namespace Bellkeeper.BellkeeperCode.Powers.CardPowers;

/// <summary>
/// 遗音（R20）：每场战斗一次，当你即将死亡时，回复至 25% 生命（升级 35%）并【叩钟】。
///
/// 挂载点：AfterDamageReceived
///   伤害结算完检查：如果这一下打死了自己，就拉回来。
///   （BeforeDamageReceived 的参数是 decimal 非 ref，改不了伤害值，
///     所以只能在结算后补救——效果等价于「不会死」。）
///
/// Amount 存的是回复百分比（25 / 35）。
/// </summary>
public class TestamentPower : BellkeeperPower
{
    public override PowerType Type => PowerType.Buff;

    public override PowerStackType StackType => PowerStackType.Single;

    /// <summary>本场战斗是否已经触发过。</summary>
    public bool Used { get; set; }

    public override async Task AfterDamageReceived(
        PlayerChoiceContext choiceContext,
        Creature target,
        DamageResult result,
        ValueProp props,
        Creature? dealer,
        CardModel? cardSource)
    {
        if (Owner is null || Used || Amount <= 0) return;
        if (target != Owner) return;
        if (!target.IsDead) return;

        Used = true;

        int maxHp = target.MaxHp;
        int healTo = (int)(maxHp * Amount / 100m);
        if (healTo <= 0) healTo = 1;

        await CreatureCmd.SetCurrentHp(target, healTo);
        await Toll.Ring(choiceContext, target);
    }
}
