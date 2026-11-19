# 负钟人（The Bellkeeper） v0.1.2

《杀戮尖塔 2》原创角色 Mod，新增黄铜色系角色「负钟人」。

- Mod ID：`Bellkeeper`
- 版本：`v0.1.2`（与 `Bellkeeper.json` 一致）
- 最低游戏版本：`0.107.0`
- 依赖：`BaseLib` ≥ `3.4.8`

## 安装
1. 先安装依赖 `BaseLib`（见其发布页）。
2. 在游戏目录下创建 `mods/Bellkeeper/`。
3. 将发布包的 3 个文件放入该目录：`Bellkeeper.dll`、`Bellkeeper.json`、`Bellkeeper.pck`。

发布包下载：见本仓库 Releases 页面（`https://github.com/ZayneHFM/Bellkeeper/releases`）。

## 从源码构建
- 环境：Godot 4.5.1（mono）+ .NET 9。
- 将 `Directory.Build.props.example` 复制为 `Directory.Build.props`，填入本机 Godot 可执行文件与游戏安装路径（也可用 `local.props` 仅设置 `Sts2Path`）。
- 依赖 NuGet 包 `Alchyr.Sts2.BaseLib`、`Alchyr.Sts2.ModAnalyzers`；编译时引用游戏目录中的 `sts2.dll`、`0Harmony.dll`（不随仓库分发）。
- 目录：代码在 `BellkeeperCode/`，资源（图像 / 本地化 / Spine / 材质 / 着色器 / 场景 / 音频）在 `Bellkeeper/`。
- 版本号与依赖以 `Bellkeeper.json` 为准。

## 许可
MIT（见 `LICENSE`），适用于本人有权授权的代码部分。游戏本体、依赖（BaseLib 等）及单独注明来源的美术素材仍适用各自条款，详见 `THIRD_PARTY_NOTICES.md`。

制作：Zayne。开发与验收使用了 DeepSeek、Codex 等 AI 工具，美术素材主要由 Gemini 生成。
