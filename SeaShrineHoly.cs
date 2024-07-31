using Server.Items;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server.Multis
{
    public class SeaShrineHoly : BaseSeaShrine
    {
        [Constructable]
        public SeaShrineHoly()
            : base(0x15A)
        {
            Name = "Sunken Holy Shrine";
            Timer.DelayCall(TimeSpan.FromSeconds(0.25), () =>
            {
                AddTeleporter(0x07A3, new Point3D(5, 0, -3));
                AddTeleporter(0x07A3, new Point3D(5, -1, -3));

                AddTeleporter(0x07A3, new Point3D(-6, 0, -3));
                AddTeleporter(0x07A3, new Point3D(-6, -1, -3));

                AddTeleporter(0x07A3, new Point3D(0, 5, -3));
                AddTeleporter(0x07A3, new Point3D(-1, 5, -3));

                AddTeleporter(0x07A3, new Point3D(0, -6, -3));
                AddTeleporter(0x07A3, new Point3D(-1, -6, -3));
            });
        }

        public override void DoBuffEffects()
        {
            SeaShrineBuffEffect buff = new SeaShrineBuffEffect(0xB6DB, 1281);
            Timer.DelayCall(TimeSpan.FromSeconds(0.2), () =>
            {
                buff.MoveToWorld(new Point3D(X, Y, Z + 21), Map);
                Addons.Add(buff);
            });
        }

        public SeaShrineHoly(Serial serial)
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
