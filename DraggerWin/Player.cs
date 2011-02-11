using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;

namespace Dragger
{
    class Player
    {
        Point location;
        readonly Point defaultLocation;

        public Player(Point location)
        {
            this.defaultLocation = this.location = location;
        }

        public Point Location
        {
            get { return location; }
            internal set { location = value; }
        }

        public void Reset()
        {
            this.location = defaultLocation;
        }                
    }
}
