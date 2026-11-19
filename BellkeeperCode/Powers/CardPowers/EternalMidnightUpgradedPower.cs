using System.Collections.Generic;
using System.Threading.Tasks;
using Bellkeeper.BellkeeperCode.Cards;
using Bellkeeper.BellkeeperCode.Extensions;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;

namespace Bellkeeper.BellkeeperCode.Powers.CardPowers;

/// <summary>
/// N36《万古长夜+》(EternalMidnightUpgradedPower)：在你的回合结束时，你每有 2 张手牌，触发 Amount 次【先行 1】、【守御 2】。
/// </summary>
public class EternalMidnightUpgradedPower : BellkeeperPower
{
    public override string CustomPackedIconPath => "eternal_midnight.png".PowerImagePath();
    public override string CustomBigIconPath => "eternal_midnight.png".BigPowerImagePath();

    public override async Task BeforeSideTurnEnd(PlayerChoiceContext choiceContext, CombatSide side, IEnumerable<Creature> participants)
    {
        if (side != Owner.Side || Amount <= 0 || Owner.Player?.PlayerCombatState?.Hand == null) return;
        int groups = Owner.Player.PlayerCombatState.Hand.Cards.Count / 2;
        if (groups > 0)
        {
            await ChimeGrants.GrantFromEffect<VanguardPower>(choiceContext, Owner, groups * Amount);
            await ChimeGrants.GrantFromEffect<GuardPower>(choiceContext, Owner, groups * Amount * 2);
        }
    }
}
