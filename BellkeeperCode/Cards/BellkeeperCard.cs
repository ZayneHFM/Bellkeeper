using System.Collections.Generic;
using BaseLib.Abstracts;
using Godot;
using BaseLib.Extensions;
using BaseLib.Utils;
using Bellkeeper.BellkeeperCode.Character;
using Bellkeeper.BellkeeperCode.Extensions;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Models;

namespace Bellkeeper.BellkeeperCode.Cards;

/// <summary>
/// 负钟人所有卡牌的基类。
///
/// 除了卡面图片路径，还统一挂「自定义名词的浮窗解释」。
///
/// 关于浮窗的关键教训：
///   LocString.KeyPathToLocString 的实现是 String.Split('/')，
///   第 1 段 = 本地化表名，第 2 段 = 表内键名。
///   所以必须写 "card_keywords/BELLKEEPER-TOLL.title"，不能写 "BELLKEEPER-TOLL.title"。
///   写错的后果不是报错，而是 Split 只得到 1 段、【1】越界抛异常，
///   如果我们再 catch 掉，就变成「浮窗静默消失」。
/// </summary>
[Pool(typeof(BellkeeperCardPool))]
public abstract class BellkeeperCard(int cost, CardType type, CardRarity rarity, TargetType target) :
    CustomCardModel(cost, type, rarity, target)
{
    // 卡面大图
    public override string CustomPortraitPath =>
        $"{Id.Entry.RemovePrefix().ToLowerInvariant()}.png".BigCardImagePath();

    // 卡面小图
    public override string PortraitPath =>
        $"{Id.Entry.RemovePrefix().ToLowerInvariant()}.png".CardImagePath();

    public override string BetaPortraitPath =>
        $"beta/{Id.Entry.RemovePrefix().ToLowerInvariant()}.png".CardImagePath();

    // ---------------------------------------------------------------
    // 卡框材质（P-01）
    // ---------------------------------------------------------------

    /// <summary>守钟人卡框材质资源路径。</summary>
    private const string FrameMaterialPath =
        "res://Bellkeeper/materials/card_frame_bellkeeper_mat.tres";

    /// <summary>
    /// 只换材质、不换贴图。
    ///
    /// BaseLib 的 CustomCardFrameMaterial.UseAltMaterial 会把这里的返回值
    /// 直接塞给原版 card.tscn 的 Frame 节点当 material，
    /// 而 Frame 的 texture 仍然是原版 card_frame_attack_s / skill_s / power_s
    /// （那些图自带多层浮雕），所以我们既保留了原版立体感，又换成了守钟人的黄。
    /// </summary>
    public override Material? CreateCustomFrameMaterial
    {
        get
        {
            if (!ResourceLoader.Exists(FrameMaterialPath)) return null;
            return GD.Load<Material>(FrameMaterialPath);
        }
    }

    // ---------------------------------------------------------------
    // 关键词浮窗
    // ---------------------------------------------------------------

    /// <summary>
    /// 文案里的标记 → card_keywords.json 里的键前缀。
    /// 【齐鸣 2】【齐鸣 3】 都会命中 "【齐鸣"。
    ///
    /// 顺序有意义：先匹配长标记，避免「【余响】」抢先命中「【余响：守御】」。
    /// </summary>
    /// <summary>
    /// 文案里的标记 → card_keywords.json 里的键前缀。
    ///
    /// 顺序：子类在前，大类在后（避免【守御】被【余响】抢先命中）。
    /// 一律用前缀匹配（不带右括号），因为 v11 语法是【钟鸣 5】这种带数字的写法。
    /// </summary>
    private static readonly (string Marker, string Key)[] KeywordMarkers =
    [
        // ---- 正面【余响】六条 ----
        ("【守御", "BELLKEEPER-GUARD"),
        ("[gold]Guard", "BELLKEEPER-GUARD"),
        ("【钟鸣", "BELLKEEPER-PEAL"),
        ("[gold]Peal", "BELLKEEPER-PEAL"),
        ("【先行", "BELLKEEPER-VANGUARD"),
        ("[gold]Vanguard", "BELLKEEPER-VANGUARD"),
        ("【聚气", "BELLKEEPER-GATHER_QI"),
        ("[gold]Gather Qi", "BELLKEEPER-GATHER_QI"),
        ("【回音", "BELLKEEPER-ECHO_TUNE"),
        ("[gold]Echo Tune", "BELLKEEPER-ECHO_TUNE"),
        ("【振奋", "BELLKEEPER-FERVOR"),
        ("[gold]Fervor", "BELLKEEPER-FERVOR"),
        // ---- 负面【失声】四条 ----
        ("【哑音", "BELLKEEPER-MUTE"),
        ("[gold]Mute", "BELLKEEPER-MUTE"),
        ("【负重", "BELLKEEPER-BURDEN"),
        ("[gold]Burden", "BELLKEEPER-BURDEN"),
        ("【惊悸", "BELLKEEPER-TREMOR"),
        ("[gold]Tremor", "BELLKEEPER-TREMOR"),
        ("【乱拍", "BELLKEEPER-OFFBEAT"),
        ("[gold]Offbeat", "BELLKEEPER-OFFBEAT"),
        // ---- 两个大类 ----
        ("【余响", "BELLKEEPER-CHIME"),
        ("[gold]Chime", "BELLKEEPER-CHIME"),
        ("【失声", "BELLKEEPER-DISSONANCE"),
        ("[gold]Dissonance", "BELLKEEPER-DISSONANCE"),
        // ---- 机制词 ----
        ("【叩钟", "BELLKEEPER-TOLL"),
        ("[gold]Toll", "BELLKEEPER-TOLL"),
        ("【延音", "BELLKEEPER-SUSTAIN"),
        ("[gold]Sustain", "BELLKEEPER-SUSTAIN"),
        ("【齐鸣", "BELLKEEPER-RESONANCE"),
        ("[gold]Harmonic", "BELLKEEPER-RESONANCE"),
        ("【沉底", "BELLKEEPER-SINK"),
        ("[gold]Sink", "BELLKEEPER-SINK"),
        ("[gold]Sunk", "BELLKEEPER-SINK"),
        ("【探底", "BELLKEEPER-DREDGE"),
        ("[gold]Dredge", "BELLKEEPER-DREDGE"),
        ("【摸底", "BELLKEEPER-DRAW_BOTTOM"),
        ("[gold]Bottom-Draw", "BELLKEEPER-DRAW_BOTTOM"),
    ];

    protected override IEnumerable<IHoverTip> ExtraHoverTips
    {
        get
        {
            string text = SafeDescription();
            if (string.IsNullOrEmpty(text))
            {
                return [];
            }

            var tips = new List<IHoverTip>();
            var seen = new HashSet<string>();
            var covered = new List<string>();

            foreach (var (marker, key) in KeywordMarkers)
            {
                if (!text.Contains(marker)) continue;
                if (!seen.Add(key)) continue;

                // 子树已经覆盖了大类时不再重复加（例如有【守御】就不再单加【余响】）
                if (key is "BELLKEEPER-CHIME" or "BELLKEEPER-LOST_VOICE"
                    && covered.Exists(k => k.StartsWith(key is "BELLKEEPER-CHIME"
                        ? "BELLKEEPER-GUARD" : "BELLKEEPER-MUTE")))
                {
                    // 大类仍然要显示（余响本身是机制说明），这里不跳过，只是避免重复
                }

                var tip = MakeTip(key);
                if (tip is not null)
                {
                    tips.Add(tip);
                    covered.Add(key);
                }
            }

            return tips;
        }
    }

    /// <summary>
    /// 用已验证的 "表名/键名" 格式取本地化文本。
    /// 取不到就返回 null，绝不再静默 catch 掉。
    /// </summary>
    private static IHoverTip? MakeTip(string key)
    {
        try
        {
            var title = LocString.KeyPathToLocString($"card_keywords/{key}.title");
            var desc = LocString.KeyPathToLocString($"card_keywords/{key}.description");

            if (!title.Exists() || !desc.Exists())
            {
                MainFile.Logger.Error($"关键词浮窗缺失: {key}（检查 card_keywords.json）");
                return null;
            }

            return new HoverTip(title, desc, (Godot.Texture2D?)null);
        }
        catch (System.Exception e)
        {
            MainFile.Logger.Error($"关键词浮窗 {key} 构造失败: {e}");
            return null;
        }
    }

    /// <summary>
    /// 取这张卡的文案，只用于扫关键词。
    ///
    /// 注意：不要用 Description.GetFormattedText()。
    /// 那个会走 SmartFormat，而 SmartFormat 需要 DynamicVars 变量表；
    /// 在卡牌还没进战斗（图鉴、卡池列表、奖励界面）时变量表是空的，
    /// 就会刷出 "No source extension could handle the selector named Damage"。
    /// 我们要的只是「文案里出现了哪些关键词标记」，用原始文本就够了。
    /// </summary>
    private string SafeDescription()
    {
        try
        {
            var d = Description;
            return d?.GetRawText() ?? string.Empty;
        }
        catch
        {
            return string.Empty;
        }
    }
}
