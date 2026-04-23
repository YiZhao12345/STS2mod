using System.Linq;
using HarmonyLib;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Models.Cards;
using MegaCrit.Sts2.Core.Rooms;
using MegaCrit.Sts2.Core.Runs;

namespace FreeRemoval.FreeRemovalCode.Patches;

[HarmonyPatch(typeof(MerchantRoom), nameof(MerchantRoom.EnterInternal))]
public static class MerchantLoanPatch
{
    [HarmonyPostfix]
    public static void Postfix(
        MerchantRoom __instance,
        IRunState? runState,
        bool isRestoringRoomStackBase)
    {
        if (isRestoringRoomStackBase)
        {
            return;
        }
        
        if (runState is not RunState concreteRunState)
        {
            return;
        }
        
        Player? player = concreteRunState.Players.FirstOrDefault();
        if (player == null)
        {
            return;
        }
        
        // ⭐ 用官方 API 加金币（不要直接改 _gold）
        PlayerCmd.GainGold(50m, player, true);
        
        // 用官方 API 加 Debt 诅咒到牌组
        CardPileCmd.AddCurseToDeck<Debt>(player);
        
        MainFile.Logger.Info($"[Loan] 进了商人房：+50 金币 + 1 张 Debt");
    }
}