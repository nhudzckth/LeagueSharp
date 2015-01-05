using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LeagueSharp;
using LeagueSharp.Common;
using SharpDX;
using System.Reflection;
using Color = System.Drawing.Color;
using Collision = LeagueSharp.Common.Collision;

namespace PerplexedOrianna
{
    class Program
    {
        static Obj_AI_Hero Player = ObjectManager.Player;
        static System.Version Version = Assembly.GetExecutingAssembly().GetName().Version;
        static void Main(string[] args)
        {
            CustomEvents.Game.OnGameLoad += Game_OnGameLoad;
        }

        static void Game_OnGameLoad(EventArgs args)
        {
            if (Player.ChampionName != "Orianna")
                return;

            if (Updater.Outdated())
            {
                Game.PrintChat("<font color=\"#ff0000\">Perplexed Orianna is outdated! Please update to {0}!</font>", Updater.GetLatestVersion());
                return;
            }

            SpellManager.Initialize();
            ItemManager.Initialize();
            Config.Initialize();

            CustomDamageIndicator.Initialize(DamageCalc.GetDrawDamage); //Credits to Hellsing for this! Borrowed it from his Kalista assembly.

            Game.OnGameUpdate += Game_OnGameUpdate;
            Drawing.OnDraw += Drawing_OnDraw;

            Game.PrintChat("<font color=\"#ff3300\">Perplexed Orianna ({0})</font> - <font color=\"#ffffff\">Loaded!</font>", Version);
        }

        static void Game_OnGameUpdate(EventArgs args)
        {
            ItemManager.UseDefensiveItemsIfInDanger(0);
            SpellManager.UseHealIfInDanger(0);
            switch (Config.Orbwalker.ActiveMode)
            {
                case Orbwalking.OrbwalkingMode.Combo:
                    Combo();
                    break;
                case Orbwalking.OrbwalkingMode.Mixed:
                    Harass();
                    break;
                default:
                    Auto();
                    break;
            }
            SpellManager.IgniteIfPossible();
        }

        static void Combo()
        {
            var target = TargetSelector.GetTarget(SpellManager.Q.Range, TargetSelector.DamageType.Magical);
            if (target.ComboKillable())
            {
                ItemManager.UseOffensiveItems();
                StandardCombo();
                if (Config.ComboR && SpellManager.R.IsReady())
                {
                    if (!Player.HasBall())
                    {
                        target = ObjectManager.Get<Obj_AI_Hero>().Where(hero => hero.IsEnemy && hero.IsValid && hero.Distance(Ball) <= SpellManager.R.Width).FirstOrDefault();
                        if (target != null)
                            SpellManager.CastSpell(SpellManager.R, Config.UsePackets);
                    }
                    else
                    {
                        target = ObjectManager.Get<Obj_AI_Hero>().Where(hero => hero.IsEnemy && hero.IsValid && hero.Distance(Player) <= SpellManager.R.Width).FirstOrDefault();
                        if (target != null)
                            SpellManager.CastSpell(SpellManager.W, Config.UsePackets);
                    }
                }
            }
            else
                StandardCombo();
        }

        static void StandardCombo()
        {
            if (Config.ComboQ && SpellManager.Q.IsReady())
            {
                var target = TargetSelector.GetTarget(SpellManager.Q.Range, TargetSelector.DamageType.Magical);
                SpellManager.CastSpell(SpellManager.Q, target, HitChance.High, Config.UsePackets);
            }
            if (Config.ComboW && SpellManager.W.IsReady())
            {
                if (!Player.HasBall())
                {
                    var target = ObjectManager.Get<Obj_AI_Hero>().Where(hero => hero.IsEnemy && hero.IsValid && hero.Distance(Ball) <= SpellManager.W.Width).FirstOrDefault();
                    if (target != null)
                        SpellManager.CastSpell(SpellManager.W, Config.UsePackets);
                }
                else
                {
                    var target = ObjectManager.Get<Obj_AI_Hero>().Where(hero => hero.IsEnemy && hero.IsValid && hero.Distance(Player) <= SpellManager.W.Width).FirstOrDefault();
                    if (target != null)
                        SpellManager.CastSpell(SpellManager.W, Config.UsePackets);
                }
            }
        }

