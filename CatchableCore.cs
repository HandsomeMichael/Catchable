using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;

namespace Catchable
{
	public struct CatchTypeNPC
	{
		internal string _mod;
		internal string _name;
		internal int _id;

		public bool IsInvalid() => GetLiteralID() == -6969;

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
				return npc.Type;
			}

			return -6969;
		}
	}
	public class CatchedItem : ModItem
	{
		public CatchTypeNPC catchType;

        public override void SaveData(TagCompound tag)
        {
            base.SaveData(tag);
        }

        public override void LoadData(TagCompound tag)
        {
            base.LoadData(tag);
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

        public override void SetDefaults()
        {
            base.SetDefaults();
        }
    }
}
