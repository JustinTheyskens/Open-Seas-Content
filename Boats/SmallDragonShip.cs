using System;
using Server;
using Server.Items;

namespace Server.Multis
{
    public class SmallDragonShip : BaseShip
    {
        public override int NorthID { get { return 0x4; } }
        public override int EastID { get { return 0x5; } }
        public override int SouthID { get { return 0x6; } }
        public override int WestID { get { return 0x7; } }

        public override int HoldDistance { get { return 4; } }
        public override int TillerManDistance { get { return -4; } }

        public override Point2D StarboardOffset { get { return new Point2D(2, 0); } }
        public override Point2D PortOffset { get { return new Point2D(-2, 0); } }

        public override Point3D MarkOffset { get { return new Point3D(0, 1, 3); } }

        public override int MaxCannons { get { return 4; } }
        public override int StartingMaxCannons { get { return 2; } }

        public override Point2D[] CannonLocations {  get { return _Locs; } }
        private Point2D[] _Locs = new Point2D[]
        {
            new Point2D(-2, 1), new Point2D(2, 1), new Point2D(-2, -1), new Point2D(2, -1)
        };

        public override BaseDockedBoat DockedBoat { get { return new SmallDockedDragonShip(this); } }

        [Constructable]
        public SmallDragonShip(Direction d) : base(d)
        {
        }

        public SmallDragonShip(Serial serial) : base(serial)
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

    public class SmallDragonShipDeed : BaseBoatDeed
    {
        public override int LabelNumber { get { return 1041205; } } // small ship deed
        public override BaseBoat Boat { get { return new SmallDragonShip(this.BoatDirection); } }

        [Constructable]
        public SmallDragonShipDeed() : base(0x0, Point3D.Zero)
        {
        }

        public SmallDragonShipDeed(Serial serial) : base(serial)
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

    public class SmallDockedDragonShip : BaseDockedBoat
    {
        public override int LabelNumber { get { return 1116741; } } //Small Ship
        public override BaseBoat Boat { get { return new SmallDragonShip(this.BoatDirection); } }

        public SmallDockedDragonShip(BaseBoat boat) : base(0x0, Point3D.Zero, boat)
        {
        }

        public SmallDockedDragonShip(Serial serial) : base(serial)
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
