using System;
using UnityEngine;
using System.Collections.Generic;

namespace GameExtensions
{
    public static class ArrayExtensions {
        public static T[] InitializeElements<T>(this T[] arr) where T : new() {
            for (int i = 0; i < arr.Length;i++) {
                arr[i] = new T();
            }

            return arr;
        }

        public static T[,] InitializeElements<T>(this T[,] arr) where T : new() {
            for (int i = 0; i < arr.GetLength(0); i++) {
                for (int j = 0; j < arr.GetLength(1); j++) {
                    arr[i, j] = new T();
                }
            }

            return arr;
        }

        public static T Last<T>(this T[] arr) where T : new()
        {
            if (arr.Length == 0) {
                return default(T);
            } else {
                return arr[arr.Length - 1];
            }
        }
    }

}