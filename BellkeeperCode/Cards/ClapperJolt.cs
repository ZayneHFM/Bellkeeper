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
/// 铜舌震击（C16）：【沉底 1】。造成 8(11) 点伤害，施加 1(2) 层虚弱。
/// </summary>
public class ClapperJolt : BellkeeperCard
{
    public ClapperJolt() : base(1, CardType.Attack, CardRarity.Common, TargetType.AnyEnemy) { }

    protected override IEnumerable<DynamicVar> CanonicalVars => new DynamicVar[]
    {
        new DamageVar(8m, DamageProps.card),
        new IntVar("Weak", 1m),
    };

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay play)
    {
        await PileBottomHelper.SelectAndSinkFromHand(choiceContext, this, Owner.Creature, 0, 1);
        await CommonActions.CardAttack(this, play).Execute(choiceContext);
        if (play.Target is not null && !play.Target.IsDead)
        {
            await PowerCmd.Apply<WeakPower>(
                choiceContext, play.Target, DynamicVars["Weak"].IntValue, Owner.Creature, this, false);
        }
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Damage.UpgradeValueBy(3m);
        DynamicVars["Weak"].UpgradeValueBy(1m);
    }
}
