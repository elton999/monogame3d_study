using System.Linq;

namespace _3dAnimation;
public class Joint
{
    private Joint[] _parents = [];
    private string _name;

    public bool HasParent => _parents.Length > 0;
    public Joint[] Parents => _parents;
    public string Name => _name;

    public Joint() { }

    public Joint(string name)
    { 
        _name = name;
    }

    public void SetName(string name)
    {
        _name = name;
    }

    public void SetParent(Joint parent)
    {
        var tempParents = _parents.ToList();
        if (tempParents.Contains(parent)) return;
        tempParents.Add(parent);
        _parents = tempParents.ToArray();
    }
}
