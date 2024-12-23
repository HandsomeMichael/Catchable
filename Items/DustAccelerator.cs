using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace Catchable.Items
{ 
    // Dust accelerator
    // Right click to copy nearest dust at mouse
    // Left click to throw the dust

	public class DustAcc : ModItem
	{

        public CatchType dustType;

		public override void SetDefaults()
		{
			Item.width = 40;
			Item.height = 40;

            Item.shoot = ProjectileID.PurificationPowder;
            Item.shootSpeed = 10f;
			Item.useTime = 20;
			Item.useAnimation = 20;
			Item.useStyle = ItemUseStyleID.HoldUp;

			Item.rare = ItemRarityID.Quest;
			// Item.UseSound = SoundID.Item1;

			// Item.autoReuse = true;
			// Item.noUseGraphic = true;
		}

        public override bool AltFunctionUse(Player player) => true;

        public override bool CanUseItem(Player player)
        {
            if (player.altFunctionUse == 2)
            {
                Item.useTime = 30;
                Item.useAnimation = 30;
                Item.autoReuse = false;
                Item.useStyle = ItemUseStyleID.HoldUp;
            }
            else 
            {
                Item.useTime = 10;
                Item.useAnimation = 10;
                Item.autoReuse = true;
                Item.useStyle = ItemUseStyleID.Swing;
            }
            return true;
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {

            if (player.altFunctionUse == 2)
            {
                // mf when checking 6000 fucking dust on 1 update
                float distance = 0f;
                dustType.IphoneFactoryReset();
                Dust finalDust = null;
                
                foreach (var dust in Main.dust)
                {
                    bool isCloser = dust.position.DistanceSQ(Main.MouseWorld) < distance;
                    if (dust.active && ( isCloser || dustType.Id == 0))
                    {
                        finalDust = dust;
                    }
                }

                if (finalDust != null)
                {
                    dustType.SetTo(finalDust);
                    Item.SetNameOverride(dustType._name);
                }
                else 
                {
                    Item.SetNameOverride("Unassigned Dust Accelerator");
                }
            }
            else 
            {
                for (int i = 0; i < 10; i++)
                {
                    Dust.NewDust(position, 12, 12, dustType.Id, velocity.X,velocity.Y);
                }
            }


            return false;
        }

        public override void PostDrawInInventory(SpriteBatch spriteBatch, Vector2 position, Rectangle frame, Color drawColor, Color itemColor, Vector2 origin, float scale)
        {
            
        }
    }
}
