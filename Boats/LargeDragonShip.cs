using System;
using Server;
using Server.Items;

namespace Server.Multis
{
    public class LargeDragonShip : BaseShip
    {
        public override int NorthID { get { return 0x14; } }
        public override int EastID { get { return 0x15; } }
        public override int SouthID { get { return 0x16; } }
        public override int WestID { get { return 0x17; } }

        public override int HoldDistance { get { return 5; } }
        public override int TillerManDistance { get { return -5; } }

        public override Point2D StarboardOffset { get { return new Point2D(2, -1); } }
        public override Point2D PortOffset { get { return new Point2D(-2, -1); } }

        public override Point3D MarkOffset { get { return new Point3D(0, 0, 3); } }

        public override int MaxCannons { get { return 8; } }
        public override int StartingMaxCannons { get { return 4; } }

        public override Point2D[] CannonLocations { get { return _Locs; } }
        private Point2D[] _Locs = new Point2D[]
        {
            new Point2D(-2, 1), new Point2D(2, 1), new Point2D(-2, 0), new Point2D(2, 0), new Point2D(-2, -2), new Point2D(2, -2), new Point2D(-2, 2), new Point2D(2, 2)
        };

        public override BaseDockedBoat DockedBoat { get { return new LargeDockedDragonShip(this); } }

        [Constructable]
        public LargeDragonShip(Direction d) : base(d)
        {
        }

        public LargeDragonShip(Serial serial) : base(serial)
        {
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write((int)0);
        }
    }

    public class LargeDragonShipDeed : BaseBoatDeed
    {
        public override int LabelNumber { get { return 1041209; } } // large ship deed
        public override BaseBoat Boat { get { return new LargeDragonShip(this.BoatDirection); } }

        [Constructable]
        public LargeDragonShipDeed() : base(0x10, new Point3D(0, -1, 0))
        {
        }

        public LargeDragonShipDeed(Serial serial) : base(serial)
        {
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write((int)0);
        }
    }

    public class LargeDockedDragonShip : BaseDockedBoat
    {
        public override int LabelNumber { get { return 1116745; } } //Large Ship
        public override BaseBoat Boat { get { return new LargeShip(this.BoatDirection); } }

        public LargeDockedDragonShip(BaseShip Ship) : base(0x10, new Point3D(0, -1, 0), Ship)
        {
        }

        public LargeDockedDragonShip(Serial serial) : base(serial)
        {
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write((int)0);
        }
    }
}
