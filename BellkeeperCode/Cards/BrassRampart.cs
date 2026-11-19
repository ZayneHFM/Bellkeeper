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
/// 黄铜壁垒（U11）：获得 10 点格挡。将当前格挡的一半转为【守御】，最多 30 层。
/// </summary>
public class BrassRampart : BellkeeperCard
{
    public BrassRampart() : base(2, CardType.Skill, CardRarity.Uncommon, TargetType.Self) { }

    protected override IEnumerable<DynamicVar> CanonicalVars => new DynamicVar[]
    {
        new BlockVar(12m, BlockProps.card),
        new IntVar("MaxGuard", 30m),
    };

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay play)
    {
        await CommonActions.CardBlock(this, play);
        var owner = Owner.Creature;
        int currentBlock = (int)owner.Block;
        int half = currentBlock / 2;
        int cap = DynamicVars["MaxGuard"].IntValue;
        int toGrant = Math.Min(half, cap);

        if (toGrant > 0)
        {
            await CreatureCmd.GainBlock(owner, -half, BlockProps.nonCardUnpowered, null, false);
            await ChimeGrants.Grant<GuardPower>(choiceContext, this, toGrant);
        }
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Block.UpgradeValueBy(4m);
        DynamicVars["MaxGuard"].UpgradeValueBy(5m);
    }
}
