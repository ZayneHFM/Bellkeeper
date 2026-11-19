using System;
using Godot;
using MegaCrit.Sts2.Core.Bindings.MegaSpine;
using MegaCrit.Sts2.Core.Helpers;

namespace Bellkeeper.BellkeeperCode.Character;

/// <summary>
/// 选人界面大背景：Spine 动画播放 + 按视口居中铺满。
///
/// ── 为什么不能用本地 Position / Scale ──
///
/// 原版 CharacterSelectScreen 把所有选人背景挂在 AnimatedBg (NCharacterSelectScreenBg) 下。
/// AnimatedBg 在场景里写死了：
///   offset = (-388, -80, +252, +40), pivot_offset = (1280, 600), scale = 1.1 ~ 1.2683
/// 如果只改 _spineNode.Position = size / 2f、_spineNode.Scale = Vector2.One（本地坐标），
/// 经过 AnimatedBg 变换后，舞台中心会漂移到屏幕 (536, 520) 并被多乘一层缩放，导致巨钟偏左上。
///
/// 因此必须直接设置 _spineNode.GlobalPosition 与 _spineNode.GlobalScale，
/// 并在 _Process 中跟踪父节点全局变换，抵消 AnimatedBg 的偏移与缩放。
/// </summary>
public partial class BellkeeperSelectBgPlayer : Node
{
    private const string EnterAnimation = "select_enter";
    private const string IdleAnimation = "idle_loop";

    /// <summary>骨架舞台尺寸，与 spine-json 里的 skeleton 节点一致。</summary>
    private const float StageWidth = 1920f;
    private const float StageHeight = 1080f;

    /// <summary>兜底补边底色，取自 01_bg_dark_field.png 四角采样 (31,20,10)。</summary>
    private static readonly Color FillColor = Color.Color8(31, 20, 10);

    private MegaSprite? _sprite;
    private Node2D? _spineNode;
    private CanvasItem? _parentCanvas;
    private ColorRect? _backdrop;
    private Vector2 _lastViewportSize = Vector2.Zero;
    private Transform2D _lastParentTransform = Transform2D.Identity;
    private bool _hasLayout;

    public override void _Ready()
    {
        _spineNode = GetParent() as Node2D;
        if (_spineNode is null)
        {
            MainFile.Logger.Error("选人背景：父节点不是 Node2D，跳过");
            return;
        }

        _parentCanvas = _spineNode.GetParent() as CanvasItem;
        EnsureBackdrop();
        ApplyLayout();
        Callable.From(ApplyLayout).CallDeferred();
        GetViewport().SizeChanged += ApplyLayout;

        try
        {
            _sprite = new MegaSprite(_spineNode);
            SpineNodeExtensions.RunWhenSpineReady(this, _sprite, OnSpineReady);
        }
        catch (Exception e)
        {
            MainFile.Logger.Error("选人背景：绑定 Spine 失败 " + e);
        }
    }

    public override void _Process(double delta)
    {
        if (_spineNode is null) return;

        var size = GetViewport().GetVisibleRect().Size;
        var parentXf = _parentCanvas?.GetGlobalTransform() ?? Transform2D.Identity;
        if (!_hasLayout || size != _lastViewportSize || !parentXf.IsEqualApprox(_lastParentTransform))
        {
            ApplyLayout();
        }
    }

    public override void _ExitTree()
    {
        try
        {
            GetViewport().SizeChanged -= ApplyLayout;
        }
        catch
        {
            // 退出时视口可能已失效，忽略
        }

        _sprite?.Dispose();
        _sprite = null;
    }

    private void EnsureBackdrop()
    {
        var root = _spineNode?.GetParent();
        if (root is null) return;

        var existing = root.GetNodeOrNull<ColorRect>("BgBackdrop");
        if (existing is not null)
        {
            _backdrop = existing;
            return;
        }

        var backdrop = new ColorRect
        {
            Name = "BgBackdrop",
            Color = FillColor,
            MouseFilter = Control.MouseFilterEnum.Ignore,
        };
        backdrop.SetAnchorsPreset(Control.LayoutPreset.FullRect);
        _backdrop = backdrop;
        root.CallDeferred(Node.MethodName.AddChild, backdrop);
        root.CallDeferred(Node.MethodName.MoveChild, backdrop, 0);
    }

    /// <summary>
    /// 在视口全局坐标系下按 cover 居中铺满 1920×1080 舞台，抵消父容器 AnimatedBg 的偏移与缩放。
    /// 16:9 视口下 scale 恰好为 1.0；16:10 下为 1.111，消除上下硬切边。
    /// </summary>
    private void ApplyLayout()
    {
        if (_spineNode is null || !_spineNode.IsInsideTree()) return;

        var size = GetViewport().GetVisibleRect().Size;
        if (size.X <= 0f || size.Y <= 0f) return;

        float coverScale = Mathf.Max(size.X / StageWidth, size.Y / StageHeight);
        _spineNode.GlobalPosition = size / 2f;
        _spineNode.GlobalScale = new Vector2(coverScale, coverScale);

        if (_backdrop is not null)
        {
            _backdrop.SetAnchorsPreset(Control.LayoutPreset.FullRect);
        }

        _lastViewportSize = size;
        _lastParentTransform = _parentCanvas?.GetGlobalTransform() ?? Transform2D.Identity;
        _hasLayout = true;

        MainFile.Logger.Info(
            $"选人背景：视口 {size.X:0}x{size.Y:0}，coverScale={coverScale:0.###}，globalPos={_spineNode.GlobalPosition}");
    }

    private void OnSpineReady(MegaAnimationState state)
    {
        try
        {
            if (_sprite is null) return;

            if (_sprite.HasAnimation(EnterAnimation) && _sprite.HasAnimation(IdleAnimation))
            {
                state.SetAnimation(EnterAnimation, loop: false, trackId: 0);
                state.AddAnimation(IdleAnimation, delay: 0f, loop: true, trackId: 0);
                return;
            }

            if (_sprite.HasAnimation(IdleAnimation))
            {
                state.SetAnimation(IdleAnimation, loop: true, trackId: 0);
                return;
            }

            MainFile.Logger.Error($"选人背景：骨架里没有动画 {IdleAnimation}");
        }
        catch (Exception e)
        {
            MainFile.Logger.Error("选人背景：播放动画失败 " + e);
        }
    }
}
