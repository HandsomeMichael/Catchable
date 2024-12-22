using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Reflection;
using System.Text.Json.Serialization;

using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;
using Terraria.ModLoader.Config;

namespace Catchable
{
	public class CatchableConfig : ModConfig
	{
		public override ConfigScope Mode => ConfigScope.ServerSide;
		public static CatchableConfig Get => ModContent.GetInstance<CatchableConfig>();

        // save the config , this requires reflection though.
        public static void SaveConfig() => typeof(ConfigManager).GetMethod("Save", BindingFlags.Static | BindingFlags.NonPublic).Invoke(null, new object[1] { Get });

		[DefaultValue(true)] 
		public bool CatchProjectile;

        [DefaultValue(true)] 
		public bool CatchNPC; 

        [DefaultValue(true)] 
		public bool CatchNPC_Bosses;

		[DefaultValue(true)] 
		public bool ProjectileAmmo;
	}
}