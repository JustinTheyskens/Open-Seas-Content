using Server;
using System;
using Server.Multis;
using Server.Mobiles;
using Server.Targeting;

namespace Server.Items
{
    /*
    public enum CannonPower
    {
        Light,
        Heavy,
        Massive,
        Pumpkin
    }
    */

    public abstract class NewShipCannonDeed : Item
    {
        public abstract CannonPower CannonType { get; }

        public NewShipCannonDeed() : base(5362)
        {
        }

        public override void OnDoubleClick(Mobile from)
        {
            if (IsChildOf(from.Backpack))
            {
                BaseShip galleon = BaseShip.FindShipAt(from, from.Map);

                if (galleon != null)
                {
                    if (galleon.Owner == from)
                    {
                        from.Target = new InternalTarget(this, galleon);
                    }
                    else
                    {
                        from.SendLocalizedMessage(1116627); // You must be the owner of the ship to do this.
                    }
                }
                else
                    from.SendLocalizedMessage(1116625); //You must be on the ship to deploy a weapon.
            }
        }

        private class InternalTarget : Target
        {
            public NewShipCannonDeed Deed { get; set; }
            public BaseShip Ship { get; set; }

            public InternalTarget(NewShipCannonDeed deed, BaseShip ship)
                : base(2, false, TargetFlags.None)
            {
                Deed = deed;
                Ship = ship;
            }

            protected override void OnTarget(Mobile from, object targeted)
            {
                Map map = from.Map;

                if (targeted is IPoint3D)
                {
                    Point3D pnt = new Point3D((IPoint3D)targeted);

                    var ship = BaseShip.FindShipAt(new Point2D(pnt.X, pnt.Y), map);

                    if (ship != null && Ship == ship)
                    {
                        ship.TryAddCannon(from, pnt, Deed);
                    }
                }
            }
        }

        public NewShipCannonDeed(Serial serial) : base(serial) { }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write((int)0);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();
        }
    }

    public class SmallCannonDeed : NewShipCannonDeed
    {
        public override CannonPower CannonType { get { return CannonPower.Light; } }
        public override int LabelNumber { get { return 1095793; } }

        [Constructable]
        public SmallCannonDeed()
        {
            Name = "Small Ship Cannon";
            Hue = 1115;
        }

        public SmallCannonDeed(Serial serial) : base(serial) { }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write((int)0);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();
        }
    }

    public class LargeCannonDeed : NewShipCannonDeed
    {
        public override CannonPower CannonType { get { return CannonPower.Heavy; } }
        public override int LabelNumber { get { return 1095794; } }

        [Constructable]
        public LargeCannonDeed()
        {
            Name = "Large Ship Cannon";
            Hue = 1118;
        }

        public LargeCannonDeed(Serial serial) : base(serial) { }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write((int)0);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();
        }
    }

    public class PowerfulCannonDeed : NewShipCannonDeed
    {
        public override CannonPower CannonType { get { return CannonPower.Light; } }
        public override int LabelNumber { get { return 1095793; } }

        [Constructable]
        public PowerfulCannonDeed()
        {
            Name = "Powerful Ship Shannon";
            Hue = 1114;
        }

        public PowerfulCannonDeed(Serial serial) : base(serial) { }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write((int)0);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();
        }
    }
}
