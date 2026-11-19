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
/// 倾覆（R07）：移除你所有的【负重】，每移除 1 层就对所有敌人造成 12 点伤害。
/// </summary>
public class Capsize : BellkeeperCard
{
    public Capsize() : base(2, CardType.Attack, CardRarity.Rare, TargetType.AllEnemies) { }

    protected override IEnumerable<DynamicVar> CanonicalVars => new DynamicVar[]
    {
        new DamageVar(14m, DamageProps.card),
    };

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay play)
    {
        var owner = Owner.Creature;
        if (IsUpgraded)
        {
            await ChimeGrants.Grant<BurdenPower>(choiceContext, this, 1);
        }

        var burden = owner.Powers.OfType<BurdenPower>().FirstOrDefault();
        if (burden is not null && burden.Amount > 0)
        {
            int layers = burden.Amount;
            burden.RemoveInternal();

            int totalDmg = layers * DynamicVars.Damage.IntValue;
            foreach (var e in owner.CombatState.Enemies.ToList())
            {
                if (e.IsDead) continue;
                await CreatureCmd.Damage(choiceContext, e, totalDmg, DamageProps.card, owner, this, play);
            }
        }
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Damage.UpgradeValueBy(2m);
    }
}
