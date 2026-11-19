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
/// 卸力肩带（C11）：移除你最多 2 层【负重】。获得 7 点格挡。
/// </summary>
public class SlackenedStrap : BellkeeperCard
{
    public SlackenedStrap() : base(1, CardType.Skill, CardRarity.Common, TargetType.Self) { }

    protected override IEnumerable<DynamicVar> CanonicalVars => new DynamicVar[]
    {
        new IntVar("Remove", 2m),
        new BlockVar(8m, BlockProps.card),
    };

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay play)
    {
        await CommonActions.CardBlock(this, play);

        var owner = Owner.Creature;
        int budget = DynamicVars["Remove"].IntValue;
        var targets = IsUpgraded
            ? owner.GetPowerInstances<ChimePower>().Where(p => !p.IsPositive && p.Amount > 0).ToList()
            : owner.Powers.OfType<BurdenPower>().Where(p => p.Amount > 0).Cast<ChimePower>().ToList();

        foreach (var neg in targets)
        {
            if (budget <= 0) break;
            int take = Math.Min(budget, neg.Amount);
            if (take <= 0) continue;
            if (neg.Amount <= take)
            {
                neg.RemoveInternal();
            }
            else
            {
                neg.SetAmount(neg.Amount - take, true);
            }
            budget -= take;
        }
    }

    protected override void OnUpgrade()
    {
        DynamicVars["Remove"].UpgradeValueBy(1m);
        DynamicVars.Block.UpgradeValueBy(3m);
    }
}
