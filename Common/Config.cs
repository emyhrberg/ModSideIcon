using System.ComponentModel;
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
    }
}