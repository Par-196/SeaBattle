using SeaBattle.Model.Enum;
using System;
using System.Collections.Generic;
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
            Cells = new Cell[12, 12];
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
                    Console.Write(Cells[i ,j]);
                }
                Console.WriteLine();
            }
        }

        public void ArrangeFieldManually()
        {
            TypeShips type = TypeShips.OneCellShip;
            ShipDirection direction = ShipDirection.Up;
            var point = new Point();
            int oneDeckShips = 4;
            int twoDeckShips = 3;
            int threeDeckShips = 2;
            int fourDeckShips = 1;

            int allShips = oneDeckShips + twoDeckShips + threeDeckShips + fourDeckShips;

            while(allShips != 0) 
            {
                Console.WriteLine("Виберiть корабель" + "\n" +
                    "1. Однопалубний" + "\n" +
                    "2. Двопалубний" + "\n" +
                    "3. Трипалубний" + "\n" +
                    "4. Чотирьохпалубний");

                if (oneDeckShips == 0 && twoDeckShips == 0 || threeDeckShips == 0 || fourDeckShips == 0)
                {
                    Console.WriteLine("Ви не можете бiльше розмiстити кораблi даного типу");
                    break;
                }


                if (System.Enum.TryParse(Console.ReadLine(), out  type))
                {
                    do
                    {
                        Console.WriteLine("Enter cord x");
                        point.X = int.Parse(Console.ReadLine());

                        Console.WriteLine("Enter cord y");
                        point.Y = int.Parse(Console.ReadLine());

                    } while (point.X < 1 || point.X > 11 || point.Y < 1 || point.Y > 11 ||
                        Cells[point.X, point.Y].Value != TypeCell.Empty || AnyShipsAround(point));

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
                                    SetShip(point, direction, type);
                                    twoDeckShips--;
                                    ShowField();
                                }
                                break;
                            case TypeShips.ThreeCellShip:
                                {
                                    SetShip(point, direction, type);
                                    threeDeckShips--;
                                    ShowField();
                                }
                                break;
                            case TypeShips.FourCellShip:
                                {
                                    SetShip(point, direction, type);
                                    fourDeckShips--;
                                    ShowField();
                                }
                                break;
                            default:
                                break;
                        }
                    }
                    else if (type == TypeShips.OneCellShip)
                    {
                        SetShip(point, direction, type);
                        oneDeckShips--;
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


        public void SetShip(Point point, ShipDirection direction, TypeShips type)
        {
            var ship = CreateShip(point.X, point.Y, direction, type);


            foreach (var item in ship.Points)
            { 
                for (int x = 0; x < Cells.GetLength(0); x++)
                { 
                    for (int y = 0; y < Cells.GetLength(1); y++)
                    {
                        if (x == item.X  && y == item.Y)
                        {
                            Cells[x, y].Value = TypeCell.ShipBody;
                        }
                    }
                }
            }

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

        public bool AnyShipsAround(Point point) 
        {
            return  Cells[point.X - 1, point.Y - 1].Value == TypeCell.ShipBody && Cells[point.X - 1, point.Y].Value == TypeCell.ShipBody && Cells[point.X - 1, point.Y + 1].Value == TypeCell.ShipBody &&
                    Cells[point.X, point.Y - 1].Value == TypeCell.ShipBody && Cells[point.X, point.Y + 1].Value == TypeCell.ShipBody &&
                    Cells[point.X + 1, point.Y - 1].Value == TypeCell.ShipBody && Cells[point.X + 1, point.Y].Value == TypeCell.ShipBody && Cells[point.X + 1, point.Y + 1].Value == TypeCell.ShipBody;
        }
    }
}
