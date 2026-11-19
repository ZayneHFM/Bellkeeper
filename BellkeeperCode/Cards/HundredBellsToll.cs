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
/// N29《百钟争鸣》(HundredBellsToll) [稀有·攻击·2费]
/// 基础：对所有敌人造成 10 点伤害。【齐鸣 4】：【叩钟】2 次。
/// 升级：对所有敌人造成 14 点伤害。【齐鸣 4】：【叩钟】2 次。
/// </summary>
public class HundredBellsToll : BellkeeperCard
{
    public HundredBellsToll() : base(2, CardType.Attack, CardRarity.Rare, TargetType.AllEnemies) { }

    protected override IEnumerable<DynamicVar> CanonicalVars => new DynamicVar[]
    {
        new DamageVar(10m, DamageProps.card),
    };

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay play)
    {
        foreach (var e in Owner.Creature.CombatState.Enemies.ToList())
        {
            if (!e.IsDead)
                await CreatureCmd.Damage(choiceContext, e, DynamicVars.Damage.IntValue, DamageProps.card, Owner.Creature, this, null);
        }
        int threshold = IsUpgraded ? 3 : 4;
        if (ChimeGrants.HasResonance(this, threshold))
        {
            await ChimeGrants.Ring(choiceContext, this);
            await ChimeGrants.Ring(choiceContext, this);
        }
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Damage.UpgradeValueBy(3m);
    }
}
