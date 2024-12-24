using Catchable.Items;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
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

        // public override void Load()
        // {
        //     // Generate items like in 1.3
        //     if (!CatchableConfig.Get.GenerateItem) return;

        //     NPC npc = new NPC();
        //     CatchType npcType = new CatchType();
        //     for (int i = 1; i < NPCLoader.NPCCount; i++)
        //     {
        //         CatchedNPC item = new CatchedNPC();
        //         npc.SetDefaults(i);
        //         npcType.SetTo(npc,item.Item);
        //         AddContent(item);
        //     }
        // }
    }
}