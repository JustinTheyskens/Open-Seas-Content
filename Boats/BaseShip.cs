using Server.Engines.Blackthorn;
using Server.Engines.Plants;
using Server.Engines.Quests.Haven;
using Server.Engines.Shadowguard;
using Server.Engines.TreasuresOfKotlCity;
using Server.Items;
using Server.Mobiles;
using Server.SkillHandlers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Server.Gumps;
using System.Drawing;

// Remember to have the edited version of Hold.cs, Plank.cs & Tillerman.cs

namespace Server.Multis
{
    public interface IShipFixture
    {
        BaseShip Ship { get; set; }
    }

    public abstract class BaseShip : BaseBoat
    {
        private ShipSecurityEntry m_SecurityEntry;
        public List<Item> Fixtures { get; set; } = new List<Item>();
        public List<Item> Cannons { get; set; }
        public List<Mobile> Crew {  get; set; }

        private Dictionary<Item, Item> _InternalCannon;

        [CommandProperty(AccessLevel.GameMaster)]
        public ShipSecurityEntry SecurityEntry
        {
            get
            {
                if (m_SecurityEntry == null)
                    m_SecurityEntry = new ShipSecurityEntry(this);
                return m_SecurityEntry;
            }
            set
            {
                m_SecurityEntry = value;
                m_SecurityEntry.Ship = this;
            }
        }

        public override int MaxHits { get { return GetMaxHits(); } }
        public virtual int MaxCannons { get { return 0; } }

        public virtual int StartingMaxCannons {  get { return 0; } }  
        public virtual int CaptiveOffset { get { return 0; } }
        public virtual double CannonDamageMod { get { return 1.0; } }

        public abstract Point2D[] CannonLocations { get; }

        #region upgrades
        public bool UpgradedHold { get; set; }
        public int CurrentMaxCannons { get; set; }
        public bool UpgradedHull { get; set; }
        public bool BannerUpgrade { get; set; }
        [CommandProperty(AccessLevel.GameMaster)]
        public ShipBanner Banner { get; set; }
        #endregion

        public BaseShip(Direction direction) : base(direction, false)
        {
            TillerMan = new TillerMan(this);
            CurrentMaxCannons = StartingMaxCannons;
            
            PPlank = new Plank(this, PlankSide.Port, 0);
            SPlank = new Plank(this, PlankSide.Starboard, 0);

            PPlank.MoveToWorld(new Point3D(X + PortOffset.X, Y + PortOffset.Y, Z), Map);
            SPlank.MoveToWorld(new Point3D(X + StarboardOffset.X, Y + StarboardOffset.Y, Z), Map);

            Hold = new Hold(this);

            UpdateComponents();
        }

        public int GetMaxHits()
        {
            if (UpgradedHull == true)
                return 35000;
            else
                return 25000;
        }

        public void PaintShip(int hue)
        {
            if (TillerMan != null)
            {
                if (TillerMan is Mobile)
                    ((Mobile)TillerMan).Hue = hue;
                else if (TillerMan is Item)
                    ((Item)TillerMan).Hue = hue;
            }

            if (PPlank != null)
                PPlank.Hue = hue;
            if (SPlank != null)
                SPlank.Hue = hue; ;

            if (Hold != null)
                Hold.Hue = hue;

            Hue = hue;
        }

        protected void AddCannon(Item item)
        {
            if (Cannons == null)
            {
                Cannons = new List<Item>();
            }

            Cannons.Add(item);
        }


        public void RemoveCannon(Item cannon)
        {
            if (Cannons != null && Cannons.Contains(cannon))
            {
                Cannons.Remove(cannon);
            }
        }

        public override void OnPlacement(Mobile from)
        {
            base.OnPlacement(from);

            if (_InternalCannon != null)
            {
                foreach (KeyValuePair<Item, Item> kvp in _InternalCannon)
                {
                    Point3D p = new Point3D(kvp.Value.X, kvp.Value.Y, kvp.Value.Z + TileData.ItemTable[kvp.Value.ItemID & TileData.MaxItemValue].CalcHeight);

                    kvp.Key.MoveToWorld(p, kvp.Value.Map);
                }

                UpdateCannonIDs();
                UpdateBannerID();

                _InternalCannon.Clear();
                _InternalCannon = null;
            }
        }

