namespace game
{
    internal class Joueur
    {
        private Map map;
        public int posX;
        public int posY;
        public bool vivant;

        public Joueur(Map mapGame)
        {
            this.map = mapGame;
            this.posX = 10;
            this.posY = 10;
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
            this.vivant = false;
        }
    }
}