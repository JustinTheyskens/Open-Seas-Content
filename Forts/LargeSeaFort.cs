using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Server.Items;

namespace Server.Multis
{
    public class LargeSeaFort : BaseSeaFort
    {
        public override int MaxHits { get { return 30000; } }

        public override Point2D ControllerLocation { get { return new Point2D(0, 5); } }

        public override Point3D[] ChestLocations { get { return _ChestLocation; } }
        private Point3D[] _ChestLocation = {
            new Point3D(0, 0, 6), new Point3D(-2, -3, 6), new Point3D(-3, -3, 6),
            new Point3D(-6, -3, 6), new Point3D(2, 2, 32), new Point3D(-4, 2, 32)
        };

        public override Point2D[] Entrances { get { return new Point2D[] { new Point2D(10, 0), new Point2D(-1, 8), new Point2D(-1, -8), new Point2D(-11, 0), new Point2D(0, 0) }; } }

        [Constructable]
        public LargeSeaFort()
            : base(0x155)
        {
            Name = "Large Sea Fort";

            SeaFortInfo info1 = new SeaFortInfo(this, 0x1BE2);
            Infos.Add(info1);
            SeaFortInfo info2 = new SeaFortInfo(this, 0xADE2); // 0xA954);
            Infos.Add(info2);
            SeaFortInfo info3 = new SeaFortInfo(this, 0x1EA5);
            Infos.Add(info3);
            SeaFortInfo info4 = new SeaFortInfo(this, 0xB2DB);
            Infos.Add(info4);
            Timer.DelayCall(TimeSpan.FromSeconds(0.25), () =>
            {
                info1.MoveToWorld(new Point3D(X + 3, Y + 4, Z + 18), Map); // wood pile
                info2.MoveToWorld(new Point3D(X + 8, Y + 2, Z + 6), Map); // cage
                info3.MoveToWorld(new Point3D(X - 5, Y + 4, Z + 12), Map); // hanging net
                info4.MoveToWorld(new Point3D(X, Y - 1, Z + 29), Map); // banner

            });
        }

        public LargeSeaFort(Serial serial)
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
