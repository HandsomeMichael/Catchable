using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Catchable.Helper;
using Catchable.Items;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.GameContent.Bestiary;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;

namespace Catchable
{

	/// <summary>
	/// ModType if i was the one who implement it ( fucking worse )
	/// </summary>
	public struct CatchType
	{
		internal string _mod;
		internal string _name;
		internal int _id;

		/// <summary>
		/// Some projectile or npcs are not intended to be catched
		/// But fuck that , we just give player a warning thats it :D
		/// </summary>
		public bool notIntended;

		/// <summary>
		/// The npc type , might require updating before shit happen
		/// </summary>
		public int Id => _id;

		public CatchType(NPC npc)
		{
			SetTo(npc,null);
		}

		public void SetTo(NPC npc,Item item)
		{
			if (npc == null)
			{
				throw new ArgumentNullException("NPC is Null when creating new CatchTypeNPC");
			}

			_id = npc.type;

			if (npc.ModNPC != null)
			{
				_mod = npc.ModNPC.Mod.Name;
				_name = npc.ModNPC.Name;
			}
			else
			{
				_mod = "Terraria";
				_name = "";
			}


			if (item != null)
			{
				item.makeNPC = npc.type;
				item.color = npc.color;
				item.SetNameOverride(Lang.GetNPCName(npc.type).Value);
				if (npc.realLife != -1 || npc.CanBeChasedBy(null))
				{
					notIntended = true;
				}
			}
		}

		public void SetTo(Projectile projectile,Item item)
		{
			if (projectile == null)
			{
				throw new ArgumentNullException("Projectile is Null when creating new CatchTypeProjectile");
			}

			_id = projectile.type;

			if (projectile.ModProjectile != null)
			{
				if (projectile.ModProjectile is not SuperBugNetProj)
				{
					_mod = projectile.ModProjectile.Mod.Name;
					_name = projectile.ModProjectile.Name;
				}
			}
			else
			{
				_mod = "Terraria";
				_name = "";
			}


			if (item != null)
			{
				item.shoot = projectile.type;
				item.damage = projectile.damage/2;
				// item.shoot = ModContent.ProjectileType<MyDummyProjectile>();
				item.DamageType = projectile.DamageType;
				item.SetNameOverride(Lang.GetProjectileName(projectile.type).Value);

				if (projectile.ownerHitCheck || projectile.isAPreviewDummy || projectile.minion)
				{
					notIntended = true;
				}
			}
		}

		public void SetTerraria(int id)
		{
			_id = id;
			_mod = "Terraria";
			_name = "";
		}

		public void NetSend(BinaryWriter writer)
		{
			writer.Write(_mod);
			writer.Write(_name);
			writer.Write(_id);
			writer.Write(notIntended);
		}

		public void NetReceive(BinaryReader reader)
        {
			_mod = reader.ReadString();
			_name = reader.ReadString();
			_id = reader.ReadInt32(); // idk if this right or not
			notIntended = reader.ReadBoolean();
        }

		// One thing to notice is that terraria has auto save

		public void SaveData(TagCompound tag)
		{
			tag.Add("mod",_mod);
			tag.Add("name",_name);
			tag.Add("id",_id);
			tag.Add("intent",notIntended);
		}

		public void LoadData(TagCompound tag)
		{
			_mod = tag.GetString("mod");
			_name = tag.GetString("name");
			_id = tag.GetInt("id");
			notIntended = tag.GetBool("intent");
		}

		public bool ValidNPC() => Id > 0 && Id <= NPCLoader.NPCCount;
		public bool ValidProj() => Id > 0 && Id <= ProjectileLoader.ProjectileCount;

		/// <summary>
		/// Verify if id is a valid npc id
		/// </summary>
		/// <returns></returns>
		public string VerifyNPCID()
		{
			if (_mod == "Terraria")
			{
				if (_id == 0 || _id > NPCID.Count)
				{
					return "Terraria : invalid id";
				}
				return "Vanilla Success";
			}

			if (_mod == "" || _name == "" || _mod == null || _name == null)
			{
				return "Error : Dont grab the stickman dumbass";
			}

			if (ModContent.TryFind(_mod, _name, out ModNPC npc))
			{
				if (npc.Type != _id)
				{
					_id = npc.Type;
					return "Warn : ID Manual Fix";
				}

				if (npc.Type > NPCLoader.NPCCount)
				{
					return "Error : how the fuck";
				}

				return "Mod Success";
			}

			return "Error : No possible connection";
		}

