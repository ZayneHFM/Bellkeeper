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
/// 步履沉重状态（U16）：在你的回合开始时，移除 1 层【负重】并获得 X 点格挡。
/// </summary>
public class HeavyTreadPower : BellkeeperPower
{
    public override PowerType Type => PowerType.Buff;

    public override PowerStackType StackType => PowerStackType.Counter;

    public override async Task AfterPlayerTurnStart(PlayerChoiceContext ctx, Player player)
    {
        if (Amount > 0 && Owner == player.Creature)
        {
            var burden = Owner.Powers.OfType<BurdenPower>().FirstOrDefault();
            if (burden is not null && burden.Amount > 0)
            {
                int rem = burden.Amount - 1;
                burden.RemoveInternal();
                if (rem > 0)
                {
                    await PowerCmd.Apply<BurdenPower>(ctx, Owner, rem, Owner, null, false);
                }
            }
            await CreatureCmd.GainBlock(Owner, Amount, BlockProps.nonCardUnpowered, null, false);
        }
    }
}
