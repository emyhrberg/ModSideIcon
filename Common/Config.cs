using System.ComponentModel;
using Microsoft.Xna.Framework;
using Terraria.ModLoader.Config;

namespace ModSideIcon.Common
{
    public class Config : ModConfig
    {
        public override ConfigScope Mode => ConfigScope.ClientSide;

        [DefaultValue(true)]
        public bool ShowModSideIcon = true;

        [DefaultValue(true)]
        public bool ShowModSideName = true;

        [DefaultValue(0.85f)]
        [Range(0.6f, 1.1f)]
        [Increment(0.1f)]
        public float TextSize = 0.85f;

        [DefaultValue(typeof(Vector2), "0, 0")]
        [Increment(0.1f)]
        public Vector2 IconPosition = new Vector2(0, 0);

        [DefaultValue(typeof(Vector2), "0.5, 0")]
        [Increment(0.1f)]
        public Vector2 TextPosition = new Vector2(0.5f, 0.0f);
    }
}