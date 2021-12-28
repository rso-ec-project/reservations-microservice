namespace Reservations.Application.ChargingStationsMicroService.Chargers
{
    public class ChargerDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public double ChargingFeePerKwh { get; set; }
        public string ModelName { get; set; }
        public string Manufacturer { get; set; }
        public string ChargingStationName { get; set; }
        public string Address { get; set; }
    }
}
