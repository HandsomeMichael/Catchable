using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;

namespace ZZZCatchNPC
{
	public class DustItem : ModItem
	{
		public int dust;

		public override bool CloneNewInstances => true;

		public override string Texture => "ZZZCatchNPC/DustItem";

		public override bool Autoload(ref string name)
		{
			return false;
		}

		public override void SetStaticDefaults()
		{
			base.DisplayName.SetDefault(RandomMethod.GetDustName(dust) + " Dust");
			base.Tooltip.SetDefault("Throws a " + RandomMethod.GetDustName(dust) + $" [{dust}] dusts");
		}

		public override void SetDefaults()
		{
			base.item.useStyle = 1;
			base.item.autoReuse = true;
			base.item.useTurn = true;
			base.item.useAnimation = 15;
			base.item.useTime = 15;
			base.item.maxStack = 999;
			base.item.consumable = true;
			base.item.width = 12;
			base.item.rare = Main.rand.Next(1, 9);
			base.item.height = 12;
			base.item.noUseGraphic = true;
			dust = int.Parse(base.Name.Replace("DustItem_", ""));
			base.item.shoot = 1;
			base.item.shootSpeed = 11f;
		}

		public override void PostUpdate()
		{
			Dust.NewDust(base.item.position, 12, 12, dust);
		}

		public override bool Shoot(Player player, ref Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockBack)
		{
			for (int i = 0; i < (int)base.item.shootSpeed; i++)
			{
				Dust.NewDust(position, 12, 12, dust, speedX, speedY);
			}
			return false;
		}
	}
}
