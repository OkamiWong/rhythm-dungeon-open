using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[Serializable]
public class DoubleInt
{
	public int a, b;

    public DoubleInt(int _a, int _b)
    {
        a = _a; b = _b;
    }

	public int Random()
	{
		if (a <= b) return UnityEngine.Random.Range(a, b);
		else return UnityEngine.Random.Range(b, a);
	}

    public static DoubleInt operator * (DoubleInt di, int factor)
    {
        return new DoubleInt(di.a * factor, di.b * factor);
    }

    public static DoubleInt operator * (DoubleInt di, float factor)
    {
        return new DoubleInt((int)(di.a * factor), (int)(di.b * factor));
    }
}