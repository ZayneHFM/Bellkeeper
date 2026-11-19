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
/// N35《水落石出》(TruthSurfaces) [稀有·技能·2费]
/// 基础：打出你抽牌堆底的 2 张牌。【消耗】
/// 升级：打出你抽牌堆底的 2 张牌。【消耗】
/// </summary>
public class TruthSurfaces : BellkeeperCard
{
    public TruthSurfaces() : base(2, CardType.Skill, CardRarity.Uncommon, TargetType.Self) { }

    protected override IEnumerable<DynamicVar> CanonicalVars => Array.Empty<DynamicVar>();

    public override IEnumerable<CardKeyword> CanonicalKeywords => new[] { CardKeyword.Exhaust };

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay play)
    {
        if (IsUpgraded)
        {
            await PileBottomHelper.SelectAndSinkFromHand(choiceContext, this, Owner.Creature, 0, 1);
        }
        await CardPileCmd.AutoPlayFromDrawPile(
            choiceContext, Owner.Creature.Player, 2, CardPilePosition.Bottom, false);
    }
}
