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
}