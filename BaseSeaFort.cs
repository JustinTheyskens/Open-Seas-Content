using System;
using System.Collections.Generic;

using Server.Items;
using Server.Mobiles;
using Server.Network;

namespace Server.Multis
{
    public class SeaFortController : Item
    {

        private BaseSeaFort _SeaFort;
        public BaseSeaFort SeaFort { get { return _SeaFort; } }
        public List<Item> Addons;

        public static SeaFortController FindControllertAt(IEntity entity)
        {
            return FindControllerAt(entity, entity.Map);
        }

        public static SeaFortController FindControllerAt(IPoint2D loc, Map map)
        {
            if (map == null || map == Map.Internal)
                return null;

            var items = map.GetItemsInRange(loc, 0);

            try
            {
                foreach (var item in items)
                {
                    if (item is SeaFortController controller)
                        return controller;
                }
            }
            finally
            {
                items.Free();
            }

            return null;
        }

        [Constructable]
        public SeaFortController(BaseSeaFort fort)
            : base(0x0BFC)
        {
            Movable = false;
            Visible = false;
            _SeaFort = fort;
            Addons = new List<Item>();

            Timer.DelayCall(TimeSpan.FromSeconds(0.5), () =>
            {
                if (fort.ChestLocations != null)
                {
                    fort.AddTreasure();
                    fort.SpawnEntrances();
                }
            });
        }

        public SeaFortController(Serial serial) : base(serial) { }

