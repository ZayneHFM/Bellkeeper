using System;
using System.Collections;
using System.Reflection;
using System.Runtime.CompilerServices;
using Bellkeeper.BellkeeperCode.Relics;
using HarmonyLib;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Events;
using BellkeeperCharacter = Bellkeeper.BellkeeperCode.Character.Bellkeeper;

namespace Bellkeeper.BellkeeperCode.Mechanics;

/// <summary>
/// 将先古加费遗物《断梁木架》(BrokenBeamFrame) 注入先古神《达夫》(Darv) 的遗物池（仅限负钟人）。
/// 同时覆盖 AllPossibleOptions（用于遗物图鉴展示）与 GenerateInitialOptions（用于局内抽取）。
/// </summary>
[HarmonyPatch(typeof(Darv))]
public static class DarvEnergyRelicPatch
{
    private static readonly object SyncLock = new();
    private static readonly FieldInfo? ValidRelicSetsField = AccessTools.Field(typeof(Darv), "_validRelicSets");
    private static readonly Type? ValidRelicSetType = typeof(Darv).GetNestedType("ValidRelicSet", BindingFlags.Public | BindingFlags.NonPublic);
    private static readonly FieldInfo? RelicsField = ValidRelicSetType?.GetField("relics", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
    private static readonly ConstructorInfo? ValidRelicSetCtor = ValidRelicSetType?.GetConstructor(
        BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic,
        binder: null,
        [typeof(Func<Player, bool>), typeof(RelicModel[])],
        modifiers: null);

    private static bool _injected;

    [HarmonyPrefix]
    [HarmonyPatch("GenerateInitialOptions")]
    public static void InjectBeforeGenerateInitialOptions() => EnsureRegistered();

    [HarmonyPrefix]
    [HarmonyPatch("get_AllPossibleOptions")]
    public static void InjectBeforeAllPossibleOptions() => EnsureRegistered();

    private static void EnsureRegistered()
    {
        if (_injected) return;

        lock (SyncLock)
        {
            if (_injected) return;

            RuntimeHelpers.RunClassConstructor(typeof(Darv).TypeHandle);

            if (ValidRelicSetsField?.GetValue(null) is not IList sets
                || ValidRelicSetCtor is null
                || RelicsField is null)
            {
                MainFile.Logger.Error("无法注入《断梁木架》到 Darv 遗物池：反射目标成员缺失");
                _injected = true;
                return;
            }

            var targetRelic = ModelDb.Relic<BrokenBeamFrame>();
            foreach (var entry in sets)
            {
                if (entry is null) continue;
                if (RelicsField.GetValue(entry) is RelicModel[] existing
                    && Array.Exists(existing, r => r == targetRelic))
                {
                    _injected = true;
                    return;
                }
            }

            Func<Player, bool> filter = static player => player?.Character is BellkeeperCharacter;
            var newSet = ValidRelicSetCtor.Invoke([filter, new RelicModel[] { targetRelic }]);
            sets.Add(newSet);
            _injected = true;
        }
    }
}
