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
/// 探底（C08）：【探底 2(3)】。获得 6(9) 点格挡，【摸底 1】。
/// </summary>
public class DredgeThePit : BellkeeperCard
{
    public DredgeThePit() : base(1, CardType.Skill, CardRarity.Common, TargetType.Self) { }

    protected override IEnumerable<DynamicVar> CanonicalVars => new DynamicVar[]
    {
        new BlockVar(6m, BlockProps.card),
        new IntVar("Look", 2m),
    };

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay play)
    {
        await PileBottomHelper.DredgeBottom(
            choiceContext, Owner.Creature, DynamicVars["Look"].IntValue);

        await CommonActions.CardBlock(this, play);

        await PileBottomHelper.DrawFromBottom(choiceContext, Owner.Creature, 1);
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Block.UpgradeValueBy(3m);
        DynamicVars["Look"].UpgradeValueBy(1m);
    }
}
