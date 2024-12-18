using Terraria;
using Terraria.DataStructures;
using Terraria.ModLoader;

namespace Catchable
{
    public class SpawnCatchableNPC : ModCommand
	{
		// CommandType.Chat means that command can be used in Chat in SP and MP
		public override CommandType Type=> CommandType.Chat;

		// The desired text to trigger this command
		public override string Command => "cnpc";

		// A short usage explanation for this command
		public override string Usage
			=> "/cnpc <type|name>" +
			"\n type — NPCID of NPC." +
			"\n name — name of NPC in current localization." +
			"\n Replace spaces in item name with underscores.";

		// A short description of this command
		public override string Description
			=> "Spawn an NPC Item by name or by typeId";

		public override void Action(CommandCaller caller, string input, string[] args) {
			// Checking input Arguments
			if (args.Length == 0)
				throw new UsageException("At least one argument was expected.");

			// If we can't parse the int, it means we have a name (or a wrong use of the command)
			// In that case type be equal to 0
			if (!int.TryParse(args[0], out int type)) {
				// Replacing the underscore in an element name with spaces
				string name = args[0].Replace("_", " ");

				// We go through all the subjects to find the required typeId
				// Only if the name of the item matches the desired one in the current localization (no case sensitive) 
				for (int k = 1; k < ItemLoader.ItemCount; k++) {
					if (name.ToLower() == Lang.GetItemNameValue(k).ToLower()) {
						type = k;
						break;
					}
				}
			}

			if (type <= 0 || type >= ItemLoader.ItemCount)
				throw new UsageException(string.Format("Unknown item — Must be valid name or 0 < type < {0}", ItemLoader.ItemCount));

			// If the command has at least two arguments, we try to get the stack value
			// Default stack is 1
			int stack = 1;
			if (args.Length >= 2) {
				if (!int.TryParse(args[1], out stack))
					throw new UsageException("Stack value must be integer, but met: " + args[1]);
			}

			// Spawn the item where the calling player is
			int i = caller.Player.QuickSpawnItem(new EntitySource_DebugCommand($"{nameof(Mod)}_balls"), ModContent.ItemType<CatchedNPC>(), stack);
            var item = Main.item[i].ModItem as CatchedNPC;
            item.catchType.SetTerraria(type);
		}
	}
}