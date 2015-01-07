using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LeagueSharp;
using LeagueSharp.Common;
using Color = System.Drawing.Color;

namespace PerplexedLucian
{
    class Config
    {
        public static Menu Settings = new Menu("Perplexed Lucian", "menu", true);
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
            Settings.SubMenu("menuCombo").AddItem(new MenuItem("comboE", "E").SetValue(true));
            Settings.SubMenu("menuCombo").AddItem(new MenuItem("comboR", "R").SetValue(true));
            //Harass
            Settings.AddSubMenu(new Menu("Harass", "menuHarass"));
            Settings.SubMenu("menuHarass").AddItem(new MenuItem( "harassQ", "Q").SetValue(true));
            //Summoners
            Settings.AddSubMenu(new Menu("Summoners", "menuSumms"));
            Settings.SubMenu("menuSumms").AddSubMenu(new Menu("Heal", "summHeal"));
            Settings.SubMenu("menuSumms").SubMenu("summHeal").AddItem(new MenuItem("useHeal", "Enabled").SetValue(true));
            Settings.SubMenu("menuSumms").SubMenu("summHeal").AddItem(new MenuItem("healPct", "Use On % Health").SetValue(new Slider(35, 10, 90)));
            Settings.SubMenu("menuSumms").AddSubMenu(new Menu("Ignite", "summIgnite"));
            Settings.SubMenu("menuSumms").SubMenu("summIgnite").AddItem(new MenuItem("useIgnite", "Enabled").SetValue(true));
            Settings.SubMenu("menuSumms").SubMenu("summIgnite").AddItem(new MenuItem("igniteMode", "Use Ignite For").SetValue(new StringList(new string[] { "Execution", "Combo" })));
            //Items
            Settings.AddSubMenu(new Menu("Items", "menuItems"));
            Settings.SubMenu("menuItems").AddSubMenu(new Menu("Offensive", "offItems"));
            foreach (var offItem in ItemManager.Items.Where(item => item.Type == ItemType.Offensive))
                Settings.SubMenu("menuItems").SubMenu("offItems").AddItem(new MenuItem("use" + offItem.ShortName, offItem.Name).SetValue(true));
            Settings.SubMenu("menuItems").AddSubMenu(new Menu("Cleanse", "cleanseItems"));
            foreach (var cleanseItem in ItemManager.Items.Where(item => item.Type == ItemType.Cleanse))
                Settings.SubMenu("menuItems").SubMenu("cleanseItems").AddItem(new MenuItem("use" + cleanseItem.ShortName, cleanseItem.Name).SetValue(true));
            //Drawing
            Settings.AddSubMenu(new Menu("Drawing", "menuDrawing"));
            Settings.SubMenu("menuDrawing").AddSubMenu(new Menu("Damage Indicator", "menuDamage"));
            Settings.SubMenu("menuDrawing").SubMenu("menuDamage").AddItem(new MenuItem("drawAADmg", "Draw Auto Attack Damage").SetValue(true));
            Settings.SubMenu("menuDrawing").SubMenu("menuDamage").AddItem(new MenuItem("drawQDmg", "Draw Q Damage").SetValue(true));
            Settings.SubMenu("menuDrawing").SubMenu("menuDamage").AddItem(new MenuItem("drawWDmg", "Draw W Damage").SetValue(true));
            Settings.SubMenu("menuDrawing").SubMenu("menuDamage").AddItem(new MenuItem("drawRDmg", "Draw R Damage").SetValue(true));
            Settings.SubMenu("menuDrawing").AddItem(new MenuItem("drawQ", "Draw Q Range").SetValue(new Circle(true, Color.Yellow)));
            Settings.SubMenu("menuDrawing").AddItem(new MenuItem("drawQ2", "Draw Ext. Q Range").SetValue(new Circle(true, Color.Yellow)));
            Settings.SubMenu("menuDrawing").AddItem(new MenuItem("drawW", "Draw W Range").SetValue(new Circle(true, Color.Yellow)));
            Settings.SubMenu("menuDrawing").AddItem(new MenuItem("drawE", "Draw E Range").SetValue(new Circle(true, Color.Yellow)));
            Settings.SubMenu("menuDrawing").AddItem(new MenuItem("drawR", "Draw R Range").SetValue(new Circle(true, Color.Yellow)));
            //Other
            Settings.AddItem(new MenuItem("checkPassive", "Check Passive").SetValue(true));
            Settings.AddItem(new MenuItem("usePackets", "Use Packets").SetValue(true));
            //Finish
            Settings.AddToMainMenu();
        }

        public static bool ComboQ { get { return Settings.Item("comboQ").GetValue<bool>(); } }
        public static bool ComboW { get { return Settings.Item("comboW").GetValue<bool>(); } }
        public static bool ComboE { get { return Settings.Item("comboE").GetValue<bool>(); } }
        public static bool ComboR { get { return Settings.Item("comboR").GetValue<bool>(); } }

        public static bool HarassQ { get { return Settings.Item("harassQ").GetValue<bool>(); } }

        public static bool UseHeal { get { return Settings.Item("useHeal").GetValue<bool>(); } }
        public static int HealPct { get { return Settings.Item("healPct").GetValue<Slider>().Value; } }
        public static bool UseIgnite { get { return Settings.Item("useIgnite").GetValue<bool>(); } }
        public static string IgniteMode { get { return Settings.Item("igniteMode").GetValue<StringList>().SelectedValue; } }

        public static bool ShouldUseItem(string shortName)
        {
            return Settings.Item("use" + shortName).GetValue<bool>();
        }
        public static int UseOnPercent(string shortName)
        {
            return Settings.Item("pctHealth" + shortName).GetValue<Slider>().Value;
        }

        public static bool DrawAADmg { get { return Settings.Item("drawAADmg").GetValue<bool>(); } }
        public static bool DrawQDmg { get { return Settings.Item("drawQDmg").GetValue<bool>(); } }
        public static bool DrawWDmg { get { return Settings.Item("drawWDmg").GetValue<bool>(); } }
        public static bool DrawRDmg { get { return Settings.Item("drawRDmg").GetValue<bool>(); } }
        public static Circle DrawQ { get { return Settings.Item("drawQ").GetValue<Circle>(); } }
        public static Circle DrawQ2 { get { return Settings.Item("drawQ2").GetValue<Circle>(); } }
        public static Circle DrawW { get { return Settings.Item("drawW").GetValue<Circle>(); } }
        public static Circle DrawE { get { return Settings.Item("drawE").GetValue<Circle>(); } }
        public static Circle DrawR { get { return Settings.Item("drawR").GetValue<Circle>(); } }

        public static bool CheckPassive { get { return Settings.Item("checkPassive").GetValue<bool>(); } }
        public static bool UsePackets { get { return Settings.Item("usePackets").GetValue<bool>(); } }
    }
}
