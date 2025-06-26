using SeaBattle.Model.Enum;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics.Eventing.Reader;
using System.Linq;
using System.Security.Cryptography;
using System.Security.Permissions;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;

namespace SeaBattle.Model
{
    public class Field
    {
        public Cell[,] Cells { get; set; }

        private readonly Random _random = new Random();

        public Field() 
        {
            Cells = new Cell[12, 12];
            for (int i = 0; i < Cells.GetLength(0); i++)
            {
                for (int j = 0; j < Cells.GetLength(1); j++)
                {
                    if (i == 0 || i == 11)
                    {
                        Cells[i, j] = new Cell(TypeCell.Border);
                    }
                    
                    else if (j == 0 || j == 11)
                    {
                        Cells[i, j] = new Cell(TypeCell.Border);
                    }

                    else Cells[i, j] = new Cell(TypeCell.Empty);
                }
            }
        }

        public void ShowField() 
        {
            Console.WriteLine("    1 2 3 4 5 6 7 8 9 10");

            for (int i = 0; i < Cells.GetLength(0); i++)
            {
                if (i > 0 && i < 10)
                {
                    Console.Write($"{i} ");
                }
                else if (i == 10)
                {
                    Console.Write($"{i}");
                }
                else
                {
                    Console.Write("  ");
                }
                for (int j = 0; j < Cells.GetLength(0); j++)
                {
                    
                    Console.Write($"{Cells[i, j]} ");
                }
                Console.WriteLine();
            }
        }

        public void ClearField()
        {
            for (int i = 0; i < Cells.GetLength(0); i++)
            {
                for (int j = 0; j < Cells.GetLength(1); j++)
                {
                    if (Cells[i, j].Value != TypeCell.Border)
                    {
                        Cells[i, j].Value = TypeCell.Empty;
                    }
                }
            }
        }

        public TypeCell CheckPoint(Point point) 
        {
            if (Cells[point.X, point.Y].Value == TypeCell.Empty)
            {
                return TypeCell.Empty;
            }
            else if (Cells[point.X, point.Y].Value == TypeCell.ShipBody)
            { 
                return TypeCell.ShipBody;
            }
                return TypeCell.Empty;
        }


        public int FireEnemyField(int allEnemyShips, Point point, TypeCell playerTypeCell, TypeShips playerTypeShips, Field playerEnemyField, Field botMyField)
        {
            if (playerTypeCell == TypeCell.Empty)
            {
                playerEnemyField.Cells[point.X, point.Y].Value = TypeCell.MissingBody;
            }
            else if (playerTypeCell == TypeCell.ShipBody)
            {
                playerEnemyField.Cells[point.X, point.Y].Value = TypeCell.DamageBody;
                allEnemyShips = CheckShip(allEnemyShips, point, playerTypeCell, playerTypeShips, playerEnemyField, botMyField);
            }
            return allEnemyShips;
        }

        public int CheckShip(int allShips, Point point, TypeCell typeCell, TypeShips typeShips, Field EnemyField, Field MyField)
        {
            Point firstPoint = new Point(0, 0);
            Point secondPoint = new Point(0, 0);
            Point thirdPoint = new Point(0, 0);
            Point fourthPoint = new Point(0, 0);
            List<Point> points = new List<Point>();

            points.Add(firstPoint);
            points.Add(secondPoint);
            points.Add(thirdPoint);
            points.Add(fourthPoint);

            (typeShips, points) = EnemyShipType(point, points, typeShips, MyField);


            allShips = IsShipDead(allShips, typeShips, points, EnemyField, MyField);

            return allShips;

        }

