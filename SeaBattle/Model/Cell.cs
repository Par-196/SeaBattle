using SeaBattle.Model.Enum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SeaBattle.Model
{
    public class Cell
    {
        public TypeCell Value { get; set; }

        public Cell(TypeCell type)
        {
            Value = type;
        }

        public override string ToString()
        {
            switch (Value)
            {
                case TypeCell.Empty:
                    {
                        return " ";
                    }
                case TypeCell.ShipBody:
                    {
                        return "O";
                    }
                case TypeCell.DamageBody:
                    {
                        return "X";
                    }
                case TypeCell.Border:
                    {
                        return "#";
                    }
                case TypeCell.MissingBody:
                    {
                        return "*";
                    }
            }
            return "Error";
        }

    }
}