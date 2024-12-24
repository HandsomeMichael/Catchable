using Terraria;
using Terraria.ModLoader;

namespace ZZZCatchNPC
{
	public class IamGoodAtNamingClasses : GlobalNPC
	{
		public override void SetDefaults(NPC npc)
		{
			if (!Main.npcCatchable[npc.type] && (!npc.townNPC || ModLoader.GetMod("Fargowiltas") == null) && (BruhBruh.get.moddedNPC || npc.type <= 580) && (!npc.friendly || BruhBruh.get.CatchFriend) && (!npc.boss || BruhBruh.get.CatchBoss) && (npc.friendly || BruhBruh.get.CatchHostile) && (!npc.townNPC || BruhBruh.get.CatchTown))
			{
				npc.catchItem = (short)base.mod.ItemType("NPC_" + npc.type);
			}
		}
	}
}
