using Server.Items;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server.Multis
{
    public class FishingBoat : BaseBoat
    {
        public override int NorthID { get { return 0x3C; } }
        public override int EastID { get { return 0x3D; } }
        public override int SouthID { get { return 0x3E; } }
        public override int WestID { get { return 0x3F; } }
        public override int MaxHits { get { return 5000; } }
        public override int TillerManDistance { get { return 4; } }
        [CommandProperty(AccessLevel.GameMaster)]
        public BoatBacking Backing { get; set; } 
        public override BaseDockedBoat DockedBoat { get { return new DockedFishingBoat(this); } }

        [Constructable]
        public FishingBoat(Direction direction)
            : base(direction, false)
        {
            Name = "Fishing Boat";

            Backing = new BoatBacking(this);
            Backing.SetFacing(direction);
            Backing.Map = Map;

            this.TillerMan = new TillerMan(this);

            Timer.DelayCall(TimeSpan.FromSeconds(0.25), () =>
            {
                UpdateComponents();
                ((Item)TillerMan).Map = Map.Internal;
            });        
        }

        public FishingBoat(Serial serial)
            : base(serial) { }

        public override bool SetFacing(Direction facing)
        {
            base.SetFacing(facing);

            UpdateComponents();
            if (Backing != null)
            {
                Backing.SetFacing(facing);
            }
                

            return true;
        }

        public override void UpdateComponents()
        {
            base.UpdateComponents();

            if (Backing != null)
            {
                if (Backing.Map != Map)
                    Backing.Map = Map;

                int xOffset = 0, yOffset = 0;
                //Movement.Movement.Offset(Facing, ref xOffset, ref yOffset);
                switch(Facing)
                {
                    case Direction.North: xOffset = 0; yOffset = 1; break;
                    case Direction.South: xOffset = 0; yOffset = -1; break;
                    case Direction.East: yOffset = 0; xOffset = -1; break;
                    case Direction.West: yOffset = 0; xOffset = 1; break;
                }

                
                Backing.Location = new Point3D(X + (xOffset * TillerManDistance), Y + (yOffset * TillerManDistance), Backing.Z);
                Backing.SetFacing(Facing);
                Backing.InvalidateProperties();
            }
        }

        /*
        public override void GetProperties(ObjectPropertyList list)
        {

            if (Name != null)
                list.Add(Name);

            int health = (int)(Hits * 100 / MaxHits);

            if (health >= 75)
            {
                list.Add(1158886, health.ToString());
            }
            else if (health >= 50)
            {
                list.Add(1158887, health.ToString());
            }
            else if (health >= 25)
            {
                list.Add(1158888, health.ToString());
            }
            else if (health >= 0)
            {
                list.Add(1158889, health.ToString());
            }

            base.GetProperties(list);
        }
        */

        public override void OnLocationChange(Point3D old)
        {
            base .OnLocationChange(old);

            if (Backing != null)
            {
                Backing.Location = new Point3D(X + (Backing.X - old.X), Y + (Backing.Y - old.Y), Z + (Backing.Z - old.Z));
            }
        }

        public override void OnMapChange()
        {
            base.OnMapChange();

            if (Backing != null)
            {
                Backing.Map = Map;
            }
        }

        public override bool CanMoveOver(IEntity entity)
        {

            if (entity is BoatBacking)
                return true;
            else
                return base.CanMoveOver(entity);
        }


        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();

            Backing = reader.ReadItem() as BoatBacking;
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write((int)0);
            writer.Write(Backing);
        }

    }

    public class FishingBoatDeed : BaseBoatDeed
    {
        //public override int LabelNumber { get { return 1041205; } } // small ship deed
        public override BaseBoat Boat { get { return new FishingBoat(this.BoatDirection); } }

        [Constructable]
        public FishingBoatDeed()
            : base(0x3C, Point3D.Zero)
        {
            Name = "Fishing Boat";
        }

        public FishingBoatDeed(Serial serial)
            : base(serial) { }

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

    public class DockedFishingBoat : BaseDockedBoat
    {
        public override int LabelNumber { get { return 1116741; } } //Small Ship
        public override BaseBoat Boat { get { return new FishingBoat(this.BoatDirection); } }

        public DockedFishingBoat(BaseBoat boat) : base(0x0, Point3D.Zero, boat)
        {
            Name = "Fishing Boat";
        }

        public DockedFishingBoat(Serial serial) : base(serial)
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
