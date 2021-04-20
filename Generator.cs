﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;

namespace DungeonGenerationDemo
{
    /// <summary>
    /// Used interally by the generator. Should not be used elsewhere.
    /// </summary>
    enum Tile
    {
        EMPTY,
        FLOOR,
        PATH,
        VER_WALL,
        HOR_WALL,
        WALL,
        PATH_WALL,
        EXIT,
        DOOR
    }

    /// <summary>
    /// Generates a dungeon by placing random rooms and connecting them together with procedural 
    /// paths.
    /// </summary>
    class Generator
    {
        private const int MIN_ROOM_WIDTH = 8;
        private const int MAX_ROOM_WIDTH = 20;
        private const int MIN_ROOM_HEIGHT = 4;
        private const int MAX_ROOM_HEIGHT = 10;
        //Gives the odds the path will change direction. Lower number makes paths curvier
        private const int PATH_DIR_CHANGE = 0;

        private Dungeon dungeon;
        private Tile[,] grid;
        private Random rand;
        private int width;
        private int height;
        private List<Room> rooms;
        private int sets;

        public Generator(int width, int height)
        {
            dungeon = new Dungeon(width, height);
            grid = new Tile[width, height];
            this.width = width;
            this.height = height;
            rand = new Random();
            rooms = new List<Room>();
        }

        /// <summary>
        /// Finds a valid point somewhere in a room. Only works after Generate has been called.
        /// </summary>
        /// <returns>Valid point in dungeon</returns>
        public Point getValidPoint()
        {
            Room room = rooms[rand.Next(rooms.Count)];

            return new Point(rand.Next(room.MinC.Col + 1, room.MaxC.Col - 1), rand.Next(room.MinC.Row + 1, room.MaxC.Row - 1));
        }

        /// <summary>
        /// Returns the dungeon generated by the generator. 
        /// </summary>
        /// <returns></returns>
        public Dungeon GetDungeon()
        {
            return dungeon;
        }

        /// <summary>
        /// Acts as the entrance point for the rest of the generator. Actually produces a dungeon 
        /// with the given number of rooms, in the space given in the constructor. 
        /// </summary>
        /// <param name="count">Number of rooms to be generated</param>
        public void Generate(int count)
        {
            for (int i = 0; i < count; i++)
                new Room(this, rand.Next(MIN_ROOM_WIDTH, MAX_ROOM_WIDTH), rand.Next(MIN_ROOM_HEIGHT, MAX_ROOM_HEIGHT));

            foreach (Room el in rooms)
            {
                Room near = nearest(el);
                if (near != null)
                    el.Connect(near);
            }

            place(Tile.EXIT, getValidPoint());
        }

        /// <summary>
        /// Places the given tile in the dungeon, at the same time as adding the tile to the
        /// internal array used for quick access. 
        /// </summary>
        /// <param name="tile"></param>
        /// <param name="p"></param>
        private void place(Tile tile, Point p)
        {
            grid[p.Col, p.Row] = tile;
            StaticTile obj;
            switch (tile)
            {
                case Tile.FLOOR:
                    obj = StaticTile.Floor(p);
                    break;
                case Tile.PATH:
                    obj = StaticTile.Path(p);
                    break;
                case Tile.DOOR:
                    obj = StaticTile.Door(p);
                    break;
                case Tile.VER_WALL:
                    obj = StaticTile.VerWall(p);
                    grid[p.Col, p.Row] = Tile.WALL;
                    break;
                case Tile.HOR_WALL:
                    obj = StaticTile.HorWall(p);
                    grid[p.Col, p.Row] = Tile.WALL;
                    break;
                case Tile.PATH_WALL:
                    obj = StaticTile.PathWall(p);
                    grid[p.Col, p.Row] = Tile.EMPTY;
                    break;
                case Tile.EXIT:
                    obj = StaticTile.Exit(p);
                    break;
                default:
                    obj = new StaticTile(p, false);
                    break;
            }

            dungeon.PlaceObject(obj, p);
        }

        //Not being used
        //private bool pointValid(int x, int y)
        //{
        //    return (0 < x && x < width) && (0 < y && y < height);
        //}

        private Room nearest(Room room)
        {
            int nearestDist = int.MaxValue;
            Room nearest = null;

            foreach (Room el in rooms)
            {
                int elDist = (int)(Math.Pow(el.Center.Col - room.Center.Col, 2) + Math.Pow(el.Center.Row - room.Center.Row, 2));
                if (el != room && !el.connected.Contains(room) && elDist < nearestDist)
                {
                    nearestDist = elDist;
                    nearest = el;
                }
            }

            return nearest;
        }

        private bool anyCollide(Room room)
        {
            foreach (Room el in rooms)
            {
                if (el != room && room.Collide(el))
                    return true;
            }

            return false;
        }

        private class Path
        {
            private Generator gen;
            private Point target;
            private Point current;
            private int travelCol;
            private int travelRow;
            private bool active;
            private int prev;

