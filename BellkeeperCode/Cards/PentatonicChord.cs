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
/// N25《五音繁会》(PentatonicChord) [罕见·技能·1费]
/// 基础：【钟鸣 3】，【守御 3】，【先行 1】，【聚气 1】。【哑音 1】。
/// 升级：【钟鸣 5】，【守御 5】，【先行 1】，【聚气 1】，【回音 1】。
/// </summary>
public class PentatonicChord : BellkeeperCard
{
    public PentatonicChord() : base(1, CardType.Skill, CardRarity.Uncommon, TargetType.Self) { }

    protected override IEnumerable<DynamicVar> CanonicalVars => new DynamicVar[]
    {
        new IntVar("Chime", 3m),
    };

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay play)
    {
        int amt = DynamicVars["Chime"].IntValue;
        await ChimeGrants.Grant<PealPower>(choiceContext, this, amt);
        await ChimeGrants.Grant<GuardPower>(choiceContext, this, amt);
        await ChimeGrants.Grant<VanguardPower>(choiceContext, this, 1);
        await ChimeGrants.Grant<GatherQiPower>(choiceContext, this, 1);
        if (IsUpgraded)
        {
            await ChimeGrants.Grant<EchoTunePower>(choiceContext, this, 1);
        }
        else
        {
            await ChimeGrants.Grant<MutePower>(choiceContext, this, 1);
        }
    }

    protected override void OnUpgrade()
    {
        DynamicVars["Chime"].UpgradeValueBy(2m);
    }
}
