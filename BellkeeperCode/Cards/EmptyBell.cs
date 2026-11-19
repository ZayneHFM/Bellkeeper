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
/// 空响（C13，原名：空钟）：0 费。造成 5(6) 点伤害。若手牌为空（升级后不超过 1），【叩钟】并抽 2 张牌。
/// </summary>
public class EmptyBell : BellkeeperCard
{
    public EmptyBell() : base(0, CardType.Attack, CardRarity.Common, TargetType.AnyEnemy) { }

    protected override IEnumerable<DynamicVar> CanonicalVars => new DynamicVar[]
    {
        new DamageVar(5m, DamageProps.card),
        new IntVar("Draw", 2m),
    };

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay play)
    {
        await CommonActions.CardAttack(this, play).Execute(choiceContext);
        var player = Owner.Creature.Player;
        int handThreshold = IsUpgraded ? 1 : 0;
        if (player.PlayerCombatState.Hand.Cards.Count(c => c != this) <= handThreshold)
        {
            await ChimeGrants.Ring(choiceContext, this);
            await CardPileCmd.Draw(choiceContext, DynamicVars["Draw"].IntValue, player, false);
        }
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Damage.UpgradeValueBy(1m);
    }
}
