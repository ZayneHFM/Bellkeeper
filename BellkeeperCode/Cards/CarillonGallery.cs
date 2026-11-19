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
/// 回音长廊（R05，原名：编钟回廊）：每回合你第一次【叩钟】时，正面【余响】额外触发 50% 的层数。
/// </summary>
public class CarillonGallery : BellkeeperCard
{
    public CarillonGallery() : base(1, CardType.Power, CardRarity.Rare, TargetType.Self) { }

    protected override IEnumerable<DynamicVar> CanonicalVars => new DynamicVar[]
    {
        new IntVar("Pct", 50m),
    };

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay play)
    {
        await PowerCmd.Apply<CarillonGalleryPower>(
            choiceContext, Owner.Creature, DynamicVars["Pct"].IntValue, Owner.Creature, this, false);
        if (IsUpgraded)
        {
            await ChimeGrants.Grant<SustainPower>(choiceContext, this, 1);
        }
    }

    protected override void OnUpgrade()
    {
        DynamicVars["Pct"].UpgradeValueBy(25m);
    }
}
