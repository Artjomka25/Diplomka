using System.Collections.Generic;

namespace Diplomka.Models
{
    public class Grain  //зерновые культуры
    {
        public int GrainID { get; set; }
        public string Name { get; set; }
        public int Price { get; set; }

        public List<Order> Orders { get; set; }
        public Grain()
        {
            Orders = new List<Order>();
        }

    }
}
