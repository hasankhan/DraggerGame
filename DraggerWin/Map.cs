using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.IO;
using CrackSoft.Collections;

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

    enum Direction
    {
        UP,
        Down,
        Left,
        Right,
        Unknown
    }

    class GridPoint
    {
        public Point Point;
        public MapCell Cell;

        public GridPoint(Point point, MapCell cell)
        {
            Point = point;
            Cell = cell;
        }
    }

    class RestorePoint
    {
        public Point Player;
        public GridPoint Point1;
        public GridPoint Point2;
        public bool HasCells;

        public RestorePoint(Point player)
        {
            this.Player = player;
        }

        public RestorePoint(Map map, Point point1, Point point2): this(map.Player.Location)
        {
            HasCells = true;
            Point1 = new GridPoint(point1, map[point1]);
            Point2 = new GridPoint(point2, map[point2]);
        }
    }

    class Map
    {        
        MapCell[,] grid;
        Player player;
        readonly MapCell[,] defaultGrid;
        int rows, cols;
        OverflowStack<RestorePoint> moveHistory;
        int totalMoves;
        const int cellWidth = 50;
        const int cellHeight = 50;
        Point location;

        public Map(MapCell[,] grid, Point playerPos)
        {            
            this.grid = grid;
            this.player = new Player(playerPos);
            this.defaultGrid = (MapCell[,])grid.Clone();
            this.rows = grid.GetLength(0);
            this.cols = grid.GetLength(1);
            moveHistory = new OverflowStack<RestorePoint>(10);
        }

        public Point Location
        {
            get { return location; }
            set { location = value; }
        }

        public void Reset()
        {
            player.Reset();
            grid = (MapCell[,])defaultGrid.Clone();
            moveHistory.Clear();
            totalMoves = 0;
        }

        public Player Player
        {
            get { return player; }
        }

        public Size Size
        {
            get { return new Size(cellWidth * Columns, cellHeight * Rows); }
        }

        public int Rows
        {
            get{ return rows; }
        }

        public int Columns
        {
            get { return cols; }
        }

        public int TotalMoves
        {
            get { return totalMoves; }
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

        private Point GetDisplacement(Direction towards)
        {
            Point displacement;
            switch (towards)
            {
                case Direction.UP:
                    displacement = new Point(0, -1);
                    break;
                case Direction.Down:
                    displacement = new Point(0, 1);
                    break;
                case Direction.Left:
                    displacement = new Point(-1, 0);
                    break;
                default:// Direction.Right:
                    displacement = new Point(1, 0);
                    break;
            }
            return displacement;
        }

        public bool MovePlayer(Direction towards)
        {
            if (towards == Direction.Unknown)
                return false;

            Point disp = GetDisplacement(towards);
            Point dest = player.Location, dest2 = player.Location;

            dest.Offset(disp);
            disp.X *= 2;
            disp.Y *= 2;
            dest2.Offset(disp);

            if (this[dest] == MapCell.Wall || this[dest] == MapCell.Illegal)
                return false;
            if (IsFree(dest))
            {
                moveHistory.Push(new RestorePoint(Player.Location));
                Player.Location = dest;
                totalMoves++;
                return true;
            }
            if (IsFree(dest2) && IsMovable(dest))
            {
                moveHistory.Push(new RestorePoint(this, dest, dest2));
                MoveObject(dest, dest2);
                Player.Location = dest;
                totalMoves++;
                return true;
            }
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
            DrawObject(gfx, this[x, y], x, y);
            if (Player.Location.Y == y && Player.Location.X == x)
                DrawObject(gfx, MapCell.Player, x, y);
        }

        public void DrawObject(Graphics gfx, MapCell obj, int x, int y)
        {
            switch (obj)
            {                
                case MapCell.Basket:
                    DrawImage(gfx, Properties.Resources.basket, x, y);
                    break;
                case MapCell.Ball:
                    DrawImage(gfx, Properties.Resources.ball, x, y);
                    break;
                case MapCell.BallInBasket:
                    DrawImage(gfx, Properties.Resources.basket2, x, y);
                    DrawImage(gfx, Properties.Resources.ball2, x, y);
                    break;
                case MapCell.Wall:
                    DrawImage(gfx, Properties.Resources.wall, x, y);
                    break;
                case MapCell.Player:
                    DrawImage(gfx, Properties.Resources.man, x, y);
                    break;
            }
        }

        Point CoordToPixel(int x, int y)
        {
            Point point = new Point(cellWidth * x, cellHeight * y);
            point.Offset(Location);
            return point;
        }

        void DrawImage(Graphics gfx, Image img, int x, int y)
        {
            gfx.DrawImage(img, new Rectangle(CoordToPixel(x, y), img.Size));
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

        public void UndoLastMove()
        {
            if (!moveHistory.IsEmpty)
            {
                RestorePoint point = moveHistory.Pop();
                Player.Location = point.Player;
                totalMoves++;
                if (point.HasCells)
                {
                    this[point.Point1.Point] = point.Point1.Cell;
                    this[point.Point2.Point] = point.Point2.Cell;
                }
            }
        }
    }
}
