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
/// 走音狂想（U21）：每当你获得【哑音】或【负重】，抽 1 张牌并对所有敌人造成 4 点伤害。
/// </summary>
public class DissonantRhapsody : BellkeeperCard
{
    public DissonantRhapsody() : base(1, CardType.Power, CardRarity.Rare, TargetType.Self) { }

    protected override IEnumerable<DynamicVar> CanonicalVars => new DynamicVar[]
    {
        new IntVar("Damage", 5m),
    };

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay play)
    {
        if (IsUpgraded)
        {
            await PowerCmd.Apply<DissonantRhapsodyUpgradedPower>(
                choiceContext, Owner.Creature, DynamicVars["Damage"].IntValue, Owner.Creature, this, false);
        }
        else
        {
            await PowerCmd.Apply<DissonantRhapsodyPower>(
                choiceContext, Owner.Creature, DynamicVars["Damage"].IntValue, Owner.Creature, this, false);
        }
    }

    protected override void OnUpgrade()
    {
        DynamicVars["Damage"].UpgradeValueBy(2m);
    }
}
