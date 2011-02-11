using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;
using System.Drawing;

namespace Dragger
{
    class Program
    {
        static void Main(string[] args)
        {
            GameEngine.Initialize();
            GameEngine.MainLoop();
        }        
    }
}
