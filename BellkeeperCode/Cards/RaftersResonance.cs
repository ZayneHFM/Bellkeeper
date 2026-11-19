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
/// N27《声声不息》(RaftersResonance) [罕见·攻击·1费]
/// 基础：造成 8 点伤害。你每有 1 层【延音】，获得 1 点能量。
/// 升级：造成 9 点伤害。你每有 1 层【延音】，抽 1 张牌并获得 1 点能量。
/// </summary>
public class RaftersResonance : BellkeeperCard
{
    public RaftersResonance() : base(1, CardType.Attack, CardRarity.Uncommon, TargetType.AnyEnemy) { }

    protected override IEnumerable<DynamicVar> CanonicalVars => new DynamicVar[]
    {
        new DamageVar(8m, DamageProps.card),
    };

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay play)
    {
        await CommonActions.CardAttack(this, play).Execute(choiceContext);

        int sustainAmt = Owner.Creature.Powers.OfType<SustainPower>().FirstOrDefault()?.Amount ?? 0;
        if (sustainAmt > 0)
        {
            if (IsUpgraded)
            {
                await CardPileCmd.Draw(choiceContext, sustainAmt, Owner.Creature.Player, false);
            }
            await PlayerCmd.GainEnergy(sustainAmt, Owner.Creature.Player);
        }
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Damage.UpgradeValueBy(1m);
    }
}