        public (TypeShips, List<Point>) EnemyShipType(Point point, List<Point> points, TypeShips typeShips, Field MyField)
        {
            for (int X = -1; X < 2; X++)
            {
                for (int Y = -1; Y < 2; Y++)
                {
                    if (point.X == point.X + X && point.Y == point.Y + Y)
                    {
                        continue;
                    }
                    if (MyField.Cells[point.X + X, point.Y + Y].Value == TypeCell.ShipBody)
                    {
                        typeShips = TypeShips.TwoCellShip;
                        
                        points[1] = new Point (point.X + X, point.Y + Y);

                        if (MyField.Cells[point.X + X + X, point.Y + Y + Y].Value == TypeCell.ShipBody)
                        {
                            typeShips = TypeShips.ThreeCellShip;

                            if (MyField.Cells[point.X + X + X + X, point.Y + Y + Y + Y].Value == TypeCell.ShipBody)
                            {
                                typeShips = TypeShips.FourCellShip;
                                points[3] = new Point(point.X + X + X + X, point.Y + Y + Y + Y);
                            }
                            else if (MyField.Cells[point.X - X, point.Y - Y].Value == TypeCell.ShipBody)
                            {
                                typeShips = TypeShips.FourCellShip;
                                points[3] = new Point(point.X + X + X + X, point.Y + Y + Y + Y);
                            }

                            points[2] = new Point(point.X + X + X, point.Y + Y + Y);
                        }
                        else if (MyField.Cells[point.X - X, point.Y - Y].Value == TypeCell.ShipBody)
                        {
                            typeShips = TypeShips.ThreeCellShip;

                            if (MyField.Cells[point.X - X - X, point.Y - Y - Y].Value == TypeCell.ShipBody)
                            {
                                typeShips = TypeShips.FourCellShip;
                                points[3] = new Point(point.X + X + X + X, point.Y + Y + Y + Y);
                            }
                            points[2] = new Point(point.X + X + X, point.Y + Y + Y);
                        }

                        return (typeShips, points);

                    }
                    points[0] = new Point(point.X, point.Y);

                }
            }
            typeShips = TypeShips.OneCellShip;
            return (typeShips, points);
        }

        public int IsShipDead(int allShips, TypeShips typeShips, List<Point> points, Field EnemyField, Field MyField)
        {
            if (EnemyField.Cells[points[0].X, points[0].Y].Value == TypeCell.DamageBody && MyField.Cells[points[0].X, points[0].Y].Value == TypeCell.ShipBody && points[0].X != 0 && points[0].Y != 0)
            {
                if (EnemyField.Cells[points[1].X, points[1].Y].Value == TypeCell.DamageBody && MyField.Cells[points[1].X, points[1].Y].Value == TypeCell.ShipBody && points[1].X != 0 && points[1].Y != 0)
                {
                    if (EnemyField.Cells[points[2].X, points[2].Y].Value == TypeCell.DamageBody && MyField.Cells[points[2].X, points[2].Y].Value == TypeCell.ShipBody && points[2].X != 0 && points[2].Y != 0)
                    {
                        if (EnemyField.Cells[points[3].X, points[3].Y].Value == TypeCell.DamageBody && MyField.Cells[points[3].X, points[3].Y].Value == TypeCell.ShipBody && points[3].X != 0 && points[3].Y != 0)
                        {
                            if (typeShips == TypeShips.FourCellShip)
                            {
                                allShips = ShipIsDead(allShips, typeShips, points, EnemyField);
                            }
                        }
                        else if (typeShips == TypeShips.ThreeCellShip && points[3].X == 0 && points[3].Y == 0)
                        {
                            allShips = ShipIsDead(allShips, typeShips, points, EnemyField);
                        }
                    }
                    else if (typeShips == TypeShips.TwoCellShip && points[2].X == 0 && points[2].Y == 0 && points[3].X == 0 && points[3].Y == 0)
                    {
                        allShips = ShipIsDead(allShips, typeShips, points, EnemyField);
                    }
                }
                else if (typeShips == TypeShips.OneCellShip && points[1].X == 0 && points[1].Y == 0 && points[2].X == 0 && points[2].Y == 0 && points[3].X == 0 && points[3].Y == 0)
                {
                    allShips = ShipIsDead(allShips, typeShips, points, EnemyField);
                }

            }
            return allShips;
        }

        public int ShipIsDead(int allShips, TypeShips typeShips, List<Point> points, Field EnemyField)
        {
            for (int X = -1; X < 2; X++)
            {
                for (int Y = -1; Y < 2; Y++)
                {
                    foreach (var value in points)
                    {
                        if (value.X + X < 1 || value.X + X > 11 || value.Y + Y < 1 || value.Y + Y > 11)
                        { 
                            
                        }
                        else if (EnemyField.Cells[value.X + X, value.Y + Y].Value == TypeCell.Empty && value.X != 0 && value.Y != 0)
                        {
                            EnemyField.Cells[value.X + X, value.Y + Y].Value = TypeCell.MissingBody;
                        }
                    }
                }
            }

            return --allShips;

        }



