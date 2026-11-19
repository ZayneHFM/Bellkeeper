using System.Linq;
using System.Threading.Tasks;
using Bellkeeper.BellkeeperCode.Mechanics;
using Bellkeeper.BellkeeperCode.Relics;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;

namespace Bellkeeper.BellkeeperCode.Powers;

/// <summary>
/// 【哑音 X】：在你的下个回合抽完牌后，选择并弃置 X 张手牌。
/// 若被【叩钟】提前触发，立即选择并弃置 X 张手牌。
/// 挂在 AfterPlayerTurnStartLate：此时抽牌已完成。
/// </summary>
public class MutePower : ChimePower
{
    public override ChimeAxis Axis => ChimeAxis.Mute;

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

        var tongueless = owner.Player?.Relics.OfType<TonguelessBell>().FirstOrDefault();
        if (tongueless is not null)
        {
            tongueless.Flash();
            await tongueless.OnMuteTriggeredInsteadOfDiscard(choiceContext, owner, effectiveLayers);
            return;
        }

        await Toll.DiscardFromHand(choiceContext, owner, effectiveLayers, cardSource ?? (AbstractModel)this);
    }
}
