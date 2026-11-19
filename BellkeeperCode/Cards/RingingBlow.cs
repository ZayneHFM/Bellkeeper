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
/// 振击（B03）：造成 4(6) 点伤害，【钟鸣 5(6)】，【先行 1(2)】。
/// 拾取先古遗物《古老牙齿》时可与《守夜》二合一蜕变为《子夜》。
/// </summary>
public class RingingBlow : BellkeeperCard, ITranscendenceCard
{
    public RingingBlow() : base(1, CardType.Attack, CardRarity.Basic, TargetType.AnyEnemy) { }

    public CardModel GetTranscendenceTransformedCard() => ModelDb.Card<Midnight>();

    protected override IEnumerable<DynamicVar> CanonicalVars => new DynamicVar[]
    {
        new DamageVar(4m, DamageProps.card),
        new IntVar("Chime", 5m),
        new IntVar("Vanguard", 1m),
    };

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay play)
    {
        await CommonActions.CardAttack(this, play).Execute(choiceContext);
        await ChimeGrants.Grant<PealPower>(choiceContext, this, DynamicVars["Chime"].IntValue);
        await ChimeGrants.Grant<VanguardPower>(choiceContext, this, DynamicVars["Vanguard"].IntValue);
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Damage.UpgradeValueBy(2m);
        DynamicVars["Chime"].UpgradeValueBy(1m);
        DynamicVars["Vanguard"].UpgradeValueBy(1m);
    }
}
