using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using System.Threading;
using System.Diagnostics;

namespace Dragger
{
    enum GameState
    {
        Initializing,
        Ready,
        Started,
        Credits,
        Help,
        Paused,
        Running,        
        End
    }

    class GameEngine
    {
        static List<Map> maps;
        static bool initialized;
        static int level = 1;
        static Form parent;
        static Map currentMap;
        static GameState currentState;
        static GameState lastState;
        const int titleHeight = 20;
        static System.Windows.Forms.Timer alarm;
        static Stopwatch watch;
        public static Font BigFont = new Font("Comic Sans Ms", 12, FontStyle.Bold);
        public static Font NormalFont = new Font("Comic Sans Ms", 8, FontStyle.Bold);
        public static Font SmallFont = new Font("Comic Sans Ms", 6);
        static int levelsUnlocked = 1;
        static bool customMaps = false;
        static System.Windows.Forms.Timer clock;
        
        public static int LevelsUnlocked
        {
            get
            {
                if (customMaps)
                    return maps.Count;
                return levelsUnlocked; 
            }
            set
            {
                levelsUnlocked = value;
                if (!customMaps)
                    SaveSettings();
            }
        }

        private static void SaveSettings()
        {
            string data = LevelsUnlocked.ToString();
            byte[] bytes = Encoding.ASCII.GetBytes(data);
            string output = Convert.ToBase64String(bytes);
            Properties.Settings.Default.data = output;
            Properties.Settings.Default.Save();
        }

        public static GameState State
        {
            get { return currentState; }
            set
            {
                lastState = currentState;
                currentState = value;
            }
        }

        public static void RestoreState()
        {
            State = lastState;
        }

        public static void Initialize(Form parent) { Initialize(parent, String.Empty); }
        public static void Initialize(Form parent, string mapsFile)
        {
            State = GameState.Initializing;
            GameEngine.parent = parent;
            alarm = new System.Windows.Forms.Timer();
            alarm.Tick += new EventHandler(alarm_Tick);            
            watch = new Stopwatch();
            clock = new System.Windows.Forms.Timer();
            clock.Interval = 1000;
            clock.Tick += new EventHandler(clock_Tick);
            clock.Start();
            parent.KeyDown += new KeyEventHandler(parent_KeyDown);
            InitGraphics();
            LoadMaps(mapsFile);
            LoadSettings();            
            initialized = true;
            State = GameState.Ready;
        }

        static void clock_Tick(object sender, EventArgs e)
        {
            if (watch.IsRunning)
                DrawScene();
        }

        private static void LoadSettings()
        {
            if (customMaps)
                return;
            string data = Properties.Settings.Default.data;
            if (data != String.Empty)
            {
                byte[] bytes = Convert.FromBase64String(data);
                data = Encoding.ASCII.GetString(bytes);
                level = levelsUnlocked = Int32.Parse(data);
            }
        }

        static void alarm_Tick(object sender, EventArgs e)
        {
            alarm.Stop();
            OnAlarm();
        }

        private static void OnAlarm()
        {
            if (State == GameState.Running)
            {
                if (CurrentLevel == maps.Count)
                {
                    LoadEnd();
                }
                else
                {
                    currentMap = GetNextLevel();
                    if (CurrentLevel > LevelsUnlocked)
                        LevelsUnlocked = CurrentLevel;
                    LoadCurrentMap();
                }
            }
        }

        private static void LoadEnd()
        {
            State = GameState.End;
            parent.ClientSize = Properties.Resources.finish.Size;
            DrawScene();
        }

        public static int CurrentLevel
        {
            get { return level; }
        }

        public static int TotalLevels
        {
            get { return maps.Count; }
        }

        static void parent_KeyDown(object sender, KeyEventArgs e)
        {
            ProcessKeyInput(e);
        }

