using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BaseLib.Utils;
using Bellkeeper.BellkeeperCode.Powers;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;

namespace Bellkeeper.BellkeeperCode.Cards;

/// <summary>
/// U32 / N04《闻风而动》(WindborneStep)：
/// 造成 7(9) 点伤害，【先行 1(2)】。【齐鸣 2】：重放 1。
/// </summary>
public class WindborneStep : BellkeeperCard
{
    public WindborneStep() : base(1, CardType.Attack, CardRarity.Uncommon, TargetType.AnyEnemy) { }

    protected override IEnumerable<DynamicVar> CanonicalVars => new DynamicVar[]
    {
        new DamageVar(7m, DamageProps.card),
        new IntVar("Vanguard", 1m),
    };

    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
        base.ExtraHoverTips.Append(HoverTipFactory.Static(StaticHoverTip.ReplayStatic));

    public override int ModifyCardPlayCount(CardModel card, Creature? target, int playCount)
    {
        if (!ReferenceEquals(card, this) || Owner?.Creature is null) return playCount;

        var chimes = Owner.Creature.GetPowerInstances<ChimePower>()
            .Where(p => p.IsPositive && p.Amount > 0)
            .ToList();
        int kinds = chimes.Count;
        // 本牌自身提供【先行】，若尚未拥有【先行】则计入其齐鸣种数
        if (!chimes.Any(p => p is VanguardPower)) kinds += 1;

        return kinds >= 2 ? playCount + 1 : playCount;
    }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay play)
    {
        await CommonActions.CardAttack(this, play).Execute(choiceContext);
        await ChimeGrants.Grant<VanguardPower>(choiceContext, this, DynamicVars["Vanguard"].IntValue);
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Damage.UpgradeValueBy(2m);
        DynamicVars["Vanguard"].UpgradeValueBy(1m);
    }
}
