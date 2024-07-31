using System;
using Server;
using Server.Multis;
using Server.Targeting;

namespace Server.Items
{
    public class SeaHorseShipBanner : ShipBanner
    {
        public override ShipBannerDeed GetDeed { get { return new SeaHorseShipBannerDeed(); } }

        [Constructable]
        public SeaHorseShipBanner(BaseShip ship)
            : base(ship)
        {

        }

        public SeaHorseShipBanner(Serial serial) : base(serial)
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

    public class SeaHorseShipBannerDeed : ShipBannerDeed
    {
        public override ShipBannerType BannerType { get { return ShipBannerType.SeaHorse; } }

        [Constructable]
        public SeaHorseShipBannerDeed()
        {
            Name = "Sea Horse Ship Banner";
        }

        public SeaHorseShipBannerDeed(Serial serial)
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
