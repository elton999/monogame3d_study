using Microsoft.Xna.Framework;

namespace UmbrellaToolsKit.Animation3D
{
    public class TrackHelpers
    {
        public static float Interpolate(float a, float b, float t) =>  a + (b - a) * t;

        public static Vector3 Interpolate(Vector3 a,  Vector3 b, float t)  =>  Vector3.Lerp(a, b, t);

        public static Quaternion Interpolate(Quaternion a, Quaternion b, float t)
        {
            Quaternion result = QuaternionHelpers.Mix(a, b, t);
            // Neighborhood
            if (Quaternion.Dot(a, b) < 0)
                result = QuaternionHelpers.Mix(a, -b, t);
            
            return Quaternion.Normalize(result); //NLerp, not slerp
        }
        // Hermite helpers
        public static float AdjustHermiteResult(float f) => f;

        public static Vector3 AdjustHermiteResult(Vector3 v) => v;

        public static Quaternion AdjustHermiteResult(Quaternion q) => Quaternion.Normalize(q);

        public static void Neighborhood(float a, float b) { }
       
        public static void Neighborhood(Vector3 a, Vector3 b) { }

        public static void Neighborhood(Quaternion a, ref Quaternion b)
        {
            if (Quaternion.Dot(a, b) < 0)
                b = -b;
        }
    }
}
