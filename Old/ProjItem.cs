using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace ZZZCatchNPC
{
	public class ProjItem : ModItem
	{
		public int proj;

		public override bool CloneNewInstances => true;

		public override string Texture
		{
			get
			{
				if (proj >= 714)
				{
					return npcGet().modProjectile.Texture;
				}
				return $"Terraria/Projectile_{proj}";
			}
		}

		public Projectile npcGet()
		{
			Projectile projectile = new Projectile();
			projectile.SetDefaults(proj);
			return projectile;
		}

		public override bool Autoload(ref string name)
		{
			return false;
		}

		public override void SetStaticDefaults()
		{
			if (proj > 714)
			{
				string a = npcGet().modProjectile.Name;
				if (a == "" || a == null)
				{
					a = "No Name";
				}
				base.DisplayName.SetDefault(a);
			}
			else
			{
				base.DisplayName.SetDefault(npcGet().Name);
			}
			Main.RegisterItemAnimation(base.item.type, new DrawAnimationVertical(6, Main.projFrames[proj]));
		}

		public override void SetDefaults()
		{
			base.item.damage = 1;
			base.item.useStyle = 1;
			base.item.autoReuse = true;
			base.item.useTurn = true;
			base.item.useAnimation = 15;
			base.item.useTime = 15;
			base.item.maxStack = 999;
			base.item.consumable = true;
			base.item.width = 12;
			base.item.height = 12;
			base.item.noUseGraphic = true;
			proj = int.Parse(base.Name.Replace("Proj_", ""));
			base.item.shoot = proj;
			base.item.shootSpeed = 11f;
			Projectile projectile = npcGet();
			if (projectile.melee)
			{
				base.item.melee = true;
			}
			else if (projectile.magic)
			{
				base.item.magic = true;
			}
			else if (projectile.minion)
			{
				base.item.summon = true;
			}
			else
			{
				base.item.ranged = true;
			}
			if (BruhBruh.get.ProjGun)
			{
				base.item.ammo = AmmoID.Bullet;
			}
			else
			{
				base.item.ammo = 0;
			}
		}

		public override void UpdateInventory(Player player)
		{
			if (BruhBruh.get.ProjGun)
			{
				if (player.inventory[player.selectedItem].useAmmo > 0)
				{
					base.item.ammo = player.inventory[player.selectedItem].useAmmo;
				}
			}
			else
			{
				base.item.ammo = AmmoID.Bullet;
			}
		}

		public override bool Shoot(Player player, ref Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockBack)
		{
			Projectile obj = Main.projectile[Projectile.NewProjectile(position.X, position.Y, speedX, speedY, type, damage, knockBack, player.whoAmI)];
			obj.friendly = true;
			obj.hostile = false;
			return false;
		}
	}
}
