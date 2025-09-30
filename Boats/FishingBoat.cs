using Server.Items;

namespace Server.Multis
{
    public class FishingBoat : BaseBoat
    {
        public override int NorthID => 0x3C;
        public override int EastID => 0x3D;
        public override int SouthID => 0x3E;
        public override int WestID => 0x3F;

        public override int MaxHits => 5000;

        public override int TillerManDistance => 4;

        public override Point2D StarboardOffset => new Point2D(2, 0);
        public override Point2D PortOffset => new Point2D(-2, 0);

        public override BaseDockedBoat DockedBoat => new DockedFishingBoat(this);

        [Constructable]
        public FishingBoat(Direction direction)
            : base(direction, true)
        {
            Name = "Fishing Boat";
        }

        public FishingBoat(Serial serial)
            : base(serial)
        {
        }

        protected override void InitComponents(bool isClassic)
        {
            base.InitComponents(isClassic);

            if (isClassic)
            {
                if (TillerMan is IEntity e)
                {
                    e.Delete();
                }

                TillerMan = new BoatBacking(this) { Hue = TillerManHue };
            }
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write(1);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            var version = reader.ReadInt();

            if (version < 1)
            {
                if (TillerMan is IEntity e)
                {
                    e.Delete();
                }

                TillerMan = reader.ReadItem();
            }
        }
    }

    public class FishingBoatDeed : BaseBoatDeed
    {
        //public override int LabelNumber { get { return 1041205; } } // small ship deed
        public override BaseBoat Boat => new FishingBoat(BoatDirection);

        [Constructable]
        public FishingBoatDeed()
            : base(0x3C, Point3D.Zero)
        {
            Name = "Fishing Boat";
        }

        public FishingBoatDeed(Serial serial)
            : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write(0);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            _ = reader.ReadInt();
        }
    }

    public class DockedFishingBoat : BaseDockedBoat
    {
        public override int LabelNumber => 1116745;  //Small Ship
        public override BaseBoat Boat => new FishingBoat(BoatDirection);

        public DockedFishingBoat(BaseBoat boat)
            : base(0x0, Point3D.Zero, boat)
        {
            Name = "Fishing Boat";
        }

        public DockedFishingBoat(Serial serial)
            : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write(0);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            _ = reader.ReadInt();
        }
    }
}
