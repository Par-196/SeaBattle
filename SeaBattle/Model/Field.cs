using SeaBattle.Model.Enum;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics.Eventing.Reader;
using System.Linq;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;

namespace SeaBattle.Model
{
    public class Field
    {
        public Cell[,] Cells { get; set; }



        public Field() 
        {
            //todo: add boarders
            Cells = new Cell[13, 13];
            for (int i = 0; i < Cells.GetLength(0); i++)
            {
                for (int j = 0; j < Cells.GetLength(0); j++)
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
            for (int i = 0; i < Cells.GetLength(0); i++)
            {
                for (int j = 0; j < Cells.GetLength(0); j++)
                {
                    Console.Write($" {Cells[i, j]}");
                }
                Console.WriteLine();
            }
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
                                    SetShip(point, direction, type, oneCellShip, twoCellShip, threeCellShip, fourCellShip);
                                    ShowField();
                                }
                                break;
                            case TypeShips.ThreeCellShip:
                                {
                                    SetShip(point, direction, type, oneCellShip, twoCellShip, threeCellShip, fourCellShip);
                                    ShowField();
                                }
                                break;
                            case TypeShips.FourCellShip:
                                {
                                    SetShip(point, direction, type, oneCellShip, twoCellShip, threeCellShip, fourCellShip);
                                    ShowField();
                                }
                                break;
                            default:
                                break;
                        }
                    }
                    else if (type == TypeShips.OneCellShip)
                    {
                        SetShip(point, direction, type, oneCellShip, twoCellShip, threeCellShip, fourCellShip);
                        ShowField();
                    }
                    else
                    {
                        Console.WriteLine("Некоректний ввiд");
                    }
                }
            }
        }


        public void ArrangeFieldRandomly ()
        {
            
        }


        public void SetShip(Point point, ShipDirection direction, TypeShips type, int oneCellShip, int twoCellShip, int threeCellShip, int fourCellShip)
        {
            var ship = CreateShip(point.X, point.Y, direction, type);

            if (ShipIsOutsideField(ship, point, direction))
            {
                Console.WriteLine("Корабель виходить за межi поля");
                return;
            }
            if (!AnyShipAroundShip(point, ship, direction))
            {
                Console.WriteLine("Довкола є iншi кораблi, будь ласка виберiть iнше мiсце для вашого корабля");
                return;
            }
            switch (type)
            {
                case TypeShips.OneCellShip:
                    {
                        oneCellShip--;
                    }
                    break;
                case TypeShips.TwoCellShip:
                    {
                        twoCellShip--;
                    }
                    break;
                case TypeShips.ThreeCellShip:
                    {
                        threeCellShip--;
                    }
                    break;
                case TypeShips.FourCellShip:
                    {
                        fourCellShip--;
                    }
                    break;
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
        }

        public bool ShipIsOutsideField(Ship ship, Point point, ShipDirection direction)
        {
            switch (direction)
            {
                case ShipDirection.Up:
                    {
                        if (point.X < 1 || point.Y - ship.Points.Count < 0)
                        {
                            return true;
                        }
                    }
                    break;
                case ShipDirection.Down:
                    {
                        if (point.X > 12 || point.Y + ship.Points.Count > 11)
                        {
                            return true;
                        }
                    }
                    break;
                case ShipDirection.Left:
                    {
                        if (point.X < 1 || point.Y - ship.Points.Count < 0)
                        {
                            return true;
                        }
                    }
                    break;
                case ShipDirection.Right:
                    {
                        if (point.X > 12 || point.Y + ship.Points.Count > 11)
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
            return Cells[point.X - 1, point.Y - 1].Value == TypeCell.ShipBody && Cells[point.X - 1, point.Y].Value == TypeCell.ShipBody &&  Cells[point.X - 1, point.Y + 1].Value == TypeCell.ShipBody &&
                    Cells[point.X, point.Y - 1].Value == TypeCell.ShipBody && Cells[point.X, point.Y + 1].Value == TypeCell.ShipBody &&
                    Cells[point.X + 1, point.Y - 1].Value == TypeCell.ShipBody && Cells[point.X + 1, point.Y].Value == TypeCell.ShipBody && Cells[point.X + 1, point.Y + 1].Value == TypeCell.ShipBody;
        }


    }
}
