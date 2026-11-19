using MegaCrit.Sts2.Core.Entities.Powers;

namespace Bellkeeper.BellkeeperCode.Powers;

/// <summary>
/// 【延音 X】：余响自然触发并即将移除时，消耗 1 层，
/// 使本次触发的余响保留 50% 的层数到下一回合。
///
/// 收敛性：S' = 0.5S + G，稳态 S* = 2G，不会爆炸。
/// 叩钟触发也消耗；被「不移除」改写的触发不消耗。
/// </summary>
public class SustainPower : BellkeeperPower
{
    public override PowerType Type => PowerType.Buff;

    public override PowerStackType StackType => PowerStackType.Counter;
}
