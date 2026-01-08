using System.Linq;

namespace _3dAnimation;
public class Joint
{
    private Joint[] _parents = [];
    private string _name;
    private float _weight;

    public bool HasParent => _parents.Length > 0;
    public Joint[] Parents => _parents;
    public string Name => _name;
    public float Weight => _weight;

    public Joint(string name, float weight)
    { 
        _name = name;
        _weight = weight;
    }

    public void SetParent(Joint parent)
    {
        var tempParents = _parents.ToList();
        
        if (tempParents.Contains(parent)) return;
        
        tempParents.Add(parent);
        _parents = tempParents.ToArray();
    }
}
