using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Server.Items;

namespace Server.Multis
{
    public class SmallSeaFort : BaseSeaFort
    {
        public override Point2D ControllerLocation { get { return new Point2D(0, 1); } }

        public override Point3D[] ChestLocations { get { return _ChestLocations; } }
        private Point3D[] _ChestLocations = { new Point3D(-4, 2, 6), new Point3D(-4, -2, 6), new Point3D(-10, 0, 6), new Point3D(-8, -2, 6) };
        public override Point2D[] Entrances { get { return new Point2D[] { new Point2D(0, 3), new Point2D(0, -4), new Point2D(0, 4), new Point2D(0, -5), new Point2D(0, 0) }; } }

        [Constructable]
        public SmallSeaFort()
            : base(0x156)
        {
            Name = "Small Sea Fort";

            SeaFortInfo info1 = new SeaFortInfo(this, 0xA4CE); // 0xA37A);
            Infos.Add(info1);
            SeaFortInfo info2 = new SeaFortInfo(this, 0xA947);
            Infos.Add(info2);

            Timer.DelayCall(TimeSpan.FromSeconds(0.25), () =>
            {
                info1.MoveToWorld(new Point3D(X + 4, Y - 1, Z + 6), Map); // barrel of bones
                info2.MoveToWorld(new Point3D(X - 7, Y - 1, Z + 6), Map); // throne
            });
        }

        public SmallSeaFort(Serial serial)
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
