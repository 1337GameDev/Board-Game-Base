using System;
using UnityEngine;
using System.Collections.Generic;

namespace GameExtensions
{
    public static class ListExtensions
    {
        public static T Last<T>(this List<T> list) where T : new() {
            if(list.Count == 0) {
                return default(T);
            } else {
                return list[list.Count - 1];
            }
        }

    }

}