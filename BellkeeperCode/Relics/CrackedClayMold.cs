using System.Threading.Tasks;
using Bellkeeper.BellkeeperCode.Powers;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;

namespace Bellkeeper.BellkeeperCode.Relics;

/// <summary>
/// 裂口泥范（普通遗物）：每场战斗开始时，获得 4 层【守御】和 4 层【钟鸣】。
/// </summary>
public class CrackedClayMold : BellkeeperRelic
{
    public override RelicRarity Rarity => RelicRarity.Common;

    public override async Task BeforeCombatStart()
    {
        var creature = Owner?.Creature;
        if (creature?.CombatState is null) return;

        Flash();
        var ctx = new ThrowingPlayerChoiceContext();
        await PowerCmd.Apply<GuardPower>(ctx, creature, 4, creature, null, false);
        await PowerCmd.Apply<PealPower>(ctx, creature, 4, creature, null, false);
    }
}
