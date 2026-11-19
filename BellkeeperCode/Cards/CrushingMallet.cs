using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BaseLib.Utils;
using Bellkeeper.BellkeeperCode.Mechanics;
using Bellkeeper.BellkeeperCode.Powers;
using Bellkeeper.BellkeeperCode.Powers.CardPowers;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.ValueProps;

namespace Bellkeeper.BellkeeperCode.Cards;

/// <summary>
/// 重槌砸落（U31）：造成 16 点伤害。获得 1 层【负重】。
/// </summary>
public class CrushingMallet : BellkeeperCard
{
    public CrushingMallet() : base(1, CardType.Attack, CardRarity.Uncommon, TargetType.AnyEnemy) { }

    protected override IEnumerable<DynamicVar> CanonicalVars => new DynamicVar[]
    {
        new DamageVar(9m, DamageProps.card),
        new IntVar("PerStack", 5m),
    };

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay play)
    {
        var target = play.Target;
        if (target is null || target.IsDead) return;

        var owner = Owner.Creature;
        int stacks = IsUpgraded
            ? owner.GetPowerInstances<ChimePower>().Where(p => !p.IsPositive && p.Amount > 0).Sum(p => p.Amount)
            : (owner.Powers.OfType<BurdenPower>().FirstOrDefault()?.Amount ?? 0);

        int dmg = DynamicVars.Damage.IntValue + stacks * DynamicVars["PerStack"].IntValue;
        await CreatureCmd.Damage(choiceContext, target, dmg, DamageProps.card, owner, this, play);
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Damage.UpgradeValueBy(3m);
        DynamicVars["PerStack"].UpgradeValueBy(2m);
    }
}
