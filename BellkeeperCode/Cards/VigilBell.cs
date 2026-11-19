using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BaseLib.Abstracts;
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
/// 守夜（B04）：【守御 7(10)】，【延音 1(2)】。
/// 拾取先古遗物《古老牙齿》时可与《振击》二合一蜕变为《子夜》。
/// </summary>
public class VigilBell : BellkeeperCard, ITranscendenceCard
{
    public VigilBell() : base(1, CardType.Skill, CardRarity.Basic, TargetType.Self) { }

    public CardModel GetTranscendenceTransformedCard() => ModelDb.Card<Midnight>();

    protected override IEnumerable<DynamicVar> CanonicalVars => new DynamicVar[]
    {
        new IntVar("Guard", 7m),
        new IntVar("Sustain", 1m),
    };

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay play)
    {
        await ChimeGrants.Grant<GuardPower>(choiceContext, this, DynamicVars["Guard"].IntValue);
        await ChimeGrants.Grant<SustainPower>(choiceContext, this, DynamicVars["Sustain"].IntValue);
    }

    protected override void OnUpgrade()
    {
        DynamicVars["Guard"].UpgradeValueBy(3m);
        DynamicVars["Sustain"].UpgradeValueBy(1m);
    }
}