        public (int, Point) BotMove(int oldPointX, int oldPointY, int allMyShips, Point point, TypeCell botTypeCell, TypeShips botTypeShips, Field botEnemyField, Field playerMyField)
        {
            
            if (botTypeCell == TypeCell.Empty)
            {
                botEnemyField.Cells[point.X, point.Y].Value = TypeCell.MissingBody;
            }
            else if (botTypeCell == TypeCell.ShipBody)
            {
                if (Cells[oldPointX, oldPointY] != Cells[point.X, point.Y])
                {
                    botEnemyField.Cells[point.X, point.Y].Value = TypeCell.DamageBody;


                    allMyShips = CheckShip(allMyShips, point, botTypeCell, botTypeShips, botEnemyField, playerMyField);
                    

                }
                else if (Cells[oldPointX, oldPointY] == Cells[point.X, point.Y]) 
                {
                    ShipDirection shipDirection = ShipDirection.Up;

                    (oldPointX, oldPointY) = ShipDirectionFire(point, shipDirection, botEnemyField, playerMyField);

                    if (oldPointX == 0 && oldPointY == 0)
                    { 
                        return (allMyShips, point);
                    }

                    point = new Point(oldPointX, oldPointY);

                    allMyShips = CheckShip(allMyShips, point, botTypeCell, botTypeShips, botEnemyField, playerMyField);
                    
                }
            }

            return (allMyShips, point);
        }


        public (int, int) ShipDirectionFire(Point point, ShipDirection shipDirection, Field botEnemyField, Field playerMyField) 
        {
            
            shipDirection = (ShipDirection)new Random().Next(1, 5);


            switch (shipDirection)
            {
                case ShipDirection.Up:
                    {
                        if (playerMyField.Cells[point.X - 1, point.Y].Value == TypeCell.ShipBody && playerMyField.Cells[point.X - 1, point.Y].Value != TypeCell.Border)
                        {
                            botEnemyField.Cells[point.X - 1, point.Y].Value = TypeCell.DamageBody;
                            return (point.X - 1, point.Y);
                        }
                        else
                        {
                            botEnemyField.Cells[point.X - 1, point.Y].Value = TypeCell.MissingBody;
                        }
                    }
                    break;
                case ShipDirection.Right:
                    {
                        if (playerMyField.Cells[point.X, point.Y + 1].Value == TypeCell.ShipBody && playerMyField.Cells[point.X, point.Y + 1].Value != TypeCell.Border)
                        {
                            botEnemyField.Cells[point.X, point.Y + 1].Value = TypeCell.DamageBody;
                            return (point.X + 1, point.Y + 1);
                        }
                        else 
                        {
                            botEnemyField.Cells[point.X, point.Y + 1].Value = TypeCell.MissingBody;
                        }
                    }
                    break;
                case ShipDirection.Down:
                    {
                        if (playerMyField.Cells[point.X + 1, point.Y].Value == TypeCell.ShipBody && playerMyField.Cells[point.X + 1, point.Y].Value != TypeCell.Border)
                        {
                            botEnemyField.Cells[point.X + 1, point.Y].Value = TypeCell.DamageBody;
                            return (point.X + 1, point.Y);
                        }
                        else
                        {
                            botEnemyField.Cells[point.X + 1, point.Y].Value = TypeCell.MissingBody;
                        }
                    }
                    break;
                case ShipDirection.Left:
                    {
                        if (playerMyField.Cells[point.X, point.Y - 1].Value == TypeCell.ShipBody && playerMyField.Cells[point.X, point.Y - 1].Value != TypeCell.Border)
                        {
                            botEnemyField.Cells[point.X, point.Y - 1].Value = TypeCell.DamageBody;
                            return (point.X, point.Y - 1);
                        }
                        else
                        {
                            botEnemyField.Cells[point.X, point.Y - 1].Value = TypeCell.MissingBody;
                        }
                    }
                    break;
            }
            return (point.X, point.Y);
        }







