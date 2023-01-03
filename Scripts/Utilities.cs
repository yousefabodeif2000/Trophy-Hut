using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
namespace GameGrid
{
    public static class Utilities
    {
        internal static int Count(this Tester.Test types)
        {
            UInt32 v = (UInt32)types;
            v = v - ((v >> 1) & 0x55555555); // reuse input as temporary
            v = (v & 0x33333333) + ((v >> 2) & 0x33333333); // temp
            UInt32 c = ((v + (v >> 4) & 0xF0F0F0F) * 0x1010101) >> 24; // count
            return Convert.ToInt32(v);
        }
    }
}
