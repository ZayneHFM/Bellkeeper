using System.Collections.Generic;
using System.Threading.Tasks;
using BaseLib.Utils;
using Bellkeeper.BellkeeperCode.Extensions;
using Bellkeeper.BellkeeperCode.Powers;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;

namespace Bellkeeper.BellkeeperCode.Cards;

/// <summary>
/// 借时（C17）：0 费，造成 10 点伤害。获得 1 层【负重】。（升级 14 点）
/// （注：原版有 BorrowedTime，故 C17 占用类名 BorrowedStrike，卡面与文案均对应《借时》）
/// </summary>
public class BorrowedStrike : BellkeeperCard
{
    public BorrowedStrike() : base(0, CardType.Attack, CardRarity.Common, TargetType.AnyEnemy) { }

    public override string CustomPortraitPath => "borrowed_time.png".BigCardImagePath();
    public override string PortraitPath => "borrowed_time.png".CardImagePath();
    public override string BetaPortraitPath => "beta/borrowed_time.png".CardImagePath();

    protected override IEnumerable<DynamicVar> CanonicalVars => new DynamicVar[]
    {
        new DamageVar(10m, DamageProps.card),
        new IntVar("Burden", 1m),
    };

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay play)
    {
        await CommonActions.CardAttack(this, play).Execute(choiceContext);
        await ChimeGrants.Grant<BurdenPower>(choiceContext, this, DynamicVars["Burden"].IntValue);
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Damage.UpgradeValueBy(4m);
    }
}
