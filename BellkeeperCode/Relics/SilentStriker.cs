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
/// 寂静撞针（罕见遗物）：每当你【叩钟】时，每移除 1 种【余响】，获得 3 点格挡。
/// </summary>
public class SilentStriker : BellkeeperRelic
{
    public override RelicRarity Rarity => RelicRarity.Uncommon;

    public async Task OnTollRemovedTypes(Creature owner, int removedDistinctTypes)
    {
        if (removedDistinctTypes > 0)
        {
            await CreatureCmd.GainBlock(owner, removedDistinctTypes * 3, BlockProps.nonCardUnpowered, null, false);
        }
    }
}
