using System;
using System.Reflection;
using Microsoft.Xna.Framework.Graphics;
using MonoMod.RuntimeDetour;
using Terraria;
using Terraria.GameContent.UI.Elements;
using Terraria.ModLoader;

namespace ModSideIcon.Common
{
    public class CustomConciseModItem : ModSystem
    {
        private Hook _conciseModItemInitializeHook;

        public override void Load()
        {
            if (!ModLoader.TryGetMod("ConciseModList", out Mod conciseMod))
            {
                Mod.Logger.Info("ConciseModList not found, skipping ConciseModItem hook");
                return;
            }

            Type conciseModItemType = conciseMod.Code.GetType("ConciseModList.ConciseUIModItem");
            if (conciseModItemType == null)
                return;

            MethodInfo onInitializeMethod = conciseModItemType.GetMethod("OnInitialize", BindingFlags.Instance | BindingFlags.Public);
            if (onInitializeMethod == null)
                return;

            _conciseModItemInitializeHook = new Hook(
                onInitializeMethod,
                new Action<Action<UIPanel>, UIPanel>(ConciseModItem_OnInitialize)
            );
        }

        public override void Unload()
        {
            _conciseModItemInitializeHook?.Dispose();
            _conciseModItemInitializeHook = null;
        }

        private static void ConciseModItem_OnInitialize(Action<UIPanel> orig, UIPanel self)
        {
            ModContent.GetInstance<ModSideIcon>().Logger.Info("ConciseModItem_OnInitialize");

            orig(self); // Call original method

            // Add a custom icon to top right corner
            FieldInfo modField = self.GetType().BaseType.GetField("_mod", BindingFlags.Instance | BindingFlags.NonPublic);
            object localModObj, buildPropertiesObj, sideValue;
            FieldInfo propertiesField, sideField;
            if (modField != null &&
                (localModObj = modField.GetValue(self)) != null &&
                (propertiesField = localModObj.GetType().GetField("properties", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)) != null &&
                (buildPropertiesObj = propertiesField.GetValue(localModObj)) != null &&
                (sideField = buildPropertiesObj.GetType().GetField("side", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)) != null &&
                (sideValue = sideField.GetValue(buildPropertiesObj)) != null)
            {
                string sideString = sideValue.ToString();

                Config c = ModContent.GetInstance<Config>();

                if (c.ShowModSideIcon)
                {
                    Texture2D sideIcon;
                    if (c.IconGlow)
                    {
                        sideIcon = sideString switch
                        {
                            "Both" => Assets.ServerIcon.Value,
                            "Server" => Assets.ServerIcon.Value,
                            "Client" => Assets.ClientIcon.Value,
                            _ => null
                        };
                    }
                    else
                    {
                        sideIcon = sideString switch
                        {
                            "Both" => Assets.ServerIconNoGlow.Value,
                            "Server" => Assets.ServerIconNoGlow.Value,
                            "Client" => Assets.ClientIconNoGlow.Value,
                            _ => null
                        };
                    }

                    ImageIcon imageIcon = new(sideIcon, sideString);

                    // set image position with config
                    imageIcon.VAlign = c.IconPosition.Y;
                    imageIcon.HAlign = c.IconPosition.X;

                    // magic left offset extra
                    imageIcon.Left.Set(pixels: 5f, precent: 0f);
                    imageIcon.Top.Set(pixels: -5f, precent: 0f);

                    self.Append(imageIcon);
                }

                if (c.ShowModSideName)
                {
                    UIText sideText = new(sideString, textScale: c.TextSize);
                    // sideText.Top.Set(4, 0f);
                    sideText.HAlign = c.TextPosition.X;
                    sideText.VAlign = c.TextPosition.Y;
                    self.Append(sideText);
                }
            }
            else
            {
                if (modField == null)
                    ModContent.GetInstance<ModSideIcon>().Logger.Error("Failed to find field '_mod' in base type.");
                else if ((localModObj = modField.GetValue(self)) == null)
                    ModContent.GetInstance<ModSideIcon>().Logger.Error("Failed to get LocalMod object from _mod field.");
                else if ((propertiesField = localModObj.GetType().GetField("properties", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)) == null)
                    ModContent.GetInstance<ModSideIcon>().Logger.Error("Failed to find 'properties' field in LocalMod.");
                else if ((buildPropertiesObj = propertiesField.GetValue(localModObj)) == null)
                    ModContent.GetInstance<ModSideIcon>().Logger.Error("Properties object is null in LocalMod.");
                else if ((sideField = buildPropertiesObj.GetType().GetField("side", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)) == null)
                    ModContent.GetInstance<ModSideIcon>().Logger.Error("Failed to find 'side' field in BuildProperties.");
                else if ((sideValue = sideField.GetValue(buildPropertiesObj)) == null)
                    ModContent.GetInstance<ModSideIcon>().Logger.Error("Side value is null in BuildProperties.");
            }
        }
    }
}
