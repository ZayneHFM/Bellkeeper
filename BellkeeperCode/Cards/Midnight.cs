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
/// 子夜（先古卡，拾取《古老牙齿》将初始卡《震击》+《守夜》纯粹二合一合成）：
/// 造成 4(6) 点伤害。【钟鸣 5(6)】。【守御 7(10)】，【先行 1(2)】，【延音 1(2)】。
/// </summary>
public class Midnight : BellkeeperCard
{
    public Midnight() : base(1, CardType.Attack, CardRarity.Ancient, TargetType.AnyEnemy) { }

    protected override IEnumerable<DynamicVar> CanonicalVars => new DynamicVar[]
    {
        new DamageVar(4m, DamageProps.card),
        new IntVar("Chime", 5m),
        new IntVar("Guard", 7m),
        new IntVar("Vanguard", 1m),
        new IntVar("Sustain", 1m),
    };

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay play)
    {
        await CommonActions.CardAttack(this, play).Execute(choiceContext);

        await ChimeGrants.Grant<PealPower>(choiceContext, this, DynamicVars["Chime"].IntValue);
        await ChimeGrants.Grant<GuardPower>(choiceContext, this, DynamicVars["Guard"].IntValue);
        await ChimeGrants.Grant<VanguardPower>(choiceContext, this, DynamicVars["Vanguard"].IntValue);
        await ChimeGrants.Grant<SustainPower>(choiceContext, this, DynamicVars["Sustain"].IntValue);
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Damage.UpgradeValueBy(2m);
        DynamicVars["Chime"].UpgradeValueBy(1m);
        DynamicVars["Guard"].UpgradeValueBy(3m);
        DynamicVars["Vanguard"].UpgradeValueBy(1m);
        DynamicVars["Sustain"].UpgradeValueBy(1m);
    }
}
