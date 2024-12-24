using Terraria;
using Terraria.ModLoader;

namespace ZZZCatchNPC
{
	public class ZZZCatchNPC : Mod
	{
		public override void Load()
		{
			if (Main.dedServ)
			{
				return;
			}
			int num = 579;
			if (BruhBruh.get.moddedNPC)
			{
				num = NPCLoader.NPCCount;
			}
			for (int i = 0; i < num; i++)
			{
				if (i > 0)
				{
					NPCItem item = new NPCItem();
					item.ThisNPCID = i;
					AddItem("NPC_" + i, item);
				}
			}
			if (BruhBruh.get.CatchProj)
			{
				num = 713;
				if (BruhBruh.get.moddedNPC)
				{
					num = ProjectileLoader.ProjectileCount;
				}
				for (int a = 0; a < num; a++)
				{
					ProjItem item3 = new ProjItem();
					item3.proj = a;
					AddItem("Proj_" + a, item3);
				}
			}
			if (BruhBruh.get.Experimental)
			{
				num = 274;
				for (int b = 0; b < num; b++)
				{
					DustItem item2 = new DustItem();
					item2.dust = b;
					AddItem("DustItem_" + b, item2);
				}
			}
		}
	}
}
