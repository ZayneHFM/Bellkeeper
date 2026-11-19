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
/// N31《十指连心》(TenFingersTactile) [普通·技能·1费]
/// 基础：你每有 1 张手牌，获得 2 点格挡。若手牌数不少于 6，获得 1 层【先行】。
/// 升级：你每有 1 张手牌，获得 2 点格挡。若手牌数不少于 5，获得 1 层【先行】。
/// </summary>
public class TenFingersTactile : BellkeeperCard
{
    public TenFingersTactile() : base(1, CardType.Skill, CardRarity.Common, TargetType.Self) { }

    protected override IEnumerable<DynamicVar> CanonicalVars => new DynamicVar[]
    {
        new BlockVar(2m, BlockProps.card),
        new IntVar("Vanguard", 1m),
    };

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay play)
    {
        int handCount = Owner.Creature.Player.PlayerCombatState.Hand.Cards.Count;
        var blockVar = (BlockVar)DynamicVars.Block;
        for (int i = 0; i < handCount; i++)
        {
            // 按手牌张数分次执行 CardBlock 结算，每张手牌可完整触发 1 次敏捷加成
            await CreatureCmd.GainBlock(Owner.Creature, blockVar, play, fast: i < handCount - 1);
        }

        if (handCount >= 6)
        {
            await ChimeGrants.Grant<VanguardPower>(choiceContext, this, DynamicVars["Vanguard"].IntValue);
        }
    }

    protected override void OnUpgrade()
    {
        DynamicVars["Vanguard"].UpgradeValueBy(1m);
    }
}
