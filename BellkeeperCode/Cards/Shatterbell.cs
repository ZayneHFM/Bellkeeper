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
/// 崩震（R12，原名：崩钟）：对所有敌人造成与你的【钟鸣】层数相同的伤害。（升级后 130%）
/// </summary>
public class Shatterbell : BellkeeperCard
{
    public Shatterbell() : base(2, CardType.Attack, CardRarity.Rare, TargetType.AllEnemies) { }

    protected override IEnumerable<DynamicVar> CanonicalVars => new DynamicVar[]
    {
        new IntVar("Pct", 100m),
    };

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay play)
    {
        var owner = Owner.Creature;
        int pealAmt = owner.Powers.OfType<PealPower>().FirstOrDefault()?.Amount ?? 0;
        int dmg = (pealAmt * DynamicVars["Pct"].IntValue) / 100;

        if (dmg > 0)
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
        DynamicVars["Pct"].UpgradeValueBy(30m);
    }
}
