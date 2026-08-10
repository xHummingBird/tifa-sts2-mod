using System;
using System.Linq;
using Godot;
using HarmonyLib;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Context;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Nodes.Combat;
using MegaCrit.Sts2.Core.Nodes.HoverTips;
using Tifa.TifaCode.Extensions;
using Tifa.TifaCode.Powers;
using Tifa.TifaCode.Relics;

namespace Tifa.TifaCode.Mechanics.Chi;

public partial class ChiDisplayOverlay : Control
{
    public static ChiDisplayOverlay? Instance { get; private set; }

    private Control? _activeTip;

    private List<TifaHoverTipText.TextTarget> _tipTextTargets = [];

    private int _renderedChi = int.MinValue;
    private int _renderedMaxChi = int.MinValue;
    
    private Control? _chiDisplay;
    private RichTextLabel? _chiLabel;
    private RichTextLabel? _comboLabel;
    private Player? _player;

    private IHoverTip? _chiHoverTip;
    private IHoverTip? _comboHoverTip;

    private int _lastChi = -1;
    private int _lastCombo = -1;

    private Tween? _chiPopTween;
    private Tween? _comboPopTween;

    private bool _exiting;

    private static readonly Color ChiGainColor = new Color(1f, 0.35f, 0.25f);
    private static readonly Color ComboGainColor = new Color(0.55f, 0.85f, 1f);

    public override void _Ready()
    {
        Instance = this;
        Name = "ChiDisplayOverlay";

        MouseFilter = MouseFilterEnum.Pass;

        CallDeferred(nameof(Setup));
    }

    private async void Setup()
    {
        if (!IsInsideTree())
            return;

        for (int i = 0; i < 60; i++)
        {
            if (_exiting || !IsInsideTree())
                return;

            var state = CombatManager.Instance?.DebugOnlyGetState();
            var player = state?.Players.FirstOrDefault(p => LocalContext.IsMe(p));

            if (player != null)
            {
                if (player.Character is not Character.Tifa)
                {
                    QueueFree();
                    return;
                }

                _player = player;
                break;
            }

            var tree = GetTree();

            if (tree == null)
                return;

            await ToSignal(tree, SceneTree.SignalName.ProcessFrame);
        }

        if (_player == null)
        {
            QueueFree();
            return;
        }

        if (_exiting || !IsInsideTree())
            return;

        var scene = GD.Load<PackedScene>("res://Tifa/scenes/chi_display.tscn");

        if (scene == null)
        {
            GD.PushError("[Tifa Chi] Failed to load res://Tifa/scenes/ChiDisplay.tscn");
            QueueFree();
            return;
        }

        _chiDisplay = scene.Instantiate<Control>();
        AddChild(_chiDisplay);

        _chiDisplay.MouseFilter = MouseFilterEnum.Pass;
        _chiDisplay.SetAnchorsPreset(LayoutPreset.BottomLeft);

        // Adjust this to sit beside your Limit UI / Energy UI.
        _chiDisplay.Position = new Vector2(-40, 90);
        _chiDisplay.Visible = true;

        _chiLabel = _chiDisplay.GetNodeOrNull<RichTextLabel>("%ChiLabel");
        _comboLabel = _chiDisplay.GetNodeOrNull<RichTextLabel>("%ComboLabel");
        
        _chiLabel.Position += new Vector2(0, 0);
        _comboLabel.Position += new Vector2(-17, 0);

        if (_chiLabel == null)
        {
            GD.PushError("[Tifa Chi] Could not find %ChiLabel in ChiDisplay.tscn");
            QueueFree();
            return;
        }

        if (_comboLabel == null)
        {
            GD.PushError("[Tifa Chi] Could not find %ComboLabel in ChiDisplay.tscn");
            QueueFree();
            return;
        }

        SetupLabel(_chiLabel, 34);
        SetupLabel(_comboLabel, 28);

        _chiHoverTip = TifaStaticHoverTip.Chi;
        _comboHoverTip = TifaStaticHoverTip.Combo;

        _chiLabel.MouseFilter = MouseFilterEnum.Stop;
        _comboLabel.MouseFilter = MouseFilterEnum.Stop;

        _chiLabel.MouseEntered += OnChiHovered;
        _chiLabel.MouseExited += OnUnhovered;

        _comboLabel.MouseEntered += OnComboHovered;
        _comboLabel.MouseExited += OnUnhovered;

        _chiLabel.TreeExiting += () =>
        {
            _chiPopTween?.Kill();
            _chiPopTween = null;
            _chiLabel = null;
        };

        _comboLabel.TreeExiting += () =>
        {
            _comboPopTween?.Kill();
            _comboPopTween = null;
            _comboLabel = null;
        };

        RefreshDisplay();
    }

