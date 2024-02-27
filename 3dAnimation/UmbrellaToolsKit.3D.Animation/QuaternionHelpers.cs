using Microsoft.Xna.Framework;

namespace UmbrellaToolsKit.Animation3D
{
    public class QuaternionHelpers
    {
        public static Quaternion Mix(Quaternion from, Quaternion to, float t) =>  from * (1.0f - t) + to * t;
    }
}
