﻿using System;

namespace AequusRemake.Core.Util.Helpers;

public sealed class ArrayHelper {
    /// <returns>An array with the Length of <paramref name="length"/>, populated using <paramref name="dataFactory"/> where <see cref="int"/> is the index of the array being populated and <typeparamref name="T"/> is the value stored.</returns>
    public static T[] PopulateNewArray<T>(Func<int, T> dataFactory, int length) {
        var arr = new T[length];
        for (int i = 0; i < length; i++) {
            arr[i] = dataFactory(i);
        }
        return arr;
    }

    /// <summary>Safely adds <paramref name="other"/>'s contents into <paramref name="array"/> by resizing and inserting the elements from <paramref name="other"/> into <paramref name="array"/>.</summary>
    /// <returns>The combined array.</returns>
    public static T[] AddRangeSafe<T>(T[] array, T[] other) {
        if (other == null) {
            //throw new ArgumentNullException(nameof(other));
            return array;
        }

        if (array == null) {
            return ArrayExtensions.NewClone(other);
        }

        int start = array.Length;
        Array.Resize(ref array, array.Length + other.Length);

        int i = start;
        int k = 0;
        while (i < array.Length) {
            array[i] = other[k];
            i++;
            k++;
        }

        return array;
    }
}
