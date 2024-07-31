using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Server.Items;

namespace Server.Multis
{
    public class ShipSeaFort : BaseSeaFort
    {
        public override Point2D ControllerLocation { get { return new Point2D(0, 1); } }

        public override Point3D[] ChestLocations { get { return _ChestLocations; } }
        private Point3D[] _ChestLocations = { new Point3D(0, -4, 0), new Point3D(1, -4, 0) };
        public override Point2D[] Entrances { get { return new Point2D[] { new Point2D(5, 0), new Point2D(-5, 0), new Point2D(0, 0) }; } }

        [Constructable]
        public ShipSeaFort()
            : base(0x158)
        {
            Name = "Ship Sea Fort";
            SeaFortInfo info = new SeaFortInfo(this, 0xA379);
            Infos.Add(info);

            Timer.DelayCall(TimeSpan.FromSeconds(0.25), () =>
            {
                info.MoveToWorld(new Point3D(X, Y + 1, Z + 35), Map);
            });
        }

        public ShipSeaFort(Serial serial)
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
