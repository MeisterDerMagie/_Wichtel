﻿using System;
using UnityEngine;

 namespace Wichtel {
public struct circularInt
{
    private int value;

    private int minValue; //inclusive
    private int maxValue; //inclusive

    public circularInt(int _value, Range<int> _range)
    {
        minValue = _range.Minimum;
        maxValue = _range.Maximum;
        CheckForLegalRange(minValue, maxValue);
        
        value = minValue;
        this += (_value - minValue);
    }
    
    public circularInt(int _value, int _maxValue)
    {
        minValue = 0;
        maxValue = _maxValue;
        CheckForLegalRange(minValue, maxValue);
        
        value = minValue;
        this += (_value - minValue);
    }

    public circularInt(int _value, int _minValue, int _maxValue)
    {
        minValue = _minValue;
        maxValue = _maxValue;
        CheckForLegalRange(minValue, maxValue);
        
        value = minValue;
        this += (_value - minValue);
    }

    public override string ToString() => value.ToString();

    public static circularInt operator +(circularInt a, int b)
    {
        int range = (a.maxValue + 1 - a.minValue);
        b %= range;
        int value = a.value - a.minValue;
        
        int result = (value+range+b) % range;
        
        int circularValue = result + a.minValue;
        a.value = circularValue;

        return a;
    }
    
    public static circularInt operator -(circularInt a, int b) => a + -b;

    public static circularInt operator +(int b, circularInt a) => a + b;
    public static int operator -(int b, circularInt a) => b - a.value;

    public static circularInt operator ++(circularInt a) => a + 1;
    public static circularInt operator --(circularInt a) => a - 1;

    public static bool operator ==(circularInt a, circularInt b)
    {
        return ((a.value == b.value) && (a.minValue == b.minValue) && (a.maxValue == b.maxValue));
    }
    public static bool operator !=(circularInt a, circularInt b) => !(a == b);

    public static bool operator ==(circularInt a, int b) => a.value == b;
    public static bool operator !=(circularInt a, int b) => !(a == b);
    
    public static bool operator ==(int b, circularInt a) => a.value == b;
    public static bool operator !=(int b, circularInt a) => !(b == a);

    public static implicit operator int(circularInt _cInt)
    {
        return _cInt.value;
    }

    private static void CheckForLegalRange(int _min, int _max)
    {
        if(_max <= _min) Debug.LogWarning("Illegal range of circularInt! Make sure that the minValue is smaller than the maxValue.");
    }
}
}