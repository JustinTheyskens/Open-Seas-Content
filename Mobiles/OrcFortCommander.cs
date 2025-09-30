using System;
using System.Collections.Generic;
using System.Linq;

using Server.Items;
using Server.Misc;
using Server.Multis;

namespace Server.Mobiles
{
    [CorpseName("an orcish corpse")]
    public class OrcFortCommander : BaseCreature
    {
        private DateTime m_NextCrewCheck;
        private SeaFortController _Controller;
        private BaseSeaFort _Fort;
        private List<Mobile> _Crew;

        [CommandProperty(AccessLevel.GameMaster)]
        public DateTime NextCrewCheck { get { return m_NextCrewCheck; } set { m_NextCrewCheck = value; } }

        [Constructable]
        public OrcFortCommander()
            : base(AIType.AI_Melee, FightMode.Closest, 10, 1, 0.2, 0.4)
        {
            Name = "an orcish fort commander";
            Body = 753; //138;
            Hue = 2346;
            BaseSoundID = 0x45A;

            SetStr(301, 330);
            SetDex(101, 110);
            SetInt(301, 330);

            SetHits(4000);

            SetDamage(22, 26);

            SetDamageType(ResistanceType.Physical, 100);

            SetResistance(ResistanceType.Physical, 45, 55);
            SetResistance(ResistanceType.Fire, 20, 30);
            SetResistance(ResistanceType.Cold, 10, 20);
            SetResistance(ResistanceType.Poison, 30, 40);
            SetResistance(ResistanceType.Energy, 30, 40);

            SetSkill(SkillName.MagicResist, 85.1, 100.0);
            SetSkill(SkillName.Tactics, 80.1, 100.0);
            SetSkill(SkillName.Wrestling, 60.1, 100.0);

            Fame = 22000;
            Karma = -22000;

            VirtualArmor = 50;

            CanSwim = true;

            PackItem(new Copper(Utility.RandomMinMax(25, 50)));

            //     SetSpecialAbility(SpecialAbility.PrimalSlam);
            SetWeaponAbility(WeaponAbility.CrushingBlow);

            _Crew = new List<Mobile>();
            FillPack();
            Timer.DelayCall(TimeSpan.FromSeconds(0.25), () =>
            {
                SpawnFort();
            });
        }

        public OrcFortCommander(Serial serial)
            : base(serial)
        {
        }

        public override InhumanSpeech SpeechType
        {
            get
            {
                return InhumanSpeech.Orc;
            }
        }
        public override bool CanRummageCorpses
        {
            get
            {
                return true;
            }
        }
        public override int TreasureMapLevel
        {
            get
            {
                return 5;
            }
        }

        public override TribeType Tribe { get { return TribeType.Orc; } }

        public override OppositionGroup OppositionGroup
        {
            get
            {
                return OppositionGroup.SavagesAndOrcs;
            }
        }
        public override void GenerateLoot()
        {
            AddLoot(LootPack.AosUltraRich);
            AddLoot(LootPack.Gems, 1);
            AddLoot(LootPack.Potions);
        }

        public void FillPack()
        {
            Backpack pack = new Backpack();
            PackItem(pack);

            pack.DropItem(new Swab());
            pack.DropItem(new Ramrod());
            pack.DropItem(new Matches(Utility.RandomMinMax(5, 35)));
            pack.DropItem(new LightCannonball(Utility.RandomMinMax(3, 18)));
            pack.DropItem(new LightCannonball(Utility.RandomMinMax(3, 18)));
            pack.DropItem(new LightGrapeshot(Utility.RandomMinMax(3, 18)));
            pack.DropItem(new PowderCharge(Utility.RandomMinMax(3, 18)));
            pack.DropItem(new PowderCharge(Utility.RandomMinMax(3, 18)));
            pack.DropItem(new FuseCord(Utility.RandomMinMax(3, 18)));

            if (Utility.RandomBool())
                pack.DropItem(new FlameCannonball(Utility.RandomMinMax(3, 18)));
            else
                pack.DropItem(new FrostCannonball(Utility.RandomMinMax(3, 18)));
        }

        public override void OnThink()
        {
            base.OnThink();

            if (m_NextCrewCheck < DateTime.UtcNow)
            {
                CheckCrew();
            }

            if (Combatant != null && Combatant is Mobile && InRange(Combatant, 12) && !InRange(Combatant, 2))
            {
                ThrowHatchet(Combatant as Mobile);
            }
        }

        private DateTime NextAttack;
        public void ThrowHatchet(Mobile to)
        {
            if (to == null || !to.Alive || Utility.RandomDouble() > 0.1 || NextAttack > DateTime.UtcNow)
                return;

            NextAttack = DateTime.UtcNow + TimeSpan.FromSeconds(5);
            int damage = Utility.RandomMinMax(DamageMin, DamageMax);
            MovingEffect(to, 0xF43, 10, 0, false, false);
            DoHarmful(to);
            AOS.Damage(to, this, damage, 100, 0, 0, 0, 0);
        }

