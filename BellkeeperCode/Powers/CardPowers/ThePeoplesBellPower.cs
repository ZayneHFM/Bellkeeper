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
/// 万壑回响状态（U30，原名：万人钟）：每当你从抽牌堆底抽牌或【沉底】卡牌，获得 X 点格挡。
/// </summary>
public class ThePeoplesBellPower : BellkeeperPower, ICardSunkListener, IDrawnFromBottomListener
{

    public override PowerType Type => PowerType.Buff;

    public override PowerStackType StackType => PowerStackType.Counter;

    public async Task OnCardSunk(PlayerChoiceContext ctx, Creature owner, CardModel card)
    {
        if (Amount > 0 && Owner == owner)
        {
            await CreatureCmd.GainBlock(owner, Amount, BlockProps.nonCardUnpowered, null, false);
        }
    }

    public async Task OnCardDrawnFromBottom(PlayerChoiceContext ctx, Creature owner, CardModel card)
    {
        if (Amount > 0 && Owner == owner)
        {
            await CreatureCmd.GainBlock(owner, Amount, BlockProps.nonCardUnpowered, null, false);
        }
    }
}
