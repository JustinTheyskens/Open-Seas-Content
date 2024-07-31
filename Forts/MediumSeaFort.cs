using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Server.Items;

namespace Server.Multis
{
    public class MediumSeaFort : BaseSeaFort
    {
        public override int MaxHits { get { return 25000; } }
        public override Point2D ControllerLocation { get { return new Point2D(1, -3); } }

        public override Point3D[] ChestLocations { get { return _ChestLocation; } }
        private Point3D[] _ChestLocation = { new Point3D(-3, 1, 6), new Point3D(-3, 2, 6), new Point3D(-1, -4, 6), new Point3D(-2, 2, 32) };
        public override Point2D[] Entrances { get { return new Point2D[] { new Point2D(6, 0), new Point2D(0, 8), new Point2D(0, -9), new Point2D(0, 0) }; } }

        [Constructable]
        public MediumSeaFort()
            : base(0x154)
        {
            Name = "Medium Sea Fort";

            SeaFortInfo info1 = new SeaFortInfo(this, 0x042E);
            Infos.Add(info1);
            SeaFortInfo info2 = new SeaFortInfo(this, 0x042E);
            Infos.Add(info2);
            SeaFortInfo info3 = new SeaFortInfo(this, 0x042F);
            Infos.Add(info3);
            SeaFortInfo info4 = new SeaFortInfo(this, 0x042F);
            Infos.Add(info4);
            SeaFortInfo info5 = new SeaFortInfo(this, 0x0427);
            Infos.Add(info5);
            SeaFortInfo info6 = new SeaFortInfo(this, 0x0426);
            Infos.Add(info6);

            Timer.DelayCall(TimeSpan.FromSeconds(0.25), () =>
            {
                info1.MoveToWorld(new Point3D(X + 4, Y - 1, Z + 6), Map);
                info2.MoveToWorld(new Point3D(X + 4, Y + 3, Z + 6), Map);

                info3.MoveToWorld(new Point3D(X + 4, Y - 2, Z + 6), Map);
                info4.MoveToWorld(new Point3D(X + 4, Y + 2, Z + 6), Map);

                info5.MoveToWorld(new Point3D(X - 3, Y + 6, Z + 6), Map);
                info6.MoveToWorld(new Point3D(X - 2, Y + 6, Z + 6), Map);
            });
        }

        public MediumSeaFort(Serial serial)
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
