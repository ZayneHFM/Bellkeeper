using System.Linq;
using System.Threading.Tasks;
using Bellkeeper.BellkeeperCode.Mechanics;
using Bellkeeper.BellkeeperCode.Powers.CardPowers;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.ValueProps;

namespace Bellkeeper.BellkeeperCode.Powers;

/// <summary>
/// 【钟鸣 X】：在你的下个回合开始时或【叩钟】时，对所有敌人造成 X 点伤害。
/// 战钟。余响。
/// </summary>
public class PealPower : ChimePower
{
    public override ChimeAxis Axis => ChimeAxis.Peal;

    public override Task AfterPlayerTurnStart(PlayerChoiceContext choiceContext, Player player)
    {
        if (player.Creature != Owner) return Task.CompletedTask;
        if (Owner.Powers.OfType<TimeStandstillPower>().Any(p => p.Amount > 0))
            return Task.CompletedTask;
        return Toll.TriggerNatural(choiceContext, this);
    }

    public override async Task ExecuteEffect(
        PlayerChoiceContext choiceContext,
        Creature owner,
        int effectiveLayers,
        CardModel? cardSource = null)
    {
        if (effectiveLayers <= 0 || owner.IsDead) return;
        var state = owner.CombatState;
        if (state is null) return;

        foreach (var silence in owner.Powers.OfType<TheLoudestSilencePower>().ToList())
        {
            await silence.OnPealTriggered(owner, effectiveLayers);
        }

        bool affectedByVulnerable = owner.Powers.OfType<DeafeningPower>().Any(p => p.Amount > 0);
        foreach (var enemy in state.Enemies.ToList())
        {
            if (enemy.IsDead) continue;
            int dmg = effectiveLayers;
            if (affectedByVulnerable && enemy.Powers.OfType<VulnerablePower>().Any(v => v.Amount > 0))
            {
                dmg = (dmg * 3) / 2;
            }
            // 传入 cardSource=null 以便 DeafeningPower.AfterDamageGiven 识别为非卡牌【钟鸣】AOE
            await CreatureCmd.Damage(choiceContext, enemy, dmg, DamageProps.nonCardUnpowered, owner, null, null);
        }
    }
}
