using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.IO;

namespace Dragger
{
    enum MapCell
    {
        Empty = 0,
        Basket = 1,
        Ball = 2,
        BallInBasket = 3,
        Wall = 4,
        Player = 9,
        Illegal
    }

    class Map
    {
        MapCell[,] grid;
        Player player;
        readonly MapCell[,] defaultGrid;
        int rows, cols;        

        public Map(MapCell[,] grid, Point playerPos)
        {
            this.grid = grid;
            this.player = new Player(this, playerPos);
            this.defaultGrid = (MapCell[,])grid.Clone();
            this.rows = grid.GetLength(0);
            this.cols = grid.GetLength(1);
        }

        public void Reset()
        {
            player.Reset();
            grid = (MapCell[,])defaultGrid.Clone();
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

        public MapCell this[int x, int y]
        {
            get
            {
                if (y >= Rows || 
                    x >= Columns || 
                    y <0 || 
                    x < 0)
                    return MapCell.Illegal;
                else
                    return grid[y, x];

            }
            set
            {
                if (x < Columns && y < Rows)
                    grid[y, x] = value;
            }
        }

        public MapCell this[Point pt]
        {
            get { return this[pt.X, pt.Y]; }
            set { this[pt.X, pt.Y] = value; }
        }

        public bool IsFree(Point dest)
        {
            if (this[dest] == MapCell.Empty)
                return true;
            if (this[dest] == MapCell.Basket)
                return true;
            return false;
        }

        public void MoveObject(Point oldPos, Point newPos)
        {
            this[newPos] = (this[newPos] == MapCell.Basket) ? MapCell.BallInBasket : MapCell.Ball;
            this[oldPos] = (this[oldPos] == MapCell.BallInBasket) ? MapCell.Basket : MapCell.Empty;
        }

        public bool IsMovable(Point dest)
        {
            return (this[dest] == MapCell.Ball || this[dest] == MapCell.BallInBasket);
        }

        public bool IsConquered
        {
            get
            {
                for (int y = 0; y < Rows; y++)
                    for (int x = 0; x < Columns; x++)
                        if (this[x, y] == MapCell.Ball)
                            return false;
                return true;
            }
        }

        public void Draw(Graphics gfx)
        {
            for (int y = 0; y < Rows; y++)
            {
                DrawCell(gfx, 0, y);
                for (int x = 1; x < Columns; x++)
                {
                    DrawCell(gfx, x, y);
                }
            }
        }

        private void DrawCell(Graphics gfx, int x, int y)
        {            
            //DrawObject(gfx, this[x, y], x, y);
            //if (Player.Location.Y == y && Player.Location.X == x)
                //DrawObject(gfx, MapCell.Player, x, y);
        }

        public Map Invert()
        {
            Map inverse = new Map(new MapCell[Columns, Rows], new Point(Player.Location.X, Player.Location.Y));
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

                MapCell[,] grid = new MapCell[rows, cols];

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
                        grid[row, col] = (MapCell)Int32.Parse(tokens[col]);
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
