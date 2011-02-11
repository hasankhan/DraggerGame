using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.IO;

namespace Dragger
{
    class GameEngine
    {
        static List<Map> maps;
        static bool initialized;
        static int level;

        public static void Initialize()
        {
            InitGraphics();
            LoadMaps();
            initialized = true;
        }

        private static void InitGraphics()
        {
            Console.CursorVisible = false;
            Console.BackgroundColor = ConsoleColor.DarkBlue;
        }

        private static void LoadMaps()
        {
            maps = new List<Map>();
            string[] args = Environment.GetCommandLineArgs();
            string data;
            if (args.Length > 1)
                data = File.ReadAllText(Environment.CommandLine);
            else
                data = Properties.Resources.Maps;
            StringReader reader = new StringReader(data);
            while (true)
            {
                Map map = Map.Load(reader);
                if (map == null)
                    break;
                maps.Add(map);
            }
            if (maps.Count == 0)
                throw new FileLoadException("Could not load maps.");
        }        

        public static void MainLoop()
        {
            if (!initialized)
                throw new InvalidOperationException("Game engine not initialized.");

            Map map = GetNextLevel();
            bool playMore = true;

            do
            {
                map.Reset();
                Player player = map.Player;

                DrawScene(map);

                bool won = false;
                do
                {
                    ConsoleKeyInfo info = Console.ReadKey(true);
                    if (info.Key == ConsoleKey.Escape)
                        break;
                    if (player.Move(KeyToDirection(info)))
                        DrawScene(map);
                    won = map.IsConquered;
                }
                while (!won);

                Console.WriteLine();

                if (won)
                {
                    Console.Beep();
                    if (level == maps.Count)
                    {
                        ConsoleUtil.Write("Congratulation! You won :)\r\n", ConsoleAnimation.Blink);
                        level = 0;
                        map = GetNextLevel();
                    }
                    else
                    {
                        map = GetNextLevel();
                        continue;
                    }
                }
                else                
                    ConsoleUtil.Write("You gave up..tsk tsk :/\r\n", ConsoleAnimation.Typewriter);
                
                playMore = WantToPlayMore();                
            }
            while (playMore);
        }

        private static Map GetNextLevel()
        {
            Map map = maps[level++];
            return map;
        }

        private static bool WantToPlayMore()
        {
            bool playMore;
            Console.WriteLine();
            ConsoleUtil.Write("Do you want to play again (y/n)? ", ConsoleAnimation.Typewriter);
            playMore = Char.ToLower(Console.ReadKey().KeyChar) == 'y';
            return playMore;
        }

        private static Direction KeyToDirection(ConsoleKeyInfo info)
        {
            if (info.Key == ConsoleKey.UpArrow)
                return Direction.UP;
            else if (info.Key == ConsoleKey.DownArrow)
                return Direction.Down;
            else if (info.Key == ConsoleKey.LeftArrow)
                return Direction.Left;
            else if (info.Key == ConsoleKey.RightArrow)
                return Direction.Right;
            else
                return Direction.Unknown;
        }

        private static void DrawScene(Map map)
        {
            Console.Clear();
            PrintTitle();
            map.Draw();
        }

        private static void PrintTitle()
        {
            Console.Title  = "Dragger 1.0 (C) 2008 CrackSoft";
            Console.WriteLine("Level {0}/{1}", level, maps.Count);
            Console.WriteLine();
        }        
    }
}
