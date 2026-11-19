using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Bellkeeper.BellkeeperCode.Cards;
using Bellkeeper.BellkeeperCode.Mechanics;
using Bellkeeper.BellkeeperCode.Powers;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;

namespace Bellkeeper.BellkeeperCode.Relics;

/// <summary>
/// 无舌古钟（稀有遗物）：【哑音】触发时不再使你弃牌，改为每层对所有敌人造成 3 点伤害。
/// </summary>
public class TonguelessBell : BellkeeperRelic
{
    public override RelicRarity Rarity => RelicRarity.Rare;

    public async Task OnMuteTriggeredInsteadOfDiscard(PlayerChoiceContext ctx, Creature owner, int muteLayers)
    {
        if (muteLayers > 0)
        {
            int dmg = muteLayers * 3;
            foreach (var e in owner.CombatState.Enemies.ToList())
            {
                if (e.IsDead) continue;
                await CreatureCmd.Damage(ctx, e, dmg, DamageProps.nonCardUnpowered, owner, null, null);
            }
        }
    }
}
