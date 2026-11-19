using System.Reflection;
using Godot;
using HarmonyLib;
using MegaCrit.Sts2.Core.Modding;

namespace Bellkeeper.BellkeeperCode;

[ModInitializer(nameof(Initialize))]
public partial class MainFile : Node
{
    public const string ModId = "Bellkeeper";
    public const string ResPath = $"res://{ModId}";

    public static MegaCrit.Sts2.Core.Logging.Logger Logger { get; } =
        new(ModId, MegaCrit.Sts2.Core.Logging.LogType.Generic);

    public static void Initialize()
    {
        var assembly = Assembly.GetExecutingAssembly();

        // 让 Godot 检索本程序集的脚本，否则带脚本的场景会找不到程序集
        Godot.Bridge.ScriptManagerBridge.LookupScriptsInAssembly(assembly);

        Harmony harmony = new(ModId);
        harmony.PatchAll(assembly);

        // 关键词由 BellkeeperKeywords 里的 [CustomEnum] 特性自动注册，
        // BaseLib 在启动时扫描这些字段并分配枚举值，不需要手动调用。

        Logger.Info("负钟人 Mod 已初始化");
    }
}
