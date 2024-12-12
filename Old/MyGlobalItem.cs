using System.Collections.Generic;
using System.Reflection;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;
using Terraria.ModLoader.Default;

namespace ZZZCatchNPC
{
	public class MyGlobalItem : GlobalItem
	{
		public override void MeleeEffects(Item item, Player player, Rectangle hitbox)
		{
			if (item.type != 1991 && item.type != 3183)
			{
				return;
			}
			player.GetModPlayer<HitboxGet>().itemhitbox = hitbox;
			if (!BruhBruh.get.Experimental)
			{
				return;
			}
			for (int i = 0; i < 6000; i++)
			{
				Dust dusta = Main.dust[i];
				if (dusta.active && hitbox.Intersects(new Rectangle((int)dusta.position.X, (int)dusta.position.Y, 10, 10)))
				{
					dusta.active = false;
					if (BruhBruh.get.Password == "Debuging69")
					{
						Main.NewText("Dust collide pog");
					}
					Item.NewItem(player.getRect(), base.mod.ItemType("DustItem_" + dusta.type));
				}
			}
		}

		public override void ModifyTooltips(Item item, List<TooltipLine> tooltips)
		{
			if (BruhBruh.get.Password == "Debuging69")
			{
				foreach (TooltipLine tt in tooltips)
				{
					if (tt != null)
					{
						Main.NewText(tt.Name + " = " + tt.text);
					}
				}
			}
			if (item.type == 1991 || item.type == 3183)
			{
				string add = ((ModLoader.GetMod("Fargowiltas") == null) ? "" : " ( Ignore the fargo tooltip )");
				tooltips.Add(new TooltipLine(base.mod, "PpebFunni", "Can catch anything" + add));
			}
		}

		public override void UpdateInventory(Item item, Player player)
		{
			if (item.type == 1991 || item.type == 3183)
			{
				item.scale = BruhBruh.get.BugNetScale;
			}
		}

		public override void SetDefaults(Item item)
		{
			if (item.type == 1991 || item.type == 3183)
			{
				item.scale = BruhBruh.get.BugNetScale;
			}
			MysteryItem mitem;
			if (BruhBruh.get.Migrate && (mitem = item.modItem as MysteryItem) != null)
			{
				FieldInfo field = mitem.GetType().GetField("modName", BindingFlags.Instance | BindingFlags.NonPublic);
				string itemName = (string)mitem.GetType().GetField("itemName", BindingFlags.Instance | BindingFlags.NonPublic).GetValue(mitem);
				if ((string)field.GetValue(mitem) == base.mod.Name && itemName.Contains("TownNPC_"))
				{
					int stack = item.stack;
					bool favorited = item.favorited;
					int type = int.Parse(itemName.Replace("TownNPC_", ""));
					item.SetDefaults(base.mod.ItemType("NPC_" + type));
					item.stack = stack;
					item.favorited = favorited;
				}
			}
		}
	}
}
