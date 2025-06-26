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
            bool isGameOver = false;
            

            Console.WriteLine("1. Розставити кораблi в ручну " + "\n" +
                "2. Розставити кораблi рандомно");

            System.Enum.TryParse(Console.ReadLine(), out ArrangementOfShipsMenu arrangementMenu);

            switch (arrangementMenu)
            {
                case ArrangementOfShipsMenu.Manually:
                    Player.MyField.ArrangeFieldManually();
                    Player.MyField.ShowField();
                    break;
                case ArrangementOfShipsMenu.Random:

                    Player.MyField.ArrangeFieldRandomly();
                    Player.MyField.ShowField();
                    break;
                default:
                    break;
            }
            Console.WriteLine("BOT FIELD");
            Bot.MyField.ArrangeFieldRandomly();
            Bot.MyField.ShowField();

            var point = new Point();
            int myOneCellShip = 4;
            int myTwoCellShip = 3;
            int myThreeCellShip = 2;
            int myFourCellShip = 1;

            int allMyShips = myOneCellShip + myTwoCellShip + myThreeCellShip + myFourCellShip;

            int enemyOneCellShip = 4;
            int enemyTwoCellShip = 3;
            int enemyThreeCellShip = 2;
            int enemyFourCellShip = 1;

            int allEnemyShips = enemyOneCellShip + enemyTwoCellShip + enemyThreeCellShip + enemyFourCellShip;

            bool shipBeenFound = false;

            int oldPointX = 0;
            int oldPointY = 0;

            while (allMyShips != 0 && allEnemyShips != 0)
            {
                bool hit = true;
                //while (hit)
                //{
                //    Console.WriteLine("\nMy Field\n");
                //    Player.MyField.ShowField();
                //    Console.WriteLine("\nEnemy Filed\n");
                //    Player.EnemyField.ShowField();

                //    Console.WriteLine("Fire");
                //    do
                //    {
                //        Console.WriteLine("Enter cord x");
                //        int.TryParse(Console.ReadLine(), out int x);
                //        point.X = x;

                //        Console.WriteLine("Enter cord y");
                //        int.TryParse(Console.ReadLine(), out int y);
                //        point.Y = y;


                //    } while (point.X < 1 || point.X > 11 || point.Y < 1 || point.Y > 11);

                //TypeCell typeCell = Bot.MyField.Cells[point.X, point.Y].Value;

                //    TypeShips typeShips = TypeShips.OneCellShip;

                //    (allEnemyShips) = Player.EnemyField.FireEnemyField(allEnemyShips, point, typeCell, typeShips, Player.EnemyField, Bot.MyField);

                //    if (typeCell != TypeCell.ShipBody)
                //    {
                //        hit = false;
                //    }

                //}

                while (hit)
                {

                    
                    Point botPoints = new Point();
                    Random _random = new Random();

                    if (shipBeenFound == false)
                    {
                        do
                        {
                            botPoints.X = _random.Next(1, 11);

                            botPoints.Y = _random.Next(1, 11);

                        } while (botPoints.X < 1 || botPoints.X > 11 || botPoints.Y < 1 || botPoints.Y > 11 ||
                                    Bot.EnemyField.Cells[botPoints.X, botPoints.Y].Value != TypeCell.Empty || Bot.EnemyField.AnyShipsAroundPoint(botPoints));
                    }
                    else
                    {
                        botPoints.X = oldPointX;
                        botPoints.Y = oldPointY;
                    }

                    TypeCell botTypeCell = Player.MyField.Cells[botPoints.X, botPoints.Y].Value;

                    TypeShips botTypeShips = TypeShips.OneCellShip;

                    (allMyShips, botPoints) = Bot.EnemyField.BotMove(oldPointX, oldPointY, allMyShips, botPoints, botTypeCell, botTypeShips, Bot.EnemyField, Player.MyField);

                    
                    oldPointX = botPoints.X;
                    oldPointY = botPoints.Y;

                    Console.WriteLine("\n\n\n\nBot Enemy Filed\n\n\n\n");
                    Bot.EnemyField.ShowField();
                    if (botTypeCell == TypeCell.ShipBody)
                    {
                        shipBeenFound = true;
                    }
                    if (botTypeCell != TypeCell.ShipBody)
                    {
                        hit = false;
                        shipBeenFound = false;
                    }
                }
            }

            if (allMyShips == 0) 
            {
                Console.WriteLine("\n\nВигра Бот, ти лох\n");
            }
            else if (allEnemyShips == 0) 
            {
                Console.WriteLine("\n\nВиграв ти, молодець\n");
            }
            else 
            {
                Console.WriteLine("\n\nНiчия\n");
            }




        }


    }

}

