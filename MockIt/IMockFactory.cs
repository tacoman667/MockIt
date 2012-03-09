using System;

namespace MockIt
{
    public interface IMockFactory
    {
        Object CreateMockObject(Type type);
    }
}
