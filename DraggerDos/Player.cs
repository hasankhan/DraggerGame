using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;

namespace Dragger
{
    enum Direction
    {
        UP,
        Down,
        Left,
        Right,
        Unknown
    }

    class Player
    {
        Point location;
        Map map;
        readonly Point defaultLocation;

        public Player(Map map, Point location)
        {
            this.map = map;
            this.defaultLocation = this.location = location;
        }

        public Point Location
        {
            get { return location; }
            private set { location = value; }
        }

        public void Reset()
        {
            this.location = defaultLocation;
        }

        public bool Move(Direction towards)
        {
            if (towards == Direction.Unknown)
                return false;

            Point disp = GetDisplacement(towards);
            Point dest = Location, dest2 = Location;
            
            dest.Offset(disp);
            disp.X *= 2;
            disp.Y *= 2;
            dest2.Offset(disp);

            if (map[dest] == 4)
                return false;
            if (map.IsFree(dest))
            {
                Location = dest;
                return true;
            }
            if (map.IsFree(dest2) && map.IsMovable(dest))
            {
                map.MoveObject(dest, dest2);
                Location = dest;
                return true;
            }
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
    }
}
