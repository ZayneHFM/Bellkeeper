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
/// 深埋（C06）：【沉底 1】。【守御 12(16)】。（升级后【回音 1】）
/// </summary>
public class BuriedStrike : BellkeeperCard
{
    public BuriedStrike() : base(1, CardType.Skill, CardRarity.Common, TargetType.Self) { }

    protected override IEnumerable<DynamicVar> CanonicalVars => new DynamicVar[]
    {
        new IntVar("Guard", 12m),
    };

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay play)
    {
        await PileBottomHelper.SelectAndSinkFromHand(choiceContext, this, Owner.Creature, 0, 1);
        await ChimeGrants.Grant<GuardPower>(choiceContext, this, DynamicVars["Guard"].IntValue);
        if (IsUpgraded)
        {
            await ChimeGrants.Grant<EchoTunePower>(choiceContext, this, 1);
        }
    }

    protected override void OnUpgrade()
    {
        DynamicVars["Guard"].UpgradeValueBy(4m);
    }
}
