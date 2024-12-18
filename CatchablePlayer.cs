using Terraria;
using Terraria.ModLoader;

namespace Catchable
{
    public class CatchablePlayer : ModPlayer
    {
        public bool bugnetting;

        public override void ResetEffects()
        {
            bugnetting = false;
        }
    }
}