        static void Harass()
        {
            if (Config.HarassQ && SpellManager.Q.IsReady())
            {
                var target = TargetSelector.GetTarget(SpellManager.Q.Range, TargetSelector.DamageType.Magical);
                SpellManager.CastSpell(SpellManager.Q, target, HitChance.High, Config.UsePackets);
            }
            if (Config.HarassW && SpellManager.W.IsReady())
            {
                if (!Player.HasBall())
                {
                    var target = ObjectManager.Get<Obj_AI_Hero>().Where(hero => hero.IsEnemy && hero.IsValid && hero.Distance(Ball) <= SpellManager.W.Width).FirstOrDefault();
                    if (target != null)
                        SpellManager.CastSpell(SpellManager.W, Config.UsePackets);
                }
                else
                {
                    var target = ObjectManager.Get<Obj_AI_Hero>().Where(hero => hero.IsEnemy && hero.IsValid && hero.Distance(Player) <= SpellManager.W.Width).FirstOrDefault();
                    if (target != null)
                        SpellManager.CastSpell(SpellManager.W, Config.UsePackets);
                }
            }
        }

        static void Auto()
        {
            if (Config.AutoW && SpellManager.W.IsReady())
            {
                if (!Player.HasBall())
                {
                    var target = ObjectManager.Get<Obj_AI_Hero>().Where(hero => hero.IsEnemy && hero.IsValid && hero.Distance(Ball) <= SpellManager.W.Width).FirstOrDefault();
                    if (target != null)
                        SpellManager.CastSpell(SpellManager.W, Config.UsePackets);
                }
                else
                {
                    var target = ObjectManager.Get<Obj_AI_Hero>().Where(hero => hero.IsEnemy && hero.IsValid && hero.Distance(Player) <= SpellManager.W.Width).FirstOrDefault();
                    if (target != null)
                        SpellManager.CastSpell(SpellManager.W, Config.UsePackets);
                }
            }
            if (Config.AutoUlt && SpellManager.R.IsReady())
            {
                if (!Player.HasBall())
                {
                    try
                    {
                        var allyWithBall = ObjectManager.Get<Obj_AI_Hero>().First(hero => hero.IsAlly && hero.IsValidTarget() && hero.HasBall() && !hero.IsMe);
                        var targets = ObjectManager.Get<Obj_AI_Hero>().Where(hero => hero.IsEnemy && hero.IsValidTarget() && hero.HasBall() && hero.Distance(allyWithBall) <= SpellManager.R.Width);
                        if (targets.ToArray().Length >= Config.AutoUltAmount)
                            SpellManager.CastSpell(SpellManager.R, Config.UsePackets);
                    }
                    catch
                    {
                        var targets = ObjectManager.Get<Obj_AI_Hero>().Where(hero => hero.IsEnemy && hero.IsValid && hero.Distance(Ball) <= SpellManager.R.Width);
                        if (targets.ToArray().Length >= Config.AutoUltAmount)
                            SpellManager.CastSpell(SpellManager.R, Config.UsePackets);
                    }
                }
                else
                {
                    var targets = ObjectManager.Get<Obj_AI_Hero>().Where(hero => hero.IsEnemy && hero.IsValid && hero.Distance(Player) <= SpellManager.R.Width);
                    if (targets.ToArray().Length >= Config.AutoUltAmount)
                        SpellManager.CastSpell(SpellManager.R, Config.UsePackets);
                }
            }
        }

        static void Drawing_OnDraw(EventArgs args)
        {
            if (!Player.HasBall())
            {
                Vector2 playerPos = Drawing.WorldToScreen(Player.Position);
                Vector2 ballPos = Drawing.WorldToScreen(Ball.Position);
                Drawing.DrawLine(playerPos, ballPos, 2, Color.White);
            }
            if (Config.DrawQ.Active)
                Utility.DrawCircle(Player.Position, SpellManager.Q.Range, Config.DrawQ.Color);
            if (Config.DrawW.Active)
            {
                if(!Player.HasBall())
                    Utility.DrawCircle(Ball.Position, SpellManager.W.Width, Config.DrawW.Color);
            }
            if (Config.DrawE.Active)
                Utility.DrawCircle(Player.Position, SpellManager.E.Range, Config.DrawE.Color);
            if (Config.DrawW.Active)
            {
                if (!Player.HasBall())
                    Utility.DrawCircle(Ball.Position, SpellManager.R.Width, Config.DrawR.Color);
            }
        }

        static Obj_AI_Minion Ball
        {
            get
            {
                return ObjectManager.Get<Obj_AI_Minion>().FirstOrDefault(obj => obj.Name == "TheDoomBall" && obj.IsAlly && obj.IsVisible);
            }
        }

    }
}
