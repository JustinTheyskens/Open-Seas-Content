using System;
using Server;
using Server.Multis;
using Server.Targeting;

namespace Server.Items
{
    public class StarShipBanner : ShipBanner
    {
        public override ShipBannerDeed GetDeed { get { return new StarShipBannerDeed(); } }

        [Constructable]
        public StarShipBanner(BaseShip ship)
            : base(ship)
        {

        }

        public StarShipBanner(Serial serial) : base(serial)
        { }

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

    public class StarShipBannerDeed : ShipBannerDeed
    {
        public override ShipBannerType BannerType { get { return ShipBannerType.Star; } }

        [Constructable]
        public StarShipBannerDeed()
        {
            Name = "Star Ship Banner";
        }

        public StarShipBannerDeed(Serial serial)
            : base(serial) { }

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
