using System;
using System.Collections.Generic;

using Server.Items;
using Server.Multis;

namespace Server.Mobiles
{
    [CorpseName("a naga corpse")]
    public class NagaRelicHunter : BaseCreature
    {
        private BaseSeaShrine _Shrine;
        private List<Mobile> _Crew;

        [Constructable]
        public NagaRelicHunter()
            : base(AIType.AI_Spellweaving, FightMode.Closest, 10, 1, .2, .4)
        {
            Name = "a naga relic hunter";

            if (Female = Utility.RandomBool())
            {
                Body = 1633;
                ChangeAIType(AIType.AI_Spellweaving);
                BaseSoundID = 644;
                SpellHue = 1365;

                SetStr(416, 505);
                SetDex(116, 135);
                SetInt(466, 555);

                SetHits(4500);
                SetDamage(17, 22);
                SetMana(750);

                SetResistance(ResistanceType.Physical, 35, 45);
                SetResistance(ResistanceType.Fire, 30, 40);
                SetResistance(ResistanceType.Cold, 50, 65);
                SetResistance(ResistanceType.Poison, 40, 50);
                SetResistance(ResistanceType.Energy, 45, 55);

                SetSkill(SkillName.EvalInt, 98.1, 114.0);
                SetSkill(SkillName.Magery, 98.1, 114.0);
                SetSkill(SkillName.Meditation, 65.4, 85.0);
                SetSkill(SkillName.MagicResist, 105.1, 125.0);
                SetSkill(SkillName.Tactics, 89.1, 110.0);
                SetSkill(SkillName.Wrestling, 99.1, 109.0);
                SetSkill(SkillName.Spellweaving, 100.1, 124.0);
            }
            else
            {
                Body = 1615;
                ChangeAIType(AIType.AI_Samurai);
                BaseSoundID = 634;

                SetStr(416, 505);
                SetDex(296, 375);
                SetInt(166, 255);

                SetHits(5000);
                SetDamage(18, 27);
                SetMana(500);

                SetResistance(ResistanceType.Physical, 45, 65);
                SetResistance(ResistanceType.Fire, 30, 40);
                SetResistance(ResistanceType.Cold, 50, 65);
                SetResistance(ResistanceType.Poison, 40, 50);
                SetResistance(ResistanceType.Energy, 35, 45);

                SetSkill(SkillName.Anatomy, 101.1, 120.0);
                SetSkill(SkillName.MagicResist, 105.1, 125.0);
                SetSkill(SkillName.Tactics, 101.1, 120.0);
                SetSkill(SkillName.Wrestling, 101.1, 120.0);
                SetSkill(SkillName.Bushido, 100.1, 120.0);
                SetSkill(SkillName.Parry, 67.1, 87.0);
            }

            SetDamageType(ResistanceType.Physical, 20);
            SetDamageType(ResistanceType.Cold, 60);
            SetDamageType(ResistanceType.Energy, 20);

            _Crew = new List<Mobile>();
            Fame = 22000;
            Karma = -22000;
            CanSwim = true;

            PackItem(new Copper(Utility.RandomMinMax(25, 50)));
            Timer.DelayCall(TimeSpan.FromSeconds(0.25), () =>
            {
                SpawnShrine();
            });

        }

        public override void GenerateLoot()
        {
            AddLoot(LootPack.AosFilthyRich, 2);
            AddLoot(LootPack.Average, 2);
            AddLoot(LootPack.Potions);
            AddLoot(LootPack.MedScrolls, 3);
        }

        private DateTime NextRanged;
        public void RangedAttack(Mobile defender)
        {

            if (defender == null || defender.Deleted || !defender.Alive || !Alive || !CanBeHarmful(defender) || NextRanged > DateTime.UtcNow || Utility.RandomDouble() > 0.1)
                return;

            NextRanged = DateTime.UtcNow + TimeSpan.FromSeconds(5);
            Animate(AnimationType.Attack, 0);
            int itemID = Utility.RandomList(0xB92C, 0xB92F);
            defender.PlaySound(0x11F);
            MovingEffect(defender, itemID, 7, 0, false, true, 0, 0);
            Frozen = true;

            Timer.DelayCall(TimeSpan.FromSeconds(GetDistanceToSqrt(defender) / 5.0), () =>
            {
                int d = Utility.RandomMinMax(DamageMin, DamageMax);
                AOS.Damage(defender, d, 100, 0, 0, 0, 0);
                Frozen = false;

                if (Utility.RandomDouble() > 0.66)
                {
                    defender.Paralyze(TimeSpan.FromSeconds(3));
                    defender.FixedParticles(0xBEA7, 10, 30, 5052, 2100, 0, EffectLayer.LeftFoot);
                }
            });
        }

        public override void OnThink()
        {
            base.OnThink();

            if (Combatant != null && Combatant is Mobile && InRange(Combatant.Location, 10) && !InRange(Combatant.Location, 2) && !Female)
                RangedAttack(Combatant as Mobile);
        }

        public int GetBody()
        {
            switch (Utility.Random(3))
            {
                default:
                case 0:
                return 1632;
                case 1:
                return 1633;
                case 2:
                return 1615;
            }
        }

        public void SpawnShrine()
        {
            CanSwim = false;
            BaseSeaShrine shrine;

            shrine = GetShrine();

            var p = Location;
            Map map = Map;

            // Move this sucka out of the way!
            Internalize();

            shrine.MoveToWorld(p, map);
            _Shrine = shrine;

            MoveToWorld(new Point3D(p.X - 2, p.Y + 1, shrine.Z + 11), map);

            int crewCount = Utility.RandomMinMax(2, 3);

            for (int j = 0; j < crewCount; j++)
            {
                Mobile crew = new NagaRaider();

                if (crew != null)
                {
                    AddToCrew(crew);
                    crew.MoveToWorld(new Point3D(shrine.X + Utility.RandomList(-1, 1), shrine.Y + Utility.RandomList(-1, 0, 1), 1), map);
                }
            }

            return;

            /*
            if (shrine.CanFit(p, map, shrine.ItemID))
            {

            }
            else
            {
                shrine.Delete();
                Delete();
            }
            */
        }

        public BaseSeaShrine GetShrine()
        {
            switch (Utility.Random(4))
            {
                default:
                case 0:
                return new SeaShrineArcane();
                case 1:
                return new SeaShrineHoly();
                case 2:
                return new SeaShrineKing();
                case 3:
                return new SeaShrineWarrior();
            }
        }

        public void AddToCrew(Mobile mob)
        {
            if (_Crew == null)
                _Crew = new List<Mobile>();

            if (!_Crew.Contains(mob))
                _Crew.Add(mob);
        }

        public override bool OnBeforeDeath()
        {

            if (_Shrine != null)
            {
                _Shrine.Activate();

            }

            return base.OnBeforeDeath();
        }

        public override void OnDeath(Container c)
        {
            base.OnDeath(c);

            if (Utility.RandomDouble() < 0.05)
                c.DropItem(new AncientNagaRelic());
        }

        public NagaRelicHunter(Serial serial)
            : base(serial) { }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write((int)1);

            writer.Write(_Shrine);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();

            if (version > 0)
                _Shrine = reader.ReadItem() as BaseSeaShrine;
        }
    }
}
