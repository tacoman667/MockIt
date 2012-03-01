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
        public IMockFactory MockFactory { get; private set; }

        public Testable(IMockFactory mockFactory) : this(mockFactory, null) { }

        public Testable(IMockFactory mockFactory, params object[] dependancies)
        {
            this.MockFactory = mockFactory;

            // Protects against null errors
            this.Dependancies = new Dictionary<Type, object>();

            // Inject dependacies for any constructors
            var ctor = GetConstructor();
            var constructorInstances = CreateInstancesOfConstructorParameters(ctor.GetParameters(), dependancies ?? new object[0]);
            this.Instance = (T)ctor.Invoke(constructorInstances.ToArray());

            // Inject any dependancies for any property dependancies
            var properties = GetPropertiesFromType();
            CreateInstancesForProperties(properties, dependancies);
            SetPropertyObjects(properties, this.Dependancies);
        }

        public void SetPropertyObjects(IEnumerable<PropertyInfo> properties, Dictionary<Type, Object> propertyObjects)
        {
            propertyObjects.ToList().ForEach(o =>
            {
                var prop = properties.Where(p => p.PropertyType == o.Key).FirstOrDefault();
                if (prop != null) prop.SetValue(this.Instance, o.Value, null);
            });
        }

        /// <summary>
        /// Creates Moq.Mock(Of T) objects from supplies PropertyInfo collection.
        /// </summary>
        /// <param name="Instance"></param>
        /// <param name="properties"></param>
        public void CreateInstancesForProperties(IEnumerable<PropertyInfo> properties, object[] dependancies)
        {
            foreach (var prop in properties)
            {
                var type = prop.PropertyType;

                // checks the supplied dependancies array to see if the type was already provided and returns it
                var dependency = Getdependency(type, dependancies);
                if (dependency != null)
                {
                    AdddependencyToCollection(type, dependency);
                    continue;
                }

                if (type.IsAbstract | type.IsInterface)
                {
                    // Creates a Mock<T> proxy object and adds it to the returning array.
                    dynamic obj = CreateMockObjectFromType(type);
                    AdddependencyToCollection(type, obj.Object);
                }
                else
                {
                    // Creates the concrete object and adds it to the returning array.
                    var obj = CreateConcreteObjectFromType(type);
                    AdddependencyToCollection(type, obj);
                }
            }
        }

        /// <summary>
        /// Gets the dependency from the collection supplied in the parameters.
        /// </summary>
        /// <param name="type"></param>
        /// <param name="dependancies"></param>
        /// <returns></returns>
        public static object Getdependency(Type type, object[] dependancies)
        {
            if (dependancies != null)
            {
                // checks the supplied dependancies array to see if the type was already provided and returns it
                Func<Object, string, bool> InterfaceImplimentedCheck = (d, interfaceName) =>
                {
                    return d.GetType().GetInterface(interfaceName) != null;
                };
                var dependency = dependancies.Where(d => InterfaceImplimentedCheck(d, type.Name) || InterfaceImplimentedCheck(d, type.FullName)).FirstOrDefault();

                return dependency;
            }
            return null;
        }

        /// <summary>
        /// Gets the public instance properties that are interface or abstract that have yet to be mocked.
        /// </summary>
        /// <returns>IEnumerable<PropertyInfo></returns>
        public IEnumerable<PropertyInfo> GetPropertiesFromType()
        {
            var properties = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance)
                                      .Where(p => p.PropertyType.IsInterface | p.PropertyType.IsAbstract)
                                      .Where(p => p.GetValue(this.Instance, null) == null);
            return properties;
        }

        /// <summary>
        /// Gets the constructor with the most parameters. If only parameterless constructor exists, it will return it.
        /// </summary>
        /// <returns>ConstructorInfo</returns>
        public static ConstructorInfo GetConstructor()
        {
            // Gets the constructor with the most parameters
            var ctor = typeof(T).GetConstructors().Where(c => c.GetParameters().Count() > 0)
                                                  .OrderByDescending(c => c.GetParameters().Count())
                                                  .FirstOrDefault();
            return ctor ?? typeof(T).GetConstructors().First();
        }

        /// <summary>
        /// Creates and returns mock, concrete, or supplied depenency objects based on type.
        /// </summary>
        /// <param name="parameters"></param>
        /// <param name="dependancies"></param>
        /// <returns></returns>
        public IEnumerable<Object> CreateInstancesOfConstructorParameters(ParameterInfo[] parameters, object[] dependancies)
        {
            foreach (var param in parameters)
            {
                var type = param.ParameterType;

                // checks the supplied dependancies array to see if the type was already provided and returns it
                var dependency = Getdependency(type, dependancies);
                if (dependency != null)
                {
                    yield return dependency;
                    AdddependencyToCollection(type, dependency);
                    continue;
                }

                if (type.IsAbstract | type.IsInterface)
                {
                    // Creates a Mock<T> proxy object and adds it to the returning array.
                    dynamic obj = CreateMockObjectFromType(type);
                    AdddependencyToCollection(type, obj.Object);
                    yield return obj.Object;
                }
                else
                {
                    // Creates the concrete object and adds it to the returning array.
                    var obj = CreateConcreteObjectFromType(type);
                    AdddependencyToCollection(type, obj);
                    if (obj != null) yield return obj;
                }
            }
        }

        private void AdddependencyToCollection(Type type, object dependency)
        {
            if (!this.Dependancies.ContainsKey(type) && dependency != null)
                this.Dependancies.Add(type, dependency);
        }

        /// <summary>
        /// Returns a Moq.Mock(Of T) object.
        /// </summary>
        /// <param name="type"></param>
        /// <returns>dynamic</returns>
        public dynamic CreateMockObjectFromType(Type type)
        {
            return MockFactory.CreateMockObject(type);
        }

        /// <summary>
        /// Returns an object from a concrete class.
        /// </summary>
        /// <param name="type"></param>
        /// <returns>dynamic</returns>
        public dynamic CreateConcreteObjectFromType(Type type)
        {
            var ctor = type.GetConstructors().FirstOrDefault(c => c.GetParameters().Count() == 0);
            if (ctor != null)
            {
                return ctor.Invoke(null);
            }
            throw new TypeLoadException("Could not create typeof(" + type + ") because it doesn't have a default constructor.");
        }
    }
}
