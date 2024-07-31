using Server;
using System;
using Server.Items;
using Server.Multis;
using System.Collections.Generic;
using Server.Engines.Quests;
using System.Linq;

namespace Server.Mobiles
{
    public class ShipEncounterCaptain : BaseCreature
    {
        public static readonly TimeSpan DeleteAfterDeath = TimeSpan.FromMinutes(15);
        public static readonly TimeSpan ResumeTime = TimeSpan.FromSeconds(Utility.RandomMinMax(30, 45));
        public static readonly TimeSpan DecayRetry = TimeSpan.FromSeconds(30);

        private BaseShip m_Ship;
        private bool m_OnCourse;
        private DateTime m_NextCannonShot;
        private DateTime m_NextMoveCheck;
        private DateTime m_ActionTime;
        private DateTime m_NextCrewCheck;
        private SpawnZone m_Zone;
        private bool m_Blockade;
        private List<Mobile> m_Crew = new List<Mobile>();

        [CommandProperty(AccessLevel.GameMaster)]
        public BaseShip Ship { get { return m_Ship; } }

        [CommandProperty(AccessLevel.GameMaster)]
        public bool OnCourse { get { return m_OnCourse; } }

        [CommandProperty(AccessLevel.GameMaster)]
        public DateTime NextCannonShot { get { return m_NextCannonShot; } }

        [CommandProperty(AccessLevel.GameMaster)]
        public DateTime NextMoveCheck { get { return m_NextMoveCheck; } }

        [CommandProperty(AccessLevel.GameMaster)]
        public DateTime NextCrewCheck { get { return m_NextCrewCheck; } set { m_NextCrewCheck = value; } }

        [CommandProperty(AccessLevel.GameMaster)]
        public SpawnZone Zone { get { return m_Zone; } set { m_Zone = value; } }

        [CommandProperty(AccessLevel.GameMaster)]
        public bool Blockade { get { return m_Blockade; } set { m_Blockade = value; } }

        public List<Mobile> Crew { get { return m_Crew; } }

        [CommandProperty(AccessLevel.GameMaster)]
        public virtual bool Aggressive { get { return true; } }

        public override bool PlayerRangeSensitive { get { return false; } }

        public override double TreasureMapChance { get { return 0.05; } }
        public override int TreasureMapLevel { get { return 7; } }

        [CommandProperty(AccessLevel.GameMaster)]
        public virtual TimeSpan ShootFrequency
        {
            get
            {
                return TimeSpan.FromSeconds(Math.Min(20, 20.0 - ((double)m_Crew.Count * 2.5)));
            }
        }

        [Constructable]
        public ShipEncounterCaptain() : this(null) { }

        public ShipEncounterCaptain(BaseShip ship)
            : this(ship, AIType.AI_Melee, FightMode.Weakest, 10, 1, .2, .4)
        {
        }

        public ShipEncounterCaptain(BaseShip ship, AIType ai, FightMode fm, int per, int range, double passive, double active)
            : base(ai, fm, per, range, passive, active)
        {
            m_Ship = ship;
            m_OnCourse = true;
            m_StopTime = DateTime.MinValue;

            if (ship == null)
                Timer.DelayCall(TimeSpan.FromSeconds(.5), new TimerCallback(SpawnShip));
        }

        public void SpawnShip()
        {
            BaseShip ship;

            if (this is GhostShipCaptain || this is VikingShipCaptain)
            {
                ship = new LargeDragonShip(Direction.North);
            }
            else if (this is BoneShipCaptain)
                ship = new MediumDragonShip(Direction.North);
            else if (this is PrivateerShipCaptain)
                ship = new MediumShip(Direction.North);
            else
                ship = new LargeShip(Direction.North);

            var p = Location;
            Map map = Map;

            // Move this sucka out of the way!
            Internalize();

            if (ship.CanFit(p, map, ship.ItemID))
            {
                ship.Owner = this;
                ship.MoveToWorld(p, map);
                m_Ship = ship;

                //Server.Engines.Quests.BountyQuestSpawner.FillHold(m_Ship);
                FillHold(ship);
                MoveToWorld(new Point3D(p.X, p.Y + 3, ship.Z + ship.ZSurface), map);
                ship.PaintShip(GetPaint());
                ship.CurrentMaxCannons = Utility.RandomMinMax(ship.StartingMaxCannons, ship.MaxCannons); // maybe just make it max?

                int crewCount = Utility.RandomMinMax(3, 5);

                for (int j = 0; j < crewCount; j++)
                {
                    Mobile crew = GetCrew(this);

                    //if (j == 0 && this is PirateCaptain)
                        //crew.Title = "the orc captain";

                    AddToCrew(crew);
                    crew.MoveToWorld(new Point3D(ship.X + Utility.RandomList(-1, 1), ship.Y + Utility.RandomList(-1, 0, 1), ship.ZSurface), map);
                }

                ship.AutoAddCannons(this);

                return;
            }
            else
            {
                ship.Delete();
                Delete();
            }
        }

