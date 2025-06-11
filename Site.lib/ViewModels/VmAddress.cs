namespace Ore.Lib.ViewModels
{
    public class VmAddress
    {
        public string Id { get; set; }
        public long TypeId { get; set; } = 403;
        public string MasterId { get; set; }
        public string CultureId { get; set; }
        public string Address { get; set; }
        public string Lat { get; set; }
        public string Lng { get; set; }
        public string Country { get; set; }
        public string City { get; set; }
        public string Street { get; set; }
        public long CountryId { get; set; }
        public long GovId { get; set; }
        public long AreaId { get; set; }
        public string Block { get; set; }
        public string House { get; set; }
        public string Building { get; set; }
        public string Floor { get; set; }
        public string ApartmentNo { get; set; }
        public string OfficeNo { get; set; }
        public string Avenue { get; set; }
    }
}
