using System;
using System.ComponentModel;

namespace Wichtel {
[TypeConverter(typeof(circularIntConverter))]
public struct circularInt
{
    public int value;

    private int minValue;
    private int maxValue;
}

public class circularIntConverter
{
    
}
}