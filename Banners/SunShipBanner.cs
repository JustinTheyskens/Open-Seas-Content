using System;
using Server;
using Server.Multis;
using Server.Targeting;

namespace Server.Items
{
    public class SunShipBanner : ShipBanner
    {
        public override ShipBannerDeed GetDeed { get { return new SunShipBannerDeed(); } }

        [Constructable]
        public SunShipBanner(BaseShip ship)
            : base(ship)
        {

        }

        public SunShipBanner(Serial serial) : base(serial)
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

    public class SunShipBannerDeed : ShipBannerDeed
    {
        public override ShipBannerType BannerType { get { return ShipBannerType.Sun; } }

        [Constructable]
        public SunShipBannerDeed()
        {
            Name = "Sun Ship Banner";
        }

        public SunShipBannerDeed(Serial serial)
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
