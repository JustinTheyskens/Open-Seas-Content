using System;
using Server;
using Server.Multis;
using Server.Targeting;

namespace Server.Items
{
    public class UOShipBanner : ShipBanner
    {
        public override ShipBannerDeed GetDeed { get { return new UOShipBannerDeed(); } }
        public override int ZSurface { get { return 25; } }

        [Constructable]
        public UOShipBanner(BaseShip ship)
            : base(ship)
        {

        }

        public UOShipBanner(Serial serial) : base(serial)
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

    public class UOShipBannerDeed : ShipBannerDeed
    {
        public override ShipBannerType BannerType { get { return ShipBannerType.UO; } }

        [Constructable]
        public UOShipBannerDeed()
        {
            Name = "UO Ship Banner";
        }

        public UOShipBannerDeed(Serial serial)
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
