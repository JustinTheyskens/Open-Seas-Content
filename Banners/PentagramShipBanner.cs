using System;
using Server;
using Server.Multis;
using Server.Targeting;

namespace Server.Items
{
    public class PentagramShipBanner : ShipBanner
    {
        public override ShipBannerDeed GetDeed { get { return new PentagramShipBannerDeed(); } }
        public override int ZSurface { get { return 25; } }

        [Constructable]
        public PentagramShipBanner(BaseShip ship)
            : base(ship)
        {

        }

        public PentagramShipBanner(Serial serial) : base(serial)
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

    public class PentagramShipBannerDeed : ShipBannerDeed
    {
        public override ShipBannerType BannerType { get { return ShipBannerType.Pentagram; } }

        [Constructable]
        public PentagramShipBannerDeed()
        {
            Name = "Pentagram Ship Banner";
        }

        public PentagramShipBannerDeed(Serial serial)
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
