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
/// N13《空谷传响》(ValleyEcho) [普通·技能·1费]
/// 基础：【沉底 1】。获得 6 点格挡，【回音 1】。
/// 升级：【沉底 1】。获得 8 点格挡，【回音 2】。
/// </summary>
public class ValleyEcho : BellkeeperCard
{
    public ValleyEcho() : base(1, CardType.Skill, CardRarity.Common, TargetType.Self) { }

    protected override IEnumerable<DynamicVar> CanonicalVars => new DynamicVar[]
    {
        new BlockVar(6m, BlockProps.card),
        new IntVar("Echo", 1m),
    };

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay play)
    {
        await PileBottomHelper.SelectAndSinkFromHand(choiceContext, this, Owner.Creature, 0, 1);
        await CommonActions.CardBlock(this, play);
        await ChimeGrants.Grant<EchoTunePower>(choiceContext, this, DynamicVars["Echo"].IntValue);
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Block.UpgradeValueBy(2m);
        DynamicVars["Echo"].UpgradeValueBy(1m);
    }
}
