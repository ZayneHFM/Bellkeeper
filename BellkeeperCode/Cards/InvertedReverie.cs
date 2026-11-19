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
/// N22《颠倒梦想》(InvertedReverie) [罕见·技能·1费]
/// 基础：【摸底 3】。获得 2 层【乱拍】。
/// 升级：【摸底 4】。获得 2 层【乱拍】。
/// </summary>
public class InvertedReverie : BellkeeperCard
{
    public InvertedReverie() : base(1, CardType.Skill, CardRarity.Uncommon, TargetType.Self) { }

    protected override IEnumerable<DynamicVar> CanonicalVars => new DynamicVar[]
    {
        new IntVar("DrawBottom", 3m),
        new IntVar("Offbeat", 2m),
    };

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay play)
    {
        await PileBottomHelper.DrawFromBottom(choiceContext, Owner.Creature, DynamicVars["DrawBottom"].IntValue);
        await ChimeGrants.Grant<OffbeatPower>(choiceContext, this, DynamicVars["Offbeat"].IntValue);
    }

    protected override void OnUpgrade()
    {
        DynamicVars["DrawBottom"].UpgradeValueBy(1m);
        DynamicVars["Offbeat"].UpgradeValueBy(-1m);
    }
}