        public int GetPaint()
        {
            if (this is GhostShipCaptain)
                return 1175;
            else
                return Utility.RandomMinMax(1847, 1852);
        }

        public Mobile GetCrew(Mobile m)
        {
            if( m is CorsairShipCaptain)
            {
                return new CorsairCrew();
            }
            else if(m is GhostShipCaptain)
            {
                return new GhostCrew();
            }
            else if( m is BoneShipCaptain)
            {
                return new BoneCrew();
            }
            else if (m is PrivateerShipCaptain)
            {
                return new PrivateerCrew();
            }
            else if (m is VikingShipCaptain)
            {
                return new VikingCrew();
            }
            else
            {
                return new PirateCrew();
            }
        }

        public void FillHold(BaseShip ship)
        {
            if (ship.Hold != null)
            {
                Container hold = ship.Hold;
                if (hold != null)
                {
                    // ship items
                    hold.DropItem(new Swab());
                    hold.DropItem(new Ramrod());
                    hold.DropItem(new Matches(Utility.RandomMinMax(5, 35)));
                    hold.DropItem(new HeavyCannonball(Utility.RandomMinMax(3, 18)));
                    hold.DropItem(new LightCannonball(Utility.RandomMinMax(3, 18)));
                    hold.DropItem(new HeavyGrapeshot(Utility.RandomMinMax(3, 18)));
                    hold.DropItem(new LightGrapeshot(Utility.RandomMinMax(3, 18)));
                    hold.DropItem(new HeavyPowderCharge(Utility.RandomMinMax(3, 18)));
                    hold.DropItem(new LightPowderCharge(Utility.RandomMinMax(3, 18)));
                    hold.DropItem(new FuseCord(Utility.RandomMinMax(3, 18)));


                    hold.DropItem(new Gold(Utility.RandomMinMax(500, 1500))); // gold. maybe too much?
                    hold.DropItem(new Copper(Utility.RandomMinMax(10, 25))); // idk the value yet.

                    int cnt = Utility.RandomMinMax(7, 14); // random loot

                    for (int i = 0; i < cnt; i++)
                    {
                        Item item = RunicReforging.GenerateRandomItem(ship);

                        if (item != null)
                            hold.DropItem(item);
                    }
                }
            }
        }

        public void OnShipDelete()
        {
            if (this.Alive && !this.Deleted)
                Kill();

            for (int i = 0; i < m_Crew.Count; i++)
            {
                Mobile mob = m_Crew[i];

                if (mob != null && !mob.Deleted)
                    mob.Kill();
            }
        }

        public override void Delete()
        {
            //if (BountyQuestSpawner.Instance != null)
               // BountyQuestSpawner.Instance.HandleDeath(this);

            if (m_Ship != null && !m_Ship.Deleted)
            {
                m_Ship.TimeOfDecay = DateTime.UtcNow + TimeSpan.FromMinutes(30);
                Timer.DelayCall(DeleteAfterDeath, new TimerStateCallback(TryDecayShip), m_Ship);
            }

            base.Delete();
        }

        public override void OnCombatantChange()
        {
            if (Combatant == null)
                CantWalk = true;
            else
                CantWalk = false;

            base.OnCombatantChange();
        }

        public void TryDecayShip(object obj)
        {
            BaseShip ship = obj as BaseShip;

            if (ship == null)
                return;

            if (ship.PlayerCount() > 0)
            {
                Timer.DelayCall(DecayRetry, new TimerStateCallback(TryDecayShip), ship);
                return;
            }

            if (ship != null && !ship.Deleted)
                ship.ForceDecay();
        }

        public void AddToCrew(Mobile mob)
        {
            if (!m_Crew.Contains(mob))
                m_Crew.Add(mob);
        }

        private DateTime m_StopTime;
        private bool m_WillResume;

        public void ResumeCourseTimed(TimeSpan ts, bool check)
        {
            if (!m_WillResume)
            {
                Timer.DelayCall(ts, new TimerCallback(ResumeCourse));
                m_WillResume = true;
            }
        }

