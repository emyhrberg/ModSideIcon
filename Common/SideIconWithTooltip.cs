using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria.GameContent.UI.Elements;
using Terraria.ModLoader.UI;

namespace ModSideIcon.Common
{
    public class SideIconWithTooltip : UIImage
    {
        public string Tooltip { get; set; }

        public SideIconWithTooltip(Asset<Texture2D> texture, string tooltip) : base(texture)
        {
            Tooltip = tooltip;
        }

        public SideIconWithTooltip(Texture2D nonReloadingTexture, string tooltip) : base(nonReloadingTexture)
        {
            Tooltip = tooltip;
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