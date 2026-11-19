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
/// N33《潜渊铸钟》(AbyssalCasting) [罕见·技能·1费]
/// 基础：【沉底 1】，将该牌的 1 张复制品一并置于抽牌堆底。【消耗】
/// 升级：【沉底 1】，将该牌的 1 张复制品一并置于抽牌堆底。（不消耗）
/// </summary>
public class AbyssalCasting : BellkeeperCard
{
    public AbyssalCasting() : base(1, CardType.Skill, CardRarity.Uncommon, TargetType.Self) { }

    protected override IEnumerable<DynamicVar> CanonicalVars => Array.Empty<DynamicVar>();

    public override IEnumerable<CardKeyword> CanonicalKeywords => new[] { CardKeyword.Exhaust };

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay play)
    {
        var sunk = await PileBottomHelper.SelectAndSinkFromHand(choiceContext, this, Owner.Creature, 1, 1);
        if (sunk.Count > 0)
        {
            var copy = sunk[0].CreateClone();
            await PileBottomHelper.SinkCard(choiceContext, copy, Owner.Creature);
        }
    }

    protected override void OnUpgrade()
    {
        RemoveKeyword(CardKeyword.Exhaust);
    }
}
