using System.Collections.Generic;
using System.Threading.Tasks;
using Bellkeeper.BellkeeperCode.Powers;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;

namespace Bellkeeper.BellkeeperCode.Relics;

/// <summary>
/// 断梁木架（先古遗物，达夫能量遗物池）：在你的回合开始时获得 1 点能量。
/// 每场战斗开始时，获得 2 层【负重】和 1 层【哑音】。
/// </summary>
public class BrokenBeamFrame : BellkeeperRelic
{
    public override RelicRarity Rarity => RelicRarity.Ancient;

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new EnergyVar(1),
    ];

    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
    [
        HoverTipFactory.ForEnergy(this),
    ];

    public override decimal ModifyMaxEnergy(Player player, decimal amount) =>
        player == Owner ? amount + DynamicVars.Energy.IntValue : amount;

    public override async Task BeforeCombatStart()
    {
        var creature = Owner?.Creature;
        if (creature is null) return;

        Flash();
        var ctx = new ThrowingPlayerChoiceContext();
        await PowerCmd.Apply<BurdenPower>(ctx, creature, 2, creature, null, false);
        await PowerCmd.Apply<MutePower>(ctx, creature, 1, creature, null, false);
    }
}