        public void ResumeCourse()
        {
            if (m_Ship != null)
            {
                m_Ship.StartCourse(false, false);
                m_WillResume = false;
                m_OnCourse = true;
            }
        }

        public override void OnThink()
        {
            base.OnThink();

            if (m_Ship == null)
                return;

            if (m_Ship.Deleted)
                OnShipDelete();

            // Ship is fucked without his captain!!!
            if (!m_Ship.Contains(this))
                return;

            if (m_NextCrewCheck < DateTime.UtcNow)
            {
                CheckCrew();
            }

            if (!IsInBattle())
            {
                if (!m_OnCourse)
                    ResumeCourse();
                else if (m_OnCourse && !m_Ship.IsMoving && m_ActionTime < DateTime.UtcNow)
                {
                    ResumeCourseTimed(ResumeTime, true);
                    m_ActionTime = DateTime.UtcNow + ResumeTime;
                }
                return;
            }

            m_OnCourse = false;

            Mobile focusMob = GetFocusMob();

            if (m_TargetBoat == null || !InRange(m_TargetBoat.Location, 25))
                m_TargetBoat = GetFocusBoat(focusMob);

            if (focusMob == null && m_TargetBoat == null)
                return;

            if (m_NextMoveCheck < DateTime.UtcNow && !m_Ship.Scuttled && !m_Blockade)
            {
                Point3D pnt = m_TargetBoat != null ? m_TargetBoat.Location : focusMob.Location;

                int dist = (int)GetDistanceToSqrt(pnt);

                if (!Aggressive && dist < 25)
                    MoveBoat(pnt);
                else if (Aggressive && dist >= 10 && dist <= 35)
                    MoveBoat(pnt);
                else
                {
                    m_Ship.StopMove(false);
                    ResumeCourseTimed(TimeSpan.FromMinutes(2), false); //Loiter
                }
            }

            if (m_TargetBoat != null && !m_TargetBoat.Scuttled)
                ShootCannons(focusMob, true);
            else
                ShootCannons(focusMob, false);
        }

        private BaseShip m_TargetBoat;

        public Mobile GetFocusMob()
        {
            Mobile focus = Combatant as Mobile;

            if (focus == null || focus.Deleted || !focus.Alive)
            {
                int closest = 25;

                foreach (Mobile mob in m_Crew)
                {
                    if (mob.Alive && mob.Combatant is Mobile)
                    {
                        if (focus == null || (int)focus.GetDistanceToSqrt(mob) < closest)
                        {
                            focus = mob.Combatant as Mobile;
                            closest = (int)focus.GetDistanceToSqrt(mob);
                        }
                    }
                }
            }

            return focus;
        }

        public BaseShip GetFocusBoat(Mobile focusMob)
        {
            if (focusMob == null || focusMob.Deleted || focusMob.Map == null || focusMob.Map == Map.Internal)
                return null;

            BaseShip g = BaseShip.FindShipAt(focusMob, focusMob.Map);

            return g != m_Ship ? g : null;
        }

        public void MoveBoat(Point3D p)
        {
            if (m_Ship == null || m_Ship.Contains(p))
                return;

            int x = p.X;
            int y = p.Y;
            int speed;
            int flee = Aggressive ? 1 : -1;

            //Direction d = Utility.GetDirectionTo(p.X, p.Y);
            Direction dir = m_Ship.GetMovementFor(x, y, out speed);
            Direction f = m_Ship.Facing;

            if (!Aggressive)
                dir = (Direction)(((int)dir + -4) & 0x7);

            if (dir == Direction.West || dir == Direction.Left || dir == Direction.South)
            {
                m_Ship.Turn(-2 * flee, true);
                m_NextMoveCheck = DateTime.UtcNow + TimeSpan.FromSeconds(m_Ship.TurnDelay);
                return;
            }
            else if (dir == Direction.East || dir == Direction.Down)
            {
                m_Ship.Turn(2 * flee, true);
                m_NextMoveCheck = DateTime.UtcNow + TimeSpan.FromSeconds(m_Ship.TurnDelay);
                return;
            }

            m_Ship.StartMove(dir, true);
        }

        private Dictionary<IShipCannon, DateTime> m_ShootTable = new Dictionary<IShipCannon, DateTime>();

