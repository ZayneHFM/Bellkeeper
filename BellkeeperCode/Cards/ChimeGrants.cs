using System.Linq;
using System.Threading.Tasks;
using Bellkeeper.BellkeeperCode.Mechanics;
using Bellkeeper.BellkeeperCode.Powers;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;

namespace Bellkeeper.BellkeeperCode.Cards;

/// <summary>把【余响】挂到自己身上与触发【叩钟】的统一入口。</summary>
public static class ChimeGrants
{
    /// <summary>卡牌效果挂 X 层某种余响/失声。</summary>
    public static Task Grant<T>(PlayerChoiceContext choiceContext, CardModel card, int amount)
        where T : PowerModel
    {
        var owner = card.Owner?.Creature;
        if (owner is null || owner.IsDead || amount == 0) return Task.CompletedTask;
        return GrantFromEffect<T>(choiceContext, owner, amount, card);
    }

    /// <summary>非卡牌效果（能力、遗物等）挂 X 层某种余响/失声。</summary>
    public static Task GrantFromEffect<T>(
        PlayerChoiceContext choiceContext,
        Creature owner,
        int amount,
        CardModel? source = null) where T : PowerModel
    {
        if (owner is null || owner.IsDead || amount == 0) return Task.CompletedTask;
        return PowerCmd.Apply<T>(choiceContext, owner, amount, owner, source, false);
    }

    /// <summary>【叩钟】：清算所有余响。卡面若在叩钟后挂失声，靠 Toll 的快照隔离保证顺序正确。</summary>
    public static Task Ring(PlayerChoiceContext choiceContext, CardModel card, bool keepPositiveChimes = false)
    {
        var owner = card.Owner?.Creature;
        if (owner is null || owner.IsDead) return Task.CompletedTask;
        return Toll.Ring(choiceContext, owner, card, keepPositiveChimes);
    }

    /// <summary>按轴派发给予【余响】或【失声】。</summary>
    public static Task GrantByAxis(PlayerChoiceContext choiceContext, CardModel card, ChimeAxis axis, int amount)
    {
        return axis switch
        {
            ChimeAxis.Guard => Grant<GuardPower>(choiceContext, card, amount),
            ChimeAxis.Peal => Grant<PealPower>(choiceContext, card, amount),
            ChimeAxis.Vanguard => Grant<VanguardPower>(choiceContext, card, amount),
            ChimeAxis.GatherQi => Grant<GatherQiPower>(choiceContext, card, amount),
            ChimeAxis.EchoTune => Grant<EchoTunePower>(choiceContext, card, amount),
            ChimeAxis.Fervor => Grant<FervorPower>(choiceContext, card, amount),
            ChimeAxis.Mute => Grant<MutePower>(choiceContext, card, amount),
            ChimeAxis.Burden => Grant<BurdenPower>(choiceContext, card, amount),
            ChimeAxis.Tremor => Grant<TremorPower>(choiceContext, card, amount),
            ChimeAxis.Offbeat => Grant<OffbeatPower>(choiceContext, card, amount),
            _ => Task.CompletedTask,
        };
    }

    /// <summary>使自身所有【余响】层数翻倍。</summary>
    public static async Task DoubleAllPositive(PlayerChoiceContext choiceContext, CardModel card)
    {
        var owner = card.Owner?.Creature;
        if (owner is null || owner.IsDead) return;

        foreach (var p in owner.GetPowerInstances<ChimePower>().Where(c => c.IsPositive && c.Amount > 0).ToList())
        {
            await GrantByAxis(choiceContext, card, p.Axis, p.Amount);
        }
    }

    /// <summary>【齐鸣 X】：身上同时拥有的不同余响种类数是否达到 X。</summary>
    public static bool HasResonance(CardModel card, int kinds)
    {
        var owner = card.Owner?.Creature;
        if (owner is null) return false;
        int count = 0;
        foreach (var chime in owner.GetPowerInstances<ChimePower>())
        {
            if (chime.Amount > 0) count++;
        }
        return count >= kinds;
    }
}
