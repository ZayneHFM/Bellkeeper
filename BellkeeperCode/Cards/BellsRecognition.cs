using System.Collections.Generic;
using System.Threading.Tasks;
using Bellkeeper.BellkeeperCode.Powers;
using Godot;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.CardPools;
using MegaCrit.Sts2.Core.Saves.Runs;

namespace Bellkeeper.BellkeeperCode.Cards;

/// <summary>
/// 铃铛的认可（负钟人专属特殊诅咒牌）：
/// 拾取先古遗物《召唤铃铛》时替换原版《铃铛的诅咒》。
/// 不能被打出。永恒。每当你抽到此牌时，【钟鸣 1】，并使此牌本局的【钟鸣】永久增加 1。
/// 隶属 BellkeeperCardPool 以展示于负钟人图鉴（归钟旁），同时完全排除出日常奖励/商店/变化/生成卡池。
/// </summary>
public class BellsRecognition : BellkeeperCard
{
    private const string CursePortraitPath = "res://images/packed/card_portraits/curse/curse_of_the_bell.png";
    private const string CurseFrameMaterialPath = "res://materials/cards/frames/card_frame_curse_mat.tres";
    private const string CurseBannerMaterialPath = "res://materials/cards/banners/card_banner_curse_mat.tres";

    private int _currentPeal = 1;
    private int _increasedPeal;

    public BellsRecognition() : base(-1, CardType.Curse, CardRarity.Curse, TargetType.None) { }

    public override int MaxUpgradeLevel => 0;
    public override bool CanBeGeneratedByModifiers => false;
    public override bool CanBeGeneratedInCombat => false;

    public override CardPoolModel VisualCardPool => ModelDb.CardPool<CurseCardPool>();

    public override string PortraitPath => CursePortraitPath;
    public override string CustomPortraitPath => CursePortraitPath;
    public override string BetaPortraitPath => CursePortraitPath;
    public override IEnumerable<string> AllPortraitPaths => new[] { CursePortraitPath };

    public override Material? CreateCustomFrameMaterial =>
        ResourceLoader.Exists(CurseFrameMaterialPath) ? GD.Load<Material>(CurseFrameMaterialPath) : null;

    public override Material? CreateCustomBannerMaterial =>
        ResourceLoader.Exists(CurseBannerMaterialPath) ? GD.Load<Material>(CurseBannerMaterialPath) : null;

    public override string CustomBannerMaterialPath => CurseBannerMaterialPath;

    public override IEnumerable<CardKeyword> CanonicalKeywords => new[]
    {
        CardKeyword.Unplayable,
        CardKeyword.Eternal,
    };

    [SavedProperty]
    public int CurrentPeal
    {
        get => _currentPeal;
        set
        {
            AssertMutable();
            _currentPeal = value;
            DynamicVars["Peal"].BaseValue = _currentPeal;
        }
    }

    [SavedProperty]
    public int IncreasedPeal
    {
        get => _increasedPeal;
        set
        {
            AssertMutable();
            _increasedPeal = value;
        }
    }

    protected override IEnumerable<DynamicVar> CanonicalVars => new DynamicVar[]
    {
        new IntVar("Peal", CurrentPeal),
        new IntVar("Increase", 1m),
    };

    public override async Task AfterCardDrawn(PlayerChoiceContext choiceContext, CardModel card, bool fromHandDraw)
    {
        if (card != this || Owner?.Creature is not { IsDead: false }) return;

        int pealAmount = DynamicVars["Peal"].IntValue;
        int inc = DynamicVars["Increase"].IntValue;

        if (pealAmount > 0)
        {
            await ChimeGrants.Grant<PealPower>(choiceContext, this, pealAmount);
        }

        if (inc > 0)
        {
            BuffFromDraw(inc);
            if (DeckVersion is BellsRecognition deckVersion)
            {
                deckVersion.BuffFromDraw(inc);
            }
        }
    }

    public void BuffFromDraw(int extraPeal)
    {
        IncreasedPeal += extraPeal;
        CurrentPeal = 1 + IncreasedPeal;
    }
}
