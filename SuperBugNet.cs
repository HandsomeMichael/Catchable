using Terraria;
using Terraria.ModLoader;

namespace Catchable
{

    public class SuperBugNet : ModItem
    {

    }
    public abstract class SuperBugNetProj : ModProjectile
    {
        public static short[] Instances;

        public static void ResetInstances()
        {
            for (int i = 0; i < Instances.Length; i++)
            {
                Instances[i] = -1;
            }
        }

        public short instanceKey 
        {
            get { return (short)Projectile.ai[0]; }
            set { Projectile.ai[0] = value;}
        }

        public void DeleteKey()
        {
            if (instanceKey == -1)
            {
                Main.NewText("Error , key already not existed in the first place");
                return;
            }

            Instances[instanceKey] = -1;
        }

        public void AssignKey()
        {
            if (instanceKey == -1)
            {
                for (int i = 0; i < Instances.Length; i++)
                {
                    
                }
            }
        }

        public override void SetDefaults()
        {
            instanceKey = -1;
        }

        public override void AI()
        {
            AssignKey();
        }

        public override void OnKill(int timeLeft)
        {
            DeleteKey();
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            base.OnHitNPC(target, hit, damageDone);
        }
    }
}