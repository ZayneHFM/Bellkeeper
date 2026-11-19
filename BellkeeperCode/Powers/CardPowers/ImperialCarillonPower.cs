using System.Threading.Tasks;
using Bellkeeper.BellkeeperCode.Cards;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;

namespace Bellkeeper.BellkeeperCode.Powers.CardPowers;

/// <summary>
/// N30《黄钟大吕》(ImperialCarillonPower)：在你的回合开始时，获得 3 层【钟鸣】、3 层【守御】、1 层【先行】和 1 层【聚气】。
/// </summary>
public class ImperialCarillonPower : BellkeeperPower
{
    public override async Task AfterPlayerTurnStart(PlayerChoiceContext ctx, Player player)
    {
        if (Owner == player.Creature && Amount > 0)
        {
            await ChimeGrants.GrantFromEffect<PealPower>(ctx, Owner, 3 * Amount);
            await ChimeGrants.GrantFromEffect<GuardPower>(ctx, Owner, 3 * Amount);
            await ChimeGrants.GrantFromEffect<VanguardPower>(ctx, Owner, 1 * Amount);
            await ChimeGrants.GrantFromEffect<GatherQiPower>(ctx, Owner, 1 * Amount);
        }
    }
}
