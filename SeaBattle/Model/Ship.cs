using SeaBattle.Model.Enum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SeaBattle.Model
{
    public class Ship
    {
        public TypeShips Type { get; set; }
        public List<Point> Points { get; set; }
        public Ship(TypeShips type)
        {
            Type = type;
            Points = new List<Point>();
        }
    }
}