        public void ShootCannons(Mobile focus, bool shootAtBoat)
        {
            List<Item> cannons = new List<Item>(m_Ship.Cannons.Where(i => !i.Deleted));

            foreach (var cannon in cannons.OfType<IShipCannon>())
            {
                if (cannon != null)
                {
                    if (m_ShootTable.ContainsKey(cannon) && m_ShootTable[cannon] > DateTime.UtcNow)
                        continue;

                    Direction facing = cannon.GetFacing();

                    if (shootAtBoat && HasTarget(focus, cannon, true))
                    {
                        cannon.AmmoType = AmmunitionType.Cannonball;
                        //cannon.LoadedAmmo = cannon.LoadTypes[0];
                    }
                    else if (!shootAtBoat && HasTarget(focus, cannon, false))
                    {
                        cannon.AmmoType = AmmunitionType.Grapeshot;
                        //cannon.LoadedAmmo = cannon.LoadTypes[1];
                    }
                    else
                    {
                        continue;
                    }

                    cannon.Shoot(this);
                    m_ShootTable[cannon] = DateTime.UtcNow + ShootFrequency + TimeSpan.FromSeconds(Utility.RandomMinMax(0, 3));
                }
            }
        }

        private bool HasTarget(Mobile focus, IShipCannon cannon, bool shootatboat)
        {
            if (cannon == null || cannon.Deleted || cannon.Map == null || cannon.Map == Map.Internal || m_Ship == null || m_Ship.Deleted)
                return false;

            Direction d = cannon.GetFacing();
            int xOffset = 0; int yOffset = 0;
            int cannonrange = cannon.Range;
            int currentRange = 0;
            Point3D pnt = cannon.Location;

            switch (d)
            {
                case Direction.North:
                    xOffset = 0; yOffset = -1; break;
                case Direction.South:
                    xOffset = 0; yOffset = 1; break;
                case Direction.West:
                    xOffset = -1; yOffset = 0; break;
                case Direction.East:
                    xOffset = 1; yOffset = 0; break;
            }

            int xo = xOffset;
            int yo = yOffset;

            while (currentRange++ <= cannonrange)
            {
                xOffset = xo;
                yOffset = yo;

                for (int i = -1; i <= 1; i++)
                {
                    Point3D newPoint;

                    if (xOffset == 0)
                        newPoint = new Point3D(pnt.X + (xOffset + i), pnt.Y + (yOffset * currentRange), pnt.Z);
                    else
                        newPoint = new Point3D(pnt.X + (xOffset * currentRange), pnt.Y + (yOffset + i), pnt.Z);

                    if (shootatboat)
                    {
                        BaseShip g = BaseShip.FindShipAt(newPoint, this.Map);

                        if (g != null && g == m_TargetBoat && g != Ship)
                            return true;
                    }
                    else
                    {
                        if (focus == null)
                            return false;

                        if (newPoint.X == focus.X && newPoint.Y == focus.Y)
                        {
                            return true;
                        }
                    }
                }
            }

            return false;
        }

