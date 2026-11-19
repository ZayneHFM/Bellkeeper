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
/// 磨平的盲杖（普通遗物）：每场战斗第 1 次【沉底】卡牌时，抽 1 张牌并获得 1 点能量。
/// </summary>
public class WornBlindCane : BellkeeperRelic
{
    public override RelicRarity Rarity => RelicRarity.Common;
    public bool TriggeredThisCombat { get; set; }

    public override Task BeforeCombatStart()
    {
        TriggeredThisCombat = false;
        return Task.CompletedTask;
    }

    public async Task OnFirstSinkInCombat(PlayerChoiceContext ctx, Player player)
    {
        if (!TriggeredThisCombat)
        {
            TriggeredThisCombat = true;
            Flash();
            await CardPileCmd.Draw(ctx, 1, player, false);
            await PlayerCmd.GainEnergy(1, player);
        }
    }
}
