using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static DungeonGenerationDemo.Constants;

namespace DungeonGenerationDemo
{
    /// <summary>
    /// Holds all of the game object and allows access and changing.
    /// </summary>
    class Dungeon
    {
        private Stack<IGameObject>[,] map { get; }
        public int Width { get; }
        public int Height { get; }
        public Random rand { get; set; }
        public List<Monster> monsters { get; set; }
        public bool atExit { get; set; }

        public Player Player { get; set; }

        /// <summary>
        /// Constructs a dungeon with a 2D array of stacks of IGameObjects. Should never really be 
        /// called outside of generator.
        /// </summary>
        /// <param name="map"></param>
        public Dungeon(int width, int height)
        {
            Width = width;
            Height = height;
            map = new Stack<IGameObject>[width, height];
        }

        /// <summary>
        /// Paints everything in the dungeon. Should not be used in the final game, as rooms will 
        /// be drawn one by one.
        /// </summary>
        public void PaintAll()
        {
            for(int i = 0; i < map.GetLength(0); i++)
            {
                for(int j = 0; j < map.GetLength(1); j++)
                {
                    PaintAt(new Point(i, j));
                }
            }
        }

        /// <summary>
        /// Paints a single tile in the dungeon. 
        /// </summary>
        /// <param name="row"></param>
        /// <param name="col"></param>
        public void PaintAt(Point p)
        {
            Stack<IGameObject> current = map[p.Col, p.Row];

            if (!IsEmpty(new Point(p.Col, p.Row)))
                current.Peek().Paint();
        }

        /// <summary>
        /// Returns the game object at the given row and column, or null if that tile is null
        /// </summary>
        /// <param name="row"></param>
        /// <param name="col"></param>
        /// <returns></returns>
        public IGameObject GetObject(Point p)
        {
            Stack<IGameObject> current = map[p.Col, p.Row];
            if (current == null)
                return null;
            if (current.Count == 0)
                return null;
            return current.Peek();
        }

        /// <summary>
        /// Returns true is the tile at the given row and column is empty
        /// </summary>
        /// <param name="row"></param>
        /// <param name="col"></param>
        /// <returns></returns>
        public bool IsEmpty(Point p)
        {
            Stack<IGameObject> current = map[p.Col, p.Row];
            if (current == null)
                return true;
            if (current.Count == 0)
                return true;

            return false;
        }

        /// <summary>
        /// Places a game object inside of the dungeon on the top of whatever tile is chosen.
        /// Handles stack creation.
        /// </summary>
        /// <param name="gameObject"></param>
        /// <param name="row"></param>
        /// <param name="col"></param>
        public void PlaceObject(IGameObject gameObject, Point p)
        {
            Stack<IGameObject> current = map[p.Col, p.Row];
            if (current == null)
            {
                map[p.Col, p.Row] = new Stack<IGameObject>();
                current = map[p.Col, p.Row];
            }

            current.Push(gameObject);
        }

        /// <summary>
        /// Returns the internal map. Should be used sparingly, use GetObject if possible. 
        /// </summary>
        /// <returns></returns>
        public Stack<IGameObject>[,] GetInternalMap()
        {
            return map;
        }

        /// <summary>
        /// Move whatever is on the top of the position onto a new position
        /// </summary>
        /// <param name="origin"></param>
        /// <param name="Destination"></param>
        /// <returns></returns>
        public bool MoveCreature(Point origin, Point Destination)
        {

            // If there's time for the chaos of local methods in local delegates
            //public Action<Point, Point> MoveDelegate() { return MoveObject; }
            //Action<Point> draw = ConsoleDrawing.Triangle;

            //bool inside(string stuff) { return false; }

            if (map[origin.Col, origin.Row].Peek() is StaticTile) { return false; }

            ICreature tempObject = (ICreature)(map[origin.Col, origin.Row].Pop());

            tempObject.Move(Destination);

            map[Destination.Col, Destination.Row].Push(tempObject);

            PaintAt(origin);

            PaintAt(Destination);

            return false;
        }

