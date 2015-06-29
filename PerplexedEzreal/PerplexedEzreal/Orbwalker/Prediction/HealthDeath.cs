﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;
using LeagueSharp;
using LeagueSharp.Common;

namespace DetuksSharp.Prediction
{
    class HealthDeath
    {

        private static int _lastTick;

        public static readonly Dictionary<int, DamageMaker> activeDamageMakers = new Dictionary<int, DamageMaker>();


        public static readonly Dictionary<int, DamageMaker> activeTowerTargets = new Dictionary<int, DamageMaker>();

        public static int now
        {
            get { return (int) DateTime.Now.TimeOfDay.TotalMilliseconds; }
        }

        static HealthDeath()
        {
            Game.OnUpdate += onUpdate;

            GameObject.OnCreate += onCreate;
            GameObject.OnDelete += onDelete;

            Obj_AI_Base.OnProcessSpellCast += onMeleeStartAutoAttack;
            Spellbook.OnStopCast += onMeleeStopAutoAttack;
        }


        private static void onMeleeStartAutoAttack(Obj_AI_Base sender, GameObjectProcessSpellCastEventArgs args)
        {
            if (sender is Obj_AI_Turret)
            {
                activeTowerTargets.Remove(sender.NetworkId);
                var dMake = new DamageMaker(sender,
                    (Obj_AI_Base)args.Target,
                    null,
                    args.SData,
                    true);

                activeTowerTargets.Add(sender.NetworkId, dMake);


            }

            if(!sender.IsMelee() || !args.SData.IsAutoAttack())
                return;

            if (args.Target is Obj_AI_Base)
            {
                activeDamageMakers.Remove(sender.NetworkId);
                var dMake = new DamageMaker(sender,
                    (Obj_AI_Base)args.Target,
                    null,
                    args.SData,
                    true);

                activeDamageMakers.Add(sender.NetworkId, dMake);
            }

            if (sender is Obj_AI_Hero)
            {
                DeathWalker.lastDmg = now;
            }
        }

        private static void onMeleeStopAutoAttack(Spellbook sender, SpellbookStopCastEventArgs args)
        {
            //if (!sender.Owner.IsMelee())
            //    return;

            if (activeDamageMakers.ContainsKey(sender.Owner.NetworkId))
                activeDamageMakers.Remove(sender.Owner.NetworkId);

            //Ranged aswell
            if (args.DestroyMissile && activeDamageMakers.ContainsKey(args.MissileNetworkId))
                activeDamageMakers.Remove(args.MissileNetworkId);

        }

        private static void onUpdate(EventArgs args)
        {
            //Some failsafe l8er if needed

            if (now - _lastTick <= 60 * 1000)
            {
                return;
            }

            activeDamageMakers.ToList()
                .Where(pair => pair.Value.createdTick < now - 60000)
                .ToList()
                .ForEach(pair => activeDamageMakers.Remove(pair.Key));
            _lastTick = now;
        }

        private static void onCreate(GameObject sender, EventArgs args)
        {
            //most likely AA
            if (sender is MissileClient)
            {
                var mis = (MissileClient) sender;
                if (mis.Target is Obj_AI_Base)
                {
                    var dMake = new DamageMaker(mis.SpellCaster,
                        (Obj_AI_Base) mis.Target,
                        mis,
                        mis.SData);

                    activeDamageMakers.Add(mis.NetworkId, dMake);
                }
            }
        }

        private static void onDelete(GameObject sender, EventArgs args)
        {
            if (sender is MissileClient || sender is Obj_SpellMissile)
            {
                if (activeDamageMakers.ContainsKey(sender.NetworkId))
                    activeDamageMakers.Remove(sender.NetworkId);
            }

            if (sender is Obj_AI_Base )
            {
                int i = 0;
                foreach (var dmgMk in activeDamageMakers)
                {
                    if (dmgMk.Value.source == null || dmgMk.Value.missle == null)
                        continue;
                    if (dmgMk.Value.source.NetworkId == sender.NetworkId)
                    {
                        activeDamageMakers.Remove(dmgMk.Value.missle.NetworkId);
                        return;
                    }
                    i++;
                }
            }
        }
        //Maybe later change so would return data about missile
        public static DamageMaker attackedByTurret(AttackableUnit unit)
        {
            return activeTowerTargets.Values.Where(v => v.target.NetworkId == unit.NetworkId).FirstOrDefault(attack => attack.source is Obj_AI_Turret);
        }

        //Only active attacks
        public static int getTimeTillDeath(AttackableUnit unit, bool ignoreAlmostDead = true)
        {
            int HP = (int)unit.Health;
            foreach (var attacks in activeDamageMakers.Values.OrderBy(atk => atk.hitOn))
            {
                if (attacks.target == null || attacks.target.NetworkId != unit.NetworkId || (ignoreAlmostDead && almostDead(attacks.source)))
                    continue;
                int hitOn = attacks.hitOn;
                if (hitOn > now)
                {
                    HP -= (int)attacks.dealDamage;
                    if (HP <= 0)
                        return hitOn - now;
                }
            }
            return int.MaxValue;
        }

        public static bool almostDead(AttackableUnit unit)
        {
            if (unit == null)
                return true;
            try
            {

            var hitingUnitDamage = misslesHeadedOnDamage(unit);
           // if (unit.Health < hitingUnitDamage * 0.65)
            //    Console.WriteLine("Ignore cus almost dead!");

            return unit.Health < hitingUnitDamage * 0.65;
            }
            catch (Exception)
            {
                return true;
            }

        }

