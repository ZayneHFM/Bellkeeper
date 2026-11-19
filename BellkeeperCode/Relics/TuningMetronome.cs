using System.Threading.Tasks;
using Bellkeeper.BellkeeperCode.Powers;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;

namespace Bellkeeper.BellkeeperCode.Relics;

/// <summary>
/// 校音节拍器（罕见）：回合结束时，手牌数为偶数给 3 层【守御】，奇数给 3 层【钟鸣】。
/// 钩子必须是 override Task BeforeSideTurnEnd(PlayerChoiceContext, CombatSide, IEnumerable&lt;Creature&gt;)。
/// </summary>
public class TuningMetronome : BellkeeperRelic
{
    public override RelicRarity Rarity => RelicRarity.Uncommon;

    public override async Task BeforeSideTurnEnd(
        PlayerChoiceContext choiceContext,
        MegaCrit.Sts2.Core.Combat.CombatSide side,
        System.Collections.Generic.IEnumerable<MegaCrit.Sts2.Core.Entities.Creatures.Creature> participants)
    {
        var creature = Owner?.Creature;
        var player = Owner;
        if (creature is null || player is null) return;

        // 只在自己这一侧结束时触发
        if (side != MegaCrit.Sts2.Core.Combat.CombatSide.Player) return;

        int handCount = player.PlayerCombatState?.Hand.Cards.Count ?? 0;

        Flash();
        if (handCount % 2 == 0)
        {
            await PowerCmd.Apply<GuardPower>(choiceContext, creature, 3, creature, null, false);
        }
        else
        {
            await PowerCmd.Apply<PealPower>(choiceContext, creature, 3, creature, null, false);
        }
    }
}
