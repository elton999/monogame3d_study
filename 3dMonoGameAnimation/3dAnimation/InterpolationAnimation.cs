using Microsoft.Xna.Framework;

namespace _3dAnimation;

public class InterpolationAnimation
{
    public static float GetInterpolationValue(AnimationClip animationClip, int frame, float currentTimer)
    {
        // Evita erro se for o último frame
        if (frame >= animationClip.FramesTimer.Length - 1) return 0f;

        float previousTime = animationClip.FramesTimer[frame];
        float nexTime = animationClip.FramesTimer[frame + 1];

        return (currentTimer - previousTime) / (nexTime - previousTime);
    }

    public static float GetIntervalDuration(AnimationClip animationClip, int frame)
    {
        // Evita erro se for o último frame
        if (frame >= animationClip.FramesTimer.Length - 1) return 0f;
        float timeA = animationClip.FramesTimer[frame];
        float timeB = animationClip.FramesTimer[frame + 1];

        return timeB - timeA;
    }

    public static float[] GetLerp(Vector3 previewVector, Vector3 nextVector, float interpoaltionValue)
    {
        var result = Vector3.Lerp(previewVector, nextVector, interpoaltionValue);
        return new float[] { result.X, result.Y, result.Z };
    }

    public static float[] GetLerp(Quaternion previewVector, Quaternion nextVector, float interpoaltionValue)
    {
        var result = Quaternion.Lerp(previewVector, nextVector, interpoaltionValue);
        return new float[] { result.X, result.Y, result.Z, result.W };
    }

    public static float[] GetCubicSpline(Frame frameA, Frame frameB, float interpoaltionValue, float intervalDuration)
    {
        // Converter os float[] para Vector3 do XNA
        Vector3 p0 = new Vector3(frameA.mValue[0], frameA.mValue[1], frameA.mValue[2]);
        Vector3 m0 = new Vector3(frameA.OutTangent[0], frameA.OutTangent[1], frameA.OutTangent[2]) * intervalDuration;

        Vector3 p1 = new Vector3(frameB.mValue[0], frameB.mValue[1], frameB.mValue[2]);
        Vector3 m1 = new Vector3(frameB.InTangent[0], frameB.InTangent[1], frameB.InTangent[2]) * intervalDuration;

        // Coeficientes da Hermite Spline
        float t2 = interpoaltionValue * interpoaltionValue;
        float t3 = t2 * interpoaltionValue;

        float h00 = 2 * t3 - 3 * t2 + 1;
        float h10 = t3 - 2 * t2 + interpoaltionValue;
        float h01 = -2 * t3 + 3 * t2;
        float h11 = t3 - t2;

         var result = h00 * p0 + h10 * m0 + h01 * p1 + h11 * m1;
        return new float[] { result.X, result.Y, result.Z };
    }

    public static float[] GetCubicSpline(QuaternionFrame frameA, QuaternionFrame frameB, float t, float deltaTime)
    {
        // glTF diz: para rotações, use a interpolação componente a componente e NORMALIZE no final.
        // Diferente do Lerp/Slerp, splines em quatérnios podem tirar a magnitude de 1.

        Vector4 p0 = new Vector4(frameA.mValue[0], frameA.mValue[1], frameA.mValue[2], frameA.mValue[3]);
        Vector4 m0 = new Vector4(frameA.OutTangent[0], frameA.OutTangent[1], frameA.OutTangent[2], frameA.OutTangent[3]) * deltaTime;

        Vector4 p1 = new Vector4(frameB.mValue[0], frameB.mValue[1], frameB.mValue[2], frameB.mValue[3]);
        Vector4 m1 = new Vector4(frameB.InTangent[0], frameB.InTangent[1], frameB.InTangent[2], frameB.InTangent[3]) * deltaTime;

        float t2 = t * t;
        float t3 = t2 * t;

        float h00 = 2 * t3 - 3 * t2 + 1;
        float h10 = t3 - 2 * t2 + t;
        float h01 = -2 * t3 + 3 * t2;
        float h11 = t3 - t2;

        Vector4 resultVector = h00 * p0 + h10 * m0 + h01 * p1 + h11 * m1;

        // OBRIGATÓRIO: Normalizar para voltar a ser um Quatérnio válido
        Quaternion result = new Quaternion(resultVector.X, resultVector.Y, resultVector.Z, resultVector.W);
        result.Normalize();
        return new float[] { result.X, result.Y, result.Z, result.W };
    }
}
