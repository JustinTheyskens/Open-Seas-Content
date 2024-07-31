using System;
using Server.Items;
using Server.Mobiles;
using Server.Multis;
using Server.Targeting;
using Ultima;

namespace Server.Custom.Items
{
    public class BasicShipPaint : Item
    {
        private static readonly int[] hueList = { 276, 296, 516, 1900, 251, 246, 2213, 36 };

        [Constructable]
        public BasicShipPaint() : base(0xA38A)
        {
            
            Name = "boat pigment";
            Weight = 1.0;

            if (Utility.RandomBool())
            {
                Hue = Utility.RandomMinMax(1801, 1890);   
            }
            else
                Hue = hueList[Utility.Random(hueList.Length - 1)];
        }

        [Constructable]
        public BasicShipPaint(bool neutralColors) : base(0xA38A)
        {
            Name = "boat pigment";
            Weight = 1.0;

            if (neutralColors)
            {
                Hue = Utility.RandomMinMax(1801, 1890);
            }
            else
                Hue = hueList[Utility.Random(hueList.Length - 1)];
        }

        public override void OnDoubleClick(Mobile from)
        {
            if (IsChildOf(from.Backpack))
            {
                BaseShip ship = BaseShip.FindShipAt(from, from.Map);

                if (ship != null)
                {
                    if (ship.Owner == from)
                    {
                        from.Target = new InternalTarget(this, ship);
                    }
                    else
                    {
                        from.SendLocalizedMessage(1116627); // You must be the owner of the ship to do this.
                    }
                }
                else
                    from.SendMessage("You must be on the ship to use this item.");
            }
        }

        public BasicShipPaint(Serial serial) : base(serial)
        {
        }

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
        private class InternalTarget : Target
        {
            public BasicShipPaint Paint { get; set; }
            public BaseShip Ship { get; set; }

            public InternalTarget(BasicShipPaint paint, BaseShip ship)
                : base(2, false, TargetFlags.None)
            {
                Paint = paint;
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
                        ship.PaintShip(Paint.Hue);
                        Paint.Delete();
                    }
                }
            }
        }
    }
}
