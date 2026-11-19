using System.Threading.Tasks;
using Bellkeeper.BellkeeperCode.Mechanics;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;

namespace Bellkeeper.BellkeeperCode.Powers;

/// <summary>
/// 【乱拍 X】：负面失声。在你的下个回合抽完牌后（或被【叩钟】时），选择 X 张手牌置于抽牌堆顶，然后移除。
/// 挂在 AfterPlayerTurnStartLate：此时回合初抽牌与【先行】均已完成。
/// </summary>
public class OffbeatPower : ChimePower
{
    public override ChimeAxis Axis => ChimeAxis.Offbeat;

    public override Task AfterPlayerTurnStartLate(PlayerChoiceContext choiceContext, Player player)
    {
        if (player.Creature != Owner) return Task.CompletedTask;
        return Toll.TriggerNatural(choiceContext, this);
    }

    public override async Task ExecuteEffect(
        PlayerChoiceContext choiceContext,
        Creature owner,
        int effectiveLayers,
        CardModel? cardSource = null)
    {
        if (effectiveLayers <= 0 || owner.IsDead) return;
        await Toll.PutHandCardsOnTop(choiceContext, owner, effectiveLayers, cardSource ?? (AbstractModel)this);
    }
}