        /// <summary>
        /// Have the player attempt to move, and either interact with what's in the new direction
        /// or move in the new direction
        /// </summary>
        /// <param name="direction"></param>
        /// <returns></returns>
        public bool MovePlayer(ConsoleKey key)
        {
            Cardinal newDirection;

            switch (key)
            {
                case ConsoleKey.A:
                    newDirection = Cardinal.Left;
                    break;
                case ConsoleKey.D:
                    newDirection = Cardinal.Right;
                    break;
                case ConsoleKey.W:
                    newDirection = Cardinal.Up;
                    break;
                case ConsoleKey.S:
                    newDirection = Cardinal.Down;
                    break;
                case ConsoleKey.X:
                    newDirection = Cardinal.None;
                    break;
                default:
                    return false;
            }

            Point destination = Player.Point + DirectionVectors[(int)newDirection];
            IGameObject target = map[destination.Col, destination.Row].Peek();
            if (!IsEmpty(destination) &&
                !target.Solid && 
                target.Point != Player.Point)
            {
                // if the object gets destroyed/picked up it returns true)
                if (target.OnCollision(Player))
                {
                    if (!(target is StaticTile)) 
                    {
                        IGameObject tempObject = map[destination.Col, destination.Row].Pop(); 
                        if (tempObject is Monster) { monsters.Remove((Monster)tempObject); }
                    }

                    MoveCreature(Player.Point, destination);
                }
                else
                {
                    return Player.OnCollision(target); // returns true if player dies
                }
            }
            MoveMonsters();
            return false;
        }

        public void PlacePlayer()
        {

            // TODO: adding a player to test movement
            for (int i = 0; i < Width; i++)
            {
                for (int j = 0; j < Height; j++)
                {
                    if (!IsEmpty(new Point(i, j))
                        && !map[i,j].Peek().Solid
                        )
                    {
                        Player = new Player(new Point(i, j), rand);
                        map[i, j].Push(Player);
                        i = Width; j = Height;
                    }
                }
            }

        }

        public void DisplayPlayerHealth(int row, int col)
        {
            Console.SetCursorPosition(45, 33);
            Console.Write($"{Player.Health,-2}");

        }

        public void MoveMonsters()
        {
            for( int i = 0; i < monsters.Count; i++)
            {
                Monster monster = monsters[i];
                Monster mapMonster = (Monster)map[monster.Point.Col, monster.Point.Row].Peek();
                //System.Diagnostics.Debug.Print($"Monster check: index {i} list {monster.Point} map {mapMonster.Point}");
                // The monsters only move if the player is within 6 tiles
                if (Player.Point.Distance(mapMonster.Point) > 6) { mapMonster.JustMoved = false; continue; }

                // Let's only have the monster move every other turn
                if (!mapMonster.JustMoved)
                {
                    // moving it toward player
                    Point newDirection = mapMonster.Point.Normalize(Player.Point);

                    Point destination = mapMonster.Point + newDirection;
                    //System.Diagnostics.Debug.Print($"Monster move: index {i} direction {newDirection} destination {destination}");
                    if (!IsEmpty(destination) &&
                        !map[destination.Col, destination.Row].Peek().Solid)
                    {
                        IGameObject target = map[destination.Col, destination.Row].Peek();
                        // if the object gets destroyed/picked up it returns true)
                        if (target.OnCollision(mapMonster))
                        {
                            if (!(target is StaticTile)) { map[destination.Col, destination.Row].Pop(); }

                            MoveCreature(mapMonster.Point, destination);
                            //monsters[i].Move(destination); // TODO: make sure everything points to the same object so I don't have to do this everywhere
                        }
                    }

                    monsters[i].JustMoved = true;
                }
                else { monsters[i].JustMoved = false; }
            }
        }
    }
}