        public override void SetFacingComponents(Direction newDirection, Direction oldDirection, bool ignoreLastDirection)
        {
            if (oldDirection == newDirection && !ignoreLastDirection)
                return;

            var mcl = MultiData.GetComponents(ItemID);

            foreach (var mte in mcl.List.Where(e => (TileFlag)e.m_Flags == TileFlag.None))
            {
                foreach (var fixture in Fixtures.Where(f => f.X - X == mte.m_OffsetX && f.Y - Y == mte.m_OffsetY && f.Z - Z == mte.m_OffsetZ))
                {
                    fixture.ItemID = mte.m_ItemID;
                }
            }

            UpdateCannonIDs();
            UpdateBannerID();
        }

        public void AddFixture(Item item)
        {
            if (item != null && !Fixtures.Contains(item))
            {
                Fixtures.Add(item);
            }
        }

        public void RemoveFixture(Item item)
        {
            if (Fixtures != null && Fixtures.Contains(item))
            {
                Fixtures.Remove(item);
            }
        }

        public override bool Contains(int x, int y)
        {
            if (base.Contains(x, y))
                return true;

            return Fixtures.Any(f => f.X == x && f.Y == y);
        }

        public void UpdateCannonIDs()
        {
            if (Cannons != null)
            {
                foreach (var cannon in Cannons)
                {
                    UpdateCannonID(cannon);
                }
            }
        }

        public int GetValueForDirection(Direction direction)
        {
            switch (direction)
            {
                default:
                case Direction.South: return 0;
                case Direction.West: return 1;
                case Direction.North: return 2;
                case Direction.East: return 3;
            }
        }

        public void UpdateCannonID(Item cannon)
        {
            if (cannon == null)
                return;

            int type = cannon is SmallShipCannon ? 0 : 1;

            switch (Facing)
            {
                default:
                case Direction.South:
                case Direction.North:
                    {
                        if (cannon.X == X)
                            cannon.ItemID = m_CannonIDs[GetValueForDirection(Facing)][type];
                        else if (cannon.X < X)
                            cannon.ItemID = m_CannonIDs[GetValueForDirection(Direction.West)][type];
                        else
                            cannon.ItemID = m_CannonIDs[GetValueForDirection(Direction.East)][type];
                        break;
                    }
                case Direction.West:
                case Direction.East:
                    {
                        if (cannon.Y == Y)
                            cannon.ItemID = m_CannonIDs[GetValueForDirection(Facing)][type];
                        else if (cannon.Y < Y)
                            cannon.ItemID = m_CannonIDs[GetValueForDirection(Direction.North)][type];
                        else
                            cannon.ItemID = m_CannonIDs[GetValueForDirection(Direction.South)][type];
                        break;
                    }
            }
        }

        public static int[][] CannonIDs { get { return m_CannonIDs; } }
        private static int[][] m_CannonIDs = new int[][]
        { 
                      //Light  Heavy, Blunder, Pumpkin
            new int[] { 0xA7CD, 0xA7C9, 41664, 41979 }, //South
            new int[] { 0xA7CE, 0xA7CA, 41665, 41980 }, //West
            new int[] { 0xA7CB, 0xA7C8, 41666, 41981 }, //North
            new int[] { 0xA7CC, 0xA7C7, 41667, 41982 }, //East
        };

        public virtual ShipPosition GetCannonPosition(Point3D pnt)
        {
            return ShipPosition.Bow;
        }

        public SecurityLevel GetSecurityLevel(Mobile from)
        {
            if (m_SecurityEntry == null)
                m_SecurityEntry = new ShipSecurityEntry(this);

            if (from.AccessLevel > AccessLevel.Player || IsOwner(from))
                return SecurityLevel.Captain;

            return m_SecurityEntry.GetEffectiveLevel(from);
        }

        public bool IsPublic()
        {
            if (m_SecurityEntry == null)
                m_SecurityEntry = new ShipSecurityEntry(this);

            return m_SecurityEntry.IsPublic;
        }

        public override bool CanCommand(Mobile m)
        {
            return GetSecurityLevel(m) >= SecurityLevel.Crewman;
        }

        public override bool HasAccess(Mobile from)
        {
            if (Owner == null || (Scuttled && IsEnemy(from)) || (Owner is BaseCreature && !Owner.Alive))
                return true;

            return GetSecurityLevel(from) > SecurityLevel.Denied;
        }

