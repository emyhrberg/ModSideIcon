using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria.GameContent.UI.Elements;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.ModLoader.UI;
using Terraria.UI;

namespace ModSideIcon.Common
{
    public class ImageIcon : UIImage
    {
        public string Tooltip { get; set; }
        public Texture2D Texture { get; set; }

        public ImageIcon(Texture2D texture, string tooltip) : base(texture)
        {
            Texture = texture;
            Tooltip = tooltip;
            ImageScale = 0.8f;
        }

        protected override void DrawSelf(SpriteBatch spriteBatch)
        {
            Config c = ModContent.GetInstance<Config>();
            float scale = c.IconSize;

            CalculatedStyle dimensions = GetDimensions();

            // Calculate the center position within the UI element
            Vector2 centerPosition = dimensions.Position() + new Vector2(dimensions.Width, dimensions.Height) / 2f;

            // Draw the texture centered at the calculated position
            spriteBatch.Draw(
                texture: Texture,
                position: centerPosition,
                sourceRectangle: null,
                color: Color.White,
                rotation: 0f,
                origin: new Vector2(Texture.Width / 2f, Texture.Height / 2f),
                scale: scale,
                effects: SpriteEffects.None,
                layerDepth: 0f
            );

            if (IsMouseHovering && c.Tooltip)
            {
                // Show your tooltip text
                UICommon.TooltipMouseText(Tooltip);
                // UICommon.TooltipMouseText(Language.GetTextValue("Mods.ModSideIcon.Tooltip"));
            }
        }
    }
}