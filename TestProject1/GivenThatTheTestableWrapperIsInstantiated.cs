﻿using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Web;
using MockIt;

namespace TestProject1
{
    [TestClass]
    public class GivenThatTheTestableWrapperIsInstantiated
    {

        [TestInitialize]
        public void Setup()
        {

        }

        [TestMethod]
        [TestCategory("unit")]
        public void it_should_create_the_object_of_the_class()
        {
            var instance = new Testable<Foo, MoqMockFactory>().Instance;
            instance.ShouldNotBeNull();
        }

        [TestMethod]
        [TestCategory("unit")]
        public void it_should_create_the_object_of_type_Foo()
        {
            var instance = new Testable<Foo, MoqMockFactory>().Instance;
            instance.ShouldBeInstanceOf(typeof(Foo));
        }

        [TestMethod]
        [TestCategory("unit")]
        public void it_should_fill_foo_property_with_a_default_mock_proxy_object()
        {
            var instance = new Testable<Baz, MoqMockFactory>().Instance;
            instance.Foo.ShouldNotBeNull();
            instance.Foo.TestMessage.ShouldBeNull();
        }

        [TestMethod]
        [TestCategory("unit")]
        public void it_should_fill_foo_property_with_a_user_defined_object()
        {
            var foo = new Foo { TestMessage = "Hello World" };
            var instance = new Testable<Baz, MoqMockFactory>(foo).Instance;
            instance.Foo.ShouldNotBeNull();
            instance.Foo.TestMessage.ShouldEqual("Hello World");
        }

        [TestMethod]
        [TestCategory("unit")]
        public void it_should_add_1_object_to_the_dependancies_propery_of_the_wrapper()
        {
            var testableClass = new Testable<Baz, MoqMockFactory>();
            testableClass.Dependancies.Count.ShouldEqual(1);
        }

        [TestMethod]
        [TestCategory("unit")]
        public void it_should_have_a_dependency_of_type_IFoo()
        {
            var foo = new Foo { };
            var testableClass = new Testable<Baz, MoqMockFactory>(foo);
            Assert.IsTrue(testableClass.Dependancies.ContainsKey(typeof(IFoo)));
        }

        [TestMethod]
        [TestCategory("unit")]
        public void it_should_create_mock_for_public_property_dependency()
        {
            var instance = new Testable<FooBar, MoqMockFactory>().Instance;
            instance.Bar.ShouldNotBeNull();
        }

        [TestMethod]
        [TestCategory("unit")]
        public void when_no_ctor_defined_it_should_create_instance()
        {
            var instance = new Testable<FooWithoutCtor, MoqMockFactory>().Instance;
            instance.ShouldNotBeNull();
        }

        [TestMethod]
        [TestCategory("unit")]
        public void when_dependency_of_concrete_type_has_no_parameterless_constructor_it_should_throw_TypeLoadException()
        {
            Action operation = () => new Testable<HttpContext, MoqMockFactory>();
            operation.ShouldThrow<TypeLoadException>();
        }

        public class FooWithoutCtor : IFoo
        {
            public string TestMessage
            {
                get;
                set;
            }
        }

        public interface IFooBar { }
        public class FooBar : IFooBar
        {
            public IBar Bar { get; set; }
        }

        public interface IFoo
        {
            String TestMessage { get; set; }
        }
        public interface IBar { }
        public interface IBaz { }

        public class Baz : IBaz
        {
            public IFoo Foo { get; private set; }

            public Baz(IFoo foo)
            {
                this.Foo = foo;
            }
        }

        public class Bar : IBar { }

        public class Foo : IFoo
        {
            public IBar Bar { get; private set; }
            public String TestMessage { get; set; }
        }
    }
}
