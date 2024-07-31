using Server.Items;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server.Multis
{
    public class SeaShrineWarrior : BaseSeaShrine
    {
        [Constructable]
        public SeaShrineWarrior()
            : base(0x15C)
        {
            Name = "Ruins of a Warrior's Tomb";

            Timer.DelayCall(TimeSpan.FromSeconds(0.25), () =>
            {
                AddTeleporter(0xAF7B, new Point3D(-1, 6, -2));
                AddTeleporter(0xAF7B, new Point3D(-2, 6, -2));

                AddTeleporter(0xAF7B, new Point3D(6, -2, -2));
            });
        }

        public override void DoBuffEffects()
        {
            SeaShrineBuffEffect buff = new SeaShrineBuffEffect(0xBCEC);
            Timer.DelayCall(TimeSpan.FromSeconds(0.2), () =>
            {
                buff.MoveToWorld(new Point3D(X + 1, Y + 2, Z + 25), Map);
                Addons.Add(buff);
            });
        }

        public SeaShrineWarrior(Serial serial)
            : base(serial) { }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write((int)0); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();
        }
    }
}