    public override void _Process(double delta)
    {
        if (_exiting)
            return;

        if (_player == null)
            return;

        if (!CombatManager.Instance.IsInProgress)
            return;

        RefreshDisplay();
        RenderTipValues();
    }

    private void SetupLabel(RichTextLabel label, int fontSize)
    {
        var font = GD.Load<Font>("res://themes/kreon_bold_shared.tres");

        if (font != null)
        {
            label.AddThemeFontOverride("font", font);
            label.AddThemeFontOverride("normal_font", font);
        }
        else
        {
            GD.PushWarning("[Tifa Chi] Failed to load res://themes/kreon_bold_shared.tres");
        }

        label.BbcodeEnabled = true;
        label.AddThemeColorOverride("default_color", Colors.White);
        label.AddThemeColorOverride("font_outline_color", new Color(0.15f, 0.15f, 0.15f));
        label.AddThemeConstantOverride("outline_size", 10);
        label.AddThemeFontSizeOverride("normal_font_size", fontSize);
    }

    private ComboRelicBase? GetComboRelic()
    {
        return _player?.Relics
            .OfType<ComboRelicBase>()
            .FirstOrDefault();
    }

    private int GetCurrentChi()
    {
        if (_player?.Creature == null)
            return 0;

        return _player.Creature.GetPowerAmount<ChiPower>();
    }

    private int GetCurrentCombo()
    {
        return GetComboRelic()?.Combo ?? 0;
    }

    private void RefreshDisplay()
    {
        int chi = GetCurrentChi();
        int combo = GetCurrentCombo();

        UpdateChiDisplay(chi);
        UpdateComboDisplay(combo);
    }

    private void UpdateChiDisplay(int value)
    {
        var label = _chiLabel;

        if (label == null)
            return;

        if (!GodotObject.IsInstanceValid(label) || label.IsQueuedForDeletion())
            return;

        try
        {
            label.Text = $"[center]{value}[/center]";
        }
        catch (ObjectDisposedException)
        {
            return;
        }

        if (_lastChi >= 0 && value > _lastChi)
        {
            PlayPop(label, ref _chiPopTween, ChiGainColor);
        }
        else
        {
            label.Scale = Vector2.One;
            label.Modulate = Colors.White;
        }

        _lastChi = value;
    }

    private void UpdateComboDisplay(int value)
    {
        var label = _comboLabel;

        if (label == null)
            return;

        if (!GodotObject.IsInstanceValid(label) || label.IsQueuedForDeletion())
            return;

        try
        {
            label.Text = $"[center]{value}[/center]";
        }
        catch (ObjectDisposedException)
        {
            return;
        }

        if (_lastCombo >= 0 && value > _lastCombo)
        {
            PlayPop(label, ref _comboPopTween, ComboGainColor);
        }
        else
        {
            label.Scale = Vector2.One;
            label.Modulate = Colors.White;
        }

        _lastCombo = value;
    }

