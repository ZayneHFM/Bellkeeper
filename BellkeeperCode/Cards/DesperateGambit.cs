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
/// N20《险中求胜》(DesperateGambit) [罕见·技能·0费]
/// 基础：获得 12 点格挡和 1 层【先行】。获得 2 层【惊悸】。
/// 升级：获得 16 点格挡和 1 层【先行】。获得 2 层【惊悸】。
/// </summary>
public class DesperateGambit : BellkeeperCard
{
    public DesperateGambit() : base(0, CardType.Skill, CardRarity.Uncommon, TargetType.Self) { }

    protected override IEnumerable<DynamicVar> CanonicalVars => new DynamicVar[]
    {
        new BlockVar(12m, BlockProps.card),
        new IntVar("Vanguard", 1m),
        new IntVar("Tremor", 2m),
    };

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay play)
    {
        await CommonActions.CardBlock(this, play);
        await ChimeGrants.Grant<VanguardPower>(choiceContext, this, DynamicVars["Vanguard"].IntValue);
        await ChimeGrants.Grant<TremorPower>(choiceContext, this, DynamicVars["Tremor"].IntValue);
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Block.UpgradeValueBy(2m);
        DynamicVars["Vanguard"].UpgradeValueBy(1m);
    }
}
