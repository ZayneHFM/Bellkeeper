using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BaseLib.Utils;
using Bellkeeper.BellkeeperCode.Powers;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.ValueProps;

namespace Bellkeeper.BellkeeperCode.Cards;

/// <summary>
/// N11《气冲霄汉》(SurgeOfQi) [稀有·攻击·X费]
/// 基础：【聚气 X】。你每有 1 层【聚气】，造成 9 点伤害。
/// 升级：【聚气 X+1】。你每有 1 层【聚气】，造成 11 点伤害。
/// </summary>
public class SurgeOfQi : BellkeeperCard
{
    public SurgeOfQi() : base(0, CardType.Attack, CardRarity.Rare, TargetType.AnyEnemy) { }

    protected override bool HasEnergyCostX => true;

    protected override IEnumerable<DynamicVar> CanonicalVars => new DynamicVar[]
    {
        new DamageVar(9m, DamageProps.card),
    };

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay play)
    {
        int qiGain = ResolveEnergyXValue();
        if (IsUpgraded)
        {
            qiGain++;
        }

        if (qiGain > 0)
        {
            await ChimeGrants.Grant<GatherQiPower>(choiceContext, this, qiGain);
        }

        int layers = Owner.Creature.Powers.OfType<GatherQiPower>().FirstOrDefault()?.Amount ?? 0;
        if (layers > 0 && play.Target is not null && !play.Target.IsDead)
        {
            await CommonActions.CardAttack(this, play, hitCount: layers).Execute(choiceContext);
        }
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Damage.UpgradeValueBy(2m);
    }
}
