﻿using System;

namespace Wichtel {
public struct circularInt
{
    private int value;

    private int minValue;
    private int maxValue; //inclusive

    public circularInt(int _value, Range<int> _range)
    {
        minValue = _range.Minimum;
        maxValue = _range.Maximum;
        value = _value;
    }
    
    public circularInt(int _value, int _maxValue)
    {
        minValue = 0;
        maxValue = _maxValue;
        value = _value;
    }

    public circularInt(int _value, int _minValue, int _maxValue)
    {
        minValue = _minValue;
        maxValue = _maxValue;
        value = _value;
    }

    public override string ToString() => value.ToString();

    public static circularInt operator +(circularInt a, int b)
    {
        int range = (a.maxValue + 1 - a.minValue);
        b %= range;
        int value = a.value - a.minValue;
        
        int additionResult = (value+b) % range;
        
        int circularValue = additionResult + a.minValue;
        return new circularInt(circularValue, a.minValue, a.maxValue);
    }
    
    public static circularInt operator -(circularInt a, int b)
    {
        int range = (a.maxValue + 1 - a.minValue);
        b %= range;
        int value = a.value - a.minValue;
        
        int subtractionResult = (value+range-b) % range;

        int circularValue = subtractionResult + a.minValue;
        return new circularInt(circularValue, a.minValue, a.maxValue);
    }

    public static circularInt operator +(int b, circularInt a) => a + b;
    public static circularInt operator -(int b, circularInt a) => a - b;

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
}
}