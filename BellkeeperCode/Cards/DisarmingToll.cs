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
/// 拆械（U23）：移除目标的所有格挡，造成与移除值相同（升级两倍）的伤害，最多 25(50) 点。
/// </summary>
public class DisarmingToll : BellkeeperCard
{
    public DisarmingToll() : base(1, CardType.Attack, CardRarity.Uncommon, TargetType.AnyEnemy) { }

    protected override IEnumerable<DynamicVar> CanonicalVars => new DynamicVar[]
    {
        new IntVar("MaxDmg", 25m),
    };

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay play)
    {
        var target = play.Target;
        if (target is null || target.IsDead) return;

        int targetBlock = (int)target.Block;
        if (targetBlock > 0)
        {
            await CreatureCmd.GainBlock(target, -targetBlock, BlockProps.nonCardUnpowered, null, false);
            int raw = IsUpgraded ? targetBlock * 2 : targetBlock;
            int dmg = Math.Min(raw, DynamicVars["MaxDmg"].IntValue);
            await CreatureCmd.Damage(choiceContext, target, dmg, DamageProps.card, Owner.Creature, this, play);
        }
    }

    protected override void OnUpgrade()
    {
        DynamicVars["MaxDmg"].UpgradeValueBy(25m);
    }
}
