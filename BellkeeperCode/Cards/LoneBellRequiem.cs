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
/// 孤响镇魂（R26，原名：孤钟镇魂）：对所有敌人造成 20 点伤害。【沉底】手牌中所有的攻击牌。
/// </summary>
public class LoneBellRequiem : BellkeeperCard
{
    public LoneBellRequiem() : base(2, CardType.Attack, CardRarity.Rare, TargetType.AllEnemies) { }

    protected override IEnumerable<DynamicVar> CanonicalVars => new DynamicVar[]
    {
        new DamageVar(20m, DamageProps.card),
    };

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay play)
    {
        var owner = Owner.Creature;
        int dmg = DynamicVars.Damage.IntValue;
        foreach (var e in owner.CombatState.Enemies.ToList())
        {
            if (e.IsDead) continue;
            await CreatureCmd.Damage(choiceContext, e, dmg, DamageProps.card, owner, this, play);
        }

        var attacks = owner.Player.PlayerCombatState.Hand.Cards
            .Where(c => c.Type == CardType.Attack && c != this)
            .ToList();
        foreach (var atk in attacks)
        {
            await PileBottomHelper.SinkCard(choiceContext, atk, owner);
        }
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Damage.UpgradeValueBy(6m);
    }
}
