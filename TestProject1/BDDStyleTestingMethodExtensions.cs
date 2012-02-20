using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace System
{
    public static class BDDStyleTestingMethodExtensions
    {

        /// <summary>
        /// Tests if the object or value is null.
        /// </summary>
        /// <param name="context"></param>
        /// <param name="error"></param>
        public static void ShouldBeNull(this Object context, String error = null)
        {
            if (error == null) Assert.IsNull(context);
            else Assert.IsNull(context, error);
        }

        /// <summary>
        /// Tests if the object or value is not null.
        /// </summary>
        /// <param name="context"></param>
        /// <param name="error"></param>
        public static void ShouldNotBeNull(this Object context, String error = null)
        {
            if (error == null) Assert.IsNotNull(context);
            else Assert.IsNotNull(context, error);
        }

        /// <summary>
        /// Tests is the object or value equals another.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="context"></param>
        /// <param name="objectToCompare"></param>
        /// <param name="error"></param>
        public static void ShouldEqual<T>(this T context, T objectToCompare, String error = null)
        {
            if (error == null) Assert.AreEqual<T>(objectToCompare, context);
            else Assert.AreEqual<T>(objectToCompare, context, error);
        }

        /// <summary>
        /// Tests if the object or value does not equal another.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="context"></param>
        /// <param name="objectToCompare"></param>
        /// <param name="error"></param>
        public static void ShouldNotEqual<T>(this T context, T objectToCompare, String error = null)
        {
            if (error == null) Assert.AreNotEqual<T>(objectToCompare, context);
            else Assert.AreNotEqual<T>(objectToCompare, context, error);
        }

        /// <summary>
        /// Tests if the value is true.
        /// </summary>
        /// <param name="context"></param>
        /// <param name="error"></param>
        public static void ShouldBeTrue<T>(this T context, String error = null)
        {
            var value = Convert.ToBoolean(context);
            if (error == null) Assert.IsTrue(value);
            else Assert.IsTrue(value, error);
        }

        /// <summary>
        /// Tests if the value is not true.
        /// </summary>
        /// <param name="context"></param>
        /// <param name="error"></param>
        public static void ShouldNotBeTrue<T>(this T context, String error = null)
        {
            var value = Convert.ToBoolean(context);
            if (error == null) Assert.IsFalse(value);
            else Assert.IsFalse(value, error);
        }

        /// <summary>
        /// Tests if the value contains a string fragment.
        /// </summary>
        /// <param name="context"></param>
        /// <param name="text"></param>
        /// <param name="error"></param>
        public static void ShouldContain(this String context, String text, String error = null)
        {
            if (error == null) Assert.IsTrue(context.Contains(text));
            else Assert.IsTrue(context.Contains(text), error);
        }

        public static void ShouldContain<T>(this IEnumerable<T> context, T item, string error = null)
        {
            var isFound = false;
            foreach (var itm in context)
            {
                if (itm.Equals(item))
                    isFound = true;
                if (isFound)
                    break;
            }
            Assert.IsTrue(isFound);
        }

        /// <summary>
        /// Tests is the value does not contain a string fragment.
        /// </summary>
        /// <param name="context"></param>
        /// <param name="text"></param>
        /// <param name="error"></param>
        public static void ShouldNotContain(this String context, String text, String error = null)
        {
            if (error == null) Assert.IsFalse(context.Contains(text));
            else Assert.IsFalse(context.Contains(text), error);
        }
    }
}
