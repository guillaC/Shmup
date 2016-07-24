//#define DEBUG

using System;

namespace game
{
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
                        game.player.tirer(game.player.posX, game.player.posY + 2);
                        break;

                    case ConsoleKey.UpArrow:
                        if (game.player.posX != 1)
                        {
                            if (game.map.getElement(game.player.posX - 1, game.player.posY) != ' ')
                            {
                                if (game.map.getElement(game.player.posX - 1, game.player.posY) == 'B')
                                {
                                    game.bonusEvent();
                                }
                                else
                                {
                                    game.player.meurt();
                                }
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
                            if (game.map.getElement(game.player.posX + 1, game.player.posY) != ' ')
                            {
                                if (game.map.getElement(game.player.posX + 1, game.player.posY) == 'B')
                                {
                                    game.bonusEvent();
                                }
                                else
                                {
                                    game.player.meurt();
                                }
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
                            if (game.map.getElement(game.player.posX, game.player.posY + 1) != ' ')
                            {
                                if (game.map.getElement(game.player.posX, game.player.posY + 1) == 'B')
                                {
                                    game.bonusEvent();
                                }
                                else
                                {
                                    game.player.meurt();
                                }
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
                            if (game.map.getElement(game.player.posX, game.player.posY - 1) != ' ')
                            {
                                if (game.map.getElement(game.player.posX, game.player.posY - 1) == 'B')
                                {
                                    game.bonusEvent();
                                }
                                else
                                {
                                    game.player.meurt();
                                }
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