        public void ArrangeFieldManually()
        {
            TypeShips type = TypeShips.OneCellShip;
            ShipDirection direction = ShipDirection.Up;
            var point = new Point();
            int oneCellShip = 4;
            int twoCellShip = 3;
            int threeCellShip = 2;
            int fourCellShip = 1;

            int allShips = oneCellShip + twoCellShip + threeCellShip + fourCellShip;

            while (allShips != 0)
            {
                do
                {
                    Console.WriteLine("Виберiть корабель" + "\n" +
                        "1. Однопалубний" + "\n" +
                        "2. Двопалубний" + "\n" +
                        "3. Трипалубний" + "\n" +
                        "4. Чотирьохпалубний");
                    if (System.Enum.TryParse(Console.ReadLine(), out type));
                } while (type != TypeShips.OneCellShip && type != TypeShips.TwoCellShip && type != TypeShips.ThreeCellShip && type != TypeShips.FourCellShip);
                {
                    
                    switch (type)
                    {
                        case TypeShips.OneCellShip:
                            {
                                if (oneCellShip == 0) 
                                {
                                    Console.WriteLine("Ви можете розмістити тільки 4 однопалубних корабля");
                                    continue;
                                }
                            }
                            break;
                        case TypeShips.TwoCellShip:
                            {
                                if (twoCellShip == 0)
                                {
                                    Console.WriteLine("Ви можете розмістити тільки 3 двопалубних корабля");
                                    continue;
                                }
                            }
                            break;
                        case TypeShips.ThreeCellShip:
                            {
                                if (threeCellShip == 0)
                                {
                                    Console.WriteLine("Ви можете розмістити тільки 2 триопалубних корабля");
                                    continue;
                                }
                            } 
                            break;
                        case TypeShips.FourCellShip:
                            {
                                if (fourCellShip == 0)
                                {
                                    Console.WriteLine("Ви можете розмістити тільки 1 чотирипалубних корабля");
                                    continue;
                                }
                            }
                            break;
                    }
                    do
                    {
                        Console.WriteLine("Enter cord x");
                        point.X = int.Parse(Console.ReadLine());
                        
                        Console.WriteLine("Enter cord y");
                        point.Y = int.Parse(Console.ReadLine());

                    } while (point.X < 1 || point.X > 11 || point.Y < 1 || point.Y > 11 ||
                        Cells[point.X, point.Y].Value != TypeCell.Empty || AnyShipsAroundPoint(point));

                    if (type != TypeShips.OneCellShip)
                    {
                        Console.WriteLine("Enter direction" + "\n" +
                            "1. Вгору" + "\n" +
                            "2. Вниз" + "\n" +
                            "3. Вліво" + "\n" +
                            "4. Вправо");
                        System.Enum.TryParse(Console.ReadLine(), out direction);
                        
                        switch (type)
                        {
                            case TypeShips.TwoCellShip:
                                {
                                    if (SetShip(point, direction, type))
                                    {
                                        twoCellShip--;
                                        allShips--;
                                        ShowField();
                                    }
                                }
                                break;
                            case TypeShips.ThreeCellShip:
                                {
                                    if (SetShip(point, direction, type))
                                    {
                                        threeCellShip--;
                                        allShips--;
                                        ShowField();
                                    }
                                }
                                break;
                            case TypeShips.FourCellShip:
                                {
                                    if (SetShip(point, direction, type))
                                    {
                                        fourCellShip--;
                                        allShips--;
                                        ShowField();
                                    }
                                }
                                break;
                            default:
                                break;
                        }
                    }
                    else if (type == TypeShips.OneCellShip)
                    {
                        if (SetShip(point, direction, type))
                        {
                            oneCellShip--;
                            allShips--;
                            ShowField();
                        }
                    }
                    else
                    {
                        Console.WriteLine("Некоректний ввiд");
                    }
                }
            }
        }


