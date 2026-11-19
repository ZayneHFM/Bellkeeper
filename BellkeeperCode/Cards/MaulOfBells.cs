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
/// 万钧重槌（R11，原名：万钧钟锤）：造成与你的【守御】层数相同的伤害。（升级后 130%）
/// </summary>
public class MaulOfBells : BellkeeperCard
{
    public MaulOfBells() : base(1, CardType.Attack, CardRarity.Rare, TargetType.AnyEnemy) { }

    protected override IEnumerable<DynamicVar> CanonicalVars => new DynamicVar[]
    {
        new IntVar("Pct", 100m),
    };

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay play)
    {
        var target = play.Target;
        if (target is null || target.IsDead) return;

        var owner = Owner.Creature;
        int guardAmt = owner.Powers.OfType<GuardPower>().FirstOrDefault()?.Amount ?? 0;
        int dmg = (guardAmt * DynamicVars["Pct"].IntValue) / 100;

        if (dmg > 0)
        {
            await CreatureCmd.Damage(choiceContext, target, dmg, DamageProps.card, owner, this, play);
        }
    }

    protected override void OnUpgrade()
    {
        EnergyCost.UpgradeBy(-1);
        AddKeyword(CardKeyword.Retain);
    }
}
