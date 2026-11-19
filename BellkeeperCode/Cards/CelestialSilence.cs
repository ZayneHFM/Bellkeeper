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
/// 天籁绝音（A02）：触发你的所有正面【余响】3 次（不移除层数）。【消耗】（升级后 4 次）
/// </summary>
public class CelestialSilence : BellkeeperCard
{
    public CelestialSilence() : base(2, CardType.Skill, CardRarity.Ancient, TargetType.Self) { }

    protected override IEnumerable<DynamicVar> CanonicalVars => new DynamicVar[]
    {
        new IntVar("Times", 3m),
    };

    public override IEnumerable<CardKeyword> CanonicalKeywords => new[] { CardKeyword.Exhaust };

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay play)
    {
        var owner = Owner.Creature;
        int times = DynamicVars["Times"].IntValue;
        int guardAmt = owner.Powers.OfType<GuardPower>().FirstOrDefault()?.Amount ?? 0;
        int pealAmt = owner.Powers.OfType<PealPower>().FirstOrDefault()?.Amount ?? 0;

        for (int i = 0; i < times; i++)
        {
            if (guardAmt > 0)
            {
                await CreatureCmd.GainBlock(owner, guardAmt, BlockProps.nonCardUnpowered, null, false);
            }
            if (pealAmt > 0)
            {
                foreach (var e in owner.CombatState.Enemies.ToList())
                {
                    if (e.IsDead) continue;
                    await CreatureCmd.Damage(choiceContext, e, pealAmt, DamageProps.nonCardUnpowered, owner, this, play);
                }
            }
        }
    }

    protected override void OnUpgrade()
    {
        DynamicVars["Times"].UpgradeValueBy(1m);
    }
}