        public static float getLastHitPred(AttackableUnit unit, int msTime, bool ignoreAlmostDead = true)
        {
            var predDmg = 0f;

            foreach (var attacks in activeDamageMakers.Values)
            {
                if (attacks.target == null || attacks.target.NetworkId != unit.NetworkId || (ignoreAlmostDead && almostDead(attacks.source)))
                    continue;
                int hitOn = 0;
                if (attacks.missle == null || attacks.sData.MissileSpeed == 0)
                {
                    hitOn = (int)(attacks.createdTick + attacks.source.AttackCastDelay*1000);
                }
                else
                {
                    hitOn = now +  (int)((attacks.missle.Position.Distance(unit.Position)*1000) / attacks.sData.MissileSpeed)+100;
                }
                if (now < hitOn && hitOn < now + msTime)
                {
                    predDmg += attacks.dealDamage;
                }
            }
            return unit.Health - predDmg;
        }

        public static float getLastHitPredPeriodic(AttackableUnit unit, int msTime, bool ignoreAlmostDead = true)
        {
            var predDmg = 0f;

            msTime = (msTime > 10000) ? 10000 : msTime;

            foreach (var attacks in activeDamageMakers.Values)
            {
                if (attacks.target == null || attacks.target.NetworkId != unit.NetworkId || (ignoreAlmostDead && almostDead(attacks.source)))
                    continue;
                int hitOn = 0;
                if (attacks.missle == null || attacks.sData.MissileSpeed == 0)
                {
                    hitOn = (int)(attacks.createdTick + attacks.source.AttackCastDelay * 1000);
                }
                else
                {
                    hitOn = now + (int)((attacks.missle.Position.Distance(unit.Position) * 1000) / attacks.sData.MissileSpeed);
                }

                int timeTo = now + msTime;

                int hits = (attacks.cycle == 0 )?0:(int)((timeTo - hitOn)/attacks.cycle) +1;

                if (now < hitOn && hitOn <= now + msTime)
                {
                    predDmg += attacks.dealDamage * hits;
                }
            }
            return unit.Health - predDmg;
        }

        public static int misslesHeadedOn(AttackableUnit unit)
        {
            return activeDamageMakers.Count(un => un.Value.target.NetworkId == unit.NetworkId);
        }

        public static float misslesHeadedOnDamage(AttackableUnit unit)
        {
            return activeDamageMakers.Where(un => un.Value.target.NetworkId == unit.NetworkId).Sum(un=> un.Value.dealDamage);
        }

        public class DamageMaker
        {
            public readonly GameObject missle;

            public readonly Obj_AI_Base source;

            public readonly Obj_AI_Base target;

            public readonly SpellData sData;

            public readonly float fullDamage;//Unused for now

            public readonly float dealDamage;

            public readonly bool isAutoAtack;

            public readonly int createdTick;

            public readonly bool melee;

            public readonly int cycle;

            public int hitOn
            {
                get
                {
                    try
                    {
                        if (source == null || !source.IsValid)
                            return int.MaxValue;
                        if (missle == null || DeathWalker.azir)
                        {
                            return (int)(createdTick + source.AttackCastDelay * 1000);
                        }
                        else
                        {
                            return now + (int)((missle.Position.Distance(target.Position) * 1000) / ((source is Obj_AI_Turret) ? sData.MissileSpeed*0.8f : sData.MissileSpeed));//lil delay cus dunno l8er could try to values what says delay of dmg dealing
                        }

                    }
                    catch (Exception)
                    {
                        return int.MaxValue;
                    }
                }
            }

            public DamageMaker(Obj_AI_Base sourceIn, Obj_AI_Base targetIn, GameObject missleIn, SpellData dataIn, bool meleeIn = false)
            {
                source = sourceIn;
                target = targetIn;
                missle = missleIn;
                sData = dataIn;
                melee = !meleeIn;
                createdTick = now;
                isAutoAtack = sData.IsAutoAttack();

                if (isAutoAtack)
                {

                    dealDamage = (float) source.GetAutoAttackDamage(target, true);
                    if (source.IsMeele)
                        cycle = (int) (source.AttackDelay*1000);
                    else
                    {
                        //var dist = source.Distance(target);
                        cycle = (int)((source.AttackDelay * 1000)) /*+ (dist*1000)/sData.MissileSpeed)*/;
                        //Console.WriteLine("cycle: " + cycle);
                    }
                    //Console.WriteLine("cycle: " + source.AttackSpeedMod);
                }
                else
                {
                    cycle = 0;
                    if (source is Obj_AI_Hero)
                    {
                        var tSpell = TargetSpellDatabase.GetByName(sData.Name);
                        if (tSpell == null)
                        {
                            //Console.WriteLine("Unknown targeted spell: " + sData.Name);
                            dealDamage = 0;
                        }
                        else
                        {
                            try
                            {

                                dealDamage = (float)((Obj_AI_Hero)source).GetSpellDamage(target, tSpell.Spellslot);
                            }
                            catch (Exception)
                            {
                                dealDamage = 0;
                            }
                        }
                    }
                    else
                    {
                        dealDamage = 0;
                    }
                }


            }

        }

    }
}
