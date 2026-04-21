using Godot;
using HarmonyLib;
using MegaCrit.Sts2.Core.Modding;

namespace FreeRemoval.FreeRemovalCode;

//You're recommended but not required to keep all your code in this package and all your assets in the FreeRemoval folder.
[ModInitializer(nameof(Initialize))]
public partial class MainFile : Node
{
    public const string ModId = "FreeRemoval"; //At the moment, this is used only for the Logger and harmony names.

    public static MegaCrit.Sts2.Core.Logging.Logger Logger { get; } =
        new(ModId, MegaCrit.Sts2.Core.Logging.LogType.Generic);

    public static void Initialize()
    {
        Logger.Info($"[{ModId}] Hello from Slay the Spire 2!");
        Harmony harmony = new(ModId);

        harmony.PatchAll();
    }
}