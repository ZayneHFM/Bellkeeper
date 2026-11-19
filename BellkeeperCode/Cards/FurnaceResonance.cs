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
using MegaCrit.Sts2.Core.ValueProps;

namespace Bellkeeper.BellkeeperCode.Cards;

/// <summary>
/// N10《炉火纯青》(FurnaceResonance) [罕见·技能·1费]
/// 基础：移除你所有的【负重】，每移除 1 层就获得 1 层【聚气】和 4 点格挡。
/// 升级：移除你所有的【负重】，每移除 1 层就获得 1 层【聚气】和 6 点格挡。
/// </summary>
public class FurnaceResonance : BellkeeperCard
{
    public FurnaceResonance() : base(1, CardType.Skill, CardRarity.Uncommon, TargetType.Self) { }

    protected override IEnumerable<DynamicVar> CanonicalVars => new DynamicVar[]
    {
        new BlockVar(4m, BlockProps.card),
    };

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay play)
    {
        var burden = Owner.Creature.Powers.OfType<BurdenPower>().FirstOrDefault();
        if (burden != null && burden.Amount > 0)
        {
            int layers = burden.Amount;
            burden.RemoveInternal();
            await ChimeGrants.Grant<GatherQiPower>(choiceContext, this, layers);
            await CreatureCmd.GainBlock(Owner.Creature, layers * DynamicVars.Block.IntValue, BlockProps.card, play, false);
        }
        }

    protected override void OnUpgrade()
    {
        AddKeyword(CardKeyword.Retain);
        DynamicVars.Block.UpgradeValueBy(2m);
    }
}
