using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Catchable
{
    public class CatchableNPC : GlobalNPC
    {
        public override void PostAI(NPC npc)
        {
            if (CatchableSystem.BugNetCount > 0)
            {
                Main.NewText("cumming");
                int count = 0;
                for (int i = 0; i < Main.maxPlayers; i++)
                {
                    // Break if its already clear that there is no buggy netty
                    if (count >= CatchableSystem.BugNetCount) break;

                    var player = Main.player[i];
                    if (player != null && player.active && player.TryGetModPlayer<CatchablePlayer>(out var modplayer))
                    {
                        if (modplayer.bugnetting)
                        {
                            if (Main.projectile[player.heldProj].Colliding(player.Hitbox,npc.Hitbox))
                            {
                                CatchNPC(npc,i);
                            }
                        }
                    }
                }
            }
        }

        // Vanilla method of catching npc
        void CatchNPC(NPC npc, int who = -1)
		{
            int i = npc.whoAmI;
			if (!npc.active)return;
			if (who == -1)
			{
				who = Main.myPlayer;
			}
			if (Main.netMode == NetmodeID.MultiplayerClient)
			{
				npc.active = false;
				NetMessage.SendData(MessageID.BugCatching, -1, -1, null, i, who);
			}
			else
			{
				if (npc.type == 687)
				{
					TryTeleportingCaughtMysticFrog(npc);
				}
				else if (npc.SpawnedFromStatue)
				{
					Vector2 vector = npc.Center - new Vector2(20f);
					Utils.PoofOfSmoke(vector);
					npc.active = false;
					NetMessage.SendData(MessageID.SyncNPC, -1, -1, null, i);
					NetMessage.SendData(MessageID.PoofOfSmoke, -1, -1, null, (int)vector.X, vector.Y);
				}
				else
				{
                    var source = Main.player[who].GetSource_CatchEntity(npc);
                    int itemWhoAmI = 0;
                    if (npc.catchItem > 0)
                    {
                        itemWhoAmI = Item.NewItem(source,
                         (int)Main.player[who].Center.X,
                          (int)Main.player[who].Center.Y,
                           0, 0, Main.npc[i].catchItem, 1,
                            noBroadcast: true, 0, noGrabDelay: true);
                    }
                    else
                    {
                        itemWhoAmI = Item.NewItem(source,
                         (int)Main.player[who].Center.X,
                          (int)Main.player[who].Center.Y,
                           0, 0, ModContent.ItemType<CatchedNPC>(), 1,
                            noBroadcast: true, 0, noGrabDelay: true);

                        if (Main.item[itemWhoAmI].ModItem != null && Main.item[itemWhoAmI].ModItem is CatchedNPC boeingplane)
                        {
                            boeingplane.catchType = new CatchType(npc);
                        }
                    }
					NetMessage.SendData(MessageID.SyncItem, -1, -1, null, itemWhoAmI, 1f);
					npc.active = false;
					NetMessage.SendData(MessageID.SyncNPC, -1, -1, null, i);
				}
			}
		}

        // frog
        public bool TryTeleportingCaughtMysticFrog(NPC npc)
		{
            // Check
			if (Main.netMode == NetmodeID.MultiplayerClient)return false;
			if (npc.type != 687)return false;

			Vector2 chosenTile = Vector2.Zero;
			Point point = npc.Center.ToTileCoordinates();
			if (npc.AI_AttemptToFindTeleportSpot(ref chosenTile, point.X, point.Y, 15, 8))
			{
				Vector2 newPos = new Vector2(chosenTile.X * 16f - (float)(npc.width / 2), chosenTile.Y * 16f - (float)npc.height);
				NetMessage.SendData(MessageID.SyncNPC, -1, -1, null, npc.whoAmI);
				npc.Teleport(newPos, 13);
				return true;
			}
			Vector2 vector = npc.Center - new Vector2(20f);
			Utils.PoofOfSmoke(vector);
			npc.active = false;
			NetMessage.SendData(MessageID.SyncNPC, -1, -1, null, npc.whoAmI);
			NetMessage.SendData(MessageID.PoofOfSmoke, -1, -1, null, (int)vector.X, vector.Y);
			return false;
		}
    }

    

    public class CatchableProjectile : GlobalProjectile
    {
        public override void PostAI(Projectile projectile)
        {
            base.PostAI(projectile);
        }
    }

    public class CatchableSystem : ModSystem
    {
        public static ushort BugNetCount;
        public override void PreUpdateProjectiles()
        {
            BugNetCount = 0;
            
            foreach (var player in Main.player)
            {
                if (player.active)
                {
                    if (player.TryGetModPlayer<CatchablePlayer>(out var modPlayer))
                    {
                        if (modPlayer.bugnetting)
                        {
                            BugNetCount++;
                        }
                    }
                }
            }
        }
    }
}