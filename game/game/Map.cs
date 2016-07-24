using System;
using System.Threading;

namespace game
{
    internal class Map
    {

        const int height = 25;
        const int width = 80;

        private char[,] plateau = new char[height, width];
        private char[,] plateauFile = new char[height, 350];

        public Map()
        {
            for (int i = 0; i != 24; i++)
            {
                for (int j = 1; j != 80; j++)
                {
                    this.plateau[i, j] = ' '; // map vide
                }
            }

            this.plateau[10, 10] = '>'; // emplacement initial du joueur
            readShowMapFile();

            Thread threadLevelMap = new Thread(() =>
            {
                int i = 0;
                while (true)
                {
                    Thread.Sleep(300);
                    for (int j = 1; j != 24; j++)
                    {
                        if (plateauFile[j, i] != ' ')
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
                    plateauFile[i, j] = line[j].ToString()[0];
                }
                i++;
#if DEBUG
                System.Diagnostics.Debug.WriteLine(line);
#endif
            }
#if DEBUG
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
#endif
            file.Close();
        }

        public char[,] getMap()
        {
            return plateau;
        }

        public void refreshPosPlayer(int x, int y, int lx, int ly)
        {
            this.plateau[x, y] = '>';
            this.plateau[lx, ly] = ' ';
        }

        public void ajoutBlocBonus(int x, int y)
        {
            this.plateau[x, y] = 'B';
        }

        public void ajoutBlocGris(int x, int y)
        {
            this.plateau[x, y] = '#';
        }

        public void ajoutBlocVert(int x, int y, char bloc)
        {
            this.plateau[x, y] = bloc;
        }

        public void ajoutLaser(int x, int y)
        {
            this.plateau[x, y] = '■';
        }

        public void tejElement(int x, int y)
        {
            this.plateau[x, y] = ' ';
        }

        public char getElement(int x, int y)
        {
            return this.plateau[x, y];
        }
    }
}