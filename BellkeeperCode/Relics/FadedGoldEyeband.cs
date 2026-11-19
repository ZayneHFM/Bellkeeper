using System.Collections.Generic;
using System.Threading.Tasks;
using Bellkeeper.BellkeeperCode.Powers;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;

namespace Bellkeeper.BellkeeperCode.Relics;

/// <summary>
/// 褪金眼带（商店遗物）：拾取时，最大生命减 7。每场战斗开始时，获得 2 层【延音】。
/// </summary>
public class FadedGoldEyeband : BellkeeperRelic
{
    public override RelicRarity Rarity => RelicRarity.Shop;

    public override bool HasUponPickupEffect => true;

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new MaxHpVar(7m),
    ];

    public override async Task AfterObtained()
    {
        if (Owner?.Creature is not { } creature) return;

        await CreatureCmd.LoseMaxHp(
            new ThrowingPlayerChoiceContext(),
            creature,
            DynamicVars.MaxHp.BaseValue,
            null);
    }

    public override async Task BeforeCombatStart()
    {
        var creature = Owner?.Creature;
        if (creature is null) return;

        Flash();
        await PowerCmd.Apply<SustainPower>(
            new ThrowingPlayerChoiceContext(), creature, 2, creature, null, false);
    }
}
