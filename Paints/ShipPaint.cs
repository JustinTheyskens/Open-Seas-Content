using System;
using Server;
using Server.Mobiles;
using Server.Multis;
using Server.Targeting;

namespace Server.Items
{
    public class ShipPaint : Item
    {
        private static readonly int[] _TierOne ={
            2320, 2329, 2332, 2334, 2335, 1150, 1196, 1153, 1175, 1461, 1462, 1765, 1766, 1767, 1771,
            2341, 2342, 2343, 2344, 2345, 2346, 2348, 2349, 2355, 2356, 2359, 2366
        };
        private static readonly int[] _TierTwo = { 1910, 1911, 1912, 1916, 2364, 2370, 2369, 2368, 1151, 2446, 2347, 2363, 2394, 2395 };
        private static readonly int[] _TierThree = { 2470, 2476, 2479, 2734, 2717, 2716, 2724, 2722, 2726, 2733, 2746, 2747, 2762, 2371, 2396 };
        //private static readonly Random _random = new Random();

        public int Tier;

        [Constructable]
        public ShipPaint()
            : base(0x0FAB)
        {
            Name = "Ship Paint";
            Hue = GetHue();
            LootType = LootType.Cursed;
            Weight = 10.0;
        }

        [Constructable]
        public ShipPaint(int tier)
            : base(0x0FAB)
        {
            Name = "Ship Paint";

            if (tier > 3)
                Tier = 3;
            else if (tier < 1)
                Tier = 1;
            else
                Tier = tier;

            switch(Tier)
            {
                case 3: Hue = Utility.RandomList(_TierThree); break;
                case 2: Hue = Utility.RandomList(_TierTwo); break;
                case 1: Hue = Utility.RandomList(_TierOne); break;
            }

            LootType = LootType.Cursed;
            Weight = 10.0;
        }

        public ShipPaint(Serial serial)
            : base(serial)
        {
        }

        public int GetHue()
        {
            double chance = Utility.RandomDouble();

            if (chance >= 0.9)
            {
                Tier = 3;
                return Utility.RandomList(_TierThree);
            }
            else if (chance > 0.65)
            {
                Tier = 2;
                return Utility.RandomList(_TierTwo);
            }
            else
            {
                Tier = 1;
                return Utility.RandomList(_TierOne);
            }
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

        public override void AddNameProperties(ObjectPropertyList list)
        {
            base.AddNameProperties(list);

            string label = GetLabelFromHue();
            string toAdd = "";

            if (!String.IsNullOrEmpty(label))
                toAdd += label + "\n";
            //list.Add(label);

            string tierColored = "";
            switch (Tier)
            {
                case 3: tierColored = $"<basefont color=#FF8C00>Tier: III</basefont>"; break;// Use the desired HTML color code for DarkOrange
                case 2: tierColored = $"<basefont color=#B8860B>Tier: II</basefont>"; break; // Use the desired HTML color code for DarkGoldenrod
                default: tierColored = $"<basefont color=#50C878>Tier: I</basefont>"; break;
            }

            toAdd += tierColored;
            list.Add(toAdd);

        }

        private string GetLabelFromHue()
        {
            foreach (HueLabel hueLabel in HueLabelScript.HueLabels)
            {
                if (hueLabel.Hue == Hue)
                    return hueLabel.Label;
            }

            return String.Empty;
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
            public ShipPaint Paint { get; set; }
            public BaseShip Ship { get; set; }

            public InternalTarget(ShipPaint paint, BaseShip ship)
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
