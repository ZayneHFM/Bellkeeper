using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BaseLib.Utils;
using Bellkeeper.BellkeeperCode.Mechanics;
using Bellkeeper.BellkeeperCode.Powers;
using Bellkeeper.BellkeeperCode.Powers.CardPowers;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.ValueProps;

namespace Bellkeeper.BellkeeperCode.Cards;

/// <summary>
/// 失聪重击（U36）：造成 16 点伤害并施加 2(3) 层易伤。（未升级【哑音 1】，升级后无副作用）
/// </summary>
public class DeafenedBlow : BellkeeperCard
{
    public DeafenedBlow() : base(2, CardType.Attack, CardRarity.Uncommon, TargetType.AnyEnemy) { }

    protected override IEnumerable<DynamicVar> CanonicalVars => new DynamicVar[]
    {
        new DamageVar(16m, DamageProps.card),
        new IntVar("Vuln", 2m),
        new IntVar("Mute", 1m),
    };

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay play)
    {
        await CommonActions.CardAttack(this, play).Execute(choiceContext);
        if (play.Target is not null && !play.Target.IsDead)
        {
            await PowerCmd.Apply<VulnerablePower>(
                choiceContext, play.Target, DynamicVars["Vuln"].IntValue, Owner.Creature, this, false);
        }
        if (!IsUpgraded)
        {
            await ChimeGrants.Grant<MutePower>(choiceContext, this, DynamicVars["Mute"].IntValue);
        }
    }

    protected override void OnUpgrade()
    {
        DynamicVars["Vuln"].UpgradeValueBy(1m);
    }
}
