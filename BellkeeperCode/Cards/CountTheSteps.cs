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
/// 拾级（C10）：抽 2 张牌。若手牌数为偶数，再抽 1 张牌。
/// </summary>
public class CountTheSteps : BellkeeperCard
{
    public CountTheSteps() : base(1, CardType.Skill, CardRarity.Common, TargetType.Self) { }

    protected override IEnumerable<DynamicVar> CanonicalVars => new DynamicVar[]
    {
        new IntVar("Cards", 2m),
        new IntVar("Extra", 1m),
    };

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay play)
    {
        var player = Owner.Creature.Player;
        await CardPileCmd.Draw(choiceContext, DynamicVars["Cards"].IntValue, player, false);

        int handCount = player.PlayerCombatState.Hand.Cards.Count;
        if (handCount % 2 == 0)
        {
            await CardPileCmd.Draw(choiceContext, DynamicVars["Extra"].IntValue, player, false);
        }
    }

    protected override void OnUpgrade()
    {
        DynamicVars["Extra"].UpgradeValueBy(1m);
    }
}