        public void AddCrewman(Mobile m)
        {
            if (m == null)
                return;

            if (!Crew.Contains(m))
                Crew.Add(m);
        }

        public bool TryAddCannon(Mobile from, Point3D pnt, NewShipCannonDeed deed, bool force = false)
        {
            if (!IsNearLandOrDocks(this) && !force)
            {
                if (from != null)
                {
                    from.SendLocalizedMessage(1116076); // The ship must be near shore or a sea market to deploy this weapon.
                }
            }
            else
            {
                INewShipCannon cannon;

                switch (deed.CannonType)
                {
                    default:
                    case CannonPower.Light:
                        cannon = new SmallShipCannon(this);
                        break;
                    case CannonPower.Heavy:
                        cannon = new LargeShipCannon(this);
                        break;
                    case CannonPower.Massive:
                            cannon = new PowerfulShipCannon(this);
                        break;
                }

                return TryAddCannon(from, pnt, cannon, deed);
            }

            return false;
        }

        public bool TryAddCannon(Mobile from, Point3D pnt, INewShipCannon cannon, NewShipCannonDeed deed)
        {
            if (cannon == null || !(cannon is Item))
            {
                return false;
            }

            if (Owner == from)
            {
                Point2D d2 = CannonLocations[0];
                if (Cannons != null)
                {
                    if (Cannons.Count < CurrentMaxCannons)
                    {
                        int count = Cannons.Count;
                        d2 = CannonLocations[count];
                    }
                    else
                    {
                        from.SendMessage("Your ship has the maximum number of cannons it can hold.");
                        return false;
                    }
                }

                Point3D point = GetRotatedLocation( d2.X, d2.Y);
                ((Item)cannon).MoveToWorld(point, Map);
                AddCannon((Item)cannon);
                UpdateCannonID((Item)cannon);
                cannon.Position = GetCannonPosition(point);

                if (from != null)
                {
                    cannon.DoAreaMessage(1116074, 10, from); //~1_NAME~ deploys a ship cannon.
                }

                if (from != null && from.NetState != null)
                {
                    Timer.DelayCall(() =>
                    {
                        from.ClearScreen();
                        from.SendEverything();
                    });
                }

                if (deed != null && (from == null || from.AccessLevel == AccessLevel.Player))
                {
                    deed.Delete();
                }

                return true;
            }

            cannon.Delete();
            return false;
        }

        public bool AddBanner(Mobile from, ShipBannerDeed deed)
        {
            if (from == null )
                return false;

            if (from == Owner)
            {
                if (Banner != null)
                    RemoveBanner();

                Point2D location = new Point2D(0, -1);
                Point3D loc = GetRotatedLocation(location.X, location.Y);
                Point3D pnt = new Point3D(loc.X, loc.Y, loc.Z + 5);

                IShipBanner banner;
                switch(deed.BannerType)
                {
                    default:
                    case ShipBannerType.UO:
                        banner = new UOShipBanner(this);
                        break;
                    case ShipBannerType.Tree:
                        banner = new TreeShipBanner(this);
                        break;
                    case ShipBannerType.Star:
                        banner = new StarShipBanner(this);
                        break;
                    case ShipBannerType.SeaHorse:
                        banner = new SeaHorseShipBanner(this);
                        break;
                    case ShipBannerType.Flower:
                        banner = new FlowerShipBanner(this);
                        break;
                    case ShipBannerType.Sun:
                        banner = new SunShipBanner(this);
                        break;
                    case ShipBannerType.Pentagram:
                        banner = new PentagramShipBanner(this);
                        break;
                    case ShipBannerType.Riot:
                        banner = new TreeShipBanner(this);
                        break;
                }
                return AddBanner(from, pnt, banner, deed);
            }
            return false;
        }

        public bool AddBanner(Mobile from, Point3D loc, IShipBanner banner, ShipBannerDeed deed)
        {
            if (banner == null )
                return false;

            if (Owner == from)
            {
                ((Item)banner).MoveToWorld(loc, Map);
                this.Banner = ((Item)banner) as ShipBanner;
                if (((ShipBanner)banner).ZSurface > 0)
                    ((Item)banner).MoveToWorld(new Point3D(loc.X, loc.Y, loc.Z + ((ShipBanner)banner).ZSurface), Map);

                UpdateBannerID();
                return true;
            }
            else
            {
                from.SendMessage("You must be tthe owner of the ship to do this.");
                return false;
            }
        }

