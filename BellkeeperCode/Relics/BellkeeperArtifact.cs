using MegaCrit.Sts2.Core.Entities.Relics;

namespace Bellkeeper.BellkeeperCode.Relics;

/// <summary>
/// 负钟人专属遗物基类。
/// 图标按 类名下划线小写 加载，与 BellkeeperRelic 的规则一致。
/// </summary>
public abstract class BellkeeperArtifact : BellkeeperRelic
{
    public abstract override RelicRarity Rarity { get; }
}
