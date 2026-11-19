using System.Collections.Generic;
using System.Threading.Tasks;
using Bellkeeper.BellkeeperCode.Powers;
using Bellkeeper.BellkeeperCode.Powers.CardPowers;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;

namespace Bellkeeper.BellkeeperCode.Cards;

/// <summary>
/// U13 / N05《先知先觉》(Prescience)：
/// 在你的回合开始时，【先行 1】。（升级后打出时立刻【先行 1】）
/// </summary>
public class Prescience : BellkeeperCard
{
    public Prescience() : base(1, CardType.Power, CardRarity.Uncommon, TargetType.Self) { }

    protected override IEnumerable<DynamicVar> CanonicalVars => Array.Empty<DynamicVar>();

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay play)
    {
        if (IsUpgraded)
        {
            await ChimeGrants.Grant<VanguardPower>(choiceContext, this, 1);
        }
        await PowerCmd.Apply<PresciencePower>(
            choiceContext, Owner.Creature, 1, Owner.Creature, this, false);
    }
}
