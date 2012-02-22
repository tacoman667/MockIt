using System;

namespace TestProject1
{
    public interface MockFactory
    {
        Object CreateMockObject(Type type);
    }
}
