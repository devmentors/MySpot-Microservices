using IdGen;

namespace Micro.Identity;

internal sealed class IdentityGenerator : IIdGen
{
    private readonly IdGenerator _idGen;

    public IdentityGenerator(int generatorId = 0)
    {
        _idGen = new IdGenerator(generatorId);
    }

    public long Create() => _idGen.CreateId();
}