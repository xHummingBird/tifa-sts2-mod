using Godot;
using HarmonyLib;
using MegaCrit.Sts2.Core.Hooks;
using MegaCrit.Sts2.Core.Rooms;
using MegaCrit.Sts2.Core.Runs;

namespace Tifa.TifaCode.Extensions;

public static class TifaAssets
{
    private static PackedScene? _tifaScene;
    private static PackedScene? _vfxScene;
    private static PackedScene? _yellowHitVfxScene;
    private static PackedScene? _blueHitVfxScene;

    private const string TifaScenePath = "res://Tifa/scenes/tifa.tscn";
    private const string VfxPath = "res://Tifa/scenes/vfx.tscn";
    private const string BlueHitVfxPath = "res://Tifa/scenes/vfx/hit_blue.tscn";
    private const string YellowHitVfxPath = "res://Tifa/scenes/vfx/hit_yellow.tscn";
    

    public static PackedScene? TifaScene
    {
        get
        {
            _tifaScene = LoadOrReload(_tifaScene, TifaScenePath, "Tifa scene");
            return _tifaScene;
        }
    }

    public static PackedScene? IceScene
    {
        get
        {
            _vfxScene = LoadOrReload(_vfxScene, VfxPath, "Ice VFX");
            return _vfxScene;
        }
    }
    
    public static PackedScene? YellowHitScene
    {
        get
        {
            _yellowHitVfxScene = LoadOrReload(_yellowHitVfxScene, YellowHitVfxPath, "Yellow Hit VFX");
            return _yellowHitVfxScene;
        }
    }
    
    public static PackedScene? BlueHitScene
    {
        get
        {
            _blueHitVfxScene = LoadOrReload(_blueHitVfxScene, BlueHitVfxPath, "Blue Hit VFX");
            return _blueHitVfxScene;
        }
    }

    private static PackedScene? LoadOrReload(PackedScene? cachedScene, string path, string label)
    {
        if (cachedScene != null && GodotObject.IsInstanceValid(cachedScene))
            return cachedScene;

        GD.Print($"TifaAssets: Loading {label} from {path}");

        var scene = GD.Load<PackedScene>(path);

        if (scene == null)
        {
            GD.PrintErr($"TifaAssets: FAILED to load {label}: {path}");
            return null;
        }

        GD.Print($"TifaAssets: Loaded {label}");
        return scene;
    }

    public static void EnsurePreloaded()
    {
        _ = TifaScene;
        _ = IceScene;
        _ = BlueHitScene;
        _ = YellowHitScene;

        GD.Print("TifaAssets: EnsurePreloaded finished");
    }
}

[HarmonyPatch(typeof(Hook), nameof(Hook.AfterActEntered))]
public static class TifaAfterActEnteredPreloadPatch
{
    [HarmonyPrefix]
    public static void Prefix(IRunState runState)
    {
        var player = runState?.Players?.FirstOrDefault();

        if (player?.Character is not Character.Tifa)
            return;

        GD.Print("AfterActEntered: Tifa detected → preloading");

        TifaAssets.EnsurePreloaded();
    }
}


[HarmonyPatch(typeof(Hook), nameof(Hook.AfterRoomEntered))]
public static class TifaAfterRoomEnteredPreloadPatch
{
    [HarmonyPrefix]
    public static void Prefix(IRunState runState, AbstractRoom room)
    {
        var player = runState?.Players?.FirstOrDefault();

        if (player?.Character is not Character.Tifa)
            return;

        GD.Print($"AfterRoomEntered: Tifa detected → preloading. Room = {room.GetType().Name}");

        TifaAssets.EnsurePreloaded();
    }
}
