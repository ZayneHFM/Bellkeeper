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
/// 重摆（U15，原名：大钟摆）：获得 2 层【延音】。获得 12 层【守御】。
/// </summary>
public class GreatPendulum : BellkeeperCard
{
    public GreatPendulum() : base(2, CardType.Skill, CardRarity.Uncommon, TargetType.Self) { }

    protected override IEnumerable<DynamicVar> CanonicalVars => new DynamicVar[]
    {
        new IntVar("Sustain", 2m),
        new IntVar("Guard", 12m),
    };

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay play)
    {
        await ChimeGrants.Grant<SustainPower>(choiceContext, this, DynamicVars["Sustain"].IntValue);
        await ChimeGrants.Grant<GuardPower>(choiceContext, this, DynamicVars["Guard"].IntValue);
    }

    protected override void OnUpgrade()
    {
        DynamicVars["Sustain"].UpgradeValueBy(1m);
        DynamicVars["Guard"].UpgradeValueBy(4m);
    }
}
