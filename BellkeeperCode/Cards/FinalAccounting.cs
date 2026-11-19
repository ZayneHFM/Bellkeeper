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
/// 终末清算（R10）：移除你所有的【哑音】和【负重】，每移除 1 层就对所有敌人造成 10 点伤害。【消耗】
/// </summary>
public class FinalAccounting : BellkeeperCard
{
    public FinalAccounting() : base(1, CardType.Attack, CardRarity.Rare, TargetType.AllEnemies) { }

    protected override IEnumerable<DynamicVar> CanonicalVars => new DynamicVar[]
    {
        new DamageVar(12m, DamageProps.card),
    };

    public override IEnumerable<CardKeyword> CanonicalKeywords => new[] { CardKeyword.Exhaust };

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay play)
    {
        var owner = Owner.Creature;
        int removed = 0;

        foreach (var neg in owner.GetPowerInstances<ChimePower>().Where(p => !p.IsPositive && p.Amount > 0).ToList())
        {
            removed += neg.Amount;
            neg.RemoveInternal();
        }

        if (removed > 0)
        {
            int totalDmg = removed * DynamicVars.Damage.IntValue;
            foreach (var e in owner.CombatState.Enemies.ToList())
            {
                if (e.IsDead) continue;
                await CreatureCmd.Damage(choiceContext, e, totalDmg, DamageProps.card, owner, this, play);
            }
        }
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Damage.UpgradeValueBy(3m);
    }
}
