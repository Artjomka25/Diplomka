namespace Diplomka.Models
{
    public class CargoRemnant  //остатки груза на складе
    {
        public int CargoRemnantID { get; set; }

        public int? WarehouseID { get; set; }
        public Warehouse Warehouse { get; set; }

        public int? GrainID { get; set; }
        public Grain Grain { get; set; }

        public int Volume { get; set; }
    }
}
