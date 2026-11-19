using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Powers;

namespace Bellkeeper.BellkeeperCode.Powers.CardPowers;

/// <summary>
/// 负重锻炉（U07）：每当你获得【负重】，获得 2 点力量（升级 3）。
///
/// 挂载点：AfterPowerAmountChanged
///   这是框架真正的「某个状态层数变化了」通知。
/// </summary>
public class BurdenForgePower : BellkeeperPower
{
    public override PowerType Type => PowerType.Buff;

    public override PowerStackType StackType => PowerStackType.Counter;

    public override async Task AfterPowerAmountChanged(
        PlayerChoiceContext choiceContext,
        PowerModel power,
        decimal amount,
        Creature applier,
        CardModel? cardSource)
    {
        if (Owner is null) return;
        if (power is not BurdenPower) return;
        if (amount <= 0) return;
        if (power.Owner != Owner) return;
        if (Amount <= 0) return;

        await PowerCmd.Apply<StrengthPower>(choiceContext, Owner, Amount, Owner, cardSource, false);
    }
}
