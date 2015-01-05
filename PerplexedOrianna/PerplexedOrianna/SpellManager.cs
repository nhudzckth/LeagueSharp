using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LeagueSharp;
using LeagueSharp.Common;

namespace PerplexedOrianna
{
    class SpellManager
    {
        private static Obj_AI_Hero Player = ObjectManager.Player;

        private static Spell _Q, _W, _E, _R;

        public static Spell Q { get { return _Q; } }
        public static Spell W { get { return _W; } }
        public static Spell E { get { return _E; } }
        public static Spell R { get { return _R; } }

        public static void Initialize()
        {
            _Q = new Spell(SpellSlot.Q, 825);
            _Q.SetSkillshot(0.25f, 80, 1300, false, SkillshotType.SkillshotLine);

            _W = new Spell(SpellSlot.W, 250);
            _W.SetSkillshot(0f, 250, float.MaxValue, false, SkillshotType.SkillshotCircle);

            _E = new Spell(SpellSlot.E, 1095);
            _E.SetSkillshot(0.25f, 145, 1700, false, SkillshotType.SkillshotLine);

            _R = new Spell(SpellSlot.R, 370);
            _R.SetSkillshot(0.60f, 370, float.MaxValue, false, SkillshotType.SkillshotCircle);
        }

        public static void CastSpell(Spell spell, Obj_AI_Base target, HitChance hitChance, bool packetCast)
        {
            if (target.IsValidTarget(spell.Range) && spell.GetPrediction(target).Hitchance >= hitChance)
                spell.Cast(target, packetCast);
        }

        public static void CastSpell(Spell spell, Obj_AI_Base target, bool packetCast)
        {
            spell.Cast(target, packetCast);
        }

        public static void CastSpell(Spell spell, bool packetCast)
        {
            spell.Cast(packetCast);
        }

        public static void UseHealIfInDanger(double incomingDmg)
        {
            if (Config.UseHeal)
            {
                int healthToUse = (int)(Player.MaxHealth / 100) * Config.HealPct;
                if ((Player.Health - incomingDmg) <= healthToUse)
                {
                    SpellSlot healSlot = Utility.GetSpellSlot(Player, "SummonerHeal");
                    if (healSlot != SpellSlot.Unknown)
                        Player.Spellbook.CastSpell(healSlot);
                }
            }
        }

        internal static void IgniteIfPossible()
        {
            if (Config.UseIgnite)
            {
                SpellSlot igniteSlot = Utility.GetSpellSlot(Player, "SummonerDot");
                if (igniteSlot != SpellSlot.Unknown)
                {
                    var targets = ObjectManager.Get<Obj_AI_Hero>().Where(hero => hero.IsValidTarget(600) && hero.IsEnemy);
                    foreach (var target in targets)
                    {
                        if (Config.IgniteMode == "Combo" && Config.Orbwalker.ActiveMode == Orbwalking.OrbwalkingMode.Combo)
                            Player.Spellbook.CastSpell(igniteSlot, target);
                        else
                        {
                            double igniteDamage = Damage.GetSummonerSpellDamage(Player, target, Damage.SummonerSpell.Ignite);
                            if (target.Health < igniteDamage)
                                Player.Spellbook.CastSpell(igniteSlot, target);
                        }
                    }
                }
            }
        }

        internal static void ShieldAlly(Obj_AI_Hero target)
        {
            if (SpellManager.E.IsReady() && Config.AutoShield)
                SpellManager.CastSpell(SpellManager.E, target, Config.UsePackets);
        }
    }
}
