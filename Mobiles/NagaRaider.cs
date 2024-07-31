using Server.Items;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ultima;

namespace Server.Mobiles
{
    [CorpseName("a naga corpse")]
    internal class NagaRaider : BaseCreature
    {

        [Constructable]
        public NagaRaider()
            : base(AIType.AI_Melee, FightMode.Closest, 10, 1, 0.2, 0.4)
        {
            Name = "a naga raider";

            SetDamageType(ResistanceType.Physical, 60);
            SetDamageType(ResistanceType.Cold, 40);

            if (Female = Utility.RandomBool())
            {
                Body = 1593;
                BaseSoundID = 644;

                SetStr(140, 160);
                SetDex(110, 160);
                SetInt(170, 190);
                SetHits(1700);
                SetDamage(5, 7);

                SetResistance(ResistanceType.Physical, 35, 45);
                SetResistance(ResistanceType.Fire, 20, 30);
                SetResistance(ResistanceType.Cold, 50, 65);
                SetResistance(ResistanceType.Poison, 40, 50);
                SetResistance(ResistanceType.Energy, 30, 35);

                ChangeAIType(AIType.AI_Mage);

                SetSkill(SkillName.Wrestling, 55.0, 75.0);
                SetSkill(SkillName.Tactics, 60.0, 70.0);
                SetSkill(SkillName.MagicResist, 65.0, 75.0);
                SetSkill(SkillName.Magery, 70.0, 100.0);
                SetSkill(SkillName.EvalInt, 70.0, 85.0);
                SetSkill(SkillName.Meditation, 70.0, 90.0);
                SetSkill(SkillName.Focus, 80.0, 100.0);
            }
            else
            {
                Body = 1594;
                BaseSoundID = 639;

                SetStr(160, 210);
                SetDex(140, 180);
                SetInt(30, 70);
                SetHits(2000);
                SetDamage(8, 14);

                SetResistance(ResistanceType.Physical, 20, 35);
                SetResistance(ResistanceType.Fire, 30, 40);
                SetResistance(ResistanceType.Cold, 15, 25);
                SetResistance(ResistanceType.Poison, 15, 25);
                SetResistance(ResistanceType.Energy, 20, 30);

                SetSkill(SkillName.Tactics, 55.0, 70.0);
                SetSkill(SkillName.MagicResist, 50.0, 70.0);
                SetSkill(SkillName.Anatomy, 60.0, 85.0);
                SetSkill(SkillName.Healing, 60.0, 80.0);
                SetSkill(SkillName.Wrestling, 100.0, 120.0);
            }

            Fame = 8000;
            Karma = -8000;

            PackItem(new Copper(Utility.RandomMinMax(5, 10)));
        }
        public NagaRaider(Serial serial)
            : base(serial) { }

        public override void OnThink()
        {
            base.OnThink();

            if (Combatant != null && Combatant is Mobile && !Female && InRange(Combatant.Location, 10) && !InRange(Combatant.Location, 2))
                RangedAttack(Combatant as Mobile);
        }

        private DateTime NextRanged;
        public void RangedAttack(Mobile target)
        {
            if (NextRanged > DateTime.UtcNow || target == null || !target.Alive || target.Deleted || Utility.RandomDouble() > 0.1)
                return;

            NextRanged = DateTime.UtcNow + TimeSpan.FromSeconds(5);
            Animate(AnimationType.Attack, 0);
            MovingEffect(target, 0x1BFE, 7, 1, false, false, 0x481, 0);

            Timer.DelayCall(TimeSpan.FromSeconds(GetDistanceToSqrt(target) / 5.0), () =>
            {
                int d = Utility.RandomMinMax(DamageMin, DamageMax);
                AOS.Damage(target, d, 100, 0, 0, 0, 0);

                if (Utility.RandomDouble() > 0.66)
                {
                    target.Paralyze(TimeSpan.FromSeconds(3));
                    target.FixedParticles(0xBEA7, 10, 30, 5052, 2100, 0, EffectLayer.LeftFoot);
                }
            });
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write((int)0);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();

        }
    }
}
