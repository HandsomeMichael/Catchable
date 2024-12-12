using Terraria;
using Terraria.ModLoader;

namespace ZZZCatchNPC
{
	public class globalProj : GlobalProjectile
	{
		public override bool Autoload(ref string name)
		{
			return BruhBruh.get.CatchProj;
		}

		public override void PostAI(Projectile projectile)
		{
			if (!BruhBruh.get.CatchProj)
			{
				return;
			}
			if (BruhBruh.get.Password == "Debuging69")
			{
				Main.NewText("GETPROJ : " + projectile);
			}
			for (int i = 0; i < 255; i++)
			{
				Player player = Main.player[i];
				if (player.active && !player.dead && projectile.Hitbox.Intersects(player.GetModPlayer<HitboxGet>().itemhitbox))
				{
					projectile.active = false;
					projectile.timeLeft = 0;
					if (BruhBruh.get.Password == "Debuging69")
					{
						Main.NewText("Collide yes");
					}
					int a = Item.NewItem(projectile.getRect(), base.mod.ItemType("Proj_" + projectile.type));
					Item get = Main.item[a];
					get.damage = projectile.damage;
					get.knockBack = projectile.knockBack;
					if (projectile.melee)
					{
						get.melee = true;
					}
					else if (projectile.magic)
					{
						get.magic = true;
					}
					else if (projectile.minion)
					{
						get.summon = true;
					}
					else
					{
						get.ranged = true;
					}
				}
			}
		}
	}
}
