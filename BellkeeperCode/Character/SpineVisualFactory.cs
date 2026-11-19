using System;
using Godot;
using MegaCrit.Sts2.Core.Nodes.Combat;

namespace Bellkeeper.BellkeeperCode.Character;

/// <summary>
/// Spine 立绘的场景路径常量与加载辅助。
///
/// 背景（重要）：
///   早期版本试图用 ClassDB.Instantiate("SpineSprite") 在运行期手工拼节点，
///   那是错的——工程里没装 spine-godot 扩展时，那个原生类根本不存在。
///
///   正确做法（官方教程）：
///     1. 在项目根建 bin/，放入 spine-godot GDExtension
///        （已装：spine 4.2 / Godot 4.5.1，见 bin/spine_godot_extension.gdextension）
///     2. Spine 资源用 .spine-json 和 .atlas 两个扩展名，编辑器会自动导入
///     3. 手写 .tres 把两者合成 SpineSkeletonDataResource
///     4. 手写 .tscn，节点名必须是 Visuals / Bounds / IntentPos / CenterPos
///     5. CharacterModel.CustomVisualPath 指向那个 .tscn
///
///   现在这些文件都在 Bellkeeper/scenes/ 下，不再需要运行期拼装。
/// </summary>
public static class SpineVisualFactory
{
    /// <summary>战斗立绘场景（节点名按官方规范：Visuals / Bounds / IntentPos / CenterPos）。</summary>
    public const string BattleVisualScene =
        "res://Bellkeeper/scenes/creature_visuals/bellkeeper.tscn";

    /// <summary>选人界面背景场景。</summary>
    public const string SelectBgScene =
        "res://Bellkeeper/scenes/screens/bellkeeper_select_bg.tscn";

    /// <summary>
    /// 按场景路径实例化立绘节点。
    /// 失败返回 null，调用方回退 BaseLib 占位。
    /// </summary>
    public static NCreatureVisuals? TryInstantiateBattleVisuals()
    {
        try
        {
            if (!ResourceLoader.Exists(BattleVisualScene))
            {
                MainFile.Logger.Error($"Spine: 找不到场景 {BattleVisualScene}");
                return null;
            }

            var packed = GD.Load<PackedScene>(BattleVisualScene);
            if (packed is null)
            {
                MainFile.Logger.Error("Spine: 场景加载失败（PackedScene 为 null）");
                return null;
            }

            var node = packed.Instantiate();
            if (node is NCreatureVisuals visuals)
            {
                return visuals;
            }

            // 场景根是 Node2D 时，包一层 NCreatureVisuals
            if (node is Node2D visualNode)
            {
                var wrapper = new NCreatureVisuals { Name = "BellkeeperVisuals" };
                wrapper.AddChild(visualNode);
                return wrapper;
            }

            MainFile.Logger.Error("Spine: 场景根节点类型不对");
            node?.QueueFree();
            return null;
        }
        catch (Exception e)
        {
            MainFile.Logger.Error("Spine: 实例化立绘场景异常 " + e);
            return null;
        }
    }
}
