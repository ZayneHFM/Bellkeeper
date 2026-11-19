using System.Threading.Tasks;
using Bellkeeper.BellkeeperCode.Cards;
using Bellkeeper.BellkeeperCode.Mechanics;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;

namespace Bellkeeper.BellkeeperCode.Powers.CardPowers;

/// <summary>
/// N15《念念不忘》(LingeringObsessionPower)：每当你【沉底】卡牌，获得 Amount 层【回音】。
/// </summary>
public class LingeringObsessionPower : BellkeeperPower, ICardSunkListener
{
    public async Task OnCardSunk(PlayerChoiceContext ctx, Creature owner, CardModel card)
    {
        if (Owner == owner && Amount > 0)
        {
            await ChimeGrants.GrantFromEffect<EchoTunePower>(ctx, owner, Amount);
        }
    }
}
