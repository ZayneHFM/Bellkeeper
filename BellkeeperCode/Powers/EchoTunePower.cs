using System.Threading.Tasks;
using Bellkeeper.BellkeeperCode.Mechanics;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;

namespace Bellkeeper.BellkeeperCode.Powers;

/// <summary>
/// 【回音 X】：正面余响。在你的回合开始或【叩钟】时，每层【摸底 1】并使其本回合费用减 1，然后移除。
/// </summary>
public class EchoTunePower : ChimePower
{
    public override ChimeAxis Axis => ChimeAxis.EchoTune;

    public override Task AfterPlayerTurnStart(PlayerChoiceContext choiceContext, Player player)
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
        var drawn = await PileBottomHelper.DrawFromBottom(choiceContext, owner, effectiveLayers);
        foreach (var card in drawn)
        {
            PileBottomHelper.ReduceCostForTurn(card, 1);
        }
    }
}
