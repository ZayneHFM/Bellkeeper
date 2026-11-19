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
/// 盲杖点地（U33）：造成 10 点伤害。若手牌数为偶数，获得 8 点格挡。
/// </summary>
public class TapTheCane : BellkeeperCard
{
    public TapTheCane() : base(1, CardType.Attack, CardRarity.Uncommon, TargetType.AnyEnemy) { }

    protected override IEnumerable<DynamicVar> CanonicalVars => new DynamicVar[]
    {
        new DamageVar(3m, DamageProps.card),
        new IntVar("Draw", 1m),
    };

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay play)
    {
        await PileBottomHelper.DrawFromBottom(choiceContext, Owner.Creature, DynamicVars["Draw"].IntValue);
        if (play.Target is null || play.Target.IsDead) return;

        int handCount = Owner.Creature.Player.PlayerCombatState.Hand.Cards.Count;
        if (handCount > 0)
        {
            int dmg = handCount * DynamicVars.Damage.IntValue;
            await CreatureCmd.Damage(choiceContext, play.Target, dmg, DamageProps.card, Owner.Creature, this, play);
        }
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Damage.UpgradeValueBy(1m);
        DynamicVars["Draw"].UpgradeValueBy(1m);
    }
}
