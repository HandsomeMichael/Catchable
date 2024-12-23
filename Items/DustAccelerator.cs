using System;
using System.IO;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;

namespace Catchable.Items
{ 
    // Dust accelerator
    // Right click to copy nearest dust at mouse
    // Left click to throw the dust

	public class DustAcc : ModItem
	{

        // you ever heard of this guy called accelerator
        // he's like one of my friend favorite character
        // i think he is gay

        public CatchType dustType;

        public override void NetSend(BinaryWriter writer)
		{
			dustType.VerifyDustID();
			dustType.NetSend(writer);
		}
        public override void NetReceive(BinaryReader reader)
		{
			dustType.NetReceive(reader);
			if (dustType.VerifyDustID())
			{
				Item.SetNameOverride(dustType._name+" Dust Accelerator");
			}
            else 
            {
                Item.SetNameOverride("Unloaded Dust Accelerator");
            }
		}
        public override void SaveData(TagCompound tag)
		{
			dustType.VerifyDustID();
			dustType.SaveData(tag);
		}
        public override void LoadData(TagCompound tag)
		{
			dustType.LoadData(tag);
			if (dustType.VerifyDustID())
			{
				Item.SetNameOverride(dustType._name+" Dust Accelerator");
			}
             else 
            {
                Item.SetNameOverride("Unloaded Dust Accelerator");
            }
		}

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

            Item.accessory = true;
			Item.vanity = true;
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
                    var newDistance = dust.position.DistanceSQ(Main.MouseWorld);
                    bool isCloser = newDistance < distance;
                    if (dust.active && ( isCloser || dustType.Id == 0) )
                    {
                        finalDust = dust;
                    }
                }

                if (finalDust != null)
                {
                    dustType.SetTo(finalDust);
                    Item.SetNameOverride(dustType._name+" Dust Accelerator");
                    CombatText.NewText(player.Hitbox,Color.White,"Picked "+dustType._name);
                    CombatText.NewText(new Rectangle((int)finalDust.position.X,(int)finalDust.position.Y,0,0),Color.Pink,"bop");
                }
                else 
                {
                    Item.SetNameOverride("Unassigned Dust Accelerator");
                    CombatText.NewText(player.Hitbox,Color.Red,"No nearby dust found");
                }
            }
            else 
            {
                for (int i = 0; i < 20; i++)
                {
                    Dust.NewDust(position + velocity, 12, 12, dustType.Id, velocity.X,velocity.Y);
                }
            }


            return false;
        }

        public static Dust dummyDust;

        public override void Load()
        {
            dummyDust = new Dust();
        }

        public override void Unload()
        {
            dummyDust = null;
        }

        void UpdateDustVanilla()
        {
            dummyDust.type = dustType.Id;
            dummyDust.frame.X = 10 * dummyDust.type;
            dummyDust.frame.Y = 10;
            dummyDust.shader = null;
            dummyDust.customData = null;
            int row = dummyDust.type / 100;
            dummyDust.frame.X -= 1000 * row;
            dummyDust.frame.Y += 30 * row;
            dummyDust.frame.Width = 8;
            dummyDust.frame.Height = 8;
        }

        void UpdateDustMod()
        {
            ModDust md = DustLoader.GetDust(dummyDust.type);
            try
            {
                md.OnSpawn(dummyDust);
                md.Update(dummyDust);
            }
            catch
            {
                dummyDust.frame = new Rectangle(0, 0, 8, 8);
            }
        }

        public void TrailEff(Player player)
        {
            if (dustType.unloaded) return;
            // Dust.NewDust(player.Center, 12, 12, dustType.Id);
            Dust.NewDustPerfect(player.Center, dustType.Id, Vector2.Zero);
        }

        public override void UpdateVanity(Player player)
        {
            TrailEff(player);
        }
        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            TrailEff(player);
        }

        public override void PostDrawInInventory(SpriteBatch spriteBatch, Vector2 position, Rectangle frame, Color drawColor, Color itemColor, Vector2 origin, float scale)
        {
            Texture2D tex;

            if (dustType.unloaded) return;

			if (dustType.Id <= DustID.Count)
            {
                UpdateDustVanilla();
                tex = Terraria.GameContent.TextureAssets.Dust.Value;
            }
			else
            {
                UpdateDustMod();
                tex = DustLoader.GetDust(dustType.Id).Texture2D.Value;
            }

			Rectangle newFrame = dummyDust.frame;

			spriteBatch.Draw(tex, position, newFrame, Color.White, 0, new Vector2(newFrame.Width, newFrame.Height) / 2, 1f, 0, 0);
        }
    }
}
