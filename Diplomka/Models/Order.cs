using System;
using System.Collections.Generic;

namespace Diplomka.Models
{
    public class Order  //заказы
    {
        public int OrderID { get; set; }
        public int Volume { get; set; }
        public DateTime DeliveryDate { get; set; }
        public string Status { get; set; }
        public int Price { get; set; }

        public int? GrainID { get; set; }
        public Grain Grain { get; set; }

        public int? FactoryID { get; set; }
        public Factory Factory { get; set; }

        public CargoRemnant CargoRemnant { get; set; }

    }
}
