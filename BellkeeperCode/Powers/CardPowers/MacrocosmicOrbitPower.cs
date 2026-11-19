using System.Collections.Generic;
using System.Threading.Tasks;
using Bellkeeper.BellkeeperCode.Cards;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;

namespace Bellkeeper.BellkeeperCode.Powers.CardPowers;

/// <summary>
/// N12《周天运转》(MacrocosmicOrbitPower)：在你的回合结束时，每有 1 点未使用的能量，获得 Amount 层【聚气】。
/// </summary>
public class MacrocosmicOrbitPower : BellkeeperPower
{
    public override async Task BeforeSideTurnEnd(PlayerChoiceContext choiceContext, CombatSide side, IEnumerable<Creature> participants)
    {
        if (side != Owner.Side || Amount <= 0 || Owner.Player?.PlayerCombatState == null) return;
        int unspent = Owner.Player.PlayerCombatState.Energy;
        if (unspent > 0)
        {
            await ChimeGrants.GrantFromEffect<GatherQiPower>(choiceContext, Owner, unspent * Amount);
        }
    }
}
