using BaseLib.Patches.Content;
using MegaCrit.Sts2.Core.Entities.Cards;

namespace Bellkeeper.BellkeeperCode;

/// <summary>
/// 负钟人的五个自定义关键词。
///
/// 正确写法（BaseLib 官方文档）：
///   用 [CustomEnum] 标注 public static CardKeyword 字段，
///   BaseLib 在运行时自动给它分配一个新的枚举值。
///   再加 [KeywordProperties] 让它自动出现在卡面文字上。
///
/// 关键点：
///   1. 字段必须 public static，不能 readonly（否则赋值会失败）
///   2. 引用时用「类名.字段名」，不要用 CardKeyword.XXX
///   3. 本地化 ID 是 MODPREFIX-KEYWORD_NAME，放在 card_keywords.json
///      我们的是 BELLKEEPER-CHIME / BELLKEEPER-TOLL 等
///
/// 与卡牌的关系：
///   卡牌在 CanonicalKeywords 里写 BellkeeperKeywords.Toll 等，
///   卡面文字里就不用再写「【叩钟】」了，会自动加上并可悬停查看。
/// </summary>
public static class BellkeeperKeywords
{
    /// <summary>【余响】：守御 / 钟鸣 / 哑音 / 负重 四者的统称。</summary>
    [CustomEnum("CHIME"), KeywordProperties(AutoKeywordPosition.After)]
    public static CardKeyword Chime;

    /// <summary>【叩钟】：立即触发并移除所有余响。</summary>
    [CustomEnum("TOLL"), KeywordProperties(AutoKeywordPosition.After)]
    public static CardKeyword Toll;

    /// <summary>【延音】：余响自然触发后保留一半层数。</summary>
    [CustomEnum("SUSTAIN"), KeywordProperties(AutoKeywordPosition.After)]
    public static CardKeyword Sustain;

    /// <summary>【齐鸣】：同时拥有 N 种不同余响时触发额外效果。</summary>
    [CustomEnum("RESONANCE"), KeywordProperties(AutoKeywordPosition.After)]
    public static CardKeyword Resonance;

    /// <summary>【沉底】：将手牌置于抽牌堆底。</summary>
    [CustomEnum("SINK"), KeywordProperties(AutoKeywordPosition.After)]
    public static CardKeyword Sink;
}
