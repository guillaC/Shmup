using System;
using System.Threading;

namespace game
{
    internal class Map
    {
        public int[,] plateau = new int[25, 80];
        public int[,] plateauFile = new int[25, 1000];

        public Map()
        {
            for (int i = 0; i != 24; i++)
            {
                for (int j = 1; j != 80; j++)
                {
                    this.plateau[i, j] = 0; // map vide
                }
            }

            this.plateau[10, 10] = 1; // emplacement initial du joueur
            readShowMapFile();

            Thread threadLevelMap = new Thread(() =>
            {
                int i = 0;
                while (true)
                {
                    Thread.Sleep(300);
                    for (int j = 1; j != 24; j++)
                    {
                        if (plateauFile[j, i] != 0)
                        {
                            this.plateau[j, 78] = plateauFile[j, i];
                        }
                    }
                    i++;
                }
            });

            threadLevelMap.Start();
        }

        public void readShowMapFile()
        {
            string content;
            int lineLenght = 0;
            int i = 0;

            System.IO.StreamReader file = new System.IO.StreamReader(@"level.txt");
            content = file.ReadToEnd();
            string[] lines = content.Split(new string[] { "\r\n", "\n" }, StringSplitOptions.None);

            foreach (String line in lines)
            {
                lineLenght = line.Length;
                for (int j = 0; j != line.Length - 1; j++)
                {
                    /*<DEBUG>
                    if (Int32.Parse(line[j].ToString()) == 2)
                    {
                        System.Diagnostics.Debug.WriteLine("j=" + j + ": i=" + i + ";");
                        Thread.Sleep(2550);
                    }
                    //</DEBUG>*/
                    plateauFile[i, j] = Int32.Parse(line[j].ToString());
                }
                i++;
                /*<DEBUG>
                System.Diagnostics.Debug.WriteLine(line);
                //</DEBUG>*/
            }
            /*<DEBUG>
            System.Diagnostics.Debug.WriteLine("---------------------------------");
            Thread.Sleep(5000);
            System.Diagnostics.Debug.WriteLine("START");
            for (int k = 0; k != 24; k++)
            {
                for (int j = 1; j != 120; j++)
                {
                    System.Diagnostics.Debug.Write(plateauFile[k, j]);
                }
                System.Diagnostics.Debug.WriteLine("");
            }
            System.Diagnostics.Debug.WriteLine("END");
            //</DEBUG>*/
            file.Close();
        }

        public int[,] getMap()
        {
            return plateau;
        }

        public void refreshPosPlayer(int x, int y, int lx, int ly)
        {
            this.plateau[x, y] = 1;
            this.plateau[lx, ly] = 0;
        }

        public void ajoutBlocGris(int x, int y)
        {
            this.plateau[x, y] = 2;
        }

        public void ajoutBlocVert(int x, int y)
        {
            this.plateau[x, y] = 4;
        }

        public void ajoutLaser(int x, int y)
        {
            this.plateau[x, y] = 3;
        }

        public void tejElement(int x, int y)
        {
            this.plateau[x, y] = 0;
        }

        public int getElement(int x, int y)
        {
            return this.plateau[x, y];
        }
    }

    internal class Joueur
    {
        public Map map;
        public string sprite { get; set; }
        public int posX;
        public int posY;
        public bool vivant;

        public Joueur(Map mapGame)
        {
            this.map = mapGame;
            this.posX = 10;
            this.posY = 10;
            this.sprite = ">";
            this.vivant = true;
        }

        public void deplacer(int x, int y)
        {
            map.refreshPosPlayer(x, y, this.posX, this.posY);
            this.posX = x;
            this.posY = y;
        }

        public void tirer(int x, int y)
        {
            map.ajoutLaser(x, y);
        }

        public void meurt()
        {
            this.sprite = "x";
            Thread.Sleep(100);
            this.sprite = "";
            this.vivant = false;
        }
    }

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

    internal class Program
    {
        private static void Main(string[] args)
        {
            Game game = new Game();
            ConsoleKeyInfo keyInfo;
            while ((keyInfo = Console.ReadKey(true)).Key != ConsoleKey.Escape)
            {
                switch (keyInfo.Key)
                {
                    case ConsoleKey.W: // tirer
                        game.player.tirer(game.player.posX, game.player.posY + 1);
                        break;

                    case ConsoleKey.UpArrow:
                        if (game.player.posX != 1)
                        {
                            if (game.map.getElement(game.player.posX - 1, game.player.posY) != 0)
                            {
                                game.player.meurt();
                            }
                            else
                            {
                                game.player.deplacer(game.player.posX - 1, game.player.posY);
                            }
                        }
                        break;

                    case ConsoleKey.DownArrow:
                        if (game.player.posX != 23)
                        {
                            if (game.map.getElement(game.player.posX + 1, game.player.posY) != 0)
                            {
                                game.player.meurt();
                            }
                            else
                            {
                                game.player.deplacer(game.player.posX + 1, game.player.posY);
                            }
                        }
                        break;

                    case ConsoleKey.RightArrow:

                        if (game.player.posY != 79)
                        {
                            if (game.map.getElement(game.player.posX, game.player.posY + 1) != 0)
                            {
                                game.player.meurt();
                            }
                            else
                            {
                                game.player.deplacer(game.player.posX, game.player.posY + 1);
                            }
                        }
                        break;

                    case ConsoleKey.LeftArrow:
                        if (game.player.posY != 1)
                        {
                            if (game.map.getElement(game.player.posX, game.player.posY - 1) != 0)
                            {
                                game.player.meurt();
                            }
                            else
                            {
                                game.player.deplacer(game.player.posX, game.player.posY - 1);
                            }
                        }
                        break;
                }
            }
        }
    }
}