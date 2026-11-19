using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Bellkeeper.BellkeeperCode.Cards;
using Bellkeeper.BellkeeperCode.Extensions;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.ValueProps;

namespace Bellkeeper.BellkeeperCode.Powers.CardPowers;

/// <summary>
/// 苦行共鸣+（U34+）：在你的回合结束时，你每有 1 种【失声】，获得 Amount 点格挡，【先行 1】，【守御 3】。
/// </summary>
public class AsceticResonanceUpgradedPower : BellkeeperPower
{
    public override string CustomPackedIconPath => "ascetic_resonance.png".PowerImagePath();
    public override string CustomBigIconPath => "ascetic_resonance.png".BigPowerImagePath();

    public override async Task BeforeSideTurnEnd(
        PlayerChoiceContext choiceContext,
        CombatSide side,
        IEnumerable<Creature> participants)
    {
        if (Owner is null || side != Owner.Side || Amount <= 0) return;
        int dissonanceTypes = Owner.Powers.OfType<ChimePower>().Count(p => !p.IsPositive && p.Amount > 0);
        if (dissonanceTypes > 0)
        {
            await CreatureCmd.GainBlock(Owner, dissonanceTypes * Amount, BlockProps.nonCardUnpowered, null, false);
            await ChimeGrants.GrantFromEffect<VanguardPower>(choiceContext, Owner, dissonanceTypes);
            await ChimeGrants.GrantFromEffect<GuardPower>(choiceContext, Owner, dissonanceTypes * 3);
        }
    }
}
