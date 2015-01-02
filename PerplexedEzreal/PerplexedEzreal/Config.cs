using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LeagueSharp;
using LeagueSharp.Common;
using Color = System.Drawing.Color;

namespace PerplexedEzreal
{
    class Config
    {
        public static Menu Settings = new Menu("Perplexed Ezreal", "menu", true);
        public static Orbwalking.Orbwalker Orbwalker;

        public static void Initialize()
        {
            //Orbwalker
            Settings.AddSubMenu(new Menu("Orbwalker", "orbMenu"));
            Orbwalker = new Orbwalking.Orbwalker(Settings.SubMenu("orbMenu"));
            //Target Selector
            Settings.AddSubMenu(new Menu("Target Selector", "ts"));
            TargetSelector.AddToMenu(Settings.SubMenu("ts"));
            //Combo
            Settings.AddSubMenu(new Menu("Combo", "menuCombo"));
            Settings.SubMenu("menuCombo").AddItem(new MenuItem("comboQ", "Q").SetValue(true));
            Settings.SubMenu("menuCombo").AddItem(new MenuItem("comboW", "W").SetValue(true));
            //Harass
            Settings.AddSubMenu(new Menu("Harass", "menuHarass"));
            Settings.SubMenu("menuHarass").AddItem(new MenuItem( "harassQ", "Q").SetValue(true));
            Settings.SubMenu("menuHarass").AddItem(new MenuItem("harassW", "W").SetValue(true));
            //Auto
            Settings.AddSubMenu(new Menu("Auto", "menuAuto"));
            Settings.SubMenu("menuAuto").AddItem(new MenuItem("toggleAuto", "Toggle Auto").SetValue(new KeyBind("H".ToCharArray()[0], KeyBindType.Toggle)));
            Settings.SubMenu("menuAuto").AddSubMenu(new Menu("Settings", "autoSettings"));
            foreach(Obj_AI_Hero hero in ObjectManager.Get<Obj_AI_Hero>().Where(hero => hero.IsValid && hero.IsEnemy))
                Settings.SubMenu("menuAuto").SubMenu("autoSettings").AddItem(new MenuItem("auto" + hero.ChampionName, hero.ChampionName).SetValue(true));
            Settings.SubMenu("menuAuto").AddItem(new MenuItem("autoQ", "Q").SetValue(true));
            Settings.SubMenu("menuAuto").AddItem(new MenuItem("autoW", "W").SetValue<bool>(false));
            Settings.SubMenu("menuAuto").AddItem(new MenuItem("manaER", "Save Mana For E/R").SetValue(true));
            Settings.SubMenu("menuAuto").AddItem(new MenuItem("autoTurret", "Auto Enemy Under Turret").SetValue<bool>(false));
            //Ultimate
            Settings.AddSubMenu(new Menu("Ult Settings", "menuUlt"));
            Settings.SubMenu("menuUlt").AddItem(new MenuItem("ultLowest", "Ult Lowest Target").SetValue(new KeyBind("T".ToCharArray()[0], KeyBindType.Press)));
            Settings.SubMenu("menuUlt").AddItem(new MenuItem("ks", "Kill Steal With R").SetValue(true));
            Settings.SubMenu("menuUlt").AddItem(new MenuItem("ultRange", "Ult Range").SetValue<Slider>(new Slider(1000, 1000, 5000)));
            //Drawing
            Settings.AddSubMenu(new Menu("Drawing", "menuDrawing"));
            Settings.SubMenu("menuDrawing").AddSubMenu(new Menu("Damage Indicator", "menuDamage"));
            Settings.SubMenu("menuDrawing").SubMenu("menuDamage").AddItem(new MenuItem("drawAADmg", "Draw Auto Attack Damage").SetValue(true));
            Settings.SubMenu("menuDrawing").SubMenu("menuDamage").AddItem(new MenuItem("drawQDmg", "Draw Q Damage").SetValue(true));
            Settings.SubMenu("menuDrawing").SubMenu("menuDamage").AddItem(new MenuItem("drawWDmg", "Draw W Damage").SetValue(true));
            Settings.SubMenu("menuDrawing").SubMenu("menuDamage").AddItem(new MenuItem("drawEDmg", "Draw E Damage").SetValue(true));
            Settings.SubMenu("menuDrawing").SubMenu("menuDamage").AddItem(new MenuItem("drawRDmg", "Draw R Damage").SetValue(true));
            Settings.SubMenu("menuDrawing").AddItem(new MenuItem("drawQ", "Draw Q Range").SetValue(new Circle(true, Color.Yellow)));
            Settings.SubMenu("menuDrawing").AddItem(new MenuItem("drawW", "Draw W Range").SetValue(new Circle(true, Color.Yellow)));
            Settings.SubMenu("menuDrawing").AddItem(new MenuItem("drawR", "Draw R Range").SetValue(new Circle(true, Color.Yellow)));
            //Other
            Settings.AddItem(new MenuItem("dmgMode", "Damage Mode").SetValue(new StringList(new string[] { "AD", "AP" })));
            Settings.AddItem(new MenuItem("recallBlock", "Recall Block").SetValue(true));
            Settings.AddItem(new MenuItem("usePackets", "Use Packets").SetValue(true));
            //Finish
            Settings.AddToMainMenu();
        }

