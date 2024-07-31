using Server.Items;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server.Multis
{
    public class SeaShrineKing : BaseSeaShrine
    {
        [Constructable]
        public SeaShrineKing()
            : base(0x15B)
        {
            Name = "Ruins of a King's Tomb";

            Timer.DelayCall(TimeSpan.FromSeconds(0.25), () =>
            {
                AddTeleporter(0xAF7B, new Point3D(2, 5, -2));
                AddTeleporter(0xAF7B, new Point3D(3, 5, -2));

                AddTeleporter(0xAF7B, new Point3D(0, -6, -2));
                AddTeleporter(0xAF7B, new Point3D(1, -6, -2));
            });
        }
        public SeaShrineKing(Serial serial)
            : base(serial) { }

        public override void DoBuffEffects()
        {
            SeaShrineBuffEffect buff = new SeaShrineBuffEffect(0xB670, 1281);
            Timer.DelayCall(TimeSpan.FromSeconds(0.2), () =>
            {
                buff.MoveToWorld(new Point3D(X, Y, Z + 21), Map);
                Addons.Add(buff);
            });
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
    }
}
