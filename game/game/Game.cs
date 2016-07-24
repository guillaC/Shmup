using System;
using System.Threading;

namespace game
{
    internal class Game
    {
        public Map map;
        public Joueur player;

        public Game()
        {
            this.map = new Map();
            this.player = new Joueur(this.map);

            Thread threadRefresh = new Thread(() =>
            {
                while (player.vivant)
                {
                    Thread.Sleep(3);
                    RefreshMap();
                }
            });

            Thread threadLaser = new Thread(() =>
            {
                while (player.vivant)
                {
                    Thread.Sleep(10);
                    for (int i = 0; i != 24; i++)
                    {
                        for (int j = 0; j != 79; j++)
                        {
                            if (map.getElement(i, j) == 3) //laser
                            {
                                map.ajoutLaser(i, j + 1);
                                map.tejElement(i, j);
                                if (j < 78)
                                {
                                    if (checkLaserCollision(i, j + 2))
                                    {
                                        map.tejElement(i, j + 1);
                                        map.tejElement(i, j + 2);
                                    }
                                    j++;
                                }
                                else
                                {
                                    map.tejElement(i, j + 1);
                                }
                            }
                        }
                    }
                }
            });

            Thread threadBlocs = new Thread(() =>
            {
                while (player.vivant)
                {
                    Thread.Sleep(300);
                    for (int i = 0; i != 24; i++)
                    {
                        for (int j = 0; j != 79; j++)
                        {
                            if (map.getElement(i, j) == 2 || map.getElement(i, j) == 4)
                            {
                                if (checkBlocCollision(i, j - 1) == true)
                                {
                                    player.meurt();
                                }
                                else
                                {
                                    if (map.getElement(i, j) == 2)
                                    {
                                        map.ajoutBlocGris(i, j - 1);
                                    }
                                    else
                                    {
                                        map.ajoutBlocVert(i, j - 1);
                                    }

                                    map.tejElement(i, j);

                                    if (j < 2)
                                    {
                                        map.tejElement(i, j - 1);
                                    }
                                }
                            }
                        }
                    }
                }
            });

            threadRefresh.Start();
            threadBlocs.Start();
            threadLaser.Start();
        }

        public bool checkBlocCollision(int blocX, int blocY)
        {
            if (map.getElement(blocX, blocY) == 1)
            {
                System.Diagnostics.Debug.WriteLine("bloc collision: avec joueur");
                return true;
            }
            else
            {
                return false;
            }
        }

        public bool checkLaserCollision(int laserX, int laserY)
        {
            if (map.getElement(laserX, laserY) == 2)
            {
                System.Diagnostics.Debug.WriteLine("laser collision (bloc gris)");
                return true;
            }
            else if (map.getElement(laserX, laserY) == 4)
            {
                System.Diagnostics.Debug.WriteLine("laser collision (bloc vert)");
                map.plateau[laserX, laserY - 1] = 4;
                return false;
            }
            else
            {
                return false;
            }
        }

        public void RefreshMap()
        {
            int cursorPos = 0;
            int[,] plateau = new int[25, 80];

            for (int i = 0; i != 24; i++)
            {
                for (int j = 1; j != 80; j++)
                {
                    plateau = map.getMap();

                    if (plateau[i, j] == 0) //vide
                    {
                        Console.Write(" ");
                    }
                    else if (plateau[i, j] == 1) //joueur
                    {
                        Console.ForegroundColor = ConsoleColor.Cyan;
                        Console.Write(player.sprite);
                    }
                    else if (plateau[i, j] == 3) //laser
                    {
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.Write("■");
                    }
                    else if (plateau[i, j] == 2) //bloc gris
                    {
                        if (j > 5)
                        {
                            Console.ForegroundColor = ConsoleColor.Gray;
                        }
                        else
                        {
                            Console.ForegroundColor = ConsoleColor.DarkGray;
                        }
                        Console.Write("█");
                    }
                    else if (plateau[i, j] == 4) //bloc vert
                    {
                        if (j > 5)
                        {
                            Console.ForegroundColor = ConsoleColor.Green;
                        }
                        else
                        {
                            Console.ForegroundColor = ConsoleColor.DarkGreen;
                        }
                        Console.Write("█");
                    }
                }
                cursorPos++;
                Console.ResetColor();
                Console.SetCursorPosition(0, cursorPos);
            }
        }
    }
}