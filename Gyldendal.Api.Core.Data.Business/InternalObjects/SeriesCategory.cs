namespace Gyldendal.Api.CoreData.Business.InternalObjects
{
    internal class SeriesCategory
    {
        public int CategoryId { get; set; }

        public string Name { get; set; }

        public short? Niveau { get; set; }

        public int SerieId { get; set; }
    }
}
