namespace Caker.Dto
{
    public class ConfectionerSettingsDTO
    {
        public double MinDiameter { get; set; }
        public double MaxDiameter { get; set; }
        public int MinETA { get; set; }
        public int MaxETA { get; set; }
        public List<string> Fillings { get; set; }
        public bool DoImages { get; set; }
        public bool DoShapes { get; set; }
    }
}
