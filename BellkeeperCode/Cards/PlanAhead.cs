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
using MegaCrit.Sts2.Core.ValueProps;

namespace Bellkeeper.BellkeeperCode.Cards;

/// <summary>
/// N03《未雨绸缪》(Plan Ahead) [罕见·技能·1费]
/// 基础：【沉底 3】。【先行 2】，【回音 2】。
/// 升级：【沉底 2】。【先行 2】，【回音 3】。
/// </summary>
public class PlanAhead : BellkeeperCard
{
    public PlanAhead() : base(1, CardType.Skill, CardRarity.Uncommon, TargetType.Self) { }

    protected override IEnumerable<DynamicVar> CanonicalVars => new DynamicVar[]
    {
        new IntVar("Sink", 3m),
        new IntVar("Vanguard", 2m),
        new IntVar("Echo", 2m),
    };

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay play)
    {
        await PileBottomHelper.SelectAndSinkFromHand(
            choiceContext, this, Owner.Creature, 0, DynamicVars["Sink"].IntValue);
        await ChimeGrants.Grant<VanguardPower>(choiceContext, this, DynamicVars["Vanguard"].IntValue);
        await ChimeGrants.Grant<EchoTunePower>(choiceContext, this, DynamicVars["Echo"].IntValue);
    }

    protected override void OnUpgrade()
    {
        DynamicVars["Sink"].UpgradeValueBy(-1m);
        DynamicVars["Echo"].UpgradeValueBy(1m);
    }
}
