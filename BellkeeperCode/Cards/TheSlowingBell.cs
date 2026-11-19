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
/// 凝滞之音（R16，原名：迟滞之钟）：所有敌人跳过下个回合，并获得 2 点力量。【消耗】（升级后 1 点力量）
/// </summary>
public class TheSlowingBell : BellkeeperCard
{
    public TheSlowingBell() : base(3, CardType.Skill, CardRarity.Rare, TargetType.AllEnemies) { }

    protected override IEnumerable<DynamicVar> CanonicalVars => new DynamicVar[]
    {
        new IntVar("Strength", 2m),
    };

    public override IEnumerable<CardKeyword> CanonicalKeywords => new[] { CardKeyword.Exhaust };

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay play)
    {
        var owner = Owner.Creature;
        int str = DynamicVars["Strength"].IntValue;
        foreach (var e in owner.CombatState.Enemies.ToList())
        {
            if (e.IsDead) continue;
            await PowerCmd.Apply<StrengthPower>(choiceContext, e, str, owner, this, false);
            await PowerCmd.Apply<SlowingSkipTurnPower>(choiceContext, e, 1, owner, this, false);
        }
    }

    protected override void OnUpgrade()
    {
        DynamicVars["Strength"].UpgradeValueBy(-1m);
    }
}
