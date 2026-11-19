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
/// 宵禁（C15）：0 费，对所有敌人造成 9(10) 点伤害（升级施加 1 层虚弱）。【哑音 1】。
/// </summary>
public class CurfewToll : BellkeeperCard
{
    public CurfewToll() : base(0, CardType.Attack, CardRarity.Common, TargetType.AllEnemies) { }

    protected override IEnumerable<DynamicVar> CanonicalVars => new DynamicVar[]
    {
        new DamageVar(9m, DamageProps.card),
        new IntVar("Mute", 1m),
    };

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay play)
    {
        var owner = Owner.Creature;
        int dmg = DynamicVars.Damage.IntValue;
        foreach (var e in owner.CombatState.Enemies.ToList())
        {
            if (e.IsDead) continue;
            await CreatureCmd.Damage(choiceContext, e, dmg, DamageProps.card, owner, this, play);
            if (IsUpgraded && !e.IsDead)
            {
                await PowerCmd.Apply<WeakPower>(choiceContext, e, 1, owner, this, false);
            }
        }
        await ChimeGrants.Grant<MutePower>(choiceContext, this, DynamicVars["Mute"].IntValue);
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Damage.UpgradeValueBy(1m);
    }
}
