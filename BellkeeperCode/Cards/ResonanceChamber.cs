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
/// 共鸣腔（U03）：【保留】【消耗】。你的【钟鸣】和【守御】层数提高 50%。（升级：所有【余响】层数提高 50%，向上取整）
/// </summary>
public class ResonanceChamber : BellkeeperCard
{
    public ResonanceChamber() : base(1, CardType.Skill, CardRarity.Uncommon, TargetType.Self) { }

    protected override IEnumerable<DynamicVar> CanonicalVars => new DynamicVar[]
    {
        new IntVar("Pct", 50m),
    };

    public override IEnumerable<CardKeyword> CanonicalKeywords => new[]
    {
        CardKeyword.Retain,
        CardKeyword.Exhaust,
    };

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay play)
    {
        var owner = Owner.Creature;
        int pct = DynamicVars["Pct"].IntValue;

        foreach (var chime in owner.GetPowerInstances<ChimePower>().Where(p => p.IsPositive && p.Amount > 0).ToList())
        {
            if (!IsUpgraded && chime is not PealPower && chime is not GuardPower) continue;
            int addAmt = IsUpgraded ? (chime.Amount * pct + 99) / 100 : (chime.Amount * pct) / 100;
            if (addAmt > 0)
            {
                await ChimeGrants.GrantByAxis(choiceContext, this, chime.Axis, addAmt);
            }
        }
    }
}
