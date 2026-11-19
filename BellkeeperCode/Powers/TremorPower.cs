using System.Threading.Tasks;
using Bellkeeper.BellkeeperCode.Mechanics;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;

namespace Bellkeeper.BellkeeperCode.Powers;

/// <summary>
/// 【惊悸 X】：负面失声。在你的回合开始或【叩钟】时，每层受到 2 点伤害（可被格挡抵消），然后移除。
/// </summary>
public class TremorPower : ChimePower
{
    private const int DamagePerStack = 2;

    public override ChimeAxis Axis => ChimeAxis.Tremor;

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
        await CreatureCmd.Damage(
            choiceContext, owner, effectiveLayers * DamagePerStack,
            DamageProps.nonCardUnpowered, owner, null, null);
    }
}
