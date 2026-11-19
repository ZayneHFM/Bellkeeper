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
/// 无声潜行（R18）：获得 1 层无形。获得 2 层【哑音】。【消耗】
/// </summary>
public class SilentPassage : BellkeeperCard
{
    public SilentPassage() : base(1, CardType.Skill, CardRarity.Rare, TargetType.Self) { }

    protected override IEnumerable<DynamicVar> CanonicalVars => new DynamicVar[]
    {
        new IntVar("Intangible", 1m),
        new IntVar("Mute", 2m),
    };

    public override IEnumerable<CardKeyword> CanonicalKeywords => new[] { CardKeyword.Exhaust };

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay play)
    {
        var owner = Owner.Creature;
        await PowerCmd.Apply<IntangiblePower>(
            choiceContext, owner, DynamicVars["Intangible"].IntValue, owner, this, false);
        await ChimeGrants.Grant<MutePower>(choiceContext, this, DynamicVars["Mute"].IntValue);
    }

    protected override void OnUpgrade()
    {
        DynamicVars["Intangible"].UpgradeValueBy(1m);
    }
}
