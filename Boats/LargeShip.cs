using System;
using Server;
using Server.Items;

namespace Server.Multis
{
    public class LargeShip : BaseShip
    {
        public override int NorthID { get { return 0x10; } }
        public override int EastID { get { return 0x11; } }
        public override int SouthID { get { return 0x12; } }
        public override int WestID { get { return 0x13; } }

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

        public override BaseDockedBoat DockedBoat { get { return new LargeDockedShip(this); } }

        [Constructable]
        public LargeShip(Direction d) : base(d)
        {
        }

        public LargeShip(Serial serial) : base(serial)
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

    public class LargeShipDeed : BaseBoatDeed
    {
        public override int LabelNumber { get { return 1041209; } } // large ship deed
        public override BaseBoat Boat { get { return new LargeShip(this.BoatDirection); } }

        [Constructable]
        public LargeShipDeed() : base(0x10, new Point3D(0, -1, 0))
        {
        }

        public LargeShipDeed(Serial serial) : base(serial)
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

    public class LargeDockedShip : BaseDockedBoat
    {
        public override int LabelNumber { get { return 1116745; } } //Large Ship
        public override BaseBoat Boat { get { return new LargeShip(this.BoatDirection); } }

        public LargeDockedShip(BaseShip Ship) : base(0x10, new Point3D(0, -1, 0), Ship)
        {
        }

        public LargeDockedShip(Serial serial) : base(serial)
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
