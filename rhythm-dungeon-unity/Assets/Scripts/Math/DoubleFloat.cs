using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[Serializable]
public class DoubleFloat
{
    public float a, b;

    public DoubleFloat(float _a, float _b)
    {
        a = _a; b = _b;
    }

    public float Random()
    {
		if (a <= b) return UnityEngine.Random.Range(a, b);
		else return UnityEngine.Random.Range(b, a);
    }

    public static DoubleFloat operator * (DoubleFloat df, float factor)
    {
        return new DoubleFloat(df.a * factor, df.b * factor);
    }
}