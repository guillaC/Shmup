using System.Threading;

namespace game
{
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
}