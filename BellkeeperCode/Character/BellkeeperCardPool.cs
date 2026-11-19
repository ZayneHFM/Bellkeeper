using System.Collections.Generic;
using BaseLib.Abstracts;
using Bellkeeper.BellkeeperCode.Extensions;
using Godot;
using MegaCrit.Sts2.Core.Entities.Cards;

namespace Bellkeeper.BellkeeperCode.Character;

/// <summary>
/// 负钟人的卡池。
///
/// 卡框方案（P-01，已按原版机制重做）：
///   原版卡框 = 「一张带多层明暗浮雕的暖红底图」× 「hsv.gdshader 染色」。
///   底图在 images/atlases/ui_atlas_0.png 里，三种牌型各一块 region：
///     attack Rect2(1320, 83, 598, 844)
///     skill  Rect2(1221, 929, 598, 844)
///     power  Rect2(621, 929, 598, 844)
///
///   所以我们**不再提供自己画的 frame.png**（那会丢掉浮雕），
///   而是让引擎继续用原版贴图，只在 material 层把色相换成守钟人的黄。
///   材质参数见 BaseLib 的 H / S / V。
/// </summary>
public class BellkeeperCardPool : CustomCardPoolModel
{
    public override string Title => Bellkeeper.CharacterId;

    public override string BigEnergyIconPath => "charui/big_energy.png".ImagePath();
    public override string TextEnergyIconPath => "charui/text_energy.png".ImagePath();

    /// <summary>
    /// 守钟人的「黄衣之主」黄：h=0.148 把色相推到 50.5°（硫磺黄），
    /// s=1.35 取饱满，v=1.32 提亮受光面同时保留原版凹槽阴影。
    /// </summary>
    public override float H => 0.148f;
    public override float S => 1.35f;
    public override float V => 1.32f;

    /// <summary>
    /// 故意返回 null。
    ///
    /// BaseLib 的 CustomCardFrame.UseAltTexture 逻辑是：
    ///   1) 卡自己 override 了 CustomFrame 且非 null -> 用卡自己的
    ///   2) 否则如果卡池是 CustomCardPoolModel -> 用 pool.CustomFrame(card)
    ///   3) 仍然是 null -> 保持引擎原本的贴图
    ///
    /// 返回 null 就是走第 3 条，保住原版带浮雕的卡框底图。
    /// </summary>
    public override Texture2D? CustomFrame(CustomCardModel card) => null;

    //Color of small card icons —— 黄铜金
    public override Color DeckEntryCardColor => new("#C0A500");

    public override bool IsColorless => false;

    /// <summary>
    /// 图鉴全解锁（P-02）。
    /// BaseLib 的 CustomCardPoolMarkAsSeenPatch 只处理 SeenByDefault == true 的卡池，
    /// 而 CustomCardPoolModel.SeenByDefault 默认 false（IL: ldc.i4.0; ret）。
    /// </summary>
    public override bool SeenByDefault => true;
}
