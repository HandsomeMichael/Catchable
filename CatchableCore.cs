using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Catchable.Helper;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;

namespace Catchable
{

	// Still in progress , might cancel cuz requires data per stack
	// public struct ExtraNPCInfo
	// {
	// 	public static bool IsNecessary => true;
	// 	public int health;
	// 	public int[] ai;
	// }

	/// <summary>
	/// ModType if i was the one who implement it ( fucking worse )
	/// </summary>
	public struct CatchType
	{
		internal string _mod;
		internal string _name;
		internal int _id;

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
			VerifyID();
			writer.Write(_mod);
			writer.Write(_name);
			writer.Write(_id);
		}

		public void NetReceive(BinaryReader reader)
        {
			_mod = reader.ReadString();
			_name = reader.ReadString();
			_id = reader.ReadInt32(); // idk if this right or not
			VerifyID();
        }

		// One thing to notice is that terraria has auto save

		public void SaveData(TagCompound tag)
		{
			VerifyID();
			tag.Add("mod",_mod);
			tag.Add("name",_name);
			tag.Add("id",_id);
		}

		public void LoadData(TagCompound tag)
		{
			_mod = tag.GetString("mod");
			_name = tag.GetString("name");
			_id = tag.GetInt("id");
			VerifyID();
		}	

		/// <summary>
		/// Verify if id is valid
		/// </summary>
		/// <returns></returns>
		public string VerifyID()
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
	}

	public class CatchedNPC : ModItem
	{

        public override string Texture => "Catchable/Items/Dot";
		public CatchType catchType;
        public override void NetSend(BinaryWriter writer){catchType.NetSend(writer);}
        public override void NetReceive(BinaryReader reader)
		{
			catchType.NetReceive(reader);
			Item.makeNPC = catchType.Id;
		}
        public override void SaveData(TagCompound tag){catchType.SaveData(tag);}
        public override void LoadData(TagCompound tag)
		{
			catchType.LoadData(tag);
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


        public override void ModifyTooltips(List<TooltipLine> tooltips)
        {
            tooltips.Add(new TooltipLine(Mod,"desc","Unknown NPC"));
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
			if (catchType.Id == 0) return true;

			// Check texture
			Texture2D texture = Terraria.GameContent.TextureAssets.Npc[catchType.Id].Value;
			if (texture == null) return true;

			// Pulled it outta my ass
			int frameCount = Main.npcFrameCount[catchType.Id];
			int npcFrame = Helpme.MagicallyGetFrame(frameCount,6);
			var newFrame = Helpme.GetFrame(texture,npcFrame,frameCount);

			if (frameCount > 0)
			{
				spriteBatch.Draw(texture,position,newFrame,drawColor,0f,newFrame.Size() / 2f,scale, SpriteEffects.None, 0f);
			}
			else 
			{
				spriteBatch.Draw(texture,position,null,drawColor,0f,origin,scale, SpriteEffects.None, 0f);
			}
			
            return false;
        }
    }
}