        private static void ProcessKeyInput(KeyEventArgs e)
        {
            if (State == GameState.End)
            {
                State = GameState.Started;
                LoadMenu();
            }
            else if (State == GameState.Credits || State == GameState.Help)
            {
                RestoreState();
                LoadMenu();
            }
            else if (State == GameState.Started || State == GameState.Paused)
            {
                if (e.KeyCode == Keys.Return)
                {
                    switch (Menu.Current)
                    {
                        case MenuOption.Start:
                            LoadLevel(Menu.Level);
                            break;
                        case MenuOption.Continue:
                            LoadCurrentMap();
                            break;
                        case MenuOption.Credits:
                            LoadCredits();
                            break;
                        case MenuOption.Help:
                            LoadHelp();
                            break;
                        case MenuOption.Exit:
                            Application.Exit();
                            break;
                    }
                }
                else
                {
                    Menu.Move(KeyToDirection(e.KeyCode));
                    DrawScene();
                }
            }
            else if (State == GameState.Running)
            {
                if (e.KeyCode == Keys.Escape)
                {
                    State = GameState.Paused;
                    LoadMenu();
                }
                else if ((e.KeyCode == Keys.Z && e.Modifiers == Keys.Control) || e.KeyCode == Keys.Back)
                {
                    currentMap.UndoLastMove();
                    DrawScene();
                }
                else if (currentMap.MovePlayer(KeyToDirection(e.KeyCode)))
                {
                    DrawScene();
                    if (currentMap.IsConquered)
                        SetAlarm(1000);
                }
            }
        }

        private static void LoadHelp()
        {            
            State = GameState.Help;
            DrawScene();
        }

        private static void LoadCredits()
        {
            parent.ClientSize = new Size(200, 250);
            State = GameState.Credits;
            DrawScene();
        }

        private static void InitGraphics()
        {
            parent.Paint += new PaintEventHandler(parent_Paint);
            parent.Resize += new EventHandler(parent_Resize);
            parent.BackColor = Color.Black;
        }

        static void parent_Resize(object sender, EventArgs e)
        {
            if (parent.WindowState == FormWindowState.Normal)
            {
                parent.Left = Screen.PrimaryScreen.WorkingArea.Width / 2 - parent.Width / 2;
                parent.Top = Screen.PrimaryScreen.WorkingArea.Height / 2 - parent.Height / 2;
            }
        }

        static void parent_Paint(object sender, PaintEventArgs e)
        {            
            DrawScene(e.Graphics);
        }

