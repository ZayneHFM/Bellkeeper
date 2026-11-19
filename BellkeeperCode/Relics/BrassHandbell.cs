using System.Threading.Tasks;
using Bellkeeper.BellkeeperCode.Cards;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.Models;

namespace Bellkeeper.BellkeeperCode.Relics;

/// <summary>
/// R01 黄铜手摇铃（初始遗物）：每场战斗开始时，将 1 张《晨钟》置入你的手牌。
///
/// 两个易错点：
///   1. 必须是 override Task BeforeCombatStart()，写成 public Task OnCombatStart(...)
///      引擎永远不会调。
///   2. 战斗里造牌必须走 CombatState.CreateCard&lt;T&gt;(owner)，
///      new MorningBell() 造出来的牌没在战斗状态机里注册，加进手牌也打不出去。
/// </summary>
public class BrassHandbell : BellkeeperRelic
{
    public override RelicRarity Rarity => RelicRarity.Starter;

    public override RelicModel? GetUpgradeReplacement() => ModelDb.Relic<EverTollingBell>();

    public override async Task BeforeCombatStart()
    {
        var creature = Owner?.Creature;
        var state = creature?.CombatState;
        if (creature is null || state is null) return;

        Flash();

        var card = state.CreateCard<MorningBell>(Owner!);
        await CardPileCmd.AddGeneratedCardToCombat(card, PileType.Hand, Owner!, CardPilePosition.Top);
    }
}
