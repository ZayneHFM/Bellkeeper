using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Bellkeeper.BellkeeperCode.Powers;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Helpers;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Rooms;
using MegaCrit.Sts2.Core.Saves.Runs;

namespace Bellkeeper.BellkeeperCode.Relics;

/// <summary>
/// 百年钟摆（稀有遗物）：严格对齐原版《开心小花》的跨战斗 3 回合计数逻辑，
/// 每经过 3 个回合的回合开始时，获得 1 层【延音】。
/// </summary>
public class CentennialPendulum : BellkeeperRelic
{
    private const int TurnsThreshold = 3;
    private const string TurnsKey = "Turns";

    private bool _isActivating;
    private int _turnsSeen;

    public override RelicRarity Rarity => RelicRarity.Rare;

    public override bool ShowCounter => true;

    public override int DisplayAmount => IsActivating ? DynamicVars[TurnsKey].IntValue : TurnsSeen;

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new DynamicVar(TurnsKey, TurnsThreshold),
    ];

    private bool IsActivating
    {
        get => _isActivating;
        set
        {
            AssertMutable();
            _isActivating = value;
            InvokeDisplayAmountChanged();
        }
    }

    [SavedProperty]
    public int TurnsSeen
    {
        get => _turnsSeen;
        set
        {
            AssertMutable();
            _turnsSeen = value;
            InvokeDisplayAmountChanged();
        }
    }

    public override async Task AfterSideTurnStart(
        CombatSide side,
        IReadOnlyList<Creature> participants,
        ICombatState combatState)
    {
        if (Owner?.Creature is not { } creature || !participants.Contains(creature))
        {
            return;
        }

        int threshold = DynamicVars[TurnsKey].IntValue;
        TurnsSeen = (TurnsSeen + 1) % threshold;
        Status = TurnsSeen == threshold - 1 ? RelicStatus.Active : RelicStatus.Normal;

        if (TurnsSeen == 0)
        {
            _ = TaskHelper.RunSafely(DoActivateVisuals());
            await PowerCmd.Apply<SustainPower>(
                new ThrowingPlayerChoiceContext(), creature, 1, creature, null, false);
        }
    }

    private async Task DoActivateVisuals()
    {
        IsActivating = true;
        Flash();
        await Cmd.Wait(1f);
        IsActivating = false;
    }

    public override Task AfterCombatEnd(CombatRoom _)
    {
        Status = RelicStatus.Normal;
        return Task.CompletedTask;
    }
}
