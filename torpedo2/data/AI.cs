using System;
using System.Collections.Generic;
using System.Text;

namespace torpedo2.data
{
    public class AI
    {
        private readonly Random _random = new Random();

        public Position SearchForTarget(int gameSize, Position[,] playerPositions)
        {
            Position pos;
            for (int x = 0; x < gameSize; x++)
            {
                for (int y = 0; y < gameSize; y++)
                {
                    if (playerPositions[x, y].Hit && playerPositions[x, y].Ship != 0)
                    {
                        if (x > 1)
                        {
                            if (CanHit(playerPositions[x - 1, y]))
                            {
                                pos = playerPositions[x - 1, y];
                                return pos;
                            }
                        }
                        if (x < 9)
                        {
                            if (CanHit(playerPositions[x + 1, y]))
                            {
                                pos = playerPositions[x + 1, y];
                                return pos;
                            }
                        }
                        if (y > 1)
                        {
                            if (CanHit(playerPositions[x, y - 1]))
                            {
                                pos = playerPositions[x, y - 1];
                                return pos;
                            }
                        }
                        if (y < 9)
                        {
                            if (CanHit(playerPositions[x, y + 1]))
                            {
                                pos = playerPositions[x, y + 1];
                                return pos;
                            }
                        }
                    }
                }
            }
            pos = HitRandomPosition(gameSize, playerPositions);

            return pos;
        }

        private bool CanHit(Position position)
        {
            return !position.Hit;
        }

        public Position HitRandomPosition(int gameSize, Position[,] playerPositions)
        {
            Position pos = new Position();
            while (true)
            {
                int X = _random.Next(gameSize);
                int Y = _random.Next(gameSize);
                pos = playerPositions[X, Y];
                if (!pos.Hit)
                {
                    return pos;
                }
            }
        }
    }
}
