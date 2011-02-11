using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.IO;

namespace Dragger
{
    class Map
    {
        int[,] grid;
        Player player;
        readonly int[,] defaultGrid;
        int rows, cols;

        public Map(int[,] grid, Point playerPos)
        {
            this.grid = grid;
            this.player = new Player(this, playerPos);
            this.defaultGrid = (int[,])grid.Clone();
            this.rows = grid.GetLength(0);
            this.cols = grid.GetLength(1);
        }

        public void Reset()
        {
            player.Reset();
            grid = (int[,])defaultGrid.Clone();
        }

        public Player Player
        {
            get { return player; }
        }

        public int Rows
        {
            get{ return rows; }
        }

        public int Columns
        {
            get { return cols; }
        }

        public int this[int x, int y]
        {
            get
            {
                if (y >= Rows || 
                    x >= Columns || 
                    y <0 || 
                    x < 0)
                    return -1;
                else
                    return grid[y, x];

            }
            set
            {
                if (x < Columns && y < Rows)
                    grid[y, x] = value;
            }
        }

        public int this[Point pt]
        {
            get { return this[pt.X, pt.Y]; }
            set { this[pt.X, pt.Y] = value; }
        }

        public bool IsFree(Point dest)
        {
            if (this[dest] == 0)
                return true;
            if (this[dest] == 1)
                return true;
            return false;
        }

        public void MoveObject(Point oldPos, Point newPos)
        {
            this[newPos] = (this[newPos] == 1) ? 3 : 2;
            this[oldPos] = (this[oldPos] == 3) ? 1 : 0;
        }

        public bool IsMovable(Point dest)
        {
            return (this[dest] == 2 || this[dest] == 3);
        }

        public bool IsConquered
        {
            get
            {
                for (int y = 0; y < Rows; y++)
                    for (int x = 0; x < Columns; x++)
                        if (this[x, y] == 2)
                            return false;
                return true;
            }
        }

        public void Draw()
        {
            for (int y = 0; y < Rows; y++)
            {
                DrawCell(0, y);
                for (int x = 1; x < Columns; x++)
                {
                    Console.Write(" ");
                    DrawCell(x, y);
                }
                Console.WriteLine();
            }
        }

        private void DrawCell(int x, int y)
        {
            if (Player.Location.Y == y && Player.Location.X == x)
                DrawObj(9);
            else
                DrawObj(this[x, y]);
        }

        private void DrawObj(int obj)
        {
            if (obj == 1)
                ConsoleUtil.Write("#", ConsoleColor.Green);
            else if (obj == 2)
                ConsoleUtil.Write("O", ConsoleColor.Blue);
            else if (obj == 9)
                ConsoleUtil.Write("T", ConsoleColor.Cyan);
            else if (obj == 3)
                ConsoleUtil.Write("@", ConsoleColor.Yellow);
            else if (obj == 4)
                ConsoleUtil.Write("\"", ConsoleColor.Magenta);
            else
                Console.Write(" ");
        }

        public Map Invert()
        {
            Map inverse = new Map(new int[Columns, Rows], new Point(Player.Location.X, Player.Location.Y));
            for (int y = 0; y < Rows; y++)
                for (int x = 0; x < Columns; x++)
                    inverse[y, x] = this[x, y];
            return inverse;
        }

        public void Save(TextWriter writer)
        {
            writer.WriteLine("grid " + Rows + "x" + Columns);
            writer.WriteLine("spawn " + Player.Location.X + "," + Player.Location.Y);
            for (int y = 0; y < Rows; y++)
            {
                writer.Write(((int)this[0, y]).ToString());
                for (int x = 1; x < Columns; x++)
                    writer.Write(" " + ((int)this[x, y]).ToString());
                writer.WriteLine();
            }
            writer.WriteLine();
        }

        public static Map Load(TextReader reader)
        {
#if !DEBUG
            try
#endif
            {
                string temp;
                string[] tokens;

                temp = reader.ReadLine();
                if (temp == null)
                    return null;

                temp = temp.Split(' ')[1]; // grid 7x7
                tokens = temp.Split('x'); // 7x7
                int rows = Int32.Parse(tokens[0]); // 7
                int cols = Int32.Parse(tokens[1]); // 7

                int[,] grid = new int[rows, cols];

                temp = reader.ReadLine();
                temp = temp.Split(' ')[1]; // spawn 3,3
                tokens = temp.Split(','); // 3,3
                Point spawnAt = new Point(Int32.Parse(tokens[1]), Int32.Parse(tokens[0]));

                for (int row = 0; row < rows; row++)
                {
                    temp = reader.ReadLine();
                    tokens = temp.Split(' ');
                    for (int col = 0; col < cols; col++)
                    {
                        grid[row, col] = Int32.Parse(tokens[col]);
                    }
                }

                reader.ReadLine();

                return new Map(grid, spawnAt);
            }
#if !DEBUG
            catch
            {
                return null;
            }
#endif
        }
    }
}
