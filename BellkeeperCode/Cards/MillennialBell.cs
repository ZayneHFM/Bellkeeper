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
/// 千年古钟（R02）：获得 20 层【钟鸣】和 20 层【守御】。（升级后 28/28）
/// </summary>
public class MillennialBell : BellkeeperCard
{
    public MillennialBell() : base(3, CardType.Skill, CardRarity.Rare, TargetType.Self) { }

    protected override IEnumerable<DynamicVar> CanonicalVars => new DynamicVar[]
    {
        new IntVar("Chime", 22m),
        new IntVar("Guard", 20m),
    };

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay play)
    {
        await ChimeGrants.Grant<PealPower>(choiceContext, this, DynamicVars["Chime"].IntValue);
        await ChimeGrants.Grant<GuardPower>(choiceContext, this, DynamicVars["Guard"].IntValue);
    }

    protected override void OnUpgrade()
    {
        DynamicVars["Chime"].UpgradeValueBy(6m);
        DynamicVars["Guard"].UpgradeValueBy(8m);
    }
}
