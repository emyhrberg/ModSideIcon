using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria.GameContent.UI.Elements;
using Terraria.ModLoader.UI;

namespace ModSideIcon.Common
{
    public class ImageIcon : UIImage
    {
        public string Tooltip { get; set; }

        public ImageIcon(Texture2D texture, string tooltip) : base(texture)
        {
            Tooltip = tooltip;
            ImageScale = 0.8f;
        }

        protected override void DrawSelf(SpriteBatch spriteBatch)
        {
            base.DrawSelf(spriteBatch);

            if (IsMouseHovering)
            {
                // Show your tooltip text
                UICommon.TooltipMouseText(Tooltip);
            }
        }
    }
}