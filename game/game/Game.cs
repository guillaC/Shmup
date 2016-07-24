using System;
using System.Collections.Generic;
using System.Threading;

namespace game
{
    internal class Game
    {
        public Map map;
        public Joueur player;
        private Dictionary<char, ConsoleColor> mapColor;

        public Game()
        {
            this.map = new Map();
            this.player = new Joueur(this.map);

            this.mapColor = new Dictionary<char, ConsoleColor>();
            mapColor['>'] = ConsoleColor.Cyan;
            mapColor['■'] = ConsoleColor.Red;
            mapColor['B'] = ConsoleColor.Yellow;
            mapColor['#'] = ConsoleColor.Gray;
            mapColor['_'] = ConsoleColor.DarkBlue;
            mapColor['\\'] = ConsoleColor.DarkBlue;
            mapColor['/'] = ConsoleColor.DarkBlue;
            mapColor['-'] = ConsoleColor.DarkBlue;
            mapColor['+'] = ConsoleColor.DarkBlue;
            mapColor['|'] = ConsoleColor.DarkBlue;

            Thread threadRefresh = new Thread(() =>
            {
                while (player.vivant)
                {
                    Thread.Sleep(5);
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
                            if (map.getElement(i, j) == '■') //laser
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
                            if (map.getElement(i, j) == '#' || isStrongBloc(i, j))
                            {
                                if (checkBlocCollision(i, j - 1) == true)
                                {
                                    player.meurt();
                                }
                                else
                                {
                                    if (map.getElement(i, j) == '#')
                                    {
                                        map.ajoutBlocGris(i, j - 1);
                                    }
                                    else
                                    {
                                        map.ajoutBlocVert(i, j - 1, map.getElement(i, j));
                                    }

                                    map.tejElement(i, j);

                                    if (j < 2)
                                    {
                                        map.tejElement(i, j - 1);
                                    }
                                }
                            }
                            else if (map.getElement(i, j) == 'B')
                            {
                                if (checkBlocCollision(i, j - 1) == true)
                                {
                                    bonusEvent();
                                    map.tejElement(i, j - 1);
                                }
                                else
                                {
                                    map.ajoutBlocBonus(i, j - 1);
                                    map.tejElement(i, j);
                                }

                                if (j < 2)
                                {
                                    map.tejElement(i, j - 1);
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
            if (map.getElement(blocX, blocY) == '>' && map.getElement(blocX, blocY) != 'B')
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public bool checkLaserCollision(int laserX, int laserY)
        {
            if (map.getElement(laserX, laserY) == '#')
            {
                return true;
            }
            else if (isStrongBloc(laserX, laserY))
            {
                map.ajoutBlocVert(laserX, laserY - 1, map.getElement(laserX, laserY));
                return false;
            }
            else
            {
                return false;
            }
        }

        public void bonusEvent()
        {
            for (int i = 0; i != 24; i++)
            {
                for (int j = 1; j != 80; j++)
                {
                    if (isStrongBloc(i, j))
                    {
                        map.ajoutBlocGris(i, j);
                    }
                }
            }
        }

        public bool isStrongBloc(int x, int y)
        {
            char element = map.getElement(x, y);
            if (element == '_' || element == '\\' || element == '/' || element == '-' || element == '+' || element == '|')
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public void RefreshMap()
        {
            int cursorPos = 0;
            char[,] plateau = new char[25, 80];

            for (int i = 0; i != 24; i++)
            {
                for (int j = 1; j != 80; j++)
                {
                    plateau = map.getMap();
                    if (mapColor.ContainsKey(plateau[i, j]))
                    {
                        Console.ForegroundColor = mapColor[plateau[i, j]];
                    }
                    Console.Write(plateau[i, j]);
                }
                cursorPos++;
                Console.SetCursorPosition(0, cursorPos);
            }
        }
    }
}