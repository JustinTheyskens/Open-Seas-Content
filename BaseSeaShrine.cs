using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Server.Items;
using Server.Mobiles;

namespace Server.Multis
{
    public class BaseSeaShrine : BaseMulti
    {
        private bool _Active;
        public override bool Decays => _Active;
        public override bool HandlesOnMovement => _Active ? true : false;
        public bool Active
        {
            get {  return _Active;}
            set { _Active = value; }
        }

        public virtual int EffectHue { get { return 0; } }
        public List<Item> Addons = new List<Item>();

        public BaseSeaShrine(int id)
            : base(id) { }

        public BaseSeaShrine(Serial serial)
            : base(serial) { }

        public override void OnMovement(Mobile m, Point3D oldLocation)
        {
            if (m is PlayerMobile && InRange(m, 15) && !IsBlessed(m))
            {
                DoBless();
            }

        }

        public void DoBless()
        {  
            ArrayList list = new ArrayList();
            IPooledEnumerable eable = GetMobilesInRange(15);
            int amount = 20;

            //FixedParticles(0xBC6C, 10, 15, 5018, EffectLayer.LeftFoot);
            foreach (Mobile m in eable)
            {
                if (m is PlayerMobile && !IsBlessed(m))
                {
                    m.AddStatMod(new StatMod(StatType.Str, "Inspired_Str", amount, TimeSpan.FromMinutes(5)));
                    m.AddStatMod(new StatMod(StatType.Dex, "Inspired_Dex", amount, TimeSpan.FromMinutes(5)));
                    m.AddStatMod(new StatMod(StatType.Int, "Inspired_Int", amount, TimeSpan.FromMinutes(5)));

                    m.FixedParticles(0xB610, 3, 20, 5018, EffectHue, 0, EffectLayer.LeftFoot); //0xBCEC
                    m.PlaySound(0x1EA);
                    AddBless(m, TimeSpan.FromMinutes(5));
                }
            }
            eable.Free();
        }


        public void Activate()
        {
            _Active = true;
            SinkTimer timer = new SinkTimer(this);
            timer.Start();
            DoBuffEffects();
        }

        public void AddTeleporter(int id, Point3D position)
        {
            SeaShrineTeleporter teleporter = new SeaShrineTeleporter(this, id);
            Addons.Add(teleporter);
            Point3D location = new Point3D(X + position.X, Y + position.Y, Z + position.Z);
            Timer.DelayCall(TimeSpan.FromSeconds(0.2), () =>
            {   
                teleporter.MoveToWorld(location, Map);
            });
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

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write((int)0); // version

            writer.Write(Addons.Count);

            if (Addons != null && Addons.Count > 0)
            {
                for (int i = 0; i < Addons.Count; i++)
                {
                    writer.Write(Addons[i]);
                }
            }
        }

        public override void OnAfterDelete()
        {
            base.OnAfterDelete();

            if (Addons != null)
            {
                for(int i = 0; i < Addons.Count; ++i)
                {
                    Addons[i].Delete();
                }
            }
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();

            Addons = new List<Item>();
            int count = reader.ReadInt();
            if (count > 0)
            {
                for(int i  = 0; i < count; i++)
                {
                    Item item = reader.ReadItem();
                    Addons.Add(item);
                }
            }
        }

        public void Sink()
        {
            Timer.DelayCall(TimeSpan.FromSeconds(0.5), TimeSpan.FromSeconds(0.5), 4, () =>
            {
                this.Z -= 2;
                if (Addons != null && Addons.Count > 0)
                {
                    for (int i = 0;i < Addons.Count; ++i)
                    {
                        Addons[i].Z -= 2;
                    }
                }
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
                Delete();
            });
        }

        private class SinkTimer : Timer
        {
            private BaseSeaShrine Shrine;
            private DateTime SinkTime;
            public SinkTimer(BaseSeaShrine shrine)
                :base(TimeSpan.FromSeconds(30), TimeSpan.FromSeconds(30))
            {
                Shrine = shrine;
                SinkTime = DateTime.UtcNow + TimeSpan.FromMinutes(10);
            }

            protected override void OnTick()
            {
                if (Shrine == null)
                    Stop();


                if (SinkTime <= DateTime.UtcNow)
                {
                    Shrine.Sink();
                    Stop();
                }
                else if (SinkTime <= DateTime.UtcNow + TimeSpan.FromSeconds(35))
                {
                    IPooledEnumerable eable = Shrine.GetMobilesInRange(20);

                    foreach (Mobile m in eable)
                    {
                        if (Shrine.Contains(m) && m is PlayerMobile)
                            m.PublicOverheadMessage(Network.MessageType.Regular, 37, true, "The Shrine is sinking! Escape before it's too late!");
                    }
                    eable.Free();
                }
            }
        }
        #region static methods
        private static Dictionary<Mobile, InternalTimer> _Table;


        public static bool IsBlessed(Mobile m)
        {
            return _Table != null && _Table.ContainsKey(m);
        }


        public static void AddBless(Mobile m, TimeSpan duration)
        {
            if (_Table == null)
                _Table = new Dictionary<Mobile, InternalTimer>();

            if (_Table.ContainsKey(m))
            {
                _Table[m].Stop();
            }

            _Table[m] = new InternalTimer(m, duration);
        }

        public static void RemoveBless(Mobile m, bool early = false)
        {
            if (_Table != null && _Table.ContainsKey(m))
            {
                _Table[m].Stop();
                m.Delta(MobileDelta.Stat);

                _Table.Remove(m);
            }
        }
        #endregion

        public virtual void DoBuffEffects()
        {
            SeaShrineBuffEffect buff = new SeaShrineBuffEffect(0x373A);
            Timer.DelayCall(TimeSpan.FromSeconds(0.2), () =>
            {
                buff.MoveToWorld(new Point3D( X - 1, Y, Z + 10), Map);
                Addons.Add(buff);
            });
        }

        private class InternalTimer : Timer
        {
            public Mobile Mobile { get; set; }

            public InternalTimer(Mobile m, TimeSpan duration)
                : base(duration)
            {
                Mobile = m;
                Start();
            }

            protected override void OnTick()
            {
                RemoveBless(Mobile);
            }
        }
    }
}
