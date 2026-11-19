using BaseLib.Abstracts;
using BaseLib.Utils.NodeFactories;
using Bellkeeper.BellkeeperCode.Cards;
using Bellkeeper.BellkeeperCode.Extensions;
using Bellkeeper.BellkeeperCode.Relics;
using Godot;
using MegaCrit.Sts2.Core.Animation;
using MegaCrit.Sts2.Core.Bindings.MegaSpine;
using MegaCrit.Sts2.Core.Entities.Characters;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Relics;
using MegaCrit.Sts2.Core.Nodes.Combat;

namespace Bellkeeper.BellkeeperCode.Character;

/// <summary>
/// 负钟人角色。
///
/// Spine 接入方式（已按官方教程改正）：
///   1. 项目根 bin/ 放了 spine-godot GDExtension（spine 4.2 / Godot 4.5.1）
///   2. Spine 资源用 .spine-json + .atlas 扩展名，编辑器已导入
///   3. bellkeeper_skel_data.tres 把两者合成 SpineSkeletonDataResource
///   4. scenes/creature_visuals/bellkeeper.tscn 是完整立绘场景
///      节点名按官方规范：Visuals / Bounds / IntentPos / CenterPos
///   5. CustomVisualPath 直接指向 .tscn
/// </summary>
public class Bellkeeper : PlaceholderCharacterModel
{
    public const string CharacterId = "Bellkeeper";

    /// <summary>黄铜金 #E5B842。</summary>
    public static readonly Color Color = new("e5b842");

    /// <summary>地图右键画笔亮黄色 #FFE63B（在暖棕羊皮纸地图上保持高对比度）。</summary>
    public static readonly Color BrightMapYellow = new("ffe63b");

    public override Color NameColor => Color;
    public override Color MapDrawingColor => BrightMapYellow;
    public override CharacterGender Gender => CharacterGender.Neutral;
    public override int StartingHp => 74;
    public override int StartingGold => 99;

    /// <summary>正式初始牌组：打击 ×4、防御 ×4、振击、守夜。</summary>
    public override IEnumerable<CardModel> StartingDeck =>
    [
        ModelDb.Card<Strike>(),
        ModelDb.Card<Strike>(),
        ModelDb.Card<Strike>(),
        ModelDb.Card<Strike>(),
        ModelDb.Card<Defend>(),
        ModelDb.Card<Defend>(),
        ModelDb.Card<Defend>(),
        ModelDb.Card<Defend>(),
        // 拾取先古遗物《古老牙齿》(ArchaicTooth) 时，会将《振击》与《守夜》二合一蜕变为先古卡《子夜》(Midnight)
        ModelDb.Card<RingingBlow>(),
        ModelDb.Card<VigilBell>(),
    ];

    public override IReadOnlyList<RelicModel> StartingRelics =>
    [
        ModelDb.Relic<BrassHandbell>()
    ];

    public override CardPoolModel CardPool => ModelDb.CardPool<BellkeeperCardPool>();
    public override RelicPoolModel RelicPool => ModelDb.RelicPool<BellkeeperRelicPool>();
    public override PotionPoolModel PotionPool => ModelDb.PotionPool<BellkeeperPotionPool>();

    // ---------------------------------------------------------------
    // 角色图标（图片路径）
    // ---------------------------------------------------------------

    public override Control CustomIcon
    {
        get
        {
            var icon = NodeFactory<Control>.CreateFromResource(CustomIconTexturePath);
            icon.SetAnchorsAndOffsetsPreset(Control.LayoutPreset.FullRect);
            return icon;
        }
    }

    public override string CustomIconTexturePath =>
        "character_icon_char_name.png".CharacterUiPath();

    public override string CustomCharacterSelectIconPath =>
        "char_select_char_name.png".CharacterUiPath();

    public override string CustomCharacterSelectLockedIconPath =>
        "char_select_char_name_locked.png".CharacterUiPath();

    public override string CustomMapMarkerPath =>
        "map_marker_char_name.png".CharacterUiPath();

    // ---------------------------------------------------------------
    // 战斗立绘
    // ---------------------------------------------------------------

    /// <summary>
    /// 选人界面大背景。
    ///
    /// 为什么必须写这一行：
    ///   BaseLib 的 PlaceholderCharacterModel.CustomCharacterSelectBg 默认实现是
    ///     "screens/char_select/char_select_bg_" + PlaceholderID
    ///   而 PlaceholderID 的默认实现是直接返回 "ironclad"（IL: ldstr "ironclad"; ret）。
    ///   所以我们不 override 的话，选人界面会一直显示铁甲战士的大背景。
    ///
    /// BaseLib 的 CustomCharacterSelectBg.UseCustomScene 会把这个返回值
    /// 走 SceneHelper.GetScenePath 转成真实场景路径。
    /// </summary>
    public override string CustomCharacterSelectBg => SpineVisualFactory.SelectBgScene;

    /// <summary>指向战斗立绘 Spine 场景。</summary>
    public override string? CustomVisualPath => SpineVisualFactory.BattleVisualScene;

    /// <summary>篝火休息场景（Spine，动画名 idle）。</summary>
    public override string? CustomRestSiteAnimPath =>
        "res://Bellkeeper/scenes/rest_site/bellkeeper_rest_site.tscn";

    /// <summary>商店场景（Spine，动画名 idle）。</summary>
    public override string? CustomMerchantAnimPath =>
        "res://Bellkeeper/scenes/merchant/bellkeeper_merchant.tscn";

    /// <summary>
    /// 备用路径：如果 CustomVisualPath 那条路走不通，
    /// 这里手工实例化同一场景。
    /// </summary>
    public override NCreatureVisuals? CreateCustomVisuals()
    {
        return SpineVisualFactory.TryInstantiateBattleVisuals();
    }

    /// <summary>
    /// 绑定 5 个动画。
    ///
    /// 实测签名 12 参数：
    ///   (sprite, idle, dead, deadLoop, hit, hitLoop,
    ///    attack, attackLoop, cast, castLoop, relaxed, relaxedLoop)
    ///
    /// idle 没有循环参数——待机默认循环。
    /// </summary>
    public override CreatureAnimator? SetupCustomAnimationStates(MegaSprite sprite)
    {
        return SetupAnimationState(
            sprite,
            "idle_loop",          // idle
            "die", false,         // dead
            "hurt", false,        // hit
            "attack", false,      // attack
            "cast", false,        // cast
            "idle_loop", true     // relaxed
        );
    }
}
