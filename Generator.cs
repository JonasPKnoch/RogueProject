using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;

namespace DungeonGenerationDemo
{
    public enum Tile
    {
        EMTPY,
        PATH,
        ROOM,
        DOOR,
        GOLD
    }

    class Generator
    {
        private Tile[,] grid;
        private int width;
        private int height;
        private Random rand;
        private List<Room> rooms;
        private int sets;

        public Generator(int width, int height)
        {
            this.width = width;
            this.height = height;
            grid = new Tile[width, height];
            rand = new Random();
            rooms = new List<Room>();
        }

        public void Generate(int count)
        {
            for (int i = 0; i < count; i++)
            {
                new Room(this, rand.Next(8, 20), rand.Next(4, 10));
            }

            rooms[0].Connect(rooms[1]);
        }

        public void Draw()
        {
            StringBuilder col = new StringBuilder();
            for (int i = 0; i < height; i++)
            {
                col.Clear();
                for(int j = 0; j < width; j++)
                {
                    char c = '?';
                    switch(grid[j, i])
                    {
                        case Tile.EMTPY:
                            c = ' ';
                            break;
                        case Tile.ROOM:
                            c = '.';
                            break;
                        case Tile.DOOR:
                            c = '+';
                            break;
                        case Tile.PATH:
                            c = '#';
                            break;
                        case Tile.GOLD:
                            c = 'g';
                            break;
                    }
                    col.Append(c);
                }
                Console.SetCursorPosition(0, i);
                Console.Write(col);
            }
        }

        private bool pointValid(int x, int y)
        {
            return (0 < x && x < width) && (0 < y && y < height);
        }

        private void path(Room r1, Room r2)
        {
            int x1 = (int)((r1.minX + r1.maxX) * 0.5);
            int y1 = (int)((r1.minY + r1.maxY) * 0.5);
            int x2 = (int)((r2.minX + r2.maxX) * 0.5);
            int y2 = (int)((r2.minY + r2.maxY) * 0.5);

            int xDif = x2 - x1;
            int yDif = y2 - y1;

            path(x1, y1, xDif, yDif);
        }

        private void path(int x, int y, int travelX, int travelY)
        {
            if (grid[x, y] == Tile.EMTPY)
                grid[x, y] = Tile.PATH;


            List<int> moves = new List<int>();

            if (travelX > 0)
                moves.Add(0);
            if (travelX < 0)
                moves.Add(1);
            if (travelY > 0)
                moves.Add(2);
            if (travelY < 0)
                moves.Add(3);

            if (moves.Count == 0)
                return;

            int move = moves[rand.Next(moves.Count)];
            switch(move)
            {
                case 0:
                    path(x + 1, y, travelX - 1, travelY);
                    break;
                case 1:
                    path(x - 1, y, travelX + 1, travelY);
                    break;
                case 2:
                    path(x, y + 1, travelX, travelY - 1);
                    break;
                case 3:
                    path(x, y - 1, travelX, travelY + 1);
                    break;
            }
        }

        private Room nearest(Room room)
        {
            int nearestDist = int.MaxValue;
            Room nearest = null;

            foreach(Room el in rooms)
            {
                
            }

            return nearest;
        }

        private bool anyCollide(Room room)
        {
            foreach(Room el in rooms)
            {
                if (el != room && room.Collide(el))
                    return true;
            }

            return false;
        }

        private class Room
        {
            public int minX;
            public int minY;
            public int maxX;
            public int maxY;
            public int Set;
            private List<Room> connected;
            private Generator gen;

            public Room(Generator gen, int width, int height)
            {
                this.gen = gen;
                Set = gen.rooms.Count;
                gen.sets++;
                gen.rooms.Add(this);
                connected = new List<Room>();

                do
                {
                    minX = gen.rand.Next(gen.width - width - 1);
                    minY = gen.rand.Next(gen.height - height - 1);
                    maxX = minX + width;
                    maxY = minY + height;
                } while (gen.anyCollide(this));

                Place();
            }

            public bool Collide(Room other)
            {
                if (this.minX > other.maxX ||
                    this.maxX < other.minX ||
                    this.minY > other.maxY ||
                    this.maxY < other.minY)
                    return false;
                else
                    return true;
            }
            
            public int Dist(int x, int y)
            {
                int closeX;
                int closeY;

                if (x > maxX)
                    closeX = maxX;
                else if (x < minX)
                    closeX = minX;
                else
                    closeX = x;

                if (y > maxY)
                    closeY = maxY;
                else if (y < minY)
                    closeY = minY;
                else
                    closeY = y;

                return (int) Math.Sqrt(Math.Pow(x - closeX, 2) + Math.Pow(y - closeY, 2));
            }

            public void Connect(Room other)
            {
                if (this.connected.Contains(other))
                    return;

                if (this.Set != other.Set)
                {
                    other.Set = this.Set;
                    gen.sets--;
                }

                this.connected.Add(other);
                other.connected.Add(this);
                gen.path(this, other);
            }

            public void Place()
            {
                int width = maxX - minX;
                int height = maxY - minY;
                for (int i = 0; i < width; i++)
                {
                    for (int j = 0; j < height; j++)
                    {
                        gen.grid[minX + i, minY + j] = Tile.ROOM;
                    }
                }

                for (int i = 0; i < gen.rand.Next(0, 3); i++)
                {
                    gen.grid[gen.rand.Next(minX, maxX), gen.rand.Next(minY, maxY)] = Tile.GOLD;
                }
            }
        }
    }
}
