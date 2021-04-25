using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DungeonGenerationDemo
{
    class VisibilityDoor : IGameObject
    {
        public Point Point { get; }
        public bool Solid { get; } = false;
        private char character = '+';
        private ConsoleColor foreground = ConsoleColor.White;
        private ConsoleColor background = ConsoleColor.Black;
        Dungeon dungeon;
        private List<Point> visible;

        public VisibilityDoor(Point p, Dungeon d, List<Point> v)
        {
            Point = p;
            dungeon = d;
            visible = v;
        }

        public void Paint()
        {
            Console.SetCursorPosition(Point.Col, Point.Row);
            Console.ForegroundColor = foreground;
            Console.BackgroundColor = background;
            Console.Write(character);
        }

        public bool OnCollision(IGameObject other)
        {
            Debug.Print("At Door");
            foreach(Point p in visible) {
                dungeon.SetVisible(p, true);
            }
            return true;
        }
    }
}
