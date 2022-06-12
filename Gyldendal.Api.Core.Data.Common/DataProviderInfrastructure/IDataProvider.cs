namespace Gyldendal.Api.CoreData.Common.DataProviderInfrastructure
{
    public interface IDataProvider<out T, in TU>
    {
        T Get(TU request);
    }
}