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
/// 磨损垫肩（罕见遗物）：每场战斗你第 1 次触发【负重】时，少失去 1 点能量，并抽 1 张牌。
/// </summary>
public class FrayedShoulderPad : BellkeeperRelic
{
    public override RelicRarity Rarity => RelicRarity.Uncommon;
    public bool UsedThisCombat { get; set; }

    public override Task BeforeCombatStart()
    {
        UsedThisCombat = false;
        return Task.CompletedTask;
    }
}
