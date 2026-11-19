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
/// 盲步（C20）：造成 6 点伤害，获得 6 点格挡。若手牌数为奇数，获得 3 层【守御】。
/// </summary>
public class BlindStep : BellkeeperCard
{
    public BlindStep() : base(1, CardType.Attack, CardRarity.Common, TargetType.AnyEnemy) { }

    protected override IEnumerable<DynamicVar> CanonicalVars => new DynamicVar[]
    {
        new DamageVar(6m, DamageProps.card),
        new IntVar("Guard", 2m),
    };

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay play)
    {
        await CommonActions.CardAttack(this, play).Execute(choiceContext);

        int handCount = Owner.Creature.Player.PlayerCombatState.Hand.Cards.Count;
        if (handCount > 0)
        {
            await ChimeGrants.Grant<GuardPower>(choiceContext, this, handCount * DynamicVars["Guard"].IntValue);
        }
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Damage.UpgradeValueBy(2m);
        DynamicVars["Guard"].UpgradeValueBy(1m);
    }
}
