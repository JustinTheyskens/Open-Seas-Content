using System;
using Server;
using Server.Multis;
using Server.Network;
using Server.ContextMenus;
using System.Collections.Generic;

namespace Server.Items
{
    public class BoatBacking : Item
    {
        public BaseBoat Boat { get; private set; }

        [Constructable]
        public BoatBacking()
            : this(null)
        {
        }

        [Constructable]
        public BoatBacking(BaseBoat boat)
            : base(0x3EAB)
        {
            Boat = boat;
            Movable = false;
        }

        public BoatBacking(Serial serial)
            : base(serial)
        {
        }

        public virtual void SetFacing(Direction dir)
        {
            switch (dir)
            {
                case Direction.South: ItemID = 0x3EC4; break;
                case Direction.North: ItemID = 0x3EBE; break;
                case Direction.West: ItemID = 0x3E76; break;
                case Direction.East: ItemID = 0x3E63; break;
            }
        }

        public override void GetProperties(ObjectPropertyList list)
        {
            //base.GetProperties(list);

            if (Boat != null)
            {
                list.Add("Fishing Boat");

                int health = (int)(Boat.Hits * 100 / Boat.MaxHits);

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

            list.Add(Boat.Status);
            list.Add(1116580 + (int)Boat.DamageTaken); //State: Prisine            
        }

        public virtual void Say(int number)
        {
            PublicOverheadMessage(MessageType.Regular, 0x3B2, number);
        }

        public virtual void Say(int number, string args)
        {
            PublicOverheadMessage(MessageType.Regular, 0x3B2, number, args);
        }

        public override void AddNameProperty(ObjectPropertyList list)
        {
            if (Boat != null && Boat.ShipName != null)
                list.Add(1042884, Boat.ShipName); // the tiller man of the ~1_SHIP_NAME~
            else
                base.AddNameProperty(list);
        }

        public override void OnSingleClick(Mobile from)
        {
            if (Boat != null && Boat.ShipName != null)
                LabelTo(from, 1042884, Boat.ShipName); // the tiller man of the ~1_SHIP_NAME~
            else
                base.OnSingleClick(from);
        }

        public Mobile Pilot { get { return Boat != null ? Boat.Pilot : null; } }

        public override void OnDoubleClickDead(Mobile m)
        {
            OnDoubleClick(m);
        }

        public override void OnDoubleClick(Mobile from)
        {
            from.RevealingAction();

            BaseBoat boat = BaseBoat.FindBoatAt(from, from.Map);
            Item mount = from.FindItemOnLayer(Layer.Mount);

            if (boat == null || Boat == null || Boat != boat)
            {
                from.SendLocalizedMessage(1116724); // You cannot pilot a ship unless you are aboard it!
            }
            else if (Pilot != null && Pilot != from && Pilot == Boat.Owner)
            {
                from.SendLocalizedMessage(502221); // Someone else is already using this item.
            }
            else if (from.Flying)
            {
                from.SendLocalizedMessage(1116615); // You cannot pilot a ship while flying!
            }
            else if (from.Mounted && !(mount is BoatMountItem))
            {
                from.SendLocalizedMessage(1010097); // You cannot use this while mounted or flying.
            }
            else if (Pilot == null && Boat.Scuttled)
            {
                from.SendLocalizedMessage(1116725); // This ship is too damaged to sail!
            }
            else if (Pilot != null)
            {
                if (from != Pilot) // High authorized player takes control of the ship
                {
                    boat.RemovePilot(from);
                    boat.LockPilot(from);
                }
                else
                {
                    boat.RemovePilot(from);
                }
            }
            else
            {
                boat.LockPilot(from);
            }
        }

        public override bool OnDragDrop(Mobile from, Item dropped)
        {
            if (dropped is MapItem && Boat != null && Boat.CanCommand(from) && Boat.Contains(from))
            {
                Boat.AssociateMap((MapItem)dropped);
            }

            return false;
        }



        public override void OnAfterDelete()
        {
            if (Boat != null)
                Boat.Delete();
        }

        public override void GetContextMenuEntries(Mobile from, List<ContextMenuEntry> list)
        {
            base.GetContextMenuEntries(from, list);

            if (Boat.IsRowBoat)
                return;

            if (Boat != null)
            {
                if (Boat.Contains(from))
                {
                    if (Boat.IsOwner(from))
                        list.Add(new RenameShipEntry(this, from));

                    list.Add(new EmergencyRepairEntry(this, from));
                    list.Add(new ShipRepairEntry(this, from));
                }
                else if (Boat.IsOwner(from))
                {
                    list.Add(new DryDockEntry(Boat, from));
                }
            }
        }

        private class EmergencyRepairEntry : ContextMenuEntry
        {
            private BoatBacking m_BoatBacking;
            private Mobile m_From;

            public EmergencyRepairEntry(BoatBacking tillerman, Mobile from)
                : base(1116589, 5)
            {
                m_BoatBacking = tillerman;
                m_From = from;
            }

            public override void OnClick()
            {
                if (m_BoatBacking != null && m_BoatBacking.Boat != null)
                {
                    BaseBoat g = m_BoatBacking.Boat;

                    if (!g.Scuttled)
                        m_From.SendLocalizedMessage(1116595); //Your ship is not in need of emergency repairs in order to sail.
                    else if (g.IsUnderEmergencyRepairs())
                    {
                        TimeSpan left = g.GetEndEmergencyRepairs();
                        m_From.SendLocalizedMessage(1116592, left != TimeSpan.Zero ? left.TotalMinutes.ToString() : "0"); //Your ship is underway with emergency repairs holding for an estimated ~1_TIME~ more minutes.
                    }
                    else if (!g.TryEmergencyRepair(m_From))
                        m_From.SendLocalizedMessage(1116591, String.Format("{0}\t{1}", BaseGalleon.EmergencyRepairClothCost.ToString(), BaseGalleon.EmergencyRepairWoodCost)); //You need a minimum of ~1_CLOTH~ yards of cloth and ~2_WOOD~ pieces of lumber to effect emergency repairs.
                }
            }
        }

        private class ShipRepairEntry : ContextMenuEntry
        {
            private BoatBacking m_BoatBacking;
            private Mobile m_From;

            public ShipRepairEntry(BoatBacking tillerman, Mobile from)
                : base(1116590, 5)
            {
                m_BoatBacking = tillerman;
                m_From = from;
            }

            public override void OnClick()
            {
                if (m_BoatBacking != null && m_BoatBacking.Boat != null)
                {
                    if (!BaseGalleon.IsNearLandOrDocks(m_BoatBacking.Boat))
                        m_From.SendLocalizedMessage(1116594); //Your ship must be near shore or a sea market in order to effect permanent repairs.
                    else if (m_BoatBacking.Boat.DamageTaken == DamageLevel.Pristine)
                        m_From.SendLocalizedMessage(1116596); //Your ship is in pristine condition and does not need repairs.
                    else
                        m_BoatBacking.Boat.TryRepairs(m_From);
                }
            }
        }

        private class RenameShipEntry : ContextMenuEntry
        {
            private BoatBacking m_BoatBacking;
            private readonly Mobile m_From;

            public RenameShipEntry(BoatBacking tillerman, Mobile from)
                : base(1111680, 3)
            {
                m_BoatBacking = tillerman;
                m_From = from;
            }

            public override void OnClick()
            {
                if (m_BoatBacking != null && m_BoatBacking.Boat != null)
                    m_BoatBacking.Boat.BeginRename(m_From);
            }
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write((int)0);//version

            writer.Write(Boat);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();

            Boat = reader.ReadItem() as BaseBoat;

            if (Boat == null)
                Delete();
        }
    }
}
