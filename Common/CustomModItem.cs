using System;
using System.Diagnostics;
using System.Reflection;
using Microsoft.Xna.Framework.Graphics;
using MonoMod.RuntimeDetour;
using ReLogic.Content;
using Terraria;
using Terraria.GameContent.UI.Elements;
using Terraria.ModLoader;
using Terraria.ModLoader.UI;
namespace ModSideIcon.Common
{
    /// <summary>
    /// Uses a hook to add a custom side icon to mod items.
    /// The typeof assembly uses a method called "detour" to hook into the game's code.
    /// This allows you to modify the game's code without changing the original source code.
    /// It is also known as a "hook" or "patch" or even "IL patch".
    /// Also known as "reflection", by using, for example GetType, GetMethod, GetField, GetValue and BindingFlags.
    /// </summary>
    public class CustomModItem : ModSystem
    {
        private Hook _modItemInitializeHook;

        public override void Load()
        {
            if (ModLoader.TryGetMod("aConciseModList", out Mod mod))
            {
                // Completely different UIConciseModItem, implement different behaviour here
                return;
            }
            else
            {
                // Vanilla UIModItem, implement behaviour here
                CreateModItemHook();
            }
        }

        public override void Unload()
        {
            _modItemInitializeHook?.Dispose();
            _modItemInitializeHook = null;
        }

        private void CreateModItemHook()
        {
            // Get the UIModItem type from the tModLoader assembly.
            Type modItemType = typeof(Main).Assembly.GetType("Terraria.ModLoader.UI.UIModItem");
            if (modItemType == null)
                return;

            // Get the OnInitialize method so we can hook it.
            MethodInfo onInitializeMethod = modItemType.GetMethod("OnInitialize", BindingFlags.Public | BindingFlags.Instance);
            if (onInitializeMethod == null)
                return;

            // Create the hook using MonoMod.RuntimeDetour
            _modItemInitializeHook = new Hook(
                onInitializeMethod,
                new Action<Action<UIPanel>, UIPanel>(UIModItem_OnInitialize)
            );
        }

        /// <summary>
        /// Our hook method that intercepts UIModItem.OnInitialize.
        /// We'll reflect out the '_mod' field, then its 'properties.side' enum,
        /// and finally append a UIText that displays the ModSide name.
        /// </summary>
        private static void UIModItem_OnInitialize(Action<UIPanel> orig, UIPanel self)
        {
            // 1) Call the original method so the UIModItem sets itself up
            orig(self);

            try
            {
                // 2) Reflect out the "_mod" field from UIModItem
                FieldInfo modField = self.GetType().GetField("_mod", BindingFlags.Instance | BindingFlags.NonPublic);
                if (modField == null)
                    return; // Can't find the field

                object localModObj = modField.GetValue(self);
                if (localModObj == null)
                    return; // No LocalMod present

                // 3) Reflect out the "properties" field (or property) on LocalMod
                Type localModType = localModObj.GetType();
                FieldInfo propertiesField = localModType.GetField("properties", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
                if (propertiesField == null)
                    return; // Possibly a property if not a field

                object buildPropertiesObj = propertiesField.GetValue(localModObj);
                if (buildPropertiesObj == null)
                    return;

                // 4) Reflect out the "side" field on BuildProperties
                Type buildPropertiesType = buildPropertiesObj.GetType();
                FieldInfo sideField = buildPropertiesType.GetField("side", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
                if (sideField == null)
                    return;

                object sideValue = sideField.GetValue(buildPropertiesObj);
                if (sideValue == null)
                    return;

                // sideValue is the enum ModSide (Both, Client, Server, NoSync)
                // Convert to string
                string sideString = sideValue.ToString(); // e.g. "Both", "Client", etc.
                                                          // if (sideString == "Both")
                                                          // {
                                                          // sideString = "Server";
                                                          // }

                // Check config
                Config c = ModContent.GetInstance<Config>();

                // 5) Append a UIText showing the ModSide name
                if (c.ShowModSideName)
                {
                    // if "both", move text -72, otherwise "client" -50
                    // note: this was when both was two icons, now it's one icon
                    // float left = sideString == "Both" ? -72 : -50;
                    float left = -50;

                    if (!c.ShowModSideIcon)
                    {
                        // If no icon, move text a bit more left
                        left = -23;
                    }

                    UIText sideText = new UIText(sideString)
                    {
                        // If both, make text a bit more left

                        // Position it however you want. For example, near the top-right corner:
                        HAlign = 1f,          // Align to the right
                        VAlign = 0f,          // Align to the top
                        Top = { Pixels = 3 }, // No offset from the top edge
                        Left = { Pixels = left } // Slight offset from the right edge
                    };
                    self.Append(sideText);
                }

                // 5b) Append a UIImage showing the ModSide name
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

                    if (sideIcon != null)
                    {
                        ImageIcon sideButton = new(sideIcon, sideString)
                        {
                            HAlign = 1f,
                            Left = { Pixels = -20 },
                        };
                        self.Append(sideButton);
                    }
                }
            }
            catch (Exception e)
            {
                // For debugging purposes, log any exceptions that occur when trying to reflect out the fields
                ModContent.GetInstance<ModSideIcon>().Logger.Error("Error in UIModItem_OnInitialize: " + e);
            }
        }
    }
}