        public void ArrangeFieldRandomly()
        {
            TypeShips type = TypeShips.FourCellShip;
            ShipDirection direction = ShipDirection.Up;
            var point = new Point();
            int oneCellShip = 4;
            int twoCellShip = 3;
            int threeCellShip = 2;
            int fourCellShip = 1;

            int allShips = oneCellShip + twoCellShip + threeCellShip + fourCellShip;

            while (allShips != 0)
            {
                switch(type)
                { 
                    case TypeShips.FourCellShip:
                    {
                        if (fourCellShip == 0)
                        {
                            type = TypeShips.ThreeCellShip;
                            
                            }
                        break;
                    }
                    case TypeShips.ThreeCellShip:
                    {
                        if (threeCellShip == 0)
                        {
                            type = TypeShips.TwoCellShip;
                        }
                    }
                        break;
                    case TypeShips.TwoCellShip:
                    {
                        if (twoCellShip == 0)
                        {
                            type = TypeShips.OneCellShip;
                        }
                    }
                        break;
                    case TypeShips.OneCellShip:
                    {
                        if (oneCellShip == 0)
                        {
                            
                        }
                    }
                        break;
                }

                do
                {
                    
                    point.X = _random.Next(1, 11);

                    point.Y = _random.Next(1, 11);

                } while (point.X < 1 || point.X > 11 || point.Y < 1 || point.Y > 11 ||
                        Cells[point.X, point.Y].Value != TypeCell.Empty || AnyShipsAroundPoint(point));


                if (type != TypeShips.OneCellShip)
                {
                    direction = (ShipDirection)new Random().Next(1, 5);

                    switch (type)
                    {
                        case TypeShips.TwoCellShip:
                            {
                                if (SetShip(point, direction, type))
                                {
                                    twoCellShip--;
                                    allShips--;
                                    ShowField();
                                }
                            }
                            break;
                        case TypeShips.ThreeCellShip:
                            {
                                if (SetShip(point, direction, type))
                                {
                                    threeCellShip--;
                                    allShips--;
                                    ShowField();
                                }
                            }
                            break;
                        case TypeShips.FourCellShip:
                            {
                                if (SetShip(point, direction, type))
                                {
                                    fourCellShip--;
                                    allShips--;
                                    ShowField();
                                }
                            }
                            break;
                        default:
                            break;
                    }
                }
                else if (type == TypeShips.OneCellShip)
                {
                    if (SetShip(point, direction, type))
                    {
                        oneCellShip--;
                        allShips--;
                        ShowField();
                    }
                }
                else
                {
                    Console.WriteLine("Некоректний ввiд");
                }

            }
        }


        public bool SetShip(Point point, ShipDirection direction, TypeShips type)
        {
            var ship = CreateShip(point.X, point.Y, direction, type);

            if (ShipIsOutsideField(ship, point, direction))
            {
                Console.WriteLine("Корабель виходить за межi поля");
                return false;
            }
            if (!AnyShipAroundShip(point, ship, direction))
            {
                Console.WriteLine("Довкола є iншi кораблi, будь ласка виберiть iнше мiсце для вашого корабля");
                return false;
            }
            
            foreach (var item in ship.Points)
            {
                for (int x = 0; x < Cells.GetLength(0); x++)
                {
                    for (int y = 0; y < Cells.GetLength(1); y++)
                    {
                        if (x == item.X && y == item.Y)
                        {
                            Cells[x, y].Value = TypeCell.ShipBody;
                        }
                    }
                }
            }
            return true;
        }


        public bool ShipIsOutsideField(Ship ship, Point point, ShipDirection direction)
        {
            switch (direction)
            {
                case ShipDirection.Up:
                    {
                        if (point.X - ship.Points.Count < 0)
                        {
                            return true;
                        }
                    }
                    break;
                case ShipDirection.Down:
                    {
                        if (point.X + ship.Points.Count > 11)
                        {
                            return true;
                        }
                    }
                    break;
                case ShipDirection.Left:
                    {
                        if (point.Y - ship.Points.Count < 0)
                        {
                            return true;
                        }
                    }
                    break;
                case ShipDirection.Right:
                    {
                        if (point.Y + ship.Points.Count > 11)
                        {
                            return true;
                        }
                    }
                    break;
            }
            return false;
        }


