namespace Diplomka.Models
{
    public class DistanceReference  //справочник расстояний между пунктами
    {
        public int DistanceReferenceID { get; set; }
        public string TypeFirstPoint { get; set; } // тип начального пункта
        public int ID_FirstPoint { get; set; }
        public string NameFirstPoint { get; set; } // название начального пункта
        public string TypeSecondPoint { get; set; }  // тип конечного пункта
        public int ID_SecondPoint { get; set; }
        public string NameSecondPoint { get; set; } // название конечного пункта
        public int Distance { get; set; }  // расстояние между пунктами
    }
}
