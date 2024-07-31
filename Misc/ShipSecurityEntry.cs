using System;
using System.Collections.Generic;
using Server;
using Server.Accounting;
using Server.Engines.PartySystem;
using Server.Guilds;

namespace Server.Multis
{
    [PropertyObject]
    public class ShipSecurityEntry
    {
        private readonly SecurityLevel DefaultImpliedAccessLevel = SecurityLevel.Passenger;
        private Dictionary<Mobile, SecurityLevel> m_Manifest;

        [CommandProperty(AccessLevel.GameMaster)]
        public BaseShip Ship { get; set; }

        [CommandProperty(AccessLevel.GameMaster)]
        public PartyAccess PartyAccess { get; set; }

        [CommandProperty(AccessLevel.GameMaster)]
        public SecurityLevel DefaultPublicAccess { get; set; }

        [CommandProperty(AccessLevel.GameMaster)]
        public SecurityLevel DefaultPartyAccess { get; set; }

        [CommandProperty(AccessLevel.GameMaster)]
        public SecurityLevel DefaultGuildAccess { get; set; }

        public Dictionary<Mobile, SecurityLevel> Manifest { get { return m_Manifest; } }

        [CommandProperty(AccessLevel.GameMaster)]
        public bool IsPublic { get { return DefaultPublicAccess != SecurityLevel.Denied; } }

        public ShipSecurityEntry(BaseShip ship)
        {
            Ship = ship;
            m_Manifest = new Dictionary<Mobile, SecurityLevel>();

            AddToManifest(Ship.Owner, SecurityLevel.Captain);
            SetToDefault();
        }

        public void AddToManifest(Mobile from, SecurityLevel access)
        {
            if (from == null || m_Manifest == null)
                return;

            m_Manifest[from] = access;
        }

        public void RemoveFromAccessList(Mobile from)
        {
            if (!m_Manifest.ContainsKey(from))
                return;

            m_Manifest.Remove(from);
        }

        public bool HasImpliedAccess(Account acct, Mobile from)
        {
            for (int i = 0; i < acct.Length; ++i)
            {
                Mobile m = acct[i];

                if (m == from)
                    continue;

                if (GetEffectiveLevel(m, false) > SecurityLevel.Denied)
                    return true;
            }

            return false;
        }

        public SecurityLevel GetImpliedAccess(Mobile from)
        {
            if (from == null)
                return SecurityLevel.Denied;

            Account acct = from.Account as Account;

            if (acct != null)
            {
                if (HasImpliedAccess(acct, from))
                    return DefaultImpliedAccessLevel;
            }

            return SecurityLevel.Denied;
        }

        public SecurityLevel GetEffectiveLevel(Mobile from)
        {
            return GetEffectiveLevel(from, true);
        }

        public SecurityLevel GetEffectiveLevel(Mobile from, bool checkImplied)
        {
            if (from == null)
                return SecurityLevel.Denied;

            //Owner is always a captain!
            if (from == Ship.Owner)
                return SecurityLevel.Captain;

            SecurityLevel highest = SecurityLevel.Denied;

            if (m_Manifest.ContainsKey(from))
            {
                if (m_Manifest[from] == highest) //denied
                    return highest;

                highest = m_Manifest[from];
            }

            if (highest < DefaultPublicAccess)
                highest = DefaultPublicAccess;

            if (IsInParty(from) && highest < DefaultPartyAccess)
                highest = DefaultPartyAccess;

            if (IsInGuild(from) && highest < DefaultGuildAccess)
                highest = DefaultGuildAccess;

            if (checkImplied && highest == SecurityLevel.Denied)
                highest = GetImpliedAccess(from);

            return highest;
        }

        public bool IsInParty(Mobile from)
        {
            if (from == null || Ship == null || Ship.Owner == null)
                return false;

            Party fromParty = Party.Get(from);
            Party ownerParty = Party.Get(Ship.Owner);

            if (fromParty == null || ownerParty == null)
                return false;

            if (fromParty == ownerParty)
            {
                switch (PartyAccess)
                {
                    case PartyAccess.Never: return false;
                    case PartyAccess.LeaderOnly: return ownerParty.Leader == Ship.Owner;
                    case PartyAccess.MemberOnly: return true;
                }
            }
            return false;
        }

        public bool IsInGuild(Mobile from)
        {
            if (from == null || Ship == null || Ship.Owner == null)
                return false;

            Guild fromGuild = from.Guild as Guild;
            Guild ownerGuild = Ship.Owner.Guild as Guild;

            return fromGuild != null && ownerGuild != null && fromGuild == ownerGuild;
        }

        public void SetToDefault()
        {
            PartyAccess = PartyAccess.Never;
            DefaultPublicAccess = SecurityLevel.NA;
            DefaultPartyAccess = SecurityLevel.NA;
            DefaultGuildAccess = SecurityLevel.NA;
        }

        public void Serialize(GenericWriter writer)
        {
            writer.Write((int)0);

            writer.Write((int)PartyAccess);
            writer.Write((int)DefaultPublicAccess);
            writer.Write((int)DefaultPartyAccess);
            writer.Write((int)DefaultGuildAccess);

            writer.Write(m_Manifest.Count);
            foreach (KeyValuePair<Mobile, SecurityLevel> kvp in m_Manifest)
            {
                writer.Write(kvp.Key);
                writer.Write((int)kvp.Value);
            }
        }

        public ShipSecurityEntry(BaseShip ship, GenericReader reader)
        {
            m_Manifest = new Dictionary<Mobile, SecurityLevel>();
            Ship = ship;

            int version = reader.ReadInt();

            PartyAccess = (PartyAccess)reader.ReadInt();
            DefaultPublicAccess = (SecurityLevel)reader.ReadInt();
            DefaultPartyAccess = (SecurityLevel)reader.ReadInt();
            DefaultGuildAccess = (SecurityLevel)reader.ReadInt();

            int count = reader.ReadInt();
            for (int i = 0; i < count; i++)
            {
                Mobile mob = reader.ReadMobile();
                SecurityLevel sl = (SecurityLevel)reader.ReadInt();

                AddToManifest(mob, sl);
            }
        }

        public override string ToString()
        {
            return "...";
        }
    }
}
