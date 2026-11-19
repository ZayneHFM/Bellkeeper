using System.Threading.Tasks;
using Bellkeeper.BellkeeperCode.Cards;
using Bellkeeper.BellkeeperCode.Mechanics;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;

namespace Bellkeeper.BellkeeperCode.Powers.CardPowers;

/// <summary>
/// 盲眼先知+状态（R22+）：你将从抽牌堆底抽牌，而非抽牌堆顶。
/// </summary>
public class BlindSeerUpgradedPower : BellkeeperPower
{
    public override PowerType Type => PowerType.Buff;

    public override PowerStackType StackType => PowerStackType.Single;

    public override async Task AfterCardDrawn(PlayerChoiceContext choiceContext, CardModel card, bool fromHandDraw)
    {
        if (Owner is null || card.Owner?.Creature != Owner) return;
        if (!BlindSeerDrawPatch.ConsumePendingBottomDraw(card)) return;

        if (card is SunkenMomentum sm)
        {
            sm.OnSunk();
        }
        await PileBottomHelper.NotifyCardDrawnFromBottom(choiceContext, Owner, card);
    }
}
