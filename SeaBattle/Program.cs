using System;
using SeaBattle.Model;
using SeaBattle.Model.Enum;

namespace SeaBattle
{
    public class Program
    {
        static void Main(string[] args)
        {
            Player player = new Player("Test player");
            Bot bot = new Bot("Test bot");
            Game game = new Game(player, bot);
            
            do
            {
                Console.WriteLine("1 Змiнити iм'я\n" +
                "2 Start\n" +
                "3 Rules\n" +
                "4 Exit\n");
                Enum.TryParse(Console.ReadLine(), out Menu choice);
                switch (choice)
                {
                    case Menu.Settings:
                        {
                            Enum.TryParse(Console.ReadLine(), out ChangeName changeName);
                            switch (changeName)
                            {
                                case ChangeName.Player:
                                    {
                                        Console.WriteLine("Введiть iм'я");
                                        player.Name = Console.ReadLine();
                                    }
                                    break;
                                case ChangeName.Bot:
                                    {
                                        Console.WriteLine("Введiть iм'я");
                                        bot.Name = Console.ReadLine();
                                    }
                                    break;
                                default:
                                    break;
                            }
                        }
                        break;
                    case Menu.Start:
                        {
                            game.Player.MyField.ShowField();
                            game.StartGame();
                        }
                        break;
                    case Menu.Rules:
                        {
                            Console.WriteLine("Rules");
                        }
                        break;
                    case Menu.Exit:
                        return;
                    default:
                        Console.WriteLine("Error choice");
                        break;
                }
            } while (true);

        }
    }
}
