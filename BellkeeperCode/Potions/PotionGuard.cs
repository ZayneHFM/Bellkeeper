using Bellkeeper.BellkeeperCode.Mechanics;
using Bellkeeper.BellkeeperCode.Powers;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Potions;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;

namespace Bellkeeper.BellkeeperCode.Potions;

/// <summary>
/// 补钟水：立刻获得 10 层【守御】。
/// 图标：images/potions/potion_guard.png
/// </summary>
public class PotionGuard : BellkeeperPotion
{
    protected override string ImageStem => "potion_guard";

    public override PotionRarity Rarity => PotionRarity.Common;
    public override PotionUsage Usage => PotionUsage.CombatOnly;
    public override TargetType TargetType => TargetType.Self;

    protected override async Task OnUse(PlayerChoiceContext ctx, Creature? target)
    {
        var owner = Owner?.Creature;
        if (owner is null) return;

        await PowerCmd.Apply<GuardPower>(ctx, owner, 10, owner, null, false);
    }
}

/// <summary>
/// 哑药：移除你身上所有的【哑音】和【负重】。
/// 图标：images/potions/potion_purge.png
/// </summary>
public class PotionPurge : BellkeeperPotion
{
    protected override string ImageStem => "potion_purge";

    public override PotionRarity Rarity => PotionRarity.Uncommon;
    public override PotionUsage Usage => PotionUsage.CombatOnly;
    public override TargetType TargetType => TargetType.Self;

    protected override Task OnUse(PlayerChoiceContext ctx, Creature? target)
    {
        var owner = Owner?.Creature;
        if (owner is null) return Task.CompletedTask;

        owner.GetPower<MutePower>()?.RemoveInternal();
        owner.GetPower<BurdenPower>()?.RemoveInternal();

        return Task.CompletedTask;
    }
}

/// <summary>
/// 加速铃：立刻【叩钟】一次。
/// 图标：images/potions/potion_haste.png
/// </summary>
public class PotionHaste : BellkeeperPotion
{
    protected override string ImageStem => "potion_haste";

    public override PotionRarity Rarity => PotionRarity.Rare;
    public override PotionUsage Usage => PotionUsage.CombatOnly;
    public override TargetType TargetType => TargetType.Self;

    protected override async Task OnUse(PlayerChoiceContext ctx, Creature? target)
    {
        var owner = Owner?.Creature;
        if (owner is null) return;

        await Toll.Ring(ctx, owner);
    }
}