        public override void GetProperties(ObjectPropertyList list)
        {
            if (_SeaFort != null)
            {
                list.Add("Sea Fort Controller");

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

        public override void OnAfterDelete()
        {
            base.OnAfterDelete();
            if (_SeaFort != null)
                _SeaFort.Delete();

            if (Addons != null)
            {
                for (int i = 0; i < Addons.Count; i++)
                {
                    Addons[i]?.Delete();
                }
            }
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write((int)0);

            writer.Write(_SeaFort);
            if (Addons != null)
                writer.Write(Addons.Count);
            else
                writer.Write(0);

            if (Addons != null && Addons.Count > 0)
            {
                for (int i = 0; i < Addons.Count; i++)
                {
                    writer.Write(Addons[i]);
                }
            }
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();

            _SeaFort = reader.ReadItem() as BaseSeaFort;

            if (_SeaFort == null)
                Delete();

            Addons = new List<Item>();
            int count = reader.ReadInt();
            if (count > 0)
            {
                for (int i = 0; i < count; i++)
                {
                    Item item = reader.ReadItem();
                    Addons.Add(item);
                }
            }
        }
    }

    public abstract class BaseSeaFort : BaseMulti
    {
        public DamageLevel m_DamageTaken;

        [CommandProperty(AccessLevel.GameMaster)]
        public DamageLevel DamageTaken
        {
            get { return m_DamageTaken; }
            set
            {
                DamageLevel oldDamage = m_DamageTaken;

                m_DamageTaken = value;

            }
        }

        public virtual int DamageValue
        {
            get
            {
                switch (m_DamageTaken)
                {
                    default:
                    case DamageLevel.Pristine:
                    case DamageLevel.Slightly:
                    return 0;
                    case DamageLevel.Moderately:
                    case DamageLevel.Heavily:
                    return 1;
                    case DamageLevel.Severely:
                    return 2;
                }
            }
        }

        public virtual TimeSpan DecayDelay { get { return TimeSpan.FromHours(1); } }
        public virtual int MaxHits { get { return 20000; } }

        public int m_Hits;

        [CommandProperty(AccessLevel.GameMaster)]
        public int Hits { get { return m_Hits; } set { m_Hits = value; ComputeDamage(); InvalidateProperties(); } }

        [CommandProperty(AccessLevel.GameMaster)]
        public double Durability { get { return m_Hits / (double)MaxHits * 100.0; } }

        private DateTime m_DecayTime;

        [CommandProperty(AccessLevel.GameMaster)]
        public DateTime TimeOfDecay { get { return m_DecayTime; } set { m_DecayTime = value; } }

        private SeaFortController _Controller;
        [CommandProperty(AccessLevel.GameMaster)]
        public SeaFortController Controller { get { return _Controller; } }

        public virtual Point2D ControllerLocation { get { return new Point2D(0, 0); } }
        public virtual Point2D[] Entrances { get; set; }
        public virtual int ZEntrance { get { return 5; } }

        public List<Item> Infos;

        public abstract Point3D[] ChestLocations { get; }

        public Mobile Owner { get; set; }

        public BaseSeaFort(int itemID)
            : base(itemID)
        {
            Movable = false;

            m_Hits = MaxHits;
            m_DamageTaken = DamageLevel.Pristine;
            _Controller = new SeaFortController(this);
            Infos = new List<Item>();

            Timer.DelayCall(TimeSpan.FromSeconds(0.5), () =>
            {
                if (ControllerLocation != Point2D.Zero)
                    _Controller.MoveToWorld(new Point3D(X + ControllerLocation.X, Y + ControllerLocation.Y, Z + 5), Map);
                else
                    _Controller.MoveToWorld(new Point3D(X, Y, Z + 5), Map);
            });
        }

        public BaseSeaFort(Serial serial)
            : base(serial) { }

        public override bool ForceShowProperties { get { return true; } }

        private void ComputeDamage()
        {
            if (Durability >= 100)
                DamageTaken = DamageLevel.Pristine;
            else if (Durability >= 75.0)
                DamageTaken = DamageLevel.Slightly;
            else if (Durability >= 50.0)
                DamageTaken = DamageLevel.Moderately;
            else if (Durability >= 25.0)
                DamageTaken = DamageLevel.Heavily;
            else
                DamageTaken = DamageLevel.Severely;
        }

        public virtual void OnTakenDamage(int damage)
        {
            OnTakenDamage(null, damage);
        }

        public virtual void OnTakenDamage(Mobile damager, int damage)
        {
            Hits -= damage;

            if (damager != null)
                SendDamagePacket(damager, damage);

            if (Hits < 0)
                Hits = 0;

            if (Hits > MaxHits)
                Hits = MaxHits;

            if (_Controller != null)
                _Controller.InvalidateProperties();

            if (Infos != null && Infos.Count > 0)
            {
                for (int i = 0; i < Infos.Count; i++)
                {
                    Infos[i].InvalidateProperties();
                }
            }

            if (Hits <= 0)
            {
                ActivateEntrances();
            }
        }

        public virtual void SendDamagePacket(Mobile from, int amount)
        {
            if (amount == 0)
                return;

            NetState theirState = (from == null ? null : from.NetState);

            if (theirState == null && from != null)
            {
                Mobile master = from.GetDamageMaster(null);

                if (master != null)
                {
                    theirState = master.NetState;
                }
            }

            if (theirState != null)
            {
                theirState.Send(new DamagePacket(this, amount));
            }
        }
        public virtual bool HasAccess(Mobile from)
        {
            return true;
        }

        public void ActivateEntrances()
        {
            if (_Controller == null)
                return;

            if (_Controller.Addons != null && _Controller.Addons.Count > 0)
            {
                for (int i = 0; i < _Controller.Addons.Count; i++)
                {
                    if (_Controller.Addons[i] != null && _Controller.Addons[i] is SeaFortTeleporter)
                        ((SeaFortTeleporter)_Controller.Addons[i]).Active = true;
                }
            }
        }

        public void SpawnEntrances()
        {
            if (Entrances == null || Entrances.Length == 0 || Controller == null || Controller.Deleted)
                return;

            for (int i = 0; i < Entrances.Length - 1; i++)
            {
                SeaFortTeleporter entrance = new SeaFortTeleporter(this);
                Point3D p = new Point3D(X + Entrances[i].X, Y + Entrances[i].Y, Z + ZEntrance);
                entrance.MoveToWorld(p, Map);
                Controller.Addons.Add(entrance);
                entrance.Controller = Controller;
            }

        }

        public bool CanFit(Point3D p, Map map, int itemID)
        {
            if (map == null || map == Map.Internal || Deleted)
                return false;

            MultiComponentList newComponents = MultiData.GetComponents(itemID);

            for (int x = 0; x < newComponents.Width; ++x)
            {
                for (int y = 0; y < newComponents.Height; ++y)
                {
                    int tx = p.X + newComponents.Min.X + x;
                    int ty = p.Y + newComponents.Min.Y + y;

                    if (newComponents.Tiles[x][y].Length == 0 || Contains(tx, ty) || IsExcludedTile(newComponents.Tiles[x][y]))
                        continue;

                    LandTile landTile = map.Tiles.GetLandTile(tx, ty);
                    StaticTile[] tiles = map.Tiles.GetStaticTiles(tx, ty, true);

                    bool hasWater = false;
                    int dif = Math.Abs(landTile.Z - p.Z);

                    if (dif >= 0 && dif <= 1 && ((landTile.ID >= 168 && landTile.ID <= 171) || (landTile.ID >= 310 && landTile.ID <= 311)))
                        hasWater = true;

                    int z = p.Z;

                    for (int i = 0; i < tiles.Length; ++i)
                    {
                        StaticTile tile = tiles[i];

                        if (IsExcludedTile(tile))
                            continue;
                        else
                            return false;

                        /* 
                        bool isWater = tile.ID >= 0x1796 && tile.ID <= 0x17B2;

                        if (tile.Z == p.Z && isWater)
                        {
                            hasWater = true;
                        }
                        else if (tile.Z >= p.Z && !isWater)
                        {
                            return false;
                        }
                        */
                    }

                    //if (!hasWater)
                    //return false; 
                }
            }

            IPooledEnumerable eable = map.GetObjectsInBounds(new Rectangle2D(p.X + newComponents.Min.X, p.Y + newComponents.Min.Y, newComponents.Width, newComponents.Height));

            foreach (IEntity e in eable)
            {
                if (e == this)
                    continue;

                int x = e.X - p.X + newComponents.Min.X;
                int y = e.Y - p.Y + newComponents.Min.Y;

                // No multi tiles on that point -or- mast/sail tiles
                if (x >= 0 && x < newComponents.Width && y >= 0 && y < newComponents.Height)
                {
                    if (newComponents.Tiles[x][y].Length == 0 || IsExcludedTile(newComponents.Tiles[x][y]))
                        continue;
                }

                eable.Free();
                return false;
            }

            eable.Free();
            return true;
        }

        public virtual bool IsExcludedTile(StaticTile tile)
        {
            return false;
        }

        public virtual bool IsExcludedTile(StaticTile[] tile)
        {
            return false;
        }

        public virtual Point2D GetSpawnPosition(int range)
        {
            Point2D loc = new Point2D(0, 0);
            return GetSpawnPosition(loc, Map, range);
        }

        public static Point2D GetSpawnPosition(Point2D from, Map map, int range)
        {
            if (map == null)
                return from;

            for (int i = 0; i < 10; i++)
            {
                int x = from.X + Utility.RandomMinMax(-range, range);
                int y = from.Y + Utility.RandomMinMax(-range, range);
                int z = map.GetAverageZ(x, y);

                Point2D p = new Point2D(x, y);

                return p;
            }

            return from;
        }

        public void AddTreasure()
        {
            if (ChestLocations == null)
                return;

            if (ChestLocations.Length > 0)
            {
                for (int i = 0; i < ChestLocations.Length; i++)
                {
                    BaseTreasureChestMod chest = RandomEnigmaChest();
                    Point3D p = ChestLocations[i];
                    Point3D loc = new Point3D(X + p.X, Y + p.Y, Z + p.Z);
                    chest.MoveToWorld(loc, Map);

                    if (_Controller != null)
                    {
                        _Controller.Addons.Add(chest);
                    }
                }
            }
        }

        public BaseTreasureChestMod RandomEnigmaChest()
        {
            double random = Utility.RandomDouble();

            if (random > 0.95)
                return new EnigmaChest4();
            else if (random > 0.75)
                return new EnigmaChest3();
            else if (random > 0.50)
                return new EnigmaChest2();
            else
                return new EnigmaChest1();

        }

        public override bool Contains(int x, int y)
        {
            if (base.Contains(x, y))
                return true;

            if (_Controller != null && x == _Controller.X && y == _Controller.Y)
                return true;

            return false;
        }

        public void BeginSink()
        {
            Timer.DelayCall(TimeSpan.FromMinutes(15), () =>
            {
                Sink();
            });
        }

        public void Sink()
        {
            _ = Timer.DelayCall(TimeSpan.FromSeconds(0.5), TimeSpan.FromSeconds(0.5), 6, () =>
            {
                Z -= 2;
                for (int i = 0; i < Infos.Count; i++)
                {
                    Infos[i].Z -= 2;
                }
            });

            IPooledEnumerable eable = GetMobilesInRange(20);

            foreach (Mobile m in eable)
            {
                if (m == null || m.Deleted)
                    continue;

                m.PlaySound(0x020);

                if (m.Alive && !m.IsDeadBondedPet && this.Contains(m))
                {
                    if (m is PlayerMobile || (m is BaseCreature && ((BaseCreature)m).GetMaster() is PlayerMobile))
                        m.Kill();
                    else
                        m.Delete();
                }

                if (m is PlayerMobile && !m.Alive)
                    GhostsOnTheWater.GetPlayer(m);

            }
            eable.Free();

            _ = Timer.DelayCall(TimeSpan.FromSeconds(3.0), () =>
            {
                if (_Controller != null)
                    _Controller.Delete();
                else
                    Delete();
            });
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write((int)0);

            writer.Write(m_Hits);
            writer.Write((int)m_DamageTaken);
            writer.WriteDeltaTime(m_DecayTime);

            writer.Write(_Controller);

            writer.Write((int)Infos.Count);

            if (Infos.Count > 0)
            {
                for (int i = 0; i < Infos.Count; i++)
                {
                    writer.Write(Infos[i]);
                }
            }

        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();

            m_Hits = reader.ReadInt();
            m_DamageTaken = (DamageLevel)reader.ReadInt();

            m_DecayTime = reader.ReadDeltaTime();

            _Controller = reader.ReadItem() as SeaFortController;

            Infos = new List<Item>();
            int count = reader.ReadInt();
            if (count > 0)
            {
                for (int i = 0; i < count; i++)
                {
                    SeaFortInfo info = reader.ReadItem() as SeaFortInfo;
                    Infos.Add(info);
                }
            }

            if (Owner == null || Owner.Deleted)
            {
                Sink();
            }
        }

        public virtual IEnumerable<IEntity> GetComponents()
        {
            yield break;
        }

        public override void OnAfterDelete()
        {
            base.OnAfterDelete();

            if (Infos != null && Infos.Count > 0)
            {
                for (int i = 0; i < Infos.Count; i++)
                {
                    Infos[i].Delete();
                }
            }
        }
    }
}
