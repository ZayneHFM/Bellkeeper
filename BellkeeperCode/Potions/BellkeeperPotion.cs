using BaseLib.Abstracts;
using BaseLib.Utils;
using Bellkeeper.BellkeeperCode.Character;
using Bellkeeper.BellkeeperCode.Extensions;
using MegaCrit.Sts2.Core.Entities.Potions;

namespace Bellkeeper.BellkeeperCode.Potions;

/// <summary>
/// 负钟人所有药水的基类。
///
/// 图标命名：
///   按类名推会得到 potionguard.png，
///   但美术交付的是 potion_guard.png（带下划线），
///   所以子类覆写 ImageStem 指定文件名。
/// </summary>
[Pool(typeof(BellkeeperPotionPool))]
public abstract class BellkeeperPotion : CustomPotionModel
{
    /// <summary>图标文件名（不含扩展名）。子类覆写。</summary>
    protected abstract string ImageStem { get; }

    public override string? CustomPackedImagePath =>
        $"{ImageStem}.png".PotionImagePath();

    public override string? CustomPackedOutlinePath =>
        $"{ImageStem}.png".PotionOutlineImagePath();
}
