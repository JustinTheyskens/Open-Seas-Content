using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server.Items
{
    public class SeaShrineBuffEffect : Item
    {

        public SeaShrineBuffEffect(int id)
            : this(id, 0)
        {    
        }

        public SeaShrineBuffEffect(int id, int hue)
            :base(id)
        {
            Name = "Magical Energy";
            Movable = false;
            Hue = hue;
        }

        public SeaShrineBuffEffect(Serial serial)
            : base(serial) { }

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
    }
}