		/// <summary>
		/// Verify if id is a valid projectile id
		/// </summary>
		/// <returns></returns>
		public string VerifyProjID()
		{
			if (_mod == "Terraria")
			{
				if (_id == 0 || _id > ProjectileID.Count)
				{
					return "Terraria : invalid id";
				}
				return "Vanilla Success";
			}

			if (_mod == "" || _name == "" || _mod == null || _name == null)
			{
				return "Error : Dont grab the stickman dumbass";
			}

			if (ModContent.TryFind(_mod, _name, out ModProjectile npc))
			{
				if (npc.Type != _id)
				{
					_id = npc.Type;
					return "Warn : ID Manual Fix";
				}

				if (npc.Type > ProjectileLoader.ProjectileCount)
				{
					return "Error : how the fuck";
				}

				return "Mod Success";
			}

			return "Error : No possible connection";
		}
	}

	public class CatchedNPC : ModItem
	{
        public override string Texture => "Catchable/Items/Dot";
		public CatchType catchType;
        public override void NetSend(BinaryWriter writer)
		{
			catchType.VerifyNPCID();
			catchType.NetSend(writer);
		}
        public override void NetReceive(BinaryReader reader)
		{
			catchType.NetReceive(reader);
			catchType.VerifyNPCID();
			Item.makeNPC = catchType.Id;
		}
        public override void SaveData(TagCompound tag)
		{
			catchType.VerifyNPCID();
			catchType.SaveData(tag);
		}
        public override void LoadData(TagCompound tag)
		{
			catchType.LoadData(tag);
			catchType.VerifyNPCID();
			Item.makeNPC = catchType.Id;
		}

		// Stacking
        public override bool CanStack(Item source)
        {
			if (source.ModItem != null && source.ModItem is CatchedNPC target)
			{
				if ( target.catchType.Id == catchType.Id)
				{
					return true;
				}
			}
            return false;
        }

		public override void SetDefaults() 
		{
			// By defaults create a bunny looking ass
			Item.CloneDefaults(ItemID.Bunny);
			Item.rare = ItemRarityID.Blue;
		}

		internal static FieldInfo bestiaryKeyField;

        public override void Load()
        {
            bestiaryKeyField = typeof(FlavorTextBestiaryInfoElement).GetField("_key", BindingFlags.Instance | BindingFlags.NonPublic);
        }

        public override void Unload()
        {
            bestiaryKeyField = null;
        }

        public bool TryBestiaryDescription(List<TooltipLine> tooltips)
		{
			if (!catchType.ValidNPC()) return false;

			// now this is the tricky part , idk if the npc will find its entry correctly without shooting itself
			var bestiaryEntry = Main.BestiaryDB.FindEntryByNPCID(catchType.Id);
			if (bestiaryEntry == null || bestiaryEntry.Info == null) return false;

			foreach (var infoNode in bestiaryEntry.Info)
			{
				// somehow we need reflection just for this... wow
				if (infoNode is FlavorTextBestiaryInfoElement element)
				{
					string keyValue = (string)bestiaryKeyField.GetValue(element);

					// we word wrap this chud
					var list = Helpme.WordWrap(Language.GetText(keyValue).Value,50);
					for (int i = 0; i < list.Count; i++)
					{
						tooltips.Add(new TooltipLine(Mod,"Bestiary_"+i,list[i]));	
					}
					return true;
				}
			}

			return false;
		}
        public override void ModifyTooltips(List<TooltipLine> tooltips)
        {
			if (!TryBestiaryDescription(tooltips))
			{
				tooltips.Add(new TooltipLine(Mod,"BestiaryNone","No description found for this NPC"));
			}

			if (catchType.notIntended)
			{
				tooltips.Add(new TooltipLine(Mod,"Intented","Might not be intended to catch") {OverrideColor = Color.LightYellow});
			}

			foreach (var item in tooltips)
			{
				if (item.Name == "ItemName")
				{
					item.Text = Lang.GetNPCNameValue(catchType.Id);
				}
			}
        }

