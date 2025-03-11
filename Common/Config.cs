using System.ComponentModel;
using Microsoft.Xna.Framework;
using Terraria.ModLoader.Config;

namespace ModSideIcon.Common
{
    public class Config : ModConfig
    {
        public override ConfigScope Mode => ConfigScope.ClientSide;

        [Header("Icon")]
        [DefaultValue(true)]
        public bool ShowModSideIcon = true;

        [DefaultValue(true)]
        public bool IconGlow = true;

        [DefaultValue(true)]
        public bool Tooltip = true;

        [DefaultValue(1.0f)]
        [Range(0.1f, 2.0f)]
        [Increment(0.1f)]
        public float IconSize = 1.0f;

        [DefaultValue(typeof(Vector2), "1, 0")]
        [Increment(0.1f)]
        public Vector2 IconPosition = new Vector2(1, 0);

        [Header("Text")]
        [DefaultValue(false)]
        public bool ShowModSideName = false;

        [DefaultValue(0.8f)]
        [Range(0.1f, 2.0f)]
        [Increment(0.1f)]
        public float TextSize = 0.8f;

        [DefaultValue(typeof(Vector2), "0.5, 0")]
        [Increment(0.1f)]
        public Vector2 TextPosition = new Vector2(0.5f, 0.0f);
    }
}