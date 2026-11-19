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
/// 卸架（U08）：移除你所有的【负重】，每移除 1 层就抽 2 张牌。（升级后每移除 1 层额外获得 3 点格挡）
/// </summary>
public class UnloadTheFrame : BellkeeperCard
{
    public UnloadTheFrame() : base(1, CardType.Skill, CardRarity.Uncommon, TargetType.Self) { }

    protected override IEnumerable<DynamicVar> CanonicalVars => new DynamicVar[]
    {
        new IntVar("Draw", 2m),
    };

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay play)
    {
        var owner = Owner.Creature;
        var burden = owner.Powers.OfType<BurdenPower>().FirstOrDefault();
        if (burden is not null && burden.Amount > 0)
        {
            int layers = burden.Amount;
            burden.RemoveInternal();

            int totalDraw = layers * DynamicVars["Draw"].IntValue;
            await CardPileCmd.Draw(choiceContext, totalDraw, owner.Player, false);
        }
    }

    protected override void OnUpgrade()
    {
        EnergyCost.UpgradeBy(-1);
        AddKeyword(CardKeyword.Retain);
    }
}
