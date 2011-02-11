using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.Threading;

namespace Dragger
{
    enum ConsoleAnimation
    {
        None,
        Typewriter,
        Blink
    }

    enum TextAlign
    {     
        Default,
        Left,
        Center,
        Right
    }

    static class ConsoleUtil
    {
        public static void Write(string message, ConsoleColor color)
        {
            ConsoleColor oldColor = Console.ForegroundColor;
            Console.ForegroundColor = color;
            Console.Write(message);
            Console.ForegroundColor = oldColor;
        }

        public static void Write(string message, ConsoleAnimation anim) { Write(message, Console.ForegroundColor, anim); }
        
        public static void Write(string message, TextAlign align)
        {
            Write(message, Console.ForegroundColor, align);
        }        

        public static void Write(string message, ConsoleColor color, TextAlign align)
        {
            int left = Console.CursorLeft;
            switch (align)
            {
                case TextAlign.Left:
                    Console.CursorLeft = 0;
                    break;
                case TextAlign.Center:
                    Console.CursorLeft = Console.BufferWidth / 2;
                    break;
                case TextAlign.Right:
                    Console.CursorLeft = Console.BufferWidth - message.Length;
                    break;
                default:
                    break;
            }
            Write(message, color);
            Console.CursorLeft = left;
        }        

        public static void Write(string message, ConsoleColor color, ConsoleAnimation anim)
        {
            switch (anim)
            {
                case ConsoleAnimation.Typewriter:
                    WriteTwriter(message, color);
                    break;
                case ConsoleAnimation.Blink:
                    WriteBlink(message, color);
                    break;
                default:
                    Write(message, color);
                    break;
            }
        }        
        

        private static void WriteBlink(string message, ConsoleColor color)
        {            
            int top = Console.CursorTop;
            int left = Console.CursorLeft;

            for (int i = 0; i < 7; i++)
            {
                Console.CursorTop = top;
                Console.CursorLeft = left;
                Write(message, (i & 1)==1 ? Console.BackgroundColor: color);
                Thread.Sleep(200);
            }
        }

        private static void WriteTwriter(string message, ConsoleColor color)
        {
            foreach (char ch in message)
            {
                Write(ch.ToString(), color);
                Thread.Sleep(5);
            }
        }
    }
}
