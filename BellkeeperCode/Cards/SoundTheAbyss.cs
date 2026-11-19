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
/// 探渊（R23）：选择弃牌堆的最多 2(3) 张牌置于抽牌堆底，依次执行【探底 1】【探底 2】（升级后再【探底 3】）。【消耗】（升级后【保留】）
/// </summary>
public class SoundTheAbyss : BellkeeperCard
{
    public SoundTheAbyss() : base(1, CardType.Skill, CardRarity.Rare, TargetType.Self) { }

    protected override IEnumerable<DynamicVar> CanonicalVars => new DynamicVar[]
    {
        new IntVar("Cards", 2m),
    };

    public override IEnumerable<CardKeyword> CanonicalKeywords => new[] { CardKeyword.Exhaust };

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay play)
    {
        var player = Owner.Creature.Player;
        var discardPile = PileType.Discard.GetPile(player);
        int maxSelect = Math.Min(DynamicVars["Cards"].IntValue, discardPile.Cards.Count);
        if (maxSelect > 0)
        {
            var prompt = PileBottomHelper.GetPrompt("BELLKEEPER-SELECT_FROM_DISCARD_TO_BOTTOM", CardSelectorPrefs.DiscardSelectionPrompt);
            prompt.Add("MaxCount", maxSelect);
            var prefs = new CardSelectorPrefs(prompt, 0, maxSelect);
            var chosen = await CardSelectCmd.FromCombatPile(choiceContext, discardPile, player, prefs);
            if (chosen is not null)
            {
                foreach (var c in chosen.ToList())
                {
                    await CardPileCmd.Add(c, PileType.Draw, CardPilePosition.Bottom, null, false);
                }
            }
        }

        await PileBottomHelper.DredgeBottom(choiceContext, Owner.Creature, 1);
        await PileBottomHelper.DredgeBottom(choiceContext, Owner.Creature, 2);
        if (IsUpgraded)
        {
            await PileBottomHelper.DredgeBottom(choiceContext, Owner.Creature, 3);
        }
    }

    protected override void OnUpgrade()
    {
        DynamicVars["Cards"].UpgradeValueBy(1m);
        AddKeyword(CardKeyword.Retain);
    }
}
