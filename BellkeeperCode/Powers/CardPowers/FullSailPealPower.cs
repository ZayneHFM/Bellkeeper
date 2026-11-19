using System;
using BaseLib.Hooks;
using Bellkeeper.BellkeeperCode.Extensions;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Powers;

namespace Bellkeeper.BellkeeperCode.Powers.CardPowers;

/// <summary>
/// 千帆竞发状态（R18）：你的手牌上限本场战斗变为 12。
/// </summary>
public class FullSailPealPower : BellkeeperPower, IMaxHandSizeModifier
{
    public override string CustomPackedIconPath => "vanguard.png".PowerImagePath();
    public override string CustomBigIconPath => "vanguard.png".BigPowerImagePath();

    public override PowerType Type => PowerType.Buff;

    public override PowerStackType StackType => PowerStackType.Single;

    public int ModifyMaxHandSize(Player player, int currentMaxHandSize)
    {
        return Owner is not null && player.Creature == Owner
            ? Math.Max(currentMaxHandSize, 12)
            : currentMaxHandSize;
    }
}
