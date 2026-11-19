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
/// N23《苦行共鸣》(AsceticResonance) [罕见·能力·1费]
/// 基础：在你的回合结束时，你每有 1 种【失声】，获得 4 点格挡和 1 层【先行】。
/// 升级：在你的回合结束时，你每有 1 种【失声】，获得 6 点格挡和 1 层【先行】。
/// </summary>
public class AsceticResonance : BellkeeperCard
{
    public AsceticResonance() : base(1, CardType.Power, CardRarity.Uncommon, TargetType.Self) { }

    protected override IEnumerable<DynamicVar> CanonicalVars => new DynamicVar[]
    {
        new IntVar("BlockPerType", 4m),
    };

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay play)
    {
        await PowerCmd.Apply<AsceticResonancePower>(
            choiceContext, Owner.Creature, DynamicVars["BlockPerType"].IntValue, Owner.Creature, this, false);
    }

    protected override void OnUpgrade()
    {
        DynamicVars["BlockPerType"].UpgradeValueBy(2m);
    }
}
