using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server.Multis
{
    public class SeaShrineArcane : BaseSeaShrine
    {
        public override int EffectHue { get { return 1461; } }
        [Constructable]
        public SeaShrineArcane()
            : base(0x159)
        {
            Name = "Sunken Arcane Shrine";
            Timer.DelayCall(TimeSpan.FromSeconds(0.25), () =>
            {
                AddTeleporter(0x07A3, new Point3D(0, 4, 0));
                AddTeleporter(0x07A3, new Point3D(-1, 4, 0));

                AddTeleporter(0x07A3, new Point3D(4, 0, 0));
                AddTeleporter(0x07A3, new Point3D(4, -1, 0));
                AddTeleporter(0x07A3, new Point3D(-5, 0, 0));

                AddTeleporter(0x07A3, new Point3D(0, -4, 0));
                AddTeleporter(0x07A3, new Point3D(-1, -4, 0));
            });
        }
        public SeaShrineArcane(Serial serial)
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
