using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;

namespace Catchable
{
	public struct CatchTypeNPC
	{
		public const int InvalidID = -696969;
		internal string _mod;
		internal string _name;
		internal int _id;

		public CatchTypeNPC(NPC npc)
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

	public class CatchedItem : ModItem
	{
		public CatchTypeNPC catchType;

        public override void SaveData(TagCompound tag)
        {
            catchType.SaveData(tag);
        }

        public override void LoadData(TagCompound tag)
        {
            catchType.LoadData(tag);
        }

        public override bool CanStack(Item source)
        {
			if (source.ModItem is CatchedItem target)
			{
				if ( target.catchType.GetLiteralID() == catchType.GetLiteralID() )
				{
					return true;
				}
			}
            return false;
        }

        public override bool PreDrawInInventory(SpriteBatch spriteBatch, Vector2 position, Rectangle frame, Color drawColor, Color itemColor, Vector2 origin, float scale)
        {
            return false;
        }
    }
}
