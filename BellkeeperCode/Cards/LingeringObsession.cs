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
/// N15《念念不忘》(LingeringObsession) [稀有·能力·1费]
/// 基础：每当你【沉底】卡牌，获得 1 层【回音】。
/// 升级：【固有】。每当你【沉底】卡牌，获得 1 层【回音】。
/// </summary>
public class LingeringObsession : BellkeeperCard
{
    public LingeringObsession() : base(1, CardType.Power, CardRarity.Rare, TargetType.Self) { }

    protected override IEnumerable<DynamicVar> CanonicalVars => Array.Empty<DynamicVar>();

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay play)
    {
        await PowerCmd.Apply<LingeringObsessionPower>(choiceContext, Owner.Creature, 1, Owner.Creature, this, false);
        }

    protected override void OnUpgrade()
    {
        AddKeyword(CardKeyword.Innate);
    }
}