        public static bool ComboQ { get { return Settings.Item("comboQ").GetValue<bool>(); } }
        public static bool ComboW { get { return Settings.Item("comboW").GetValue<bool>(); } }

        public static bool HarassQ { get { return Settings.Item("harassQ").GetValue<bool>(); } }
        public static bool HarassW { get { return Settings.Item("harassW").GetValue<bool>(); } }

        public static KeyBind UltLowest { get { return Settings.Item("ultLowest").GetValue<KeyBind>(); } }
        public static bool KillSteal { get { return Settings.Item("ks").GetValue<bool>(); } }
        public static int UltRange { get { return Settings.Item("ultRange").GetValue<Slider>().Value; } }

        public static KeyBind ToggleAuto { get { return Settings.Item("toggleAuto").GetValue<KeyBind>(); } }
        public static bool ShouldAuto(string championName)
        {
            return Settings.Item("auto" + championName).GetValue<bool>();
        }
        public static bool AutoQ { get { return Settings.Item("autoQ").GetValue<bool>(); } }
        public static bool AutoW { get { return Settings.Item("autoW").GetValue<bool>(); } }
        public static bool ManaER { get { return Settings.Item("manaER").GetValue<bool>(); } }
        public static bool AutoTurret { get { return Settings.Item("autoTurret").GetValue<bool>(); } }

        public static bool DrawAADmg { get { return Settings.Item("drawAADmg").GetValue<bool>(); } }
        public static bool DrawQDmg { get { return Settings.Item("drawQDmg").GetValue<bool>(); } }
        public static bool DrawWDmg { get { return Settings.Item("drawWDmg").GetValue<bool>(); } }
        public static bool DrawEDmg { get { return Settings.Item("drawEDmg").GetValue<bool>(); } }
        public static bool DrawRDmg { get { return Settings.Item("drawRDmg").GetValue<bool>(); } }
        public static bool DrawQ { get { return Settings.Item("drawQ").GetValue<Circle>().Active; } }
        public static bool DrawW { get { return Settings.Item("drawW").GetValue<Circle>().Active; } }
        public static bool DrawR { get { return Settings.Item("drawR").GetValue<Circle>().Active; } }

        public static string DamageMode { get { return Settings.Item("dmgMode").GetValue<StringList>().SelectedValue; } }
        public static bool RecallBlock { get { return Settings.Item("recallBlock").GetValue<bool>(); } }
        public static bool UsePackets { get { return Settings.Item("usePackets").GetValue<bool>(); } }
    }
}