            public Path(Generator gen, Room r1, Room r2)
            {
                this.gen = gen;

                target = r1.Center;
                current = r2.Center;

                travelCol = target.Col - current.Col;
                travelRow = target.Row - current.Row;

                pathStep();
            }

            void placeWall(Point p)
            {
                if (gen.grid[p.Col, p.Row] == Tile.EMPTY)
                    gen.place(Tile.PATH_WALL, p);
            }

            private void pathStep()
            {
                List<int> moves = new List<int>();

                if (travelCol > 0)
                    moves.Add(0);
                if (travelCol < 0)
                    moves.Add(1);
                if (travelRow > 0)
                    moves.Add(2);
                if (travelRow < 0)
                    moves.Add(3);

                if (moves.Count == 0)
                    return;

                int move = moves[gen.rand.Next(moves.Count)];
                foreach (int el in moves)
                {
                    if(el == prev)
                    {
                        if (gen.rand.Next(PATH_DIR_CHANGE) != 0)
                            move = prev;
                    }
                }
                prev = move;
                Point last = current;
                switch (move)
                {
                    case 0:
                        current = new Point(current.Col + 1, current.Row);
                        travelCol--;
                        break;
                    case 1:
                        current = new Point(current.Col - 1, current.Row);
                        travelCol++;
                        break;
                    case 2:
                        current = new Point(current.Col, current.Row + 1);
                        travelRow--;
                        break;
                    case 3:
                        current = new Point(current.Col, current.Row - 1);
                        travelRow++;
                        break;
                }

                if (!active)
                {
                    if (gen.grid[current.Col, current.Row] == Tile.EMPTY)
                    {
                        active = true;
                        gen.place(Tile.DOOR, last);
                    }
                }

                if (active)
                {
                    switch (gen.grid[current.Col, current.Row])
                    {
                        case Tile.EMPTY:
                            gen.place(Tile.PATH, current);
                            break;
                        case Tile.WALL:
                            gen.place(Tile.DOOR, current);
                            return;
                        case Tile.PATH:
                            return;
                    }
                }

                placeWall(new Point(current.Col + 1, current.Row));
                placeWall(new Point(current.Col - 1, current.Row));
                placeWall(new Point(current.Col, current.Row + 1));
                placeWall(new Point(current.Col, current.Row - 1));

                pathStep();
            }
        }

        private class Room
        {
            public Point MinC;
            public Point MaxC;
            public Point Center;
            public int Set;
            public List<Room> connected;
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
                    MinC = new Point(gen.rand.Next(gen.width - width - 1), gen.rand.Next(gen.height - height - 1));
                    MaxC = new Point(MinC.Col + width, MinC.Row + height);
                } while (gen.anyCollide(this));

                Center = new Point((int)((MinC.Col + MaxC.Col) * 0.5), (int)((MinC.Row + MaxC.Row) * 0.5));

                place();
            }

            public bool Collide(Room other)
            {
                if (this.MinC.Col > other.MaxC.Col ||
                    this.MaxC.Col < other.MinC.Col ||
                    this.MinC.Row > other.MaxC.Row ||
                    this.MaxC.Row < other.MinC.Row)
                    return false;
                else
                    return true;
            }

            public int Dist(int x, int y)
            {
                int closeX;
                int closeY;

                if (x > MaxC.Col)
                    closeX = MaxC.Col;
                else if (x < MinC.Col)
                    closeX = MinC.Col;
                else
                    closeX = x;

                if (y > MaxC.Row)
                    closeY = MaxC.Row;
                else if (y < MinC.Row)
                    closeY = MinC.Row;
                else
                    closeY = y;

                return (int)Math.Sqrt(Math.Pow(x - closeX, 2) + Math.Pow(y - closeY, 2));
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
                new Path(gen, this, other);
            }

            private void place()
            {
                int width = MaxC.Col - MinC.Col;
                int height = MaxC.Row - MinC.Row;
                for (int i = 1; i < width - 1; i++)
                {
                    for (int j = 1; j < height - 1; j++)
                    {
                        gen.place(Tile.FLOOR, new Point(MinC.Col + i, MinC.Row + j));
                    }
                }
                for(int i = 1; i < width - 1; i++)
                {
                    gen.place(Tile.HOR_WALL, new Point(MinC.Col + i, MinC.Row));
                }

                for (int i = 1; i < width - 1; i++)
                {
                    gen.place(Tile.HOR_WALL, new Point(MinC.Col + i, MinC.Row + height - 1));
                }

                for (int i = 1; i < height - 1; i++)
                {
                    gen.place(Tile.VER_WALL, new Point(MinC.Col, MinC.Row + i));
                }

                for (int i = 1; i < height - 1; i++)
                {
                    gen.place(Tile.VER_WALL, new Point(MinC.Col + width - 1, MinC.Row + i));
                }
            }
        }
    }
}