using System;
using Server;
using Server.Multis;
using System.Collections.Generic;
using Server.Network;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.ToolTip;

namespace Server.Gumps
{
    public class ShipAccessListGump : Gump
    {
        public static readonly int NAHue = 0x5EF7;
        public static readonly int PassengerHue = 0x1CFF;
        public static readonly int CrewHue = 0x1FE7;
        public static readonly int OfficerHue = 0x7FE7;
        public static readonly int DenyHue = 0x7CE7;
        public static readonly int CaptainHue = 0x7DE7;
        public static readonly int LabelColor = 0x7FFF;
        public static readonly int NoHue = 0x3DEF;

        private BaseShip m_Ship;
        private ShipSecurityEntry m_Entry;
        private List<Mobile> m_UseList;

        public ShipAccessListGump(Mobile from, BaseShip ship)
            : base(100, 100)
        {
            from.CloseGump(typeof(ShipAccessListGump));

            AddPage(0);

            AddBackground(0, 0, 320, 385, 0xA3C);
            AddHtmlLocalized(10, 10, 300, 18, 1149724, 0x7FEF, false, false); //<CENTER>Passenger and Crew Manifest</CENTER>

            string shipName = "unnamed ship";
            if (ship.ShipName != null && ship.ShipName != string.Empty && ship.ShipName != "")
                shipName = ship.ShipName;

            AddHtmlLocalized(10, 38, 75, 18, 1149761, LabelColor, false, false); //Ship:
            AddLabel(80, 38, 0x53, shipName);

            AddHtmlLocalized(10, 56, 75, 18, 1149762, LabelColor, false, false); //Owner:
            AddLabel(80, 56, 0x53, ship.Owner != null ? ship.Owner.Name : "Unknown");

            m_Ship = ship;
            m_Entry = ship.SecurityEntry;

            if (m_Entry == null)
            {
                m_Entry = new ShipSecurityEntry(m_Ship);
                m_Ship.SecurityEntry = m_Entry;
            }

            AddButton(10, 355, 0xFA5, 0xFA7, 1, GumpButtonType.Reply, 0);
            AddHtmlLocalized(45, 357, 100, 18, 1149777, LabelColor, false, false); // MAIN MENU

            m_UseList = new List<Mobile>(m_Entry.Manifest.Keys);

            int page = 1;
            int y = 79;

            AddPage(page);

            for (int i = 0; i < m_UseList.Count; i++)
            {
                if (page > 1)
                    AddButton(270, 390, 4014, 4016, 0, GumpButtonType.Page, page - 1);

                Mobile mob = m_UseList[i];

                if (mob == null || m_Ship.IsOwner(mob))
                    continue;

                string name = mob.Name;
                SecurityLevel level = m_Entry.GetEffectiveLevel(mob);

                AddButton(10, y, 0xFA5, 0xFA7, i + 2, GumpButtonType.Reply, 0);
                AddLabel(45, y + 2, 0x3E7, name);
                AddHtmlLocalized(160, y + 2, 150, 18, GetLevel(level), GetHue(level), false, false);

                y += 25;

                bool pages = (i + 1) % 10 == 0;

                if (pages && m_UseList.Count - 1 != i)
                {
                    AddButton(310, 390, 4005, 4007, 0, GumpButtonType.Page, page + 1);
                    page++;
                    y = 0;

                    AddPage(page);
                }
            }
        }
        public int GetHue(SecurityLevel level)
        {
            switch (level)
            {
                case SecurityLevel.Captain: return CaptainHue;
                case SecurityLevel.Officer: return OfficerHue;
                case SecurityLevel.Crewman: return CrewHue;
                case SecurityLevel.Passenger: return PassengerHue;
                case SecurityLevel.NA: return NAHue;
                case SecurityLevel.Denied: return DenyHue;
                default: return LabelColor;
            }
        }

        public static int GetLevel(SecurityLevel level)
        {
            switch (level)
            {
                default:
                case SecurityLevel.Denied: return 1149726;
                case SecurityLevel.Passenger: return 1149727;
                case SecurityLevel.Crewman: return 1149728;
                case SecurityLevel.Officer: return 1149729;
                case SecurityLevel.Captain: return 1149730;
                case SecurityLevel.NA: return 1149725;
            }
        }

        public override void OnResponse(NetState sender, RelayInfo info)
        {
            Mobile from = sender.Mobile;

            if (info.ButtonID == 1)
                from.SendGump(new NewShipSecurityGump(from, m_Ship));
            else if (info.ButtonID > 1)
            {
                int index = info.ButtonID - 2;

                if (index < 0 || index >= m_UseList.Count)
                    return;

                Mobile mob = m_UseList[index];

                from.SendGump(new GrantShipAccessGump(mob, m_Ship));
            }
        }
    }
}