        public override bool OnBeforeDeath()
        {

            if (_Fort != null)
            {
                FortSinkTimer timer = new FortSinkTimer(_Fort);
                timer.Start();
                _Fort.BeginSink();
            }

            return base.OnBeforeDeath();
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write((int)0);

            writer.Write(_Controller);

            if (_Crew != null && _Crew.Count > 0)
            {
                writer.Write(_Crew.Count);
                if (_Crew.Count > 0)
                {
                    foreach (Mobile mob in _Crew)
                        writer.Write(mob);
                }
            }
            else
                writer.Write(0);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();

            _Controller = reader.ReadItem() as SeaFortController;

            if (_Controller != null && _Controller.SeaFort != null)
                _Fort = _Controller.SeaFort;

            _Crew = new List<Mobile>();
            int count = reader.ReadInt();
            if (count > 0)
            {
                for (int i = 0; i < count; i++)
                {
                    Mobile mob = reader.ReadMobile();
                    _Crew.Add(mob);
                }
            }

            if (_Fort != null)
            {

                Timer.DelayCall(TimeSpan.FromSeconds(30), CheckCrew);
            }
        }

        public void SpawnFort()
        {
            CanSwim = true;
            BaseSeaFort fort;

            fort = GetFort();

            var p = Location;
            Map map = Map;

            // Move this sucka out of the way!
            Internalize();

            fort.MoveToWorld(p, map);
            _Fort = fort;
            fort.Owner = this;
            if (fort.Controller != null)
                _Controller = fort.Controller;

            MoveToWorld(new Point3D(p.X - 2, p.Y + 1, fort.ZEntrance + 1), map);

            int crewCount = GetCrewSize(fort);

            for (int j = 0; j < crewCount; j++)
            {
                Mobile crew = new OrcRaider();

                if (crew != null)
                {
                    AddToCrew(crew);
                    crew.MoveToWorld(new Point3D(fort.X + Utility.RandomList(-1, 1), fort.Y + Utility.RandomList(-1, 0, 1), 1), map);
                }
            }

            return;
        }

        public BaseSeaFort GetFort()
        {
            switch (Utility.Random(5))
            {
                default:
                case 0:
                return new ShipSeaFort();
                case 1:
                return new SmallSeaFort();
                case 2:
                return new MediumSeaFort();
                case 3:
                return new LargeSeaFort();
                case 4:
                return new SeaFortStronghold();
            }
        }

        public void AddToCrew(Mobile mob)
        {
            if (_Crew == null)
                _Crew = new List<Mobile>();

            if (!_Crew.Contains(mob))
                _Crew.Add(mob);
        }

        public void CheckCrew()
        {
            if (_Fort == null || _Crew == null || Map == null || Map == Map.Internal)
                return;

            List<Mobile> crew = new List<Mobile>(_Crew.Where(m => m != null && m.Alive && m.Map != null && m.Map != Map.Internal));

            crew.Add(this);

            foreach (var crewman in crew)
            {
                if (!_Fort.Contains(crewman))
                {
                    crewman.MoveToWorld(new Point3D(_Fort.X + Utility.RandomList(-1, 1), _Fort.Y + Utility.RandomList(-1, 0, 1), _Fort.Z), Map);
                }
            }

            ColUtility.Free(crew);
            m_NextCrewCheck = DateTime.UtcNow + TimeSpan.FromMinutes(30);
        }

        public int GetCrewSize(BaseSeaFort fort)
        {
            if (fort == null)
                return 0;

            if (fort is SeaFortStronghold)
                return 6;
            else if (fort is LargeSeaFort)
                return 5;
            else if (fort is MediumSeaFort)
                return 4;
            else if (fort is SmallSeaFort)
                return 3;
            else
                return 2;
        }
    }

    public class FortSinkTimer : Timer
    {
        private BaseSeaFort Fort;
        private DateTime SinkTime;
        public FortSinkTimer(BaseSeaFort fort)
            : base(TimeSpan.FromSeconds(30), TimeSpan.FromSeconds(30))
        {
            Fort = fort;
            SinkTime = DateTime.UtcNow + TimeSpan.FromMinutes(15);
        }

        protected override void OnTick()
        {
            if (Fort == null)
                Stop();

            if (SinkTime <= DateTime.UtcNow)
            {
                //Fort.Sink();
                Stop();
            }
            else if (SinkTime <= DateTime.UtcNow + TimeSpan.FromSeconds(35))
            {
                IPooledEnumerable eable = Fort.GetMobilesInRange(20);

                foreach (Mobile m in eable)
                {
                    if (Fort.Contains(m) && m is PlayerMobile)
                        m.PublicOverheadMessage(Network.MessageType.Regular, 37, true, "The Fort is sinking! Escape before it's too late!");
                }
                eable.Free();
            }
        }
    }
}