    private void PlayPop(
        RichTextLabel label,
        ref Tween? tween,
        Color gainColor)
    {
        if (_exiting)
            return;

        if (!GodotObject.IsInstanceValid(label) || label.IsQueuedForDeletion())
            return;

        if (tween != null && GodotObject.IsInstanceValid(tween))
            tween.Kill();

        label.Scale = Vector2.One;
        label.Modulate = gainColor;

        tween = label.CreateTween();

        tween.TweenProperty(label, "scale", new Vector2(1.25f, 1.25f), 0.10f)
            .SetTrans(Tween.TransitionType.Quad)
            .SetEase(Tween.EaseType.Out);

        tween.TweenProperty(label, "scale", Vector2.One, 0.35f)
            .SetTrans(Tween.TransitionType.Quad)
            .SetEase(Tween.EaseType.Out);

        tween.Parallel().TweenProperty(
                label,
                "modulate",
                Colors.White,
                0.35f
            )
            .SetTrans(Tween.TransitionType.Quad)
            .SetEase(Tween.EaseType.Out);
    }

    private void OnChiHovered()
    {
        ShowHoverTip(_chiHoverTip, new Vector2(-75f, -650f));
    }

    private void OnComboHovered()
    {
        ShowHoverTip(_comboHoverTip, new Vector2(-75f, -550f));
    }

    private void ShowHoverTip(IHoverTip? hoverTip, Vector2 offset)
    {
        if (_exiting)
            return;

        if (hoverTip == null)
            return;

        NHoverTipSet.Clear();

        var tip = NHoverTipSet.CreateAndShow(this, hoverTip);
        tip.GlobalPosition = GlobalPosition + offset;
        tip.MouseFilter = MouseFilterEnum.Ignore;
        
        TrackTip(tip);
    }
    
    private void TrackTip(Control tip)
    {
        _activeTip = tip;

        _tipTextTargets =
            TifaHoverTipText.CollectTargets(tip);

        _renderedChi = int.MinValue;
        _renderedMaxChi = int.MinValue;

        RenderTipValues();
    }
    
    private void RenderTipValues()
    {
        if (_activeTip == null)
            return;

        if (!IsInstanceValid(_activeTip))
            return;

        if (_tipTextTargets.Count == 0)
            return;

        var relic = GetComboRelic();

        if (relic == null)
            return;

        int chi = relic.GetChiLevelForUI();
        int maxChi = relic.GetMaxChiLevelForUI();

        if (chi == _renderedChi &&
            maxChi == _renderedMaxChi)
        {
            return;
        }

        _renderedChi = chi;
        _renderedMaxChi = maxChi;

        TifaHoverTipText.Render(
            _tipTextTargets,
            chi,
            maxChi);
    }

    private void OnUnhovered()
    {
        NHoverTipSet.Remove(this);

        _activeTip = null;
        _tipTextTargets.Clear();
    }

    public override void _ExitTree()
    {
        _exiting = true;

        if (_chiPopTween != null && GodotObject.IsInstanceValid(_chiPopTween))
            _chiPopTween.Kill();

        if (_comboPopTween != null && GodotObject.IsInstanceValid(_comboPopTween))
            _comboPopTween.Kill();

        _chiPopTween = null;
        _comboPopTween = null;

        NHoverTipSet.Remove(this);

        _chiLabel = null;
        _comboLabel = null;
        _chiDisplay = null;
        _player = null;
        _chiHoverTip = null;
        _comboHoverTip = null;

        if (Instance == this)
            Instance = null;
    }
}

[HarmonyPatch(typeof(NEnergyCounter), nameof(NEnergyCounter._Ready))]
public static class ChiDisplayOverlayPatch
{
    public static void Postfix(NEnergyCounter __instance)
    {
        if (__instance == null)
            return;

        if (!GodotObject.IsInstanceValid(__instance) || __instance.IsQueuedForDeletion())
            return;

        if (__instance.GetNodeOrNull<ChiDisplayOverlay>("ChiDisplayOverlay") != null)
            return;

        var overlay = new ChiDisplayOverlay
        {
            Name = "ChiDisplayOverlay"
        };

        __instance.AddChild(overlay);
    }
}