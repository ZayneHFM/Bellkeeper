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
/// 苦役之躯状态（R13）：在你的回合开始时，获得 1 层【负重】、X 点力量和 X 点敏捷。
/// </summary>
public class BeastOfBurdenPower : BellkeeperPower
{
    public override PowerType Type => PowerType.Debuff;

    public override PowerStackType StackType => PowerStackType.Counter;

    public override async Task AfterPlayerTurnStart(PlayerChoiceContext ctx, Player player)
    {
        if (Amount > 0 && Owner == player.Creature)
        {
            await PowerCmd.Apply<BurdenPower>(ctx, Owner, 1, Owner, null, false);
            await PowerCmd.Apply<StrengthPower>(ctx, Owner, Amount, Owner, null, false);
            await PowerCmd.Apply<DexterityPower>(ctx, Owner, Amount, Owner, null, false);
        }
    }
}
