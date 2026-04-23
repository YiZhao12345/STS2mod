using HarmonyLib;
using MegaCrit.Sts2.Core.Entities.Merchant;

namespace FreeRemoval.FreeRemovalCode.Patches;

[HarmonyPatch(typeof(MerchantCardRemovalEntry), nameof(MerchantCardRemovalEntry.CalcCost))]
public static class FreeRemovalPatch
{
    [HarmonyPostfix]
    public static void Postfix(MerchantCardRemovalEntry __instance)
    {
        __instance._cost = 0;
    }
}