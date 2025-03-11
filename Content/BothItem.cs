using Terraria.ModLoader;

namespace ModSideIcon.Content
{
    public class BothItem : ModItem
    {
        // just a placeholder item to show both icons
        // 46x22

        public override void SetDefaults()
        {
            base.SetDefaults();

            Item.width = 46 * 2;
            Item.height = 22 * 2;
        }
    }

}