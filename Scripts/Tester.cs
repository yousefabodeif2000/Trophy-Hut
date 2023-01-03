using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using GameGrid;
public class Tester : MonoBehaviour
{
    [System.Flags]
    public enum Test
    {
        test1 = 1,
        test2 = 2,
        test3 = 3
    }
    public Test test;
    // Start is called before the first frame update
    void Start()
    {
        print("count is " + Utilities.Count(test));

    }
}
