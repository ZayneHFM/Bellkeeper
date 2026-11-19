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
/// 绝响崩摧（R06）：造成 18 点伤害。【叩钟】，并造成与移除层数相同的伤害。
/// </summary>
public class FinalPeal : BellkeeperCard
{
    public FinalPeal() : base(3, CardType.Attack, CardRarity.Rare, TargetType.AnyEnemy) { }

    protected override IEnumerable<DynamicVar> CanonicalVars => new DynamicVar[]
    {
        new DamageVar(18m, DamageProps.card),
    };

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay play)
    {
        await CommonActions.CardAttack(this, play).Execute(choiceContext);

        var owner = Owner.Creature;
        int beforeTotal = owner.Powers.OfType<ChimePower>().Sum(p => p.Amount);
        await ChimeGrants.Ring(choiceContext, this);
        int afterTotal = owner.Powers.OfType<ChimePower>().Sum(p => p.Amount);

        int removedLayers = Math.Max(0, beforeTotal - afterTotal);
        if (removedLayers > 0 && play.Target is not null && !play.Target.IsDead)
        {
            int bonusDmg = IsUpgraded ? removedLayers * 2 : removedLayers;
            await CreatureCmd.Damage(choiceContext, play.Target, bonusDmg, DamageProps.card, owner, this, play);
        }
    }

    protected override void OnUpgrade()
    {
        AddKeyword(CardKeyword.Retain);
        DynamicVars.Damage.UpgradeValueBy(4m);
    }
}
