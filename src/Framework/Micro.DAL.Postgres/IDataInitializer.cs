namespace Micro.DAL.Postgres;

public interface IDataInitializer
{
    Task InitAsync();
}