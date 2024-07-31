using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Server.Items;

namespace Server.Multis
{
    public class SeaFortStronghold : BaseSeaFort
    {
        public override int MaxHits { get { return 35000; } }

        public override Point2D ControllerLocation { get { return new Point2D(4, -3); } }

        public override Point3D[] ChestLocations { get { return _ChestLocation; } }
        private Point3D[] _ChestLocation = {
            new Point3D(0, 0, 6), new Point3D(-4, -1, 30), new Point3D(-4, -2, 30),
            new Point3D(0, -5, 6), new Point3D(-4, 0, 6), new Point3D(-4, -1, 6),
            new Point3D(5, -5, 6), new Point3D(-6, 5, 6), new Point3D(0, 5, 6) };
        public override Point2D[] Entrances { get { return new Point2D[] { new Point2D(10, 0), new Point2D(-11, 0), new Point2D(-1, 10), new Point2D(-1, -11), new Point2D(0, 0) }; } }

        [Constructable]
        public SeaFortStronghold()
            : base(0x157)
        {
            Name = "Sea Fort Stronghold";
            SeaFortInfo info1 = new SeaFortInfo(this, 0x041F);
            Infos.Add(info1);
            SeaFortInfo info2 = new SeaFortInfo(this, 0xB2DB);
            Infos.Add(info2);
            SeaFortInfo info3 = new SeaFortInfo(this, 0xA954);
            Infos.Add(info3);
            SeaFortInfo info4 = new SeaFortInfo(this, 0xADE2);
            Infos.Add(info4);

            Timer.DelayCall(TimeSpan.FromSeconds(0.25), () =>
            {
                info1.MoveToWorld(new Point3D(X + 9, Y - 2, Z + 6), Map); // orc standard 
                info2.MoveToWorld(new Point3D(X - 1, Y + 5, Z + 39), Map); // banner
                info3.MoveToWorld(new Point3D(X - 7, Y + 3, 6 + 6), Map); // barbarian display
                info4.MoveToWorld(new Point3D(X - 2, Y - 10, Z + 6), Map); // cage up
                
            });
        }

        public SeaFortStronghold(Serial serial)
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
