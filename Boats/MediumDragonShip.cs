using System;
using Server;
using Server.Items;

namespace Server.Multis
{
    public class MediumDragonShip : BaseShip
    {
        public override int NorthID { get { return 0xC; } }
        public override int EastID { get { return 0xD; } }
        public override int SouthID { get { return 0xE; } }
        public override int WestID { get { return 0xF; } }

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

        public override BaseDockedBoat DockedBoat { get { return new MediumDockedDragonShip(this); } }

        [Constructable]
        public MediumDragonShip(Direction d) : base(d)
        {
        }

        public MediumDragonShip(Serial serial) : base(serial)
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

    public class MediumDragonShipDeed : BaseBoatDeed
    {
        public override int LabelNumber { get { return 1041207; } }
        public override BaseBoat Boat { get { return new MediumDragonShip(this.BoatDirection); } }

        [Constructable]
        public MediumDragonShipDeed() : base(0x8, Point3D.Zero)
        {
        }

        public MediumDragonShipDeed(Serial serial) : base(serial)
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

    public class MediumDockedDragonShip : BaseDockedBoat
    {
        public override int LabelNumber { get { return 1116743; } }
        public override BaseBoat Boat { get { return new MediumDragonShip(this.BoatDirection); } }

        public MediumDockedDragonShip(BaseShip Ship) : base(0x8, Point3D.Zero, Ship)
        {
        }

        public MediumDockedDragonShip(Serial serial) : base(serial)
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
