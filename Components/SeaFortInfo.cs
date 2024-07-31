using System;
using System.Collections.Generic;
using System.Linq;
using Server.Items;

namespace Server.Multis
{
    public class SeaFortInfo : Item
    {
        private BaseSeaFort _SeaFort;
        public BaseSeaFort Fort { get { return _SeaFort; } }
        [Constructable]
        public SeaFortInfo(BaseSeaFort fort, int itemid)
            : base(itemid)
        {
            _SeaFort = fort;
            Movable = false;
        }

        public SeaFortInfo(Serial serial)
            : base(serial) { }

        public override void GetProperties(ObjectPropertyList list)
        {
            //base.GetProperties(list);

            if (_SeaFort != null)
            {
                list.Add("Sea Fort");

                int health = (int)(_SeaFort.Hits * 100 / _SeaFort.MaxHits);

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
            }
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write((int)0); // version
            writer.Write(_SeaFort);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();
            _SeaFort = reader.ReadItem() as BaseSeaFort;
        }
    }
}
