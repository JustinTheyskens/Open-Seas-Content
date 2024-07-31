using System.Collections.Generic;
using System;
using System.Linq;

using Server;
using Server.Mobiles;
using Server.ContextMenus;
using Server.Gumps;
using Server.Multis;
using Server.Network;
using Server.Engines.Quests.Haven;

namespace Server.Items
{
    public interface IShipBanner : IEntity
    {
        DamageLevel DamageState { get; set; }
        Direction Facing { get; }
        ShipBannerDeed GetDeed { get; }
        Direction GetFacing();
        void DoAreaMessage(int cliloc, int range, Mobile from);
    }
    public abstract class ShipBanner : Item, IShipBanner
    {
        [CommandProperty(AccessLevel.GameMaster)]
        public BaseShip Ship { get; set; }

        [CommandProperty(AccessLevel.GameMaster)]
        public DamageLevel DamageState { get; set; }

        [CommandProperty(AccessLevel.GameMaster)]
        public Direction Facing { get { return GetFacing(); } }

        public virtual int ZSurface { get { return 0; } }

        public abstract ShipBannerDeed GetDeed { get; }

        public List<Mobile> Viewing { get; set; } = new List<Mobile>();

        public static int[][] BannerIDs { get { return _BannerIDs; } }
        private static int[][] _BannerIDs = new int[][]
        {
                      // UO     Tree    Star    Sea     Flower  Sun     Penta   Roit
            new int[] { 0x42C9, 0x64AA, 0x64AC, 0x64AE, 0x64B0, 0x64B2, 0xBC02, 0xBAE9 }, // south
            new int[] { 0x42CA, 0x64A9, 0x64AB, 0x64AD, 0x64AF, 0x64B1, 0xBC01, 0xBAE8 }, // east
            new int[] { 0x52CA, 0x52CA, 0x52CA, 0x52CA, 0x52CA, 0x52CA, 0x52CA, 0x52CA },   // north & west
        };

        public ShipBanner(BaseShip ship)
            : base(0x2198)
        {
            Movable = false;
            Ship = ship;
        }

        public override void OnDoubleClickDead(Mobile m)
        {
            OnDoubleClick(m);
        }

        public override void OnDoubleClick(Mobile from)
        {
            //nothing
        }

        public Direction GetFacing()
        {
            if (BannerIDs[0].Any(id => id == ItemID))
            {
                return Direction.South;
            }
            if (BannerIDs[1].Any(id => id == ItemID))
            {
                return Direction.East;
            }
            if (BaseShip.CannonIDs[2].Any(id => id == ItemID))
            {
                return Direction.North;
            }

            return Direction.West;
        }

        public void DoAreaMessage(int cliloc, int range, Mobile from)
        {
            if (from == null)
                return;

            Ship.GetEntitiesOnBoard().OfType<PlayerMobile>().Where(x => x != from && Ship.GetSecurityLevel(x) > SecurityLevel.Denied)
                .ToList().ForEach(y =>
                {
                    if (from != null)
                        y.SendLocalizedMessage(cliloc, from.Name);
                    else
                        y.SendLocalizedMessage(cliloc);
                });
        }


        public override void GetProperties(ObjectPropertyList list)
        {
            /*
            base.GetProperties(list);

            list.Add(1116026, Charged == CannonAction.Finish ? "#1116031" : "#1116032"); // Charged: ~1_VALUE~
            list.Add(1116027, string.Format("{0}", ShipAmmoInfo.GetAmmoName(this).ToString())); // Ammo: ~1_VALUE~
            list.Add(1116028, Primed == CannonAction.Finish ? "#1116031" : "#1116032"); //Primed: ~1_VALUE~
            list.Add(1116580 + (int)DamageState);
            list.Add(1072241, "{0}\t{1}\t{2}\t{3}", TotalItems, MaxItems, TotalWeight, MaxWeight);
            */
            // do nothing ?
        }


  

        public override void Delete()
        {
            if (Ship != null)
            {
                Ship.RemoveCannon(this);
            }

            base.Delete();
        }

        public ShipBanner(Serial serial) : base(serial) { }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write(1);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();

        }
    }
}
