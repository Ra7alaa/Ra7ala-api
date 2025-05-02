namespace Infrastructure.Data.DataSeed
{
    internal class StationSeedDto
    {
        public string Name { get; set; } = string.Empty;
        public decimal Latitude { get; set; }
        public decimal Longitude { get; set; }
        public int CityId { get; set; }
        public int? CompanyId { get; set; }
    }
}