using System.Collections.Generic;
using Terraria;
using Terraria.DataStructures;
using Terraria.Localization;
using Terraria.ModLoader;

namespace ZZZCatchNPC
{
	public class NPCItem : ModItem
	{
		public int ThisNPCID;

		private string chat;

		public override bool CloneNewInstances => true;

		public override string Texture
		{
			get
			{
				if (ThisNPCID >= 580)
				{
					return npcGet().modNPC.Texture;
				}
				return $"Terraria/NPC_{ThisNPCID}";
			}
		}

		public NPC npcGet()
		{
			NPC nPC = new NPC();
			nPC.SetDefaults(ThisNPCID);
			return nPC;
		}

		public override bool Autoload(ref string name)
		{
			return false;
		}

		public override void SetStaticDefaults()
		{
			if (ThisNPCID > 580)
			{
				string a = npcGet().modNPC.Name;
				if (a == "" || a == null)
				{
					a = "No Name";
				}
				base.DisplayName.SetDefault(a);
			}
			else
			{
				base.DisplayName.SetDefault(Lang.GetNPCNameValue(ThisNPCID));
			}
			Main.RegisterItemAnimation(base.item.type, new DrawAnimationVertical(6, Main.npcFrameCount[ThisNPCID]));
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
			base.item.height = 12;
			base.item.noUseGraphic = true;
			ThisNPCID = int.Parse(base.Name.Replace("NPC_", ""));
			chat = npcGet().GetChat();
		}

		public override bool UseItem(Player player)
		{
			NPC.NewNPC((int)Main.MouseWorld.X, (int)Main.MouseWorld.Y, ThisNPCID);
			return true;
		}

		public override void ModifyTooltips(List<TooltipLine> tooltips)
		{
			if (chat != Language.GetTextValue("tModLoader.DefaultTownNPCChat"))
			{
				tooltips.Add(new TooltipLine(base.mod, "Chat", "'" + chat + "'"));
			}
		}
	}
}