        public bool AnyShipAroundShip(Point point, Ship ship, ShipDirection direction)
        {
            int countOperation = 0;
            foreach (var item in ship.Points)
            {
                switch (direction)
                {
                    case ShipDirection.Up:
                    {
                        countOperation++;
                        if (Cells[item.X - 1, item.Y - 1].Value == TypeCell.ShipBody || Cells[item.X - 1, item.Y].Value == TypeCell.ShipBody || Cells[item.X - 1, item.Y + 1].Value == TypeCell.ShipBody ||
                        Cells[item.X, item.Y - 1].Value == TypeCell.ShipBody || Cells[item.X, item.Y + 1].Value == TypeCell.ShipBody ||
                        Cells[item.X + 1, item.Y - 1].Value == TypeCell.ShipBody || Cells[item.X + 1, item.Y + 1].Value == TypeCell.ShipBody)
                        {
                                
                            return false;
                        }
                        else
                        {
                            if (countOperation == ship.Points.Count)
                            return true;
                        }
                    }
                        break;
                    case ShipDirection.Down:
                    {
                        countOperation++;
                        if (Cells[item.X - 1, item.Y - 1].Value == TypeCell.ShipBody || Cells[item.X - 1, item.Y + 1].Value == TypeCell.ShipBody ||
                        Cells[item.X, item.Y - 1].Value == TypeCell.ShipBody || Cells[item.X, item.Y + 1].Value == TypeCell.ShipBody ||
                        Cells[item.X + 1, item.Y - 1].Value == TypeCell.ShipBody || Cells[item.X + 1, item.Y].Value == TypeCell.ShipBody || Cells[item.X + 1, item.Y + 1].Value == TypeCell.ShipBody)
                        {
                            return false;
                        }
                        else
                        {
                            if (countOperation == ship.Points.Count)
                                return true;
                        }
                    }
                        break;
                    case ShipDirection.Left:
                    {
                        countOperation++;
                        if (Cells[item.X - 1, item.Y - 1].Value == TypeCell.ShipBody || Cells[item.X - 1, item.Y].Value == TypeCell.ShipBody || Cells[item.X - 1, item.Y + 1].Value == TypeCell.ShipBody ||
                        Cells[item.X, item.Y - 1].Value == TypeCell.ShipBody ||
                        Cells[item.X + 1, item.Y - 1].Value == TypeCell.ShipBody || Cells[item.X + 1, item.Y].Value == TypeCell.ShipBody || Cells[item.X + 1, item.Y + 1].Value == TypeCell.ShipBody)
                        {
                            return false;
                        }
                        else
                        {
                            if (countOperation == ship.Points.Count)
                                return true;
                        }
                    }
                        break;
                    case ShipDirection.Right:
                    {
                        countOperation++;
                        if (Cells[item.X - 1, item.Y - 1].Value == TypeCell.ShipBody || Cells[item.X - 1, item.Y].Value == TypeCell.ShipBody || Cells[item.X - 1, item.Y + 1].Value == TypeCell.ShipBody ||
                        Cells[item.X, item.Y + 1].Value == TypeCell.ShipBody ||
                        Cells[item.X + 1, item.Y - 1].Value == TypeCell.ShipBody || Cells[item.X + 1, item.Y].Value == TypeCell.ShipBody || Cells[item.X + 1, item.Y + 1].Value == TypeCell.ShipBody)
                        {
                            return false;
                        }
                        else
                        {
                            if (countOperation == ship.Points.Count)
                                return true;
                        }
                    }
                    break;
                }
            }
            return true;
        }


        public Ship CreateShip(int x, int y, ShipDirection direction, TypeShips type)
        {
            Ship newShip = new Ship(type);
            switch (direction)
            {
                case ShipDirection.Up:
                    {
                        for (int i = 0; i < (int)type; i++)
                        {
                            newShip.Points.Add(new Point(x, y));
                            x--;
                        }
                    }
                    break;
                case ShipDirection.Down:
                    {
                        for (int i = 0; i < (int)type; i++)
                        {
                            newShip.Points.Add(new Point(x, y));
                            x++;
                        }
                    }
                    break;
                case ShipDirection.Left:
                    {
                        for (int i = 0; i < (int)type; i++)
                        {
                            newShip.Points.Add(new Point(x, y));
                            y--;
                        }
                    }
                    break;
                case ShipDirection.Right:
                    {
                        for (int i = 0; i < (int)type; i++)
                        {
                            newShip.Points.Add(new Point(x, y));
                            y++;
                        }
                    }
                    break;
                default:
                    break;
            }

            return newShip;
        }


        public bool AnyShipsAroundPoint(Point point) 
        {
            return Cells[point.X - 1, point.Y - 1].Value == TypeCell.ShipBody || Cells[point.X - 1, point.Y].Value == TypeCell.ShipBody || Cells[point.X - 1, point.Y + 1].Value == TypeCell.ShipBody ||
                    Cells[point.X, point.Y - 1].Value == TypeCell.ShipBody || Cells[point.X, point.Y + 1].Value == TypeCell.ShipBody ||
                    Cells[point.X + 1, point.Y - 1].Value == TypeCell.ShipBody || Cells[point.X + 1, point.Y].Value == TypeCell.ShipBody || Cells[point.X + 1, point.Y + 1].Value == TypeCell.ShipBody;
        }


    }
}
