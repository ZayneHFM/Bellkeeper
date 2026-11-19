using System;
using BaseLib.Abstracts;
using Godot;
using MegaCrit.Sts2.Core.Helpers;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Models;

namespace Bellkeeper.BellkeeperCode.Character;

/// <summary>
/// 负钟人的选人界面入口。
///
/// 为什么要这个类：
///   继承 CustomCharacterModel 的角色不会自动出现在选人界面。
///   BaseLib 走 CustomCharacterSelectEntry 体系，有入口才有可点击的按钮。
///
/// 选人背景：
///   原版的选人背景是 Spine 动画场景，我们做了一套 14 层拆件、
///   2 个动画（select_enter / idle_loop）的 Spine 工程。
///   场景在 scenes/screens/bellkeeper_select_bg.tscn。
/// </summary>
public class BellkeeperSelectEntry : CustomCharacterSelectEntry
{
    public override string EntryId => "BELLKEEPER";

    /// <summary>选人按钮上的头像图（132×195 竖版）。</summary>
    public override string ButtonIconPath =>
        ImageHelper.GetImagePath("packed/character_select/char_select_bellkeeper.png");

    public override string EntryTitle =>
        SafeLoc("characters/BELLKEEPER-BELLKEEPER.title", "负钟人");

    public override string EntryDescription =>
        SafeLoc("characters/BELLKEEPER-BELLKEEPER.description", "盲眼的负钟人背着塌掉钟楼的最后一口钟，要把它送还给造钟的人。");

    /// <summary>排在原版五角色之后。</summary>
    public override int SortOrder => 100;

    public override bool VisibleInCharacterSelect => true;
    public override bool UnlockedInCharacterSelect => true;
    public override CharacterModel? AvailabilitySourceCharacter => null;

    /// <summary>选中后实际使用的角色。</summary>
    public override CharacterModel? InitialCharacter => ModelDb.Character<Bellkeeper>();

    public override string LockedTitle => EntryTitle;
    public override string LockedDescription => "用其他角色完成一局游戏后解锁。";

    private static string SafeLoc(string path, string fallback)
    {
        try
        {
            var loc = LocString.KeyPathToLocString(path);
            return loc.Exists() ? loc.GetFormattedText() : fallback;
        }
        catch
        {
            return fallback;
        }
    }

    public override bool ShowVanillaInfoPanelWhenUnresolved => false;
    public override bool ShowVanillaInfoPanelWhenResolved => false;

    /// <summary>选人背景场景路径。</summary>
    public override string? CharacterSelectScenePath => SpineVisualFactory.SelectBgScene;

    /// <summary>
    /// 备用路径：手工实例化背景场景。
    /// 如果 CharacterSelectScenePath 那条路走不通，走这里。
    /// </summary>
    public override Control? CreateCharacterSelectScene()
    {
        try
        {
            var path = SpineVisualFactory.SelectBgScene;
            if (!ResourceLoader.Exists(path))
            {
                MainFile.Logger.Error($"选人背景: 找不到场景 {path}");
                return null;
            }

            var packed = GD.Load<PackedScene>(path);
            if (packed is null)
            {
                MainFile.Logger.Error("选人背景: PackedScene 加载失败");
                return null;
            }

            var node = packed.Instantiate();
            if (node is Control control)
            {
                MainFile.Logger.Info("选人背景: 场景已实例化");
                return control;
            }

            if (node is Node2D n2d)
            {
                var wrapper = new Control();
                wrapper.SetAnchorsPreset(Control.LayoutPreset.FullRect);
                wrapper.AddChild(n2d);
                MainFile.Logger.Info("选人背景: Node2D 已包进 Control");
                return wrapper;
            }

            MainFile.Logger.Error("选人背景: 场景根节点类型不对");
            node?.QueueFree();
            return null;
        }
        catch (Exception e)
        {
            MainFile.Logger.Error("选人背景实例化异常: " + e);
            return null;
        }
    }
}
