using Godot;
using HarmonyLib;
using MegaCrit.Sts2.Core.Modding;

namespace FreeRemoval.FreeRemovalCode;

[ModInitializer(nameof(Initialize))]
public partial class MainFile : Node
{
    public const string ModId = "FreeRemoval";

    public static MegaCrit.Sts2.Core.Logging.Logger Logger { get; } =
        new(context: ModId, MegaCrit.Sts2.Core.Logging.LogType.Generic);

    public static void Initialize()
    {
        Logger.Info($"[{ModId}] Hello from Slay the Spire 2!");
        Harmony harmony = new(ModId);
        harmony.PatchAll();
    }
}