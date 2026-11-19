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
/// 翻找（C09）：0 费手牌置换。先【摸底 2(3)】，再【沉底 1(2)】（不消耗）。
/// </summary>
public class Rummage : BellkeeperCard
{
    public Rummage() : base(0, CardType.Skill, CardRarity.Common, TargetType.Self) { }

    protected override IEnumerable<DynamicVar> CanonicalVars => new DynamicVar[]
    {
        new IntVar("Draw", 2m),
        new IntVar("Sink", 1m),
    };

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay play)
    {
        await PileBottomHelper.DrawFromBottom(choiceContext, Owner.Creature, DynamicVars["Draw"].IntValue);
        int sinkCount = DynamicVars["Sink"].IntValue;
        await PileBottomHelper.SelectAndSinkFromHand(choiceContext, this, Owner.Creature, sinkCount, sinkCount);
    }

    protected override void OnUpgrade()
    {
        DynamicVars["Draw"].UpgradeValueBy(1m);
        DynamicVars["Sink"].UpgradeValueBy(1m);
    }
}
