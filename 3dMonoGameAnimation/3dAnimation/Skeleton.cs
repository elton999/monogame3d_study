using System.Collections.Generic;
using System.Linq;

namespace _3dAnimation;

public class Skeleton
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
}
