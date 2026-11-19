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
/// N14《抛砖引玉》(CastJade) [罕见·技能·1费]
/// 基础：【探底 2】，【回音 2】。
/// 升级：【探底 3】，【回音 3】。
/// </summary>
public class CastJade : BellkeeperCard
{
    public CastJade() : base(1, CardType.Skill, CardRarity.Uncommon, TargetType.Self) { }

    protected override IEnumerable<DynamicVar> CanonicalVars => new DynamicVar[]
    {
        new IntVar("Dredge", 2m),
        new IntVar("Echo", 2m),
    };

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay play)
    {
        await PileBottomHelper.DredgeBottom(
            choiceContext, Owner.Creature, DynamicVars["Dredge"].IntValue);
        await ChimeGrants.Grant<EchoTunePower>(
            choiceContext, this, DynamicVars["Echo"].IntValue);
    }

    protected override void OnUpgrade()
    {
        DynamicVars["Dredge"].UpgradeValueBy(1m);
        DynamicVars["Echo"].UpgradeValueBy(1m);
    }
}