        public void RemoveBanner()
        {
            if (Banner != null)
            {
                Banner.Delete();
                Banner = null;
            }
        }

        public void UpdateBannerID()
        {
            if (Banner == null)
                return;

            int type = Banner is PentagramShipBanner ? 6 : Banner is SunShipBanner ? 5 :
                Banner is FlowerShipBanner ? 4 : Banner is SeaHorseShipBanner ? 3 :
                Banner is StarShipBanner ? 2 : Banner is TreeShipBanner ? 1 : 0;


            switch (Facing)
            {
                default:
                case Direction.South:
                    Banner.ItemID = ShipBanner.BannerIDs[0][type]; break;
                case Direction.North:
                    Banner.ItemID = ShipBanner.BannerIDs[2][type]; break;
                case Direction.West:
                    Banner.ItemID = ShipBanner.BannerIDs[2][type]; break;
                case Direction.East:
                    Banner.ItemID = ShipBanner.BannerIDs[1][type]; break;

            }
        }

        public void AutoAddCannons(Mobile captain)
        {
            bool heavy = Utility.RandomBool();

            for ( int i = 0; i < MaxCannons; i++)
            {
                INewShipCannon cannon;

                if (heavy)
                {
                    cannon = new LargeShipCannon(this);
                }
                else
                {
                    cannon = new SmallShipCannon(this);
                }

                if (!TryAddCannon(captain, Location, cannon, null))
                {
                    cannon.Delete();
                }
            }
        }

        public override void GetProperties(ObjectPropertyList list)
        {

            if (Name != null)
                list.Add(Name);
            else
                list.Add(GetTypeName());

            int health = (int)(Hits * 100 / MaxHits);

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

            //base.GetProperties(list);
        }

        public string GetTypeName()
        {
            string name = "";
            char[] chars = new char[GetType().Name.Length];
            for(int i = 0; i <  chars.Length; i++)
            {
                chars[i] = GetType().Name[i];
            }

            for (int j = 0; j < chars.Length; j++)
            {
                if (Char.IsUpper(chars[j]) && j != 0)
                {

                    name += " " + chars[j];
                }
                else
                    name += chars[j];
            }

            return name;

        }

        public override TimeSpan GetMovementInterval(bool fast, bool drifting, out int clientSpeed)
        {
            if (DamageTaken < DamageLevel.Heavily)
                return base.GetMovementInterval(fast, drifting, out clientSpeed);

            if (fast)
            {
                clientSpeed = 0x3;
                return FastDriftInterval;
            }

            clientSpeed = 0x2;
            return SlowDriftInterval;
        }

        #region Static Methods
        public static BaseShip FindShipAt(IPoint2D pnt, Map map)
        {
            BaseBoat boat = FindBoatAt(pnt, map);

            if (boat is BaseShip)
                return boat as BaseShip;

            return null;
        }
        
        public static bool CheckForBoat(IPoint3D p, Mobile caster)
        {
            BaseBoat boat = FindBoatAt(caster, caster.Map);
            BaseShip ship = FindShipAt(p, caster.Map);

            if (ship == null || caster.AccessLevel > AccessLevel.Player)
                return false;

            if (ship.Scuttled || ship.GetSecurityLevel(caster) >= SecurityLevel.Crewman)
                return false;

            return true;
        }

        public static bool IsNearLandOrDocks(BaseBoat boat)
        {
            return IsNearLand(boat) || IsNearDocks(boat);
        }

        public static bool IsNearLand(BaseBoat boat)
        {
            return IsNearLand(boat, 12);
        }

        public static bool IsNearLand(BaseBoat boat, int range)
        {
            if (boat == null || boat.Map == null || boat.Map.Tiles == null)
                return false;

            Map map = boat.Map;


            for (int x = boat.X - range; x <= boat.X + range; x++)
            {
                for (int y = boat.Y - range; y <= boat.Y + range; y++)
                {
                    LandTile lt = map.Tiles.GetLandTile(x, y);

                    
                    TileFlag landFlags = TileData.LandTable[lt.ID & TileData.MaxLandValue].Flags;

                    if ((landFlags & TileFlag.Impassable) == 0)
                        return true;
                }
            }
            return false;
        }

