using Godot;
using HarmonyLib;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Models.Cards;
using MegaCrit.Sts2.Core.Nodes.Screens.Shops;
using FreeRemoval.FreeRemovalCode;

namespace FreeRemoval.FreeRemovalCode.Patches;

[HarmonyPatch(typeof(NMerchantInventory), nameof(NMerchantInventory._Ready))]
public static class NMerchantInventoryUIPatch
{
    [HarmonyPostfix]
    public static void Postfix(NMerchantInventory __instance)
    {
        if (__instance.FindChild("LoanContainer") != null) return;

        var cardRemoval = __instance.GetNodeOrNull<Control>("%MerchantCardRemoval");
        if (cardRemoval == null)
        {
            MainFile.Logger.Info("[Loan] 找不到 MerchantCardRemoval 节点");
            return;
        }

        var slotsContainer = __instance.GetNodeOrNull<Control>("%SlotsContainer");
        if (slotsContainer == null)
        {
            MainFile.Logger.Info("[Loan] 找不到 SlotsContainer 节点");
            return;
        }

        var costLabel = cardRemoval.GetNodeOrNull<Label>("Cost/CostLabel");
        var font = costLabel?.GetThemeFont("font");

        var goldTexture = ResourceLoader.Load<Texture2D>(
            "res://images/atlases/ui_atlas.sprites/top_bar/top_bar_gold.tres");

        // ── 外层容器（竖排） ──────────────────────────────────
        var container = new VBoxContainer();
        container.Name = "LoanContainer";
        container.AddThemeConstantOverride("separation", 8);
        container.CustomMinimumSize = new Vector2(300f, 0f);

        // ── 标题文字 ──────────────────────────────────────────
        var titleLabel = new Label();
        titleLabel.Text = "借借你好，戒戒你好";
        titleLabel.HorizontalAlignment = HorizontalAlignment.Center;
        titleLabel.AutowrapMode = TextServer.AutowrapMode.Off;
        titleLabel.CustomMinimumSize = new Vector2(300f, 0f);
        if (font != null) titleLabel.AddThemeFontOverride("font", font);
        titleLabel.AddThemeFontSizeOverride("font_size", 25);
        titleLabel.AddThemeConstantOverride("outline_size", 10);
        titleLabel.AddThemeColorOverride("font_outline_color", new Color(0.067f, 0f, 0f, 0.431f));
        titleLabel.AddThemeColorOverride("font_color", new Color(1f, 1f, 1f));
        titleLabel.MouseFilter = Control.MouseFilterEnum.Ignore;

        // ── 横排按钮区（居中） ────────────────────────────────
        var hbox = new HBoxContainer();
        hbox.AddThemeConstantOverride("separation", 20);
        hbox.Alignment = BoxContainer.AlignmentMode.Center;

        var slot50 = MakeLoanSlot(goldTexture, font, "50", scale: 0.8f);
        var slot100 = MakeLoanSlot(goldTexture, font, "100", scale: 1.0f);

        hbox.AddChild(slot50);
        hbox.AddChild(slot100);

        container.AddChild(titleLabel);
        container.AddChild(hbox);

        container.Position = cardRemoval.Position + new Vector2(-600f, -600f);
        slotsContainer.AddChild(container);

        slot50.GuiInput += (InputEvent e) =>
        {
            if (e is InputEventMouseButton mb && mb.ButtonIndex == MouseButton.Left && mb.Pressed)
                OnLoan(slot50, slot100, 50, false);
        };

        slot100.GuiInput += (InputEvent e) =>
        {
            if (e is InputEventMouseButton mb && mb.ButtonIndex == MouseButton.Left && mb.Pressed)
                OnLoan(slot50, slot100, 100, true);
        };

        MainFile.Logger.Info("[Loan] 借贷 UI 已注入 SlotsContainer");
    }

    private static Control MakeLoanSlot(Texture2D? goldTexture, Font? font, string amount, float scale)
    {
        var slot = new HBoxContainer();
        slot.AddThemeConstantOverride("separation", 6);
        slot.MouseFilter = Control.MouseFilterEnum.Stop;
        slot.Scale = new Vector2(scale, scale);
        slot.Alignment = BoxContainer.AlignmentMode.Center;

        var icon = new TextureRect();
        icon.Texture = goldTexture;
        icon.CustomMinimumSize = new Vector2(35f, 35f);
        icon.ExpandMode = TextureRect.ExpandModeEnum.IgnoreSize;
        icon.StretchMode = TextureRect.StretchModeEnum.KeepAspectCentered;
        icon.MouseFilter = Control.MouseFilterEnum.Ignore;

        var label = new Label();
        label.Text = amount;
        label.HorizontalAlignment = HorizontalAlignment.Left;
        label.VerticalAlignment = VerticalAlignment.Center;
        if (font != null) label.AddThemeFontOverride("font", font);
        label.AddThemeFontSizeOverride("font_size", 25);
        label.AddThemeConstantOverride("outline_size", 15);
        label.AddThemeColorOverride("font_outline_color", new Color(0.067f, 0f, 0f, 0.431f));
        label.AddThemeColorOverride("font_color", new Color(1f, 1f, 1f));
        label.MouseFilter = Control.MouseFilterEnum.Ignore;

        slot.AddChild(icon);
        slot.AddChild(label);
        return slot;
    }

    private static void OnLoan(
        Control btn50,
        Control btn100,
        int amount,
        bool isLarge)
    {
        Player? player = MerchantLoanPatch.CachedPlayer;
        if (player == null) return;

        PlayerCmd.GainGold((decimal)amount, player, true);

        if (isLarge)
            CardPileCmd.AddCurseToDeck<Greed>(player);
        else
            CardPileCmd.AddCurseToDeck<Debt>(player);

        btn50.MouseFilter = Control.MouseFilterEnum.Ignore;
        btn100.MouseFilter = Control.MouseFilterEnum.Ignore;
        btn50.Modulate = new Color(0.5f, 0.5f, 0.5f);
        btn100.Modulate = new Color(0.5f, 0.5f, 0.5f);

        MainFile.Logger.Info($"[Loan] 借贷成功：+{amount} 金币");
    }
}