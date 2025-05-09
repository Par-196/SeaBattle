using System;
using SeaBattle.Model.Enum;

namespace SeaBattle.Model
{
    public class Game
    {
        public Player Player { get; set; }
        public Bot Bot { get; set; }


        public Game(Player player, Bot bot)
        {
            Player = player;
            Bot = bot;
        }

        public void StartGame() 
        {
            Console.WriteLine("1. Розставити кораблi в ручну " + "\n" +
                "2. Розставити кораблi рандомно");

            System.Enum.TryParse(Console.ReadLine(), out ArrangementOfShipsMenu arrangementMenu);

            switch (arrangementMenu)
            {
                case ArrangementOfShipsMenu.Manually:
                    Player.MyField.ArrangeFieldManually();
                    break;
                case ArrangementOfShipsMenu.Random:
                    Player.MyField.ArrangeFieldRandomly();
                    break;
                default:
                    break;
            }
            Bot.MyField.ArrangeFieldRandomly();
        }

    }
}
