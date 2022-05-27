namespace Diplomka.Models
{
    public class Car
    {
        public int CarID { get; set; }
        public string Name { get; set; }
        public string PlateNumber { get; set; }
        public string Status { get; set; }

        public int? DepotID { get; set; }
        public Depot Depot { get; set; }
    }
}
