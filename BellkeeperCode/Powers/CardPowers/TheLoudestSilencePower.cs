using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.ValueProps;

namespace Bellkeeper.BellkeeperCode.Powers.CardPowers;

/// <summary>
/// 大音希声（R24）：【钟鸣】触发时，同时获得等同于其层数 X% 的格挡（50 / 75）。
/// 由 PealPower.ExecuteEffect 直接调用 OnPealTriggered，确保自然触发与【叩钟】均稳定生效。
/// </summary>
public class TheLoudestSilencePower : BellkeeperPower
{
    public override PowerType Type => PowerType.Buff;

    public override PowerStackType StackType => PowerStackType.Counter;

    public async Task OnPealTriggered(Creature owner, int pealLayers)
    {
        if (Owner is null || Owner != owner || Amount <= 0 || pealLayers <= 0) return;

        int block = (pealLayers * Amount) / 100;
        if (block <= 0) return;

        await CreatureCmd.GainBlock(Owner, block, BlockProps.nonCardUnpowered, null, false);
    }
}
