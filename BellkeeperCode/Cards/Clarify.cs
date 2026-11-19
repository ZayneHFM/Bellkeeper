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
/// 化音为空（U19，原名：化浊为清）：0 费，不消耗。消耗抽牌堆底部的 1 张牌。（升级后：2 张）
/// </summary>
public class Clarify : BellkeeperCard
{
    public Clarify() : base(0, CardType.Skill, CardRarity.Uncommon, TargetType.Self) { }

    protected override IEnumerable<DynamicVar> CanonicalVars => new DynamicVar[]
    {
        new IntVar("Count", 1m),
    };

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay play)
    {
        var player = Owner.Creature.Player;
        var drawPile = player?.PlayerCombatState?.DrawPile;
        if (player is null || drawPile is null || drawPile.Cards.Count == 0) return;

        int count = Math.Min(DynamicVars["Count"].IntValue, drawPile.Cards.Count);
        var bottomCards = drawPile.Cards.TakeLast(count).Reverse().ToList();
        foreach (var card in bottomCards)
        {
            if (card.Pile == drawPile)
            {
                await CardCmd.Exhaust(choiceContext, card);
            }
        }
    }

    protected override void OnUpgrade()
    {
        DynamicVars["Count"].UpgradeValueBy(1m);
    }
}
