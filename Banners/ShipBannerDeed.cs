using Server;
using System;
using Server.Multis;
using Server.Mobiles;
using Server.Targeting;

namespace Server.Items
{
    public enum ShipBannerType
    {
        UO = 0,
        Tree = 1,
        Star = 2,
        SeaHorse = 3,
        Flower = 4,
        Sun = 5,
        Pentagram = 6,
        Riot = 7
    }

    public abstract class ShipBannerDeed : Item
    {
        public abstract ShipBannerType BannerType { get; }

        public ShipBannerDeed() : base(5362)
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
            public ShipBannerDeed Deed { get; set; }
            public BaseShip Ship { get; set; }

            public InternalTarget(ShipBannerDeed deed, BaseShip ship)
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
                        if (Ship.BannerUpgrade)
                            ship.AddBanner(from, Deed);
                        else
                            from.SendMessage("Your ship must have the Sail Banner upgrade before you can add this.");
                    }
                }
            }
        }

        public ShipBannerDeed(Serial serial) : base(serial) { }

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
