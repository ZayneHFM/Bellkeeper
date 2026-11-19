using System.Threading.Tasks;
using Bellkeeper.BellkeeperCode.Extensions;
using Bellkeeper.BellkeeperCode.Mechanics;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;

namespace Bellkeeper.BellkeeperCode.Powers.CardPowers;

/// <summary>
/// 拽绳（U35）：每当你洗牌时，【探底 Amount】。
/// </summary>
public class YankTheRopePower : BellkeeperPower
{
    public override string CustomPackedIconPath => "the_watch.png".PowerImagePath();
    public override string CustomBigIconPath => "the_watch.png".BigPowerImagePath();

    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Counter;

    public override async Task AfterShuffle(PlayerChoiceContext choiceContext, Player shuffler)
    {
        if (Owner is null || shuffler.Creature != Owner || Amount <= 0 || Owner.IsDead) return;
        await PileBottomHelper.DredgeBottom(choiceContext, Owner, Amount);
    }
}
