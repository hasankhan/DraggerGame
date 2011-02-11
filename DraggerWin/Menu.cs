using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;

namespace Dragger
{
    enum MenuOption
    {
        Start,
        Continue,
        Credits,
        Help,
        Exit
    }    

    static class Menu
    {
        static MenuOption[] options = new MenuOption[] 
                                        { 
                                            MenuOption.Continue, 
                                            MenuOption.Start,                                             
                                            MenuOption.Credits,
                                            MenuOption.Help,
                                            MenuOption.Exit
                                        };
        static int menuIndex = 1;
        public static int Level = 1;

        public static MenuOption Current
        {
            get { return options[menuIndex]; }
            set
            {
                menuIndex = Array.IndexOf<MenuOption>(options, value);
            }
        }

        private static bool IsPaused
        {
            get { return GameEngine.State == GameState.Paused; }
        }

        public static void Draw(Graphics gfx)
        {
            Bitmap splash = Properties.Resources.splash;
            gfx.DrawImage(splash, 0, 0, splash.Width, splash.Height);
            for (int i=0; i<options.Length; i++)
                DrawMenuItem(gfx, i);
        }

        public static void Move(Direction towards)
        {
            if (towards == Direction.UP && (menuIndex > 1 || (menuIndex>0 && IsPaused)))
                menuIndex--;
            else if (towards == Direction.Down && menuIndex < options.Length - 1)
                menuIndex++;
            else if (towards == Direction.Left && Current == MenuOption.Start && Level > 1)
                Level--;
            else if (towards == Direction.Right && Current == MenuOption.Start && Level < GameEngine.LevelsUnlocked)
                Level++;
        }

        private static void DrawMenuItem(Graphics gfx, int i)
        {
            MenuOption item = options[i];
            if (item == MenuOption.Continue && !IsPaused)
                return;

            Brush brush;
            if (menuIndex == i)
                brush = Brushes.SteelBlue;
            else
                brush = Brushes.White;

            string text = item.ToString();

            if (item == MenuOption.Start)
            {
                text += " (" + Level + ")";
            }

            gfx.DrawString(text, GameEngine.BigFont, brush, 120, 80 + i * GameEngine.BigFont.Height);
        }
    }
}
