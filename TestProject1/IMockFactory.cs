using System;

namespace TestProject1
{
    public interface IMockFactory
    {
        Object CreateMockObject(Type type);
    }
}
