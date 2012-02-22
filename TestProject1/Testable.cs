﻿using System;
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
            var constructorInstances = CreateInstancesOfConstructorParameters(ctor.GetParameters(), dependancies ?? new object[0]);
            Instance = (T)ctor.Invoke(constructorInstances.ToArray());

            // Inject any dependancies for any property dependancies
            // Not done yet
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
                var dependancy = GetDependancy(type, dependancies);
                if (dependancy != null)
                {
                    this.Dependancies.Add(type, dependancy);
                    continue;
                }

                if (type.IsAbstract | type.IsInterface)
                {
                    // Creates a Mock<T> proxy object and adds it to the returning array.
                    dynamic obj = CreateMockObjectFromType(type);
                    this.Dependancies.Add(type, obj.Object);
                }
                else
                {
                    // Creates the concrete object and adds it to the returning array.
                    var obj = CreateConcreteObjectFromType(type);
                    this.Dependancies.Add(type, obj);
                }
            }
        }

        /// <summary>
        /// Gets the dependency from the collection supplied in the parameters.
        /// </summary>
        /// <param name="type"></param>
        /// <param name="dependancies"></param>
        /// <returns></returns>
        public static object GetDependancy(Type type, object[] dependancies)
        {
            if (dependancies != null)
            {
                // checks the supplied dependancies array to see if the type was already provided and returns it
                Func<Object, string, bool> InterfaceImplimentedCheck = (d, interfaceName) =>
                {
                    return d.GetType().GetInterface(interfaceName) != null;
                };
                var dependancy = dependancies.Where(d => InterfaceImplimentedCheck(d, type.Name) || InterfaceImplimentedCheck(d, type.FullName)).FirstOrDefault();

                return dependancy;
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
        /// Gets the constructor with the most parameters.
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

        public IEnumerable<Object> CreateInstancesOfConstructorParameters(ParameterInfo[] parameters, object[] dependancies)
        {
            foreach (var param in parameters)
            {
                var type = param.ParameterType;

                // checks the supplied dependancies array to see if the type was already provided and returns it
                var dependancy = GetDependancy(type, dependancies);
                if (dependancy != null)
                {
                    yield return dependancy;
                    this.Dependancies.Add(type, dependancy);
                    continue;
                }

                if (type.IsAbstract | type.IsInterface)
                {
                    // Creates a Mock<T> proxy object and adds it to the returning array.
                    dynamic obj = CreateMockObjectFromType(type);
                    this.Dependancies.Add(type, obj.Object);
                    yield return obj.Object;
                }
                else
                {
                    // Creates the concrete object and adds it to the returning array.
                    var obj = CreateConcreteObjectFromType(type);
                    this.Dependancies.Add(type, obj);
                    yield return obj;
                }
            }
        }

        /// <summary>
        /// Returns a Moq.Mock(Of T) object.
        /// </summary>
        /// <param name="type"></param>
        /// <returns>dynamic</returns>
        public dynamic CreateMockObjectFromType(Type type)
        {
            var mockType = typeof(Moq.Mock<>);
            var objType = mockType.MakeGenericType(new Type[] { type });
            dynamic obj = Activator.CreateInstance(objType);
            return obj;
        }

        /// <summary>
        /// Returns an object from a concrete class.
        /// </summary>
        /// <param name="type"></param>
        /// <returns>dynamic</returns>
        public dynamic CreateConcreteObjectFromType(Type type)
        {
            dynamic obj = Activator.CreateInstance(type);
            return obj;
        }
    }
}
