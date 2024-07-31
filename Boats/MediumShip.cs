using System;
using Server;
using Server.Items;

namespace Server.Multis
{
    public class MediumShip : BaseShip
    {
        public override int NorthID { get { return 0x8; } }
        public override int EastID { get { return 0x9; } }
        public override int SouthID { get { return 0xA; } }
        public override int WestID { get { return 0xB; } }

        public override int HoldDistance { get { return 4; } }
        public override int TillerManDistance { get { return -5; } }

        public override Point2D StarboardOffset { get { return new Point2D(2, 0); } }
        public override Point2D PortOffset { get { return new Point2D(-2, 0); } }

        public override Point3D MarkOffset { get { return new Point3D(0, 1, 3); } }

        public override int MaxCannons { get { return 6; } }
        public override int StartingMaxCannons { get { return 2; } }
        public override Point2D[] CannonLocations { get { return _Locs; } }
        private Point2D[] _Locs = new Point2D[]
        {
            new Point2D(-2, 1), new Point2D(2, 1), new Point2D(-2, -1), new Point2D(2, -1), new Point2D(-2, -2), new Point2D(2, -2)
        };

        public override BaseDockedBoat DockedBoat { get { return new MediumDockedShip(this); } }

        [Constructable]
        public MediumShip(Direction d) : base(d)
        {
        }

        public MediumShip(Serial serial) : base(serial)
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

    public class MediumShipDeed : BaseBoatDeed
    {
        public override int LabelNumber { get { return 1041207; } }
        public override BaseBoat Boat { get { return new MediumShip(this.BoatDirection); } }

        [Constructable]
        public MediumShipDeed() : base(0x8, Point3D.Zero)
        {
        }

        public MediumShipDeed(Serial serial) : base(serial)
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

    public class MediumDockedShip : BaseDockedBoat
    {
        public override int LabelNumber { get { return 1116743; } }
        public override BaseBoat Boat { get { return new MediumShip(this.BoatDirection); } }

        public MediumDockedShip(BaseShip Ship) : base(0x8, Point3D.Zero, Ship)
        {
        }

        public MediumDockedShip(Serial serial) : base(serial)
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
