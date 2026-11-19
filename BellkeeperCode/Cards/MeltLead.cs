using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BaseLib.Utils;
using Bellkeeper.BellkeeperCode.Mechanics;
using Bellkeeper.BellkeeperCode.Powers;
using Bellkeeper.BellkeeperCode.Powers.CardPowers;
using MegaCrit.Sts2.Core.CardSelection;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.ValueProps;

namespace Bellkeeper.BellkeeperCode.Cards;

/// <summary>
/// 熔铅（C14）：消耗 1 张手牌，获得 6 层【守御】。若为状态牌，抽 1 张牌。
/// </summary>
public class MeltLead : BellkeeperCard
{
    public MeltLead() : base(1, CardType.Skill, CardRarity.Common, TargetType.Self) { }

    protected override IEnumerable<DynamicVar> CanonicalVars => new DynamicVar[]
    {
        new IntVar("Guard", 8m),
        new IntVar("Draw", 2m),
    };

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay play)
    {
        var prompt = PileBottomHelper.GetPrompt("BELLKEEPER-SELECT_ANY", CardSelectorPrefs.ExhaustSelectionPrompt);
        var chosen = await CommonActions.SelectSingleCard(this, prompt, choiceContext, PileType.Hand);
        if (chosen is not null)
        {
            bool isStatus = chosen.Type == CardType.Status;
            await CardCmd.Exhaust(choiceContext, chosen);
            await ChimeGrants.Grant<GuardPower>(choiceContext, this, DynamicVars["Guard"].IntValue);
            if (isStatus)
            {
                await CardPileCmd.Draw(choiceContext, DynamicVars["Draw"].IntValue, Owner.Creature.Player, false);
            }
        }
    }

    protected override void OnUpgrade()
    {
        DynamicVars["Guard"].UpgradeValueBy(3m);
        DynamicVars["Draw"].UpgradeValueBy(1m);
    }
}
