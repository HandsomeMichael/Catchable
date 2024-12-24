using System.ComponentModel;
using Terraria;
using Terraria.ModLoader;
using Terraria.ModLoader.Config;

namespace ZZZCatchNPC
{
	[Label("Catch Config")]
	public class BruhBruh : ModConfig
	{
		[Header("Common setting")]
		[Label("Migrate item from previous version")]
		[DefaultValue(true)]
		public bool Migrate;

		[Label("Modded Support")]
		[Tooltip("Allow mod support \n[require reload to unload items")]
		[DefaultValue(true)]
		public bool moddedNPC;

		[Label("Bug Net Extra Size")]
		[Tooltip("Its hard to catch things with tiny bug net\nSo i made this config that increase Bug Net size")]
		[Range(1f, 5f)]
		[Increment(1f)]
		[DefaultValue(1f)]
		[Slider]
		public float BugNetScale;

		[Header("NPC")]
		[Label("Catch boss")]
		[Tooltip("Allow the player to catch npc bosses")]
		[DefaultValue(true)]
		public bool CatchBoss;

		[Label("Catch hostile npc")]
		[Tooltip("Allow the player to catch hostile npc")]
		[DefaultValue(true)]
		public bool CatchHostile;

		[Label("Catch town npc")]
		[Tooltip("Allow the player to catch town npc")]
		[DefaultValue(true)]
		public bool CatchTown;

		[Label("Catch friendly npc")]
		[Tooltip("Allow the player to catch friendly npc")]
		[DefaultValue(true)]
		public bool CatchFriend;

		[Header("Projectile")]
		[Label("Catch Projectile")]
		[Tooltip("Allow the player to catch projectiles\n[require reload to unload items]")]
		[DefaultValue(true)]
		public bool CatchProj;

		[Label("Catched Projectile Can be used as Ammo")]
		[Tooltip("Allow the player to use catched projectile as Ammo")]
		[DefaultValue(true)]
		public bool ProjGun;

		[Header("Other")]
		[Label("Experimental Mode")]
		[Tooltip("Allows some feature that may broke game")]
		[DefaultValue(false)]
		public bool Experimental;

		[Label("Password")]
		[Tooltip("Insert password here")]
		[DefaultValue("Twitter")]
		public string Password;

		public override ConfigScope Mode => ConfigScope.ServerSide;

		public static BruhBruh get => ModContent.GetInstance<BruhBruh>();

		public override void OnChanged()
		{
			for (int i = 0; i < 200; i++)
			{
				NPC npc = Main.npc[i];
				if (!Main.npcCatchable[npc.type] && npc.active && npc != null)
				{
					if ((get.moddedNPC || npc.type <= 580) && (!npc.friendly || get.CatchFriend) && (!npc.boss || get.CatchBoss) && (npc.friendly || get.CatchHostile) && (!npc.townNPC || get.CatchTown))
					{
						npc.catchItem = (short)base.mod.ItemType("NPC_" + npc.type);
					}
					else if (ModLoader.GetMod("Fargowiltas") == null || !npc.townNPC)
					{
						npc.catchItem = -1;
					}
				}
			}
		}
	}
}
