using System.Linq;
using HarmonyLib;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Rooms;
using MegaCrit.Sts2.Core.Runs;

namespace FreeRemoval.FreeRemovalCode.Patches;

[HarmonyPatch(typeof(MerchantRoom), nameof(MerchantRoom.EnterInternal))]
public static class MerchantLoanPatch
{
    public static Player? CachedPlayer { get; private set; }

    [HarmonyPostfix]
    public static void Postfix(
        MerchantRoom __instance,
        IRunState? runState,
        bool isRestoringRoomStackBase)
    {
        if (isRestoringRoomStackBase) return;
        if (runState is not RunState concreteRunState) return;

        CachedPlayer = concreteRunState.Players.FirstOrDefault();
        MainFile.Logger.Info("[Loan] 缓存 Player 成功");
    }
}