        private static void LoadMaps(string file)
        {
            maps = new List<Map>();            
            string data;
            
            if (String.IsNullOrEmpty(file))
                data = Properties.Resources.maps;
            else
            {
                customMaps = true;
                try
                {
                    data = File.ReadAllText(file);
                }
                catch
                {
                    throw new FileLoadException("Could not load maps from " + file + ".");
                }
            }
                
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

        public static void Start()
        {
            State = GameState.Started;
            LoadMenu();
        }

        private static void LoadMenu()
        {
            watch.Stop();
            parent.ClientSize = Properties.Resources.splash.Size + new Size(0, 50);
            Menu.Level = CurrentLevel;
            if (State == GameState.Paused)
                Menu.Current = MenuOption.Continue;
            else
                Menu.Current = MenuOption.Start;
            DrawScene();
        }

        private static void SetAlarm(int interval)
        {
            alarm.Interval = interval;
            alarm.Start();
        }

        private static void LoadLevel(int level)
        {
            if (!initialized)
                throw new InvalidOperationException("Game engine not initialized.");

            currentMap = GetLevel(level - 1);
            LoadCurrentMap();
        }

        private static void LoadCurrentMap()
        {
            watch.Start();
            parent.ClientSize = currentMap.Size + new Size(0, titleHeight);
            State = GameState.Running;            
            DrawScene();
        }

        private static Map GetLevel(int level)
        {
            if (level < maps.Count)
            {
                GameEngine.level = level;
                return GetNextLevel();
            }
            else
                return null;
        }

        private static Map GetNextLevel()
        {
            Map map = maps[level++];            
            map.Reset();
            watch.Reset();
            return map;
        }

        private static bool WantToPlayMore()
        {            
            return true;
        }

        private static Direction KeyToDirection(Keys key)
        {
            if (key == Keys.Up)
                return Direction.UP;
            else if (key == Keys.Down)
                return Direction.Down;
            else if (key == Keys.Left)
                return Direction.Left;
            else if (key == Keys.Right)
                return Direction.Right;
            else
                return Direction.Unknown;
        }

        private static void DrawScene() 
        {
            parent.Invalidate();
        }

        private static void DrawScene(Graphics gfx)
        {
            gfx.Clear(parent.BackColor);
            if (State == GameState.Credits)
                DrawCredits(gfx);
            else if (State == GameState.Started || State == GameState.Paused)
                DrawSplash(gfx);
            else if (State == GameState.Help)
                DrawHelp(gfx);
            else if (State == GameState.Running)
                DrawLevel(gfx);
            else if (State == GameState.End)
                DrawEnd(gfx);
        }

        private static void DrawEnd(Graphics gfx)
        {
            Bitmap finish = Properties.Resources.finish;
            gfx.DrawImage(finish, 0, 0, finish.Width, finish.Height);
            DrawPressAnyKey(gfx);
        }

        private static void DrawHelp(Graphics gfx)
        {
            Bitmap help = Properties.Resources.help;
            gfx.DrawImage(help, 0, 0, help.Width, help.Height);
            DrawPressAnyKey(gfx);
        }

        private static void DrawCredits(Graphics gfx)
        {
            gfx.DrawString("Maps created by: ", NormalFont, Brushes.White, 0, 0);
            gfx.DrawString("Bubbles", BigFont, Brushes.Purple, 1, BigFont.Height - 1);
            gfx.DrawString("Bubbles", BigFont, Brushes.LightBlue, 0, BigFont.Height);
            gfx.DrawString("Haider", BigFont, Brushes.Green, 1, BigFont.Height * 2 - 1);
            gfx.DrawString("Haider", BigFont, Brushes.LightYellow, 0, BigFont.Height * 2);
            gfx.DrawString("Programmed by: ", NormalFont, Brushes.White, 0, BigFont.Height * 4);
            gfx.DrawString("M. Hasan Khan", BigFont, Brushes.Crimson, 1, BigFont.Height * 5 - 1);
            gfx.DrawString("M. Hasan Khan", BigFont, Brushes.Bisque, 0, BigFont.Height * 5);
            gfx.DrawString("(c) 2008 CrackSoft", NormalFont, Brushes.White, 0, BigFont.Height * 8);
            DrawPressAnyKey(gfx);
        }

        private static void DrawPressAnyKey(Graphics gfx)
        {
            gfx.DrawString("Press any key", SmallFont, Brushes.White, 0, parent.ClientSize.Height - SmallFont.Height);
        }

        private static void DrawLevel(Graphics gfx)
        {
            if (currentMap != null)
            {
                currentMap.Draw(gfx);
                DrawStatusBar(gfx);
            }
        }

        private static void DrawSplash(Graphics gfx)
        {            
            Menu.Draw(gfx);
        }

        private static void DrawStatusBar(Graphics gfx)
        {
            string text = String.Format("Level {0}/{1}", CurrentLevel, TotalLevels);
            gfx.DrawString(text, BigFont, Brushes.White, 0, currentMap.Size.Height);
            text = String.Format("Moves = {0}", currentMap.TotalMoves);
            string text2 = String.Format("Time = {0:d2}:{1:d2}:{2:d2}", watch.Elapsed.Hours, watch.Elapsed.Minutes, watch.Elapsed.Seconds);
            int text2Size = (int)gfx.MeasureString(text2, BigFont).Width;
            gfx.DrawString(text, BigFont, Brushes.White, parent.ClientSize.Width - gfx.MeasureString(text, BigFont).Width - text2Size - 10, currentMap.Size.Height);
            gfx.DrawString(text2, BigFont, Brushes.White, parent.ClientSize.Width - text2Size, currentMap.Size.Height);
        }
    }
}
