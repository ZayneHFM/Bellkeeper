using System.Threading.Tasks;
using Bellkeeper.BellkeeperCode.Cards;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.Models;

namespace Bellkeeper.BellkeeperCode.Relics;

/// <summary>
/// 永鸣大钟（初始升级遗物）：仅通过先古遗物《奥罗巴斯之触》替换《黄铜手摇铃》获得。
/// 每场战斗开始时，将 1 张《恒钟》置入你的手牌。
/// </summary>
public class EverTollingBell : BellkeeperRelic
{
    public override RelicRarity Rarity => RelicRarity.Starter;

    public override async Task BeforeCombatStart()
    {
        var creature = Owner?.Creature;
        var state = creature?.CombatState;
        if (creature is null || state is null) return;

        Flash();

        var card = state.CreateCard<EternalBell>(Owner!);
        await CardPileCmd.AddGeneratedCardToCombat(card, PileType.Hand, Owner!, CardPilePosition.Top);
    }
}
