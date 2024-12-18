using Terraria;
using Terraria.ModLoader;

namespace Catchable.Items
{

    public class SuperBugNet : ModItem
    {

    }
    public class SuperBugNetProj : ModProjectile
    {

        public const int Instances_Max = 100;
        public static int[] Instances;

        public override void Load()
        {
            ResetInstances();
        }

        public override void Unload()
        {
            Instances = null;
        }

        public static void ResetInstances()
        {
            Instances = new int[Instances_Max];
            for (int i = 0; i < Instances.Length; i++)
            {
                Instances[i] = -1;
            }
        }

        public int instanceKey 
        {
            get { return (int)Projectile.ai[1]; }
            set { Projectile.ai[1] = value;}
        }

        public void DeleteKey()
        {
            if (instanceKey == -1)
            {
                Main.NewText("Error , key already not existed in the first place");
                return;
            }

            Instances[instanceKey] = -1;
            instanceKey = -1;
        }

        public void AssignKey()
        {
            if (instanceKey == -1)
            {
                for (int i = 0; i < Instances.Length; i++)
                {
                    if (Instances[i] == -1)
                    {
                        Instances[i] = Projectile.whoAmI;
                        instanceKey = i;
                        Main.NewText("Sucessfully assigned key to "+i);
                    }
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
    }
}