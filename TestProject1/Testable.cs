using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace TestProject1
{
    public class Testable<T> where T : class
    {
        public T Instance { get; private set; }

        public Dictionary<Type, Object> Dependancies { get; private set; }

        public Testable() : this(null) { }

        public Testable(params object[] dependancies)
        {
            // Protects against null errors
            this.Dependancies = new Dictionary<Type, object>();

            // Inject dependacies for any constructors
            var ctor = GetConstructor();
            var objects = CreateInstancesOfConstructorParameters(ctor.GetParameters(), dependancies ?? new object[0]);
            Instance = (T)ctor.Invoke(objects.ToArray());

            // Inject any dependancies for any property dependancies
            // Not done yet
            //var properties = typeof(T).GetProperties(BindingFlags.Public & BindingFlags.NonPublic);
        }

        private static ConstructorInfo GetConstructor()
        {
            // Gets the constructor with the most parameters
            var ctor = typeof(T).GetConstructors().Where(c => c.GetParameters().Count() > 0)
                                                  .OrderByDescending(c => c.GetParameters().Count())
                                                  .FirstOrDefault();
            return ctor ?? typeof(T).GetConstructors().First();
        }

        private IEnumerable<object> CreateInstancesOfConstructorParameters(ParameterInfo[] parameters, object[] dependancies)
        {
            foreach (var param in parameters)
            {
                // checks the supplied dependancies array to see if the type was already provided and returns it
                Func<Object, string, bool> InterfaceImplimentedCheck = (d, interfaceName) =>
                {
                    return d.GetType().GetInterface(interfaceName) != null;
                };
                var dependancy = dependancies.Where(d => InterfaceImplimentedCheck(d, param.ParameterType.Name) || InterfaceImplimentedCheck(d, param.ParameterType.FullName)).FirstOrDefault();
                if (dependancy != null)
                {
                    yield return dependancy;
                    this.Dependancies.Add(param.ParameterType, dependancy);
                    continue;
                }


                if (param.ParameterType.IsAbstract | param.ParameterType.IsInterface)
                {
                    // Creates a Mock<T> proxy object and adds it to the returning array.
                    var mockType = typeof(Moq.Mock<>);
                    var objType = mockType.MakeGenericType(new Type[] { param.ParameterType });
                    dynamic obj = Activator.CreateInstance(objType);
                    this.Dependancies.Add(param.ParameterType, obj.Object);
                    yield return obj.Object;
                }
                else
                {
                    // Creates the concrete object and adds it to the returning array.
                    dynamic obj = Activator.CreateInstance(param.ParameterType);
                    this.Dependancies.Add(param.ParameterType, obj);
                    yield return obj;
                }
            }
        }
    }
}
