using System.Reflection;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria.ModLoader;

namespace ModSideIcon.Common
{
    /// <summary>
    /// Uses a hook to add a custom side icon to mod items.
    /// The typeof assembly uses a method called "detour" to hook into the game's code.
    /// This allows you to modify the game's code without changing the original source code.
    /// It is also known as a "hook" or "patch" or even "IL patch".
    /// Also known as "reflection", by using, for example GetType, GetMethod, GetField, GetValue and BindingFlags.
    /// </summary>
    public class LoadAssets : ModSystem
    {
        public override void Load()
        {
            // Ensure static constructor of Assets runs to preload textures.
            var ignore = Assets.Initialized;
        }
    }

    public static class Assets
    {
        // Add new textures here: they will be auto-loaded based on field name.
        public static Asset<Texture2D> BothIcon;
        public static Asset<Texture2D> ClientIcon;
        public static Asset<Texture2D> ServerIcon;
        public static Asset<Texture2D> ClientIconNoGlow;
        public static Asset<Texture2D> ServerIconNoGlow;

        // This property just forces the static constructor to run.
        public static bool Initialized { get; } = true;

        static Assets()
        {
            // Automatically initialize all public static fields of type Asset<Texture2D>
            foreach (FieldInfo field in typeof(Assets).GetFields(BindingFlags.Public | BindingFlags.Static))
            {
                if (field.FieldType == typeof(Asset<Texture2D>))
                {
                    // Use the field name as the asset path. For instance, "ButtonTexture" looks for "Assets/ButtonTexture"
                    Asset<Texture2D> asset = PreloadAsset(field.Name);
                    field.SetValue(null, asset);
                }
            }
        }

        private static Asset<Texture2D> PreloadAsset(string path)
        {
            // string modName = typeof(Assets).Namespace.Split('.')[0];
            string modName = "ModSideIcon";
            return ModContent.Request<Texture2D>($"{modName}/Assets/{path}", AssetRequestMode.AsyncLoad);
        }
    }
}
