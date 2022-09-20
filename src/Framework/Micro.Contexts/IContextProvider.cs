namespace Micro.Contexts;

public interface IContextProvider
{
    IContext Current();
}