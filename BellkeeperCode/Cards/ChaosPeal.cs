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
/// 乱槌齐鸣（U01）：对所有敌人造成 7 点伤害 2 次。【齐鸣 2】：再攻击 1 次。
/// </summary>
public class ChaosPeal : BellkeeperCard
{
    public ChaosPeal() : base(2, CardType.Attack, CardRarity.Uncommon, TargetType.AllEnemies) { }

    protected override IEnumerable<DynamicVar> CanonicalVars => new DynamicVar[]
    {
        new DamageVar(7m, DamageProps.card),
        new IntVar("Hits", 2m),
    };

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay play)
    {
        var owner = Owner.Creature;
        int dmg = DynamicVars.Damage.IntValue;
        int extraHits = IsUpgraded
            ? owner.GetPowerInstances<ChimePower>().Count(p => p.Amount > 0)
            : owner.GetPowerInstances<ChimePower>().Count(p => p.IsPositive && p.Amount > 0);
        int hits = DynamicVars["Hits"].IntValue + extraHits;

        for (int i = 0; i < hits; i++)
        {
            foreach (var e in owner.CombatState.Enemies.ToList())
            {
                if (e.IsDead) continue;
                await CreatureCmd.Damage(choiceContext, e, dmg, DamageProps.card, owner, this, play);
            }
        }
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Damage.UpgradeValueBy(2m);
    }
}
