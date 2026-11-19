using BaseLib.Abstracts;
using BaseLib.Extensions;
using Bellkeeper.BellkeeperCode.Extensions;
using MegaCrit.Sts2.Core.Entities.Powers;

namespace Bellkeeper.BellkeeperCode.Powers;

/// <summary>
/// 负钟人所有状态（Power）的基类。
///
/// 图标路径规则：
///   类名去掉 "Power" 后缀 → 下划线小写 → .png
///
///   例：GuardPower       → GUARD_POWER → 去后缀 GUARD → guard.png
///       BurdenForgePower → BURDEN_FORGE_POWER → BURDEN_FORGE → burden_forge.png
///
///   这样美术只要按「状态名」给图，不用关心类名后缀。
/// </summary>
public abstract class BellkeeperPower : CustomPowerModel
{
    public override string CustomPackedIconPath =>
        $"{IconFileStem}.png".PowerImagePath();

    public override string CustomBigIconPath =>
        $"{IconFileStem}.png".BigPowerImagePath();

    /// <summary>
    /// 从 Id.Entry 推出图标文件名（不含扩展名）。
    /// BELLKEEPER-GUARD_POWER → GUARD_POWER → guard
    /// </summary>
    private string IconFileStem
    {
        get
        {
            var entry = Id.Entry.RemovePrefix().ToLowerInvariant();
            const string suffix = "_power";
            if (entry.EndsWith(suffix) && entry.Length > suffix.Length)
            {
                entry = entry[..^suffix.Length];
            }
            return entry;
        }
    }

    /// <summary>
    /// 增益还是减益。默认增益；减益状态自行 override 成 Debuff。
    /// 给默认值是为了让后加的能力牌不必每个都写一遍。
    /// </summary>
    public override PowerType Type => PowerType.Buff;

    /// <summary>
    /// 重复施加时如何叠加。默认 Counter（加层数）。
    /// </summary>
    public override PowerStackType StackType => PowerStackType.Counter;

    /// <summary>
    /// 显式开启战斗钩子分发。默认应为 true，这里写明是为了以后排查方便。
    /// </summary>
    public override bool ShouldReceiveCombatHooks => true;
}
