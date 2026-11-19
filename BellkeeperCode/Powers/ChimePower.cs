using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;

namespace Bellkeeper.BellkeeperCode.Powers;

/// <summary>余响家族的 10 条轴。</summary>
public enum ChimeAxis
{
    // ---- 正面【余响】 ----
    /// <summary>【守御】：下回合开始获得 X 点格挡。</summary>
    Guard,

    /// <summary>【钟鸣】：下回合开始对所有敌人造成 X 点伤害。</summary>
    Peal,

    /// <summary>【先行】：下回合开始抽 X 张牌。</summary>
    Vanguard,

    /// <summary>【聚气】：下回合开始回气后获得 X 点能量。</summary>
    GatherQi,

    /// <summary>【回音】：下回合开始【摸底 X】。</summary>
    EchoTune,

    /// <summary>【振奋】：下回合开始本回合获得 X 点力量和 X 点敏捷。</summary>
    Fervor,

    // ---- 负面【失声】 ----
    /// <summary>【哑音】：下回合抽完牌后弃置 X 张手牌。</summary>
    Mute,

    /// <summary>【负重】：下回合开始回气后失去 X 点能量。</summary>
    Burden,

    /// <summary>【惊悸】：下回合开始受到 2X 点伤害。</summary>
    Tremor,

    /// <summary>【乱拍】：下回合开始将 X 张手牌置于抽牌堆顶。</summary>
    Offbeat,
}

/// <summary>
/// 【余响】基类。十条状态共用同一条底层规则：
/// 「在你的下个回合开始时触发，触发后自我移除」。
/// 自然触发与【叩钟】触发走同一套 ExecuteEffect 实现，只是触发时点不同。
///
/// 极性：
///   正面 = 守御 / 钟鸣 / 先行 / 聚气 / 回音 / 振奋
///   负面 = 哑音 / 负重 / 惊悸 / 乱拍（统称【失声】）
/// </summary>
public abstract class ChimePower : BellkeeperPower
{
    public abstract ChimeAxis Axis { get; }

    /// <summary>正极性状态会被【延音】保留一半；负极性一律清零。</summary>
    public bool IsPositive => Axis is
        ChimeAxis.Guard or ChimeAxis.Peal or ChimeAxis.Vanguard or
        ChimeAxis.GatherQi or ChimeAxis.EchoTune or ChimeAxis.Fervor;

    public override PowerType Type => IsPositive ? PowerType.Buff : PowerType.Debuff;

    public override PowerStackType StackType => PowerStackType.Counter;

    /// <summary>
    /// 执行该状态的本体效果，由自然触发与【叩钟】共同调用。
    /// </summary>
    public abstract Task ExecuteEffect(
        PlayerChoiceContext choiceContext,
        Creature owner,
        int effectiveLayers,
        CardModel? cardSource = null);
}