        public override bool PreDrawInInventory(SpriteBatch spriteBatch, Vector2 position, Rectangle frame, Color drawColor, Color itemColor, Vector2 origin, float scale)
        {
			// Check id
			if (!catchType.ValidNPC()) return true;

			// Check texture
			Main.instance.LoadNPC(catchType.Id);
			Texture2D texture = Terraria.GameContent.TextureAssets.Npc[catchType.Id].Value;
			if (texture == null) return true;

			int frameCount = Main.npcFrameCount[catchType.Id];

			Helpme.DrawInventory(spriteBatch,position,drawColor,texture,frameCount);
			
            return false;
        }
    }

	public class CatchedProjectile : ModItem
	{

        public override string Texture => "Catchable/Items/Dot";
		public CatchType catchType;
        public override void NetSend(BinaryWriter writer)
		{
			catchType.VerifyProjID();
			catchType.NetSend(writer);
		}
        public override void NetReceive(BinaryReader reader)
		{
			catchType.NetReceive(reader);
			catchType.VerifyProjID();
		}
        public override void SaveData(TagCompound tag)
		{
			catchType.VerifyProjID();
			catchType.SaveData(tag);
		}
        public override void LoadData(TagCompound tag)
		{
			catchType.LoadData(tag);
			catchType.VerifyProjID();
		}

		// Stacking
        public override bool CanStack(Item source)
        {
			if (source.ModItem != null && source.ModItem is CatchedProjectile target)
			{
				if ( target.catchType.Id == catchType.Id)
				{
					return true;
				}
			}
            return false;
        }

		public override void SetDefaults() 
		{
			Item.damage = 1;
			Item.useStyle = ItemUseStyleID.Swing;
			Item.autoReuse = true;
			Item.useTurn = true;
			Item.useAnimation = 15;
			Item.useTime = 15;
			Item.maxStack = 999;
			Item.consumable = true;
			Item.width = 12;
			Item.height = 12;
			Item.noUseGraphic = true;
			// Item.shoot = proj;
			Item.shootSpeed = 11f;
		}

        public override void ModifyTooltips(List<TooltipLine> tooltips)
        {
			if (catchType.notIntended)
			{
				tooltips.Add(new TooltipLine(Mod,"Intented","Might not be intended to catch") {OverrideColor = Color.LightYellow});
			}

			foreach (var item in tooltips)
			{
				if (item.Name == "ItemName")
				{
					item.Text = Lang.GetProjectileName(catchType.Id).Value;
				}
			}
        }

		public override void UpdateInventory(Player player)
		{
			Item.shoot = catchType.Id;
			if (player.HeldItem != null && player.HeldItem.useAmmo > 0)
			{
				Item.ammo = player.HeldItem.useAmmo;
			}

		}

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
			var projectile = Projectile.NewProjectileDirect(source, position,velocity,type,damage,knockback,player.whoAmI);
			projectile.hostile = false;
			projectile.friendly = true;
            return false;
        }
        public override bool PreDrawInInventory(SpriteBatch spriteBatch, Vector2 position, Rectangle frame, Color drawColor, Color itemColor, Vector2 origin, float scale)
        {
			// Check id
			if (!catchType.ValidProj()) return true;

			// Check texture
			Main.instance.LoadProjectile(catchType.Id);
			Texture2D texture = Terraria.GameContent.TextureAssets.Projectile[catchType.Id].Value;
			if (texture == null) return true;

			int frameCount = Main.projFrames[catchType.Id];

			Helpme.DrawInventory(spriteBatch,position,drawColor,texture,frameCount);
			
            return false;
        }
    }

	public class Amongus : GlobalProjectile
	{
        public override void OnSpawn(Projectile projectile, IEntitySource source)
        {
            if (source is EntitySource_ItemUse_WithAmmo ammoSource )
			{
				if (ammoSource.AmmoItemIdUsed == ModContent.ItemType<CatchedProjectile>())
				{
					projectile.hostile = false;
					projectile.friendly = true;
				}
			}
        }
    }
}
