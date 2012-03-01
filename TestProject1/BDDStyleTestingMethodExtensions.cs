using System.Diagnostics;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace System
{
    public static class BDDStyleTestingMethodExtensions
    {

        /// <summary>
        /// Tests if the object or value is null.
        /// </summary>
        /// <param name="context"></param>
        /// <param name="message"></param>
        [DebuggerNonUserCode]
        public static void ShouldBeNull(this Object context, String message = null)
        {
            if (message == null) Assert.IsNull(context);
            else Assert.IsNull(context, message);
        }

        /// <summary>
        /// Tests if the object or value is not null.
        /// </summary>
        /// <param name="context"></param>
        /// <param name="message"></param>
        [DebuggerNonUserCode]
        public static void ShouldNotBeNull(this Object context, String message = null)
        {
            if (message == null) Assert.IsNotNull(context);
            else Assert.IsNotNull(context, message);
        }

        /// <summary>
        /// Tests is the object or value equals another.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="context"></param>
        /// <param name="objectToCompare"></param>
        /// <param name="message"></param>
        [DebuggerNonUserCode]
        public static void ShouldEqual<T>(this T context, T objectToCompare, String message = null)
        {
            if (message == null) Assert.AreEqual<T>(objectToCompare, context);
            else Assert.AreEqual<T>(objectToCompare, context, message);
        }

        /// <summary>
        /// Tests if the object or value does not equal another.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="context"></param>
        /// <param name="objectToCompare"></param>
        /// <param name="message"></param>
        [DebuggerNonUserCode]
        public static void ShouldNotEqual<T>(this T context, T objectToCompare, String message = null)
        {
            if (message == null) Assert.AreNotEqual<T>(objectToCompare, context);
            else Assert.AreNotEqual<T>(objectToCompare, context, message);
        }

        /// <summary>
        /// Tests if the value is true.
        /// </summary>
        /// <param name="context"></param>
        /// <param name="message"></param>
        [DebuggerNonUserCode]
        public static void ShouldBeTrue<T>(this T context, String message = null)
        {
            var value = Convert.ToBoolean(context);
            if (message == null) Assert.IsTrue(value);
            else Assert.IsTrue(value, message);
        }

        /// <summary>
        /// Tests if the value is not true.
        /// </summary>
        /// <param name="context"></param>
        /// <param name="message"></param>
        [DebuggerNonUserCode]
        public static void ShouldNotBeTrue<T>(this T context, String message = null)
        {
            var value = Convert.ToBoolean(context);
            if (message == null) Assert.IsFalse(value);
            else Assert.IsFalse(value, message);
        }

        /// <summary>
        /// Tests if the value contains a string fragment.
        /// </summary>
        /// <param name="context"></param>
        /// <param name="text"></param>
        /// <param name="message"></param>
        [DebuggerNonUserCode]
        public static void ShouldContain(this String context, String text, String message = null)
        {
            if (message == null) Assert.IsTrue(context.Contains(text));
            else Assert.IsTrue(context.Contains(text), message);
        }

        /// <summary>
        /// Tests is the value does not contain a string fragment.
        /// </summary>
        /// <param name="context"></param>
        /// <param name="text"></param>
        /// <param name="message"></param>
        [DebuggerNonUserCode]
        public static void ShouldNotContain(this String context, String text, String message = null)
        {
            if (message == null) Assert.IsFalse(context.Contains(text));
            else Assert.IsFalse(context.Contains(text), message);
        }

        /// <summary>
        /// Checks if the object is of requested type.
        /// </summary>
        /// <param name="context"></param>
        /// <param name="type"></param>
        /// <param name="message"></param>
        [DebuggerNonUserCode]
        public static void ShouldBeInstanceOf(this object context, object type, String message = null)
        {
            var t = type as Type;
            Assert.IsInstanceOfType(context, t, message, t);
        }

        /// <summary>
        /// Tests that the integer is greater than the minimum value.
        /// </summary>
        /// <param name="context"></param>
        /// <param name="minimum"></param>
        /// <param name="message"></param>
        [DebuggerNonUserCode]
        public static void ShouldBeGreaterThan(this int context, int minimum, String message = null)
        {
            Assert.IsTrue(context > minimum, message);
        }

        /// <summary>
        /// Tests that the integer is less than the maximum value.
        /// </summary>
        /// <param name="context"></param>
        /// <param name="minimum"></param>
        /// <param name="message"></param>
        [DebuggerNonUserCode]
        public static void ShouldBeLessThan(this int context, int maximum, String message = null)
        {
            Assert.IsTrue(context < maximum, message);
        }

    }

    public static class It
    {
        public static void ShouldThrow<T>(Action operation, String message = null)
        {
            try
            {
                operation.Invoke();
            }
            catch (Exception ex)
            {
                Assert.IsInstanceOfType(ex, typeof(T), message);
            }
        }
    }
}
