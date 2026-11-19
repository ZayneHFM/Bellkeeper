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
/// 追槌（U06）：造成 5 点伤害。每当你【叩钟】，将这张牌从弃牌堆放回手牌。
/// （注：Toll.Ring() 结算末尾可调用 ChasingMallet.OnTollRing(ctx, owner) 把弃牌堆中的所有 ChasingMallet 移回 Hand）
/// </summary>
public class ChasingMallet : BellkeeperCard
{
    public ChasingMallet() : base(0, CardType.Attack, CardRarity.Uncommon, TargetType.AnyEnemy) { }

    protected override IEnumerable<DynamicVar> CanonicalVars => new DynamicVar[]
    {
        new DamageVar(6m, DamageProps.card),
    };

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay play)
    {
        await CommonActions.CardAttack(this, play).Execute(choiceContext);
    }

    public static async Task OnTollRing(PlayerChoiceContext ctx, MegaCrit.Sts2.Core.Entities.Creatures.Creature owner)
    {
        var combatState = owner.Player?.PlayerCombatState;
        if (combatState is null) return;

        var discard = combatState.DiscardPile;
        if (discard is not null)
        {
            var mallets = discard.Cards.OfType<ChasingMallet>().ToList();
            foreach (var m in mallets)
            {
                await CardPileCmd.Add(m, PileType.Hand, CardPilePosition.Top, null, false);
            }
        }

        var drawPile = combatState.DrawPile;
        if (drawPile is not null && drawPile.Cards.Count > 0)
        {
            var bottomMallets = new List<ChasingMallet>();
            for (int i = drawPile.Cards.Count - 1; i >= 0; i--)
            {
                if (drawPile.Cards[i] is ChasingMallet { IsUpgraded: true } bottomMallet)
                {
                    bottomMallets.Add(bottomMallet);
                }
                else
                {
                    break;
                }
            }
            foreach (var bm in bottomMallets)
            {
                await CardPileCmd.Add(bm, PileType.Hand, CardPilePosition.Top, null, false);
            }
        }
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Damage.UpgradeValueBy(2m);
    }
}
