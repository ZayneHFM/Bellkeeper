using System.Threading.Tasks;
using Bellkeeper.BellkeeperCode.Cards;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;

namespace Bellkeeper.BellkeeperCode.Powers.CardPowers;

/// <summary>
/// U13 / N05《先知先觉》(PresciencePower)：在你的回合开始时，获得 Amount 层【先行】。
/// </summary>
public class PresciencePower : BellkeeperPower
{
    public override async Task AfterPlayerTurnStart(PlayerChoiceContext ctx, Player player)
    {
        if (Owner == player.Creature && Amount > 0)
        {
            await ChimeGrants.GrantFromEffect<VanguardPower>(ctx, Owner, Amount);
        }
    }
}