        public static bool IsNearDocks(BaseBoat boat)
        {
            return IsNearDocks(boat, 12);
        }

        public static bool IsNearDocks(BaseBoat boat, int range)
        {
            if (boat == null)
                return false;

            Map map = boat.Map;

            for (int x = boat.X - range; x <= boat.X + range; x++)
            {
                for (int y = boat.Y - range; y <= boat.Y + range; y++)
                {
                    StaticTile[] staticTiles = map.Tiles.GetStaticTiles(x, y, true);

                    for (int i = 0; i < staticTiles.Length; i++)
                    {
                        ItemData id = TileData.ItemTable[staticTiles[i].ID & TileData.MaxItemValue];

                        if (id.Name != null && (id.Name.ToLower() == "wooden plank" || id.Name.ToLower() == "pier"))
                            return true;
                    }
                }
            }
            return false;
        }
        #endregion

        /*
        public override void OnDoubleClick(Mobile from)
        {
            if (InRange(from.Location, 2) && from == Owner)
            {
                from.SendGump(new ShipGump(this, from as PlayerMobile));
            }
        }
        */

        public override void OnDelete()
        {
            
            if (Cannons != null)
            {
                for (int i = 0; i < Cannons.Count; i++)
                {
                    if (Cannons[i] != null || !Cannons[i].Deleted)
                        Cannons[i].Delete();

                }
            }

            base.OnDelete();
        }

        //maybe get rid of this?
        public override void OnAfterDelete()
        {
            base.OnAfterDelete();

            if (Cannons != null)
            {
                for (int i = 0; i < Cannons.Count; i++)
                {
                    if (Cannons[i] != null || !Cannons[i].Deleted)
                        Cannons[i].Delete();
                }
            }
        }

        public BaseShip(Serial serial) : base(serial)
        {
        }

        public void Sink()
        {
            Timer.DelayCall(TimeSpan.FromSeconds(0.5), TimeSpan.FromSeconds(0.5), 4, () =>
            {
                this.Z -= 2;
                if (TillerMan != null)
                    ((Item)TillerMan).Z -= 2;

                if (Hold != null)
                    ((Item)Hold).Z -= 2;
            });

            IPooledEnumerable eable = GetMobilesInRange(20);

            foreach (Mobile m in eable)
            {
                m.PlaySound(0x020);

                if (this.Contains(m))
                    m.Kill();

                if (m is PlayerMobile && !m.Alive) 
                    GhostsOnTheWater.GetPlayer(m);
            }
            eable.Free();

            Timer.DelayCall(TimeSpan.FromSeconds(3.0), () =>
            {
                CleanUp();
            });
        }

        public void CleanUp()
        {
            IPooledEnumerable eable = GetItemsInRange(6);
            List<Item > list = new List<Item>();
            foreach(Item item in eable)
            {
                if (item is NewShipCannon)
                {
                    NewShipCannon cannon = (NewShipCannon)item;
                    if (cannon != null && (cannon.Ship == null || cannon.Ship.Owner is BaseCreature))
                        list.Add(cannon);
                }

                if (item is Corpse && ((Corpse)item).Owner is BaseCreature)
                    list.Add(item);
            }

            for(int i = 0; i < list.Count; i++)
            {
                list[i].Delete();
            }

            Delete();
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write((int)0);

            writer.Write(Cannons != null ? Cannons.Count : 0);

            if (Cannons != null)
            {
                for (int i = 0; i < Cannons.Count; i++)
                    writer.Write(Cannons[i]);
            }

            SecurityEntry.Serialize(writer);

            // upgrades
            writer.Write(CurrentMaxCannons);
            writer.Write(UpgradedHold);
            writer.Write(UpgradedHull);
            writer.Write(BannerUpgrade);
            writer.Write(Banner);

        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();

            int count;
            count = reader.ReadInt();
            for (int i = 0; i < count; i++)
            {
                Item cannon = reader.ReadItem();
                if (cannon != null && !cannon.Deleted)
                    AddCannon(cannon);
            }

            m_SecurityEntry = new ShipSecurityEntry(this, reader);

            // upgrades
            CurrentMaxCannons = reader.ReadInt();
            UpgradedHold = reader.ReadBool();
            UpgradedHull = reader.ReadBool();
            BannerUpgrade = reader.ReadBool();
            Banner = reader.ReadItem() as ShipBanner;
        }
    }
}
