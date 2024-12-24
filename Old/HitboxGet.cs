using Microsoft.Xna.Framework;
using Terraria.ModLoader;

namespace ZZZCatchNPC
{
	public class HitboxGet : ModPlayer
	{
		public Rectangle itemhitbox;

		public override void ResetEffects()
		{
			itemhitbox = new Rectangle(0, 0, 0, 0);
		}
	}
}
