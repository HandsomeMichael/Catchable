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
		public const int InvalidID = -696969;
		internal string _mod;
		internal string _name;
		internal int _id;

		/// <summary>
		/// The npc type , might require updating before shit happen
		/// </summary>
		public int Id => _id;

		public CatchType(NPC npc)
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
		}

		public void NetSend(BinaryWriter writer)
		{
			writer.Write(_mod);
			writer.Write(_name);
			writer.Write(_id);
		}

		public void NetReceive(BinaryReader reader)
        {
			_mod = reader.ReadString();
			_name = reader.ReadString();
			_id = reader.ReadInt32(); // idk if this right or not
        }

		public void SaveData(TagCompound tag)
		{
			tag.Add("mod",_mod);
			tag.Add("name",_name);
			tag.Add("id",_id);
		}

		public void LoadData(TagCompound tag)
		{
			_mod = tag.GetString("mod");
			_name = tag.GetString("name");
			_id = tag.GetInt("id");
		}	

		public bool IsInvalid() => GetLiteralID() == InvalidID;

		/// <summary>
		/// Get NPC ID while also properly checking it
		/// </summary>
		/// <returns></returns>
		/// <exception cref="Exception"> How the fuck did it fuck up that bad </exception>
		public int GetLiteralID()
		{
			if (_mod == "Terraria")
			{
				return _id;
			}

			if (ModContent.TryFind(_mod, _name, out ModNPC npc))
			{
				if (npc.Type != _id)
				{
					// Bad shit happen
					Main.NewText("Im sorry but i think you removed some mods, yeah thats it lol");
					_id = npc.Type;
				}

				// Might remove if my friend give me nachos
				if (npc.Type > NPCLoader.NPCCount)
				{
					throw new Exception("Uhhh. did you broke tmodloader");
				}

				return npc.Type;
			}

			// I remember terraria somehow had negative ids so just incase
			return InvalidID;
		}
	}

	public class CatchedNPCItem : ModItem
	{
		public static Dictionary<int,FrameHelp> npcFrames;

		public CatchType catchType;

        public override void NetSend(BinaryWriter writer){catchType.NetSend(writer);}
        public override void NetReceive(BinaryReader reader){catchType.NetReceive(reader);}
        public override void SaveData(TagCompound tag){catchType.SaveData(tag);}

        public override void LoadData(TagCompound tag)
        {
            catchType.LoadData(tag);
        }

        public override bool CanStack(Item source)
        {
			if (source.ModItem is CatchedNPCItem target)
			{
				if ( target.catchType.GetLiteralID() == catchType.GetLiteralID() )
				{
					return true;
				}
			}
            return false;
        }

        public override void ModifyTooltips(List<TooltipLine> tooltips)
        {
            tooltips.Add(new TooltipLine(Mod,"desc","Unknown NPC"));
        }

        public override bool PreDrawInInventory(SpriteBatch spriteBatch, Vector2 position, Rectangle frame, Color drawColor, Color itemColor, Vector2 origin, float scale)
        {
			// Asset checking
			if (catchType.IsInvalid()) return false;
			Texture2D texture = Terraria.GameContent.TextureAssets.Npc[catchType.Id].Value;
			if (texture == null) return false;

			// Actually drawing wow
			int frameCount = Main.npcFrameCount[catchType.Id];

			if (frameCount > 0)
			{
				spriteBatch.Draw(texture,position,null,drawColor,0f,origin,scale, SpriteEffects.None, 0f);
			}
			else 
			{
				spriteBatch.Draw(texture,position,null,drawColor,0f,origin,scale, SpriteEffects.None, 0f);
			}
			
            return false;
        }
    }
}
