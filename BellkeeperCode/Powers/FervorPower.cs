using System.Threading.Tasks;
using Bellkeeper.BellkeeperCode.Mechanics;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Powers;

namespace Bellkeeper.BellkeeperCode.Powers;

/// <summary>
/// 【振奋 X】：正面余响。在你的回合开始或【叩钟】时，每层获得 1 点力量和 1 点敏捷，然后移除。
/// </summary>
public class FervorPower : ChimePower
{
    public override ChimeAxis Axis => ChimeAxis.Fervor;

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
        await PowerCmd.Apply<StrengthPower>(choiceContext, owner, effectiveLayers, owner, cardSource, false);
        await PowerCmd.Apply<DexterityPower>(choiceContext, owner, effectiveLayers, owner, cardSource, false);
    }
}
