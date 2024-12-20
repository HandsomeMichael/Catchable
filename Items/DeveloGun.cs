using System;
using System.Collections.Generic;
using Catchable.Helper;
using Humanizer;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
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
            var npc = NPC.NewNPCDirect(Projectile.GetSource_ReleaseEntity("Suprise matafaka"),(int)Projectile.Center.X,(int)Projectile.Center.Y,(int)Projectile.ai[2]);

            // halfen stats
            // lets hope we dont get 0 division somehow :)
            npc.SpawnedFromStatue = true; // no loot
            SafelyHalfenStats(ref npc.lifeMax);
            SafelyHalfenStats(ref npc.life);
            SafelyHalfenStats(ref npc.defense);
            SafelyHalfenStats(ref npc.damage);

            if (npc.TryGetGlobalNPC<BrainWashed>(out var braining))
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

        // public override void PostDraw(NPC npc, SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        // {
        //     if (ownedBy != -1)
        //     Terraria.UI.Chat.ChatManager.DrawColorCodedString(spriteBatch,FontAssets.DeathText.Value,"brainwash : "+ownedBy,npc.Center - Main.screenPosition,Color.White,0f,Vector2.One,Vector2.One);
        // }

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