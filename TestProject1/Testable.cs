using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace TestProject1
{
    public class Testable<T> where T : class
    {
        public T Instance { get; set; }

        public Testable() : this(null) { }

        public Testable(params object[] dependancies)
        {
            // Inject dependacies for any constructors
            var ctor = GetConstructorContainingParameters();
            var objects = CreateInstancesOfConstructorParameters(ctor.GetParameters(), dependancies ?? new object[0]);
            Instance = (T)ctor.Invoke(objects.ToArray());

            // Inject any dependancies for any property dependancies
            // Not done yet
            //var properties = typeof(T).GetProperties(BindingFlags.Public & BindingFlags.NonPublic);
        }

        private static ConstructorInfo GetConstructorContainingParameters()
        {
            // Gets the constructor with the most parameters
            var ctor = typeof(T).GetConstructors().Where(c => c.GetParameters().Count() > 0)
                                                  .OrderByDescending(c => c.GetParameters().Count())
                                                  .FirstOrDefault();
            return ctor;
        }

        private static IEnumerable<object> CreateInstancesOfConstructorParameters(ParameterInfo[] parameters, object[] dependancies)
        {
            foreach (var param in parameters)
            {
                // checks the supplied dependancies array to see if the type was already provided and returns it
                var dependancy = dependancies.Where(d => d.GetType().GetInterface(param.ParameterType.FullName) != null).FirstOrDefault();
                if (dependancy != null) { yield return dependancy; continue; }


                if (param.ParameterType.IsAbstract | param.ParameterType.IsInterface)
                {
                    // Creates a Mock<T> proxy object and adds it to the returning array.
                    var mockType = typeof(Moq.Mock<>);
                    var objType = mockType.MakeGenericType(new Type[] { param.ParameterType });
                    dynamic obj = Activator.CreateInstance(objType);
                    yield return obj.Object;
                }
                else
                {
                    // Creates the concrete object and adds it to the returning array.
                    dynamic obj = Activator.CreateInstance(param.ParameterType);
                    yield return obj;
                }
            }
        }
    }
}
