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
/// 渐强（C01）：【钟鸣 8(9)】，【延音 1(2)】。
/// </summary>
public class Crescendo : BellkeeperCard
{
    public Crescendo() : base(1, CardType.Skill, CardRarity.Common, TargetType.Self) { }

    protected override IEnumerable<DynamicVar> CanonicalVars => new DynamicVar[]
    {
        new IntVar("Chime", 8m),
        new IntVar("Sustain", 1m),
    };

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay play)
    {
        await ChimeGrants.Grant<PealPower>(choiceContext, this, DynamicVars["Chime"].IntValue);
        await ChimeGrants.Grant<SustainPower>(choiceContext, this, DynamicVars["Sustain"].IntValue);
    }

    protected override void OnUpgrade()
    {
        DynamicVars["Chime"].UpgradeValueBy(1m);
        DynamicVars["Sustain"].UpgradeValueBy(1m);
    }
}
