using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SeaBattle.Model
{
    public abstract class User
    {
        public string Name { get; set; }
        public Field MyField { get; set; }
        public Field EnemyField { get; set; }

        public User(string name)
        {
            Name = name;
            MyField = new Field();
            EnemyField = new Field();
        }
    }
}
