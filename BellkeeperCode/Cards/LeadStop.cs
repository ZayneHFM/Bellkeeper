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
/// 塞铅止音（U20）：消耗手牌中所有的状态牌，每消耗 1 张就获得 6 点格挡和 4 层【守御】。
/// </summary>
public class LeadStop : BellkeeperCard
{
    public LeadStop() : base(1, CardType.Skill, CardRarity.Uncommon, TargetType.Self) { }

    protected override IEnumerable<DynamicVar> CanonicalVars => new DynamicVar[]
    {
        new BlockVar(6m, BlockProps.card),
        new IntVar("Guard", 4m),
    };

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay play)
    {
        var owner = Owner.Creature;
        var hand = owner.Player.PlayerCombatState.Hand;
        var statusCards = hand.Cards.Where(c => c.Type == CardType.Status).ToList();

        foreach (var sc in statusCards)
        {
            await CardCmd.Exhaust(choiceContext, sc);
        }

        int count = statusCards.Count;
        if (count > 0)
        {
            for (int i = 0; i < count; i++)
            {
                await CommonActions.CardBlock(this, play);
            }
            await ChimeGrants.Grant<GuardPower>(choiceContext, this, count * DynamicVars["Guard"].IntValue);
        }
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Block.UpgradeValueBy(2m);
        DynamicVars["Guard"].UpgradeValueBy(2m);
    }
}
