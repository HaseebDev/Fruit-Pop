using System;
using UnityEngine;

namespace DTT.DailyRewards
{
    /// <summary>
    /// A wrapper class for JSON Utility that allows the handling
    /// of arrays of objects.
    /// </summary>
    public static class JsonHelper
    {
        /// <summary>
        /// Convert a json string into a list of items.
        /// </summary>
        /// <param name="json">A json string.</param>
        /// <typeparam name="T">The object type of the array.</typeparam>
        /// <returns>An array of type  <see cref="T"/> objects.</returns>
        public static T[] FromJson<T>(string json)
        {
            Wrapper<T> wrapper = JsonUtility.FromJson<Wrapper<T>>(json);
            return wrapper.items;
        }

        /// <summary>
        /// Converts an array of objects of type <see cref="T"/> into a json string.
        /// </summary>
        /// <param name="array">The array of objects of type <see cref="T"/> </param>
        /// <typeparam name="T">The object type of the array.</typeparam>
        /// <returns>The JSON representation of the original array.</returns>
        public static string ToJson<T>(T[] array)
        {
            Wrapper<T> wrapper = new Wrapper<T>();
            wrapper.items = array;
            return JsonUtility.ToJson(wrapper);
        }

        /// <summary>
        /// Converts an array of json objects of type t into a json string.
        /// </summary>
        /// <param name="array">The array of objects of type T.</param>
        /// <param name="prettyPrint">Whether it should pretty print the objects.</param>
        /// <typeparam name="T">The type of the objects.</typeparam>
        /// <returns>A json string representation of the array.</returns>
        public static string ToJson<T>(T[] array, bool prettyPrint)
        {
            Wrapper<T> wrapper = new Wrapper<T>();
            wrapper.items = array;
            return JsonUtility.ToJson(wrapper, prettyPrint);
        }

        /// <summary>
        /// A serializable list of objects.
        /// </summary>
        /// <typeparam name="T">The type of the object to be serialised.</typeparam>
        [Serializable]
        private class Wrapper<T>
        {
            public T[] items;
        }
    }
}