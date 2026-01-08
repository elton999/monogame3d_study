using System;
using System.Collections.Generic;
using System.Linq;

namespace _3dAnimation;

public class Skeleton : IFormattable
{
    private Joint[] _joins = new Joint[] { };

    public Joint[] Joins => _joins.ToArray();

    public Skeleton() { }
    
    public Skeleton(List<Joint> joins)
    {
        _joins = joins.ToArray();
    }

    public Skeleton(Joint[] joins)
    {
        _joins = joins;
    }

    public string ToString()
    {
        string result = string.Empty;

        foreach (Joint joint in _joins)
        {
            result += joint.Name + "\n";
        }

        return result;
    }

    public string ToString(string format, IFormatProvider formatProvider)
    {
        return ToString();
    }
}