        private bool IsInBattle()
        {
            if (Combatant != null)
                return true;

            foreach (Mobile mob in m_Crew)
            {
                if (mob.Alive && mob.Combatant != null)
                    return true;
            }
            return false;
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public string AddHoldItem
        {
            get { return null; }
            set
            {
                string str = value;

                Type type = ScriptCompiler.FindTypeByName(str);

                if (type != null && (type == typeof(Item) || type.IsSubclassOf(typeof(Item))))
                {
                    Item item = Loot.Construct(type);

                    if (item != null)
                        HoldItem = item;
                }
            }
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public Item HoldItem
        {
            get { return null; }
            set
            {
                Item item = value;

                if (item != null)
                    AddItemToHold(item);
            }
        }

        public void AddItemToHold(Item item)
        {
            if (item == null)
                return;

            
            if (m_Ship != null && m_Ship.Hold != null)
                m_Ship.Hold.DropItem(item);
            else
                item.Delete();
            
        }

        public void CheckCrew()
        {
            if (m_Ship == null || m_Crew == null || Map == null || Map == Map.Internal)
                return;

            List<Mobile> crew = new List<Mobile>(m_Crew.Where(m => m != null && m.Alive && m.Map != null && m.Map != Map.Internal));

            crew.Add(this);

            foreach (var crewman in crew)
            {
                if (!m_Ship.Contains(crewman))
                {
                    crewman.MoveToWorld(new Point3D(m_Ship.X + Utility.RandomList(-1, 1), m_Ship.Y + Utility.RandomList(-1, 0, 1), m_Ship.ZSurface), this.Map);
                }
            }

            ColUtility.Free(crew);
            m_NextCrewCheck = DateTime.UtcNow + TimeSpan.FromMinutes(30);
        }

        public void CheckBlock(StaticTile tile, Point3D p)
        {
            BaseBoat check = BaseBoat.FindBoatAt(p, Map);

            if (check != null)
                CheckBlock(check, p);
        }

        public void CheckBlock(IEntity e, Point3D loc)
        {
            if (m_Ship == null || !m_OnCourse || IsInBattle())
            {
                return;
            }

            if (loc == Point3D.Zero)
            {
                loc = e.Location;
            }

            BaseBoat check = BaseBoat.FindBoatAt(new Point3D(e), Map);

            if ((check != null && check != m_Ship) || e is BaseCreature)
            {
                Direction d = Utility.GetDirection(m_Ship, e);

                int blockX = e.X;
                int blockY = e.Y;

                int x = m_Ship.X;
                int y = m_Ship.Y;
                Direction toMove = Direction.North;

                switch (m_Ship.Facing)
                {
                    case Direction.North:
                        toMove = blockX > x ? Direction.West : Direction.East;
                        break;
                    case Direction.East:
                        toMove = blockY > y ? Direction.North : Direction.South;
                        break;
                    case Direction.South:
                        toMove = blockX < x ? Direction.East : Direction.West;
                        break;
                    case Direction.West:
                        toMove = blockY < y ? Direction.South : Direction.North;
                        break;
                }

                Movement.Movement.Offset(toMove, ref x, ref y);

                int speed;
                toMove = m_Ship.GetMovementFor(x, y, out speed);

                Timer.DelayCall(TimeSpan.FromSeconds(1), () =>
                {
                    m_Ship.StartMove(toMove, false);
                });

                ResumeCourseTimed(TimeSpan.FromSeconds(11), true);
            }
        }

        public ShipEncounterCaptain(Serial serial)
            : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write((int)0);

            writer.Write(m_Blockade);

            writer.Write((int)m_Zone);
            writer.Write(m_StopTime);
            writer.Write(m_Ship);
            writer.Write(m_OnCourse);

            writer.Write(m_Crew.Count);
            foreach (Mobile mob in m_Crew)
                writer.Write(mob);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();

            m_Blockade = reader.ReadBool();
            m_Zone = (SpawnZone)reader.ReadInt();
            m_StopTime = reader.ReadDateTime();
            m_Ship = reader.ReadItem() as BaseShip;
            m_OnCourse = reader.ReadBool();

            int count = reader.ReadInt();
            for (int i = 0; i < count; i++)
            {
                Mobile mob = reader.ReadMobile() as Mobile;
                if (mob != null && !mob.Deleted && mob.Alive)
                    m_Crew.Add(mob);
            }

            if (!m_Blockade)
                ResumeCourseTimed(TimeSpan.FromSeconds(15), true);

            if (m_Ship != null)
            {
                Timer.DelayCall(TimeSpan.FromSeconds(30), CheckCrew);
            }

            m_NextMoveCheck = DateTime.UtcNow;
            m_NextCannonShot = DateTime.UtcNow;
        }

        public override bool OnBeforeDeath()
        {
            if (Ship != null)
            {
                ShipSinkTimer timer = new ShipSinkTimer(Ship);
                timer.Start();
            }

            return base.OnBeforeDeath();
        }
    }

    public class ShipSinkTimer : Timer
    {
        private BaseShip Ship;
        private DateTime SinkTime;
        public ShipSinkTimer(BaseShip ship)
            : base(TimeSpan.FromSeconds(30), TimeSpan.FromSeconds(30))
        {
            Ship = ship;
            SinkTime = DateTime.UtcNow + TimeSpan.FromMinutes(2);
        }

        protected override void OnTick()
        {
            if (Ship == null)
                Stop();


            if (SinkTime <= DateTime.UtcNow)
            {
                Ship.Sink();
                Stop();
            }
            else if (SinkTime <= DateTime.UtcNow + TimeSpan.FromSeconds(35))
            {
                IPooledEnumerable eable = Ship.GetMobilesInRange(20);

                foreach (Mobile m in eable)
                {
                    if (Ship.Contains(m) && m is PlayerMobile)
                        m.PublicOverheadMessage(Network.MessageType.Regular, 37, true, "The Ship is sinking! Escape before it's too late!");
                }
                eable.Free();
            }
        }
    }
}
