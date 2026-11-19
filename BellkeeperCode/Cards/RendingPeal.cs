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
/// 裂帛急响（U22）：对所有敌人造成 10 点伤害。【齐鸣 3】：改为 20 点。
/// </summary>
public class RendingPeal : BellkeeperCard
{
    public RendingPeal() : base(1, CardType.Attack, CardRarity.Uncommon, TargetType.AllEnemies) { }

    protected override IEnumerable<DynamicVar> CanonicalVars => new DynamicVar[]
    {
        new DamageVar(10m, DamageProps.card),
    };

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay play)
    {
        var owner = Owner.Creature;
        bool resonance = ChimeGrants.HasResonance(this, 2);
        int dmg = DynamicVars.Damage.IntValue;

        foreach (var e in owner.CombatState.Enemies.ToList())
        {
            if (e.IsDead) continue;
            await CreatureCmd.Damage(choiceContext, e, dmg, DamageProps.card, owner, this, play);
        }

        if (resonance)
        {
            if (IsUpgraded)
            {
                await ChimeGrants.Grant<SustainPower>(choiceContext, this, 1);
            }

            var peal = owner.Powers.OfType<PealPower>().FirstOrDefault();
            if (peal is not null && peal.Amount > 0)
            {
                await peal.ExecuteEffect(choiceContext, owner, peal.Amount, this);
            }
        }
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Damage.UpgradeValueBy(2m);
    }
}
