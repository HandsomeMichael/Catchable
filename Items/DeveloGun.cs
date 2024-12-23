using System;
using System.Collections.Generic;
using Catchable.Helper;
using Humanizer;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace Catchable.Items
{
    public class DeveloGun : ModItem
    {
        
        int selectedType = 0;
        public override void SetDefaults()
        {
            Item.CloneDefaults(ItemID.Handgun);
            Item.useAmmo = ModContent.ItemType<CatchedNPC>();
            Item.damage = 0;
            Item.DamageType = DamageClass.Generic;
        }

        public override bool CanConsumeAmmo(Item ammo, Player player)
        {
            if (ammo.ModItem is CatchedNPC modItem)
            {
                selectedType = modItem.catchType.Id;
            }
            return base.CanConsumeAmmo(ammo, player);
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            // && source.AmmoItemIdUsed == ModContent.ItemType<CatchedNPC>()
            if (selectedType != 0)
            {
                Projectile.NewProjectile(source, position, velocity, ModContent.ProjectileType<DeveloGunProj>(), damage, knockback, player.whoAmI, 0,0,selectedType);
            }
            return false;
        }

    }

    public class DeveloGunProj : ModProjectile
    {
        public override string Texture => "Terraria/Images/Item_"+ItemID.WoodenCrate;
        public override void SetDefaults()
        {
            Projectile.CloneDefaults(ProjectileID.SnowBallFriendly);
            Projectile.friendly = false;
            Projectile.hostile = false;
        }
        public override void PostAI()
        {
            Projectile.rotation += Projectile.velocity.X > 0 ? -0.1f : 0.1f;
        }
        public override void OnKill(int timeLeft)
        {

            // Visuals from example mod, we're going barebones with this one

			// Play explosion sound
			SoundEngine.PlaySound(SoundID.Item14, Projectile.position);
			// Smoke Dust spawn
			for (int i = 0; i < 50; i++) {
				Dust dust = Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, DustID.Smoke, 0f, 0f, 100, default, 2f);
				dust.velocity *= 1.4f;
			}

			// Fire Dust spawn
			for (int i = 0; i < 80; i++) {
				Dust dust = Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, DustID.Torch, 0f, 0f, 100, default, 3f);
				dust.noGravity = true;
				dust.velocity *= 5f;
				dust = Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, DustID.Torch, 0f, 0f, 100, default, 2f);
				dust.velocity *= 3f;
			}

			// Large Smoke Gore spawn
			for (int g = 0; g < 2; g++) {
				var goreSpawnPosition = new Vector2(Projectile.position.X + Projectile.width / 2 - 24f, Projectile.position.Y + Projectile.height / 2 - 24f);
				Gore gore = Gore.NewGoreDirect(Projectile.GetSource_FromThis(), goreSpawnPosition, default, Main.rand.Next(61, 64), 1f);
				gore.scale = 1.5f;
				gore.velocity.X += 1.5f;
				gore.velocity.Y += 1.5f;
				gore = Gore.NewGoreDirect(Projectile.GetSource_FromThis(), goreSpawnPosition, default, Main.rand.Next(61, 64), 1f);
				gore.scale = 1.5f;
				gore.velocity.X -= 1.5f;
				gore.velocity.Y += 1.5f;
				gore = Gore.NewGoreDirect(Projectile.GetSource_FromThis(), goreSpawnPosition, default, Main.rand.Next(61, 64), 1f);
				gore.scale = 1.5f;
				gore.velocity.X += 1.5f;
				gore.velocity.Y -= 1.5f;
				gore = Gore.NewGoreDirect(Projectile.GetSource_FromThis(), goreSpawnPosition, default, Main.rand.Next(61, 64), 1f);
				gore.scale = 1.5f;
				gore.velocity.X -= 1.5f;
				gore.velocity.Y -= 1.5f;
			}

            var npc = NPC.NewNPCDirect(Projectile.GetSource_ReleaseEntity("Suprise matafaka"),(int)Projectile.Center.X,(int)Projectile.Center.Y,(int)Projectile.ai[2]);

            // halfen stats
            // lets hope we dont get 0 division somehow :)
            npc.SpawnedFromStatue = true; // no loot
            SafelyHalfenStats(ref npc.lifeMax);
            SafelyHalfenStats(ref npc.life);
            SafelyHalfenStats(ref npc.defense);
            SafelyHalfenStats(ref npc.damage);

            // uhhh
            if (npc.type == NPCID.Vampire || npc.type == NPCID.VampireBat)
            SoundEngine.PlaySound(new SoundStyle("Catchable/Sound/jonathanbanging"),Projectile.Center);

            if (CatchableConfig.Get.DeveloGun_Brainwash && npc.TryGetGlobalNPC<BrainWashed>(out var braining))
            {
                braining.ownedBy = Projectile.owner;
            }
        }

        void SafelyHalfenStats(ref int stats)
        {
            if (stats > 0)
            {
                stats /= 2;
            }
        }
    }

    public class BrainWashed : GlobalNPC
    {
        public int ownedBy = -1;
        public override bool InstancePerEntity => true;
        protected override bool CloneNewInstances => true;

        public override bool IsLoadingEnabled(Mod mod)
        {
            return CatchableConfig.Get.DeveloGun_Brainwash;
        }

        public override bool PreAI(NPC npc)
        {
            if (ownedBy != -1)
            {
                Main.player[ownedBy].npcTypeNoAggro[npc.type] = true;
            }
            return base.PreAI(npc);
        }

        public override void PostAI(NPC npc)
        {
            if (ownedBy != -1)
            {
                Main.player[ownedBy].npcTypeNoAggro[npc.type] = false;
            }
        }

        public override void PostDraw(NPC npc, SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            if (ownedBy != -1 && Main.myPlayer == ownedBy)
            {
                var size = Helpme.MeasureString("Team");

                Terraria.UI.Chat.ChatManager.DrawColorCodedString(spriteBatch,
                FontAssets.MouseText.Value,
                "Team",npc.Center - Main.screenPosition,
                Color.LightGreen,0f,size / 2f,Vector2.One);
            }
        }

        // no hitting hitting
        public override bool CanHitPlayer(NPC npc, Player target, ref int cooldownSlot)
        {
            if (ownedBy != -1)
            {
                return target.whoAmI != ownedBy;
            }
            return base.CanHitPlayer(npc, target, ref cooldownSlot);
        }
    }
}