using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Bellkeeper.BellkeeperCode.Cards;
using Bellkeeper.BellkeeperCode.Mechanics;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.ValueProps;

namespace Bellkeeper.BellkeeperCode.Powers.CardPowers;

/// <summary>
/// 大钟师状态（R03）：在你的回合开始时，获得 X 层【延音】。
/// </summary>
public class TheGreatCarillonneurPower : BellkeeperPower
{
    public override PowerType Type => PowerType.Buff;

    public override PowerStackType StackType => PowerStackType.Counter;

    public override async Task AfterPlayerTurnStart(PlayerChoiceContext ctx, Player player)
    {
        if (Amount > 0 && Owner == player.Creature)
        {
            await PowerCmd.Apply<SustainPower>(ctx, Owner, Amount, Owner, null, false);
        }
    }
}
