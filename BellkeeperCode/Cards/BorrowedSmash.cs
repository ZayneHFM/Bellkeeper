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
/// 赊击（C19）：造成 15 点伤害。获得 1 层【哑音】。（升级后 19 点）
///
/// 类名说明：
///   原版铁甲战士有一张《BorrowedTime》，占用了那个类名。
///   C17《借时》因此占用 BorrowedStrike，C19《赊击》只能用 BorrowedSmash。
///
///   但美术交付的卡面叫 borrowed_strike.png，
///   所以这里显式覆写图片路径，避免基类按类名去找 borrowed_smash.png 而落空。
/// </summary>
public class BorrowedSmash : BellkeeperCard
{
    public BorrowedSmash() : base(1, CardType.Attack, CardRarity.Common, TargetType.AnyEnemy) { }

    /// <summary>卡面文件名对齐美术交付：borrowed_strike.png。</summary>
    public override string CustomPortraitPath => "borrowed_strike.png".BigCardImagePath();

    /// <summary>小图同样对齐。</summary>
    public override string PortraitPath => "borrowed_strike.png".CardImagePath();

    protected override IEnumerable<DynamicVar> CanonicalVars => new DynamicVar[]
    {
        new DamageVar(15m, DamageProps.card),
        new IntVar("Mute", 1m),
    };

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay play)
    {
        await CommonActions.CardAttack(this, play).Execute(choiceContext);
        await ChimeGrants.Grant<MutePower>(choiceContext, this, DynamicVars["Mute"].IntValue);
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Damage.UpgradeValueBy(4m);
    }
}
