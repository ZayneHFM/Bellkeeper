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
/// N19《裂肤急槌》(SkinSplittingBlow) [普通·攻击·1费]
/// 基础：造成 14 点伤害，获得 4 层【钟鸣】。获得 2 层【惊悸】。
/// 升级：造成 18 点伤害，获得 6 层【钟鸣】。获得 2 层【惊悸】。
/// </summary>
public class SkinSplittingBlow : BellkeeperCard
{
    public SkinSplittingBlow() : base(1, CardType.Attack, CardRarity.Common, TargetType.AnyEnemy) { }

    protected override IEnumerable<DynamicVar> CanonicalVars => new DynamicVar[]
    {
        new DamageVar(14m, DamageProps.card),
        new IntVar("Peal", 4m),
        new IntVar("Tremor", 2m),
    };

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay play)
    {
        await CommonActions.CardAttack(this, play).Execute(choiceContext);
        await ChimeGrants.Grant<PealPower>(choiceContext, this, DynamicVars["Peal"].IntValue);
        await ChimeGrants.Grant<TremorPower>(choiceContext, this, DynamicVars["Tremor"].IntValue);
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Damage.UpgradeValueBy(3m);
        DynamicVars["Peal"].UpgradeValueBy(2m);
        DynamicVars["Tremor"].UpgradeValueBy(-1m);
    }
}
