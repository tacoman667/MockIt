using System;
using MockIt;

namespace TestProject1
{
    public class MoqMockFactory : IMockFactory
    {
        public object CreateMockObject(Type type)
        {
            var mockType = typeof(Moq.Mock<>);
            var objType = mockType.MakeGenericType(new Type[] { type });
            dynamic obj = Activator.CreateInstance(objType);
            return obj;
        }
    }
}
