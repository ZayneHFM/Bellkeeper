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
/// 齐鸣合奏（U27）：对所有敌人施加 1 层虚弱和 1 层易伤。【齐鸣 2】：再施加 1 层。
/// </summary>
public class Ensemble : BellkeeperCard
{
    public Ensemble() : base(1, CardType.Skill, CardRarity.Uncommon, TargetType.AllEnemies) { }

    protected override IEnumerable<DynamicVar> CanonicalVars => new DynamicVar[]
    {
        new IntVar("Debuff", 1m),
    };

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay play)
    {
        var owner = Owner.Creature;
        bool resonance = ChimeGrants.HasResonance(this, 2);
        int amount = DynamicVars["Debuff"].IntValue + (resonance ? 1 : 0);

        foreach (var e in owner.CombatState.Enemies.ToList())
        {
            if (e.IsDead) continue;
            await PowerCmd.Apply<WeakPower>(choiceContext, e, amount, owner, this, false);
            await PowerCmd.Apply<VulnerablePower>(choiceContext, e, amount, owner, this, false);
        }

        if (IsUpgraded && resonance)
        {
            await ChimeGrants.Grant<SustainPower>(choiceContext, this, 1);
        }
    }

    protected override void OnUpgrade()
    {
        DynamicVars["Debuff"].UpgradeValueBy(1m);
    }
}
