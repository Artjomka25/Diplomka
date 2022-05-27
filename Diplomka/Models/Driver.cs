namespace Diplomka.Models
{
    public class Driver
    {
        public int DriverID { get; set; }
        public string FIO { get; set; }
        public string PhoneNumber { get; set; }

        public int? CarID { get; set; }
        public Car Car { get; set; }
    }
}
