using Catchable.Items;
using Microsoft.Xna.Framework.Graphics;
using Terraria.ModLoader;

namespace Catchable
{
    public class Catchable : Mod 
    {

        public static Texture2D InvalidTexture;

        public override void PostSetupContent()
        {
            InvalidTexture = ModContent.Request<Texture2D>("Catchable/Items/Invalid").Value;
        }

        public override void Unload()
        {
           InvalidTexture = null;
        }

        // We will not be doing the same code 

        // public override void Load()
        // {
        //     AddContent(new BigPoo());
        // }
    }
}