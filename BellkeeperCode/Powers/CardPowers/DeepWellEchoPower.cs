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
/// 深井回音状态（U14）：每当你【沉底】1 张牌，获得 X 层【守御】和 X 层【钟鸣】。
/// </summary>
public class DeepWellEchoPower : BellkeeperPower, ICardSunkListener
{

    public override PowerType Type => PowerType.Buff;

    public override PowerStackType StackType => PowerStackType.Counter;

    public async Task OnCardSunk(PlayerChoiceContext ctx, Creature owner, CardModel card)
    {
        if (Amount > 0 && Owner == owner)
        {
            await PowerCmd.Apply<GuardPower>(ctx, owner, Amount, owner, null, false);
            await PowerCmd.Apply<PealPower>(ctx, owner, Amount, owner, null, false);
        }
    }
}
