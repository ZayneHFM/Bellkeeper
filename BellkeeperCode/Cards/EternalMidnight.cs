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
/// N36《万古长夜》(EternalMidnight) [稀有·能力·2费]
/// 基础：在你的回合结束时，你每有 3 张手牌，【先行 1】，【守御 2】。
/// 升级：在你的回合结束时，你每有 2 张手牌，【先行 1】，【守御 2】。
/// </summary>
public class EternalMidnight : BellkeeperCard
{
    public EternalMidnight() : base(2, CardType.Power, CardRarity.Rare, TargetType.Self) { }

    protected override IEnumerable<DynamicVar> CanonicalVars => new DynamicVar[]
    {
        new IntVar("CardsPerTrigger", 3m),
    };

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay play)
    {
        if (IsUpgraded)
        {
            await PowerCmd.Apply<EternalMidnightUpgradedPower>(choiceContext, Owner.Creature, 1, Owner.Creature, this, false);
        }
        else
        {
            await PowerCmd.Apply<EternalMidnightPower>(choiceContext, Owner.Creature, 1, Owner.Creature, this, false);
        }
    }

    protected override void OnUpgrade()
    {
        DynamicVars["CardsPerTrigger"].UpgradeValueBy(-1m);
    }
}
