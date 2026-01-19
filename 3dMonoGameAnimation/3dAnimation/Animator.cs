using System;
using System.Linq;
using Microsoft.Xna.Framework;

namespace _3dAnimation;

public class Animator
{
    public enum LoopMode
    {
        REPEAT = 0,
        PING_PONG = 1,
        ONCE = 2,
    }

    private string _animationName;
    private float _timer;
    private int _currentFrame;
    private Skeleton _skeleton;
    private LoopMode _currentLoopMode;

    public Animator(Skeleton skeleton) => _skeleton = skeleton;

    public void PlayAnimation(string animationName, LoopMode loopMode = LoopMode.REPEAT)
    {
        _currentLoopMode = loopMode;
        ResetAnimation();
        _animationName = animationName;
    }

    public void Update(float deltaTimer)
    {
        if (string.IsNullOrEmpty(_animationName))
        {
            for (int i = 0; i < _skeleton.SkinMatrices.Length; i++)
                _skeleton.SkinMatrices[i] = Matrix.Identity;

            return;
        }

        var animationClips = _skeleton.AnimationClips;
        var joints = _skeleton.Joints;

        var clip = animationClips.FirstOrDefault(c => c.Name == _animationName);
        if (clip == null || clip.JoinByFrameTransform == null)
            return;

        UpdateLoop(clip);

        // 1. Sempre volte ao bind pose antes de aplicar animação
        foreach (var joint in joints)
            joint.ResetToBindPose();

        // 2. Aplica animação LOCAL (sem world ainda)
        var currentFrame = clip.JoinByFrameTransform[_currentFrame];

        if (currentFrame != null)
        {
            for (int jointIndex = 0; jointIndex < joints.Length; jointIndex++)
            {
                if (currentFrame.TryGetValue(jointIndex, out var animTransform))
                {
                    float interpolationValue = InterpolationAnimation.GetInterpolationValue(clip, _currentFrame, _timer);
                    float intervalDuration = InterpolationAnimation.GetIntervalDuration(clip, _currentFrame);
                    Console.WriteLine(interpolationValue);

                    var nextAnimTransform = animTransform;
                    var tempAnimTransform = animTransform.GetCopy();

                    if (clip.JoinByFrameTransform.Length -1 > _currentFrame + 1)
                    {
                        var nextFrame = clip.JoinByFrameTransform[_currentFrame+1];
                        if (nextFrame != null && nextFrame.TryGetValue(jointIndex, out var tempNextAnimTransform))
                        {
                            nextAnimTransform = tempNextAnimTransform;
                        }
                    }

                    nextAnimTransform = nextAnimTransform.GetCopy();

                    if (tempAnimTransform.HasTranslation && tempAnimTransform.Translation.Interpolation is InterpolationType.LINEAR)
                    {
                        var currentValue = tempAnimTransform.Translation.GetVector3();
                        var nextValue = nextAnimTransform.Translation.GetVector3();
                        tempAnimTransform.Translation.mValue = InterpolationAnimation.GetLerp(currentValue, nextValue, interpolationValue);
                    }

                    if (tempAnimTransform.HasScalar && tempAnimTransform.Scalar.Interpolation is InterpolationType.LINEAR)
                    {
                        var currentValue = tempAnimTransform.Scalar.GetVector3();
                        var nextValue = nextAnimTransform.Scalar.GetVector3();
                        tempAnimTransform.Scalar.mValue = InterpolationAnimation.GetLerp(currentValue, nextValue, interpolationValue);
                    }

                    if (tempAnimTransform.HasRotation && tempAnimTransform.Rotation.Interpolation is InterpolationType.LINEAR)
                    {
                        var currentValue = tempAnimTransform.Rotation.GetQuaternion();
                        var nextValue = nextAnimTransform.Rotation.GetQuaternion();
                        tempAnimTransform.Rotation.mValue = InterpolationAnimation.GetLerp(currentValue, nextValue, interpolationValue);
                    }

                    if (tempAnimTransform.HasTranslation && tempAnimTransform.Translation.Interpolation is InterpolationType.CUBICSPLINE)
                    {
                        var currentValue = tempAnimTransform.Translation;
                        var nextValue = nextAnimTransform.Translation;
                        tempAnimTransform.Translation.mValue = InterpolationAnimation.GetCubicSpline(currentValue, nextValue, interpolationValue, intervalDuration);
                    }

                    if (tempAnimTransform.HasScalar && tempAnimTransform.Scalar.Interpolation is InterpolationType.CUBICSPLINE)
                    {
                        var currentValue = tempAnimTransform.Scalar;
                        var nextValue = nextAnimTransform.Scalar;
                        tempAnimTransform.Scalar.mValue = InterpolationAnimation.GetCubicSpline(currentValue, nextValue, interpolationValue, intervalDuration);
                    }

                    if (tempAnimTransform.HasRotation && tempAnimTransform.Rotation.Interpolation is InterpolationType.CUBICSPLINE)
                    {
                        var currentValue = tempAnimTransform.Rotation;
                        var nextValue = nextAnimTransform.Rotation;
                        tempAnimTransform.Rotation.mValue = InterpolationAnimation.GetCubicSpline(currentValue, nextValue, interpolationValue, intervalDuration);
                    }

                    joints[jointIndex].ApplyTransformAnimation(tempAnimTransform);
                }
            }
        }

        _timer += deltaTimer;

        // 3. Atualiza world matrices (hierarquia correta)
        foreach (var root in joints.Where(j => !j.HasParent))
            root.UpdateWorld(Matrix.Identity);

        // 4. Calcula skin matrices finais
        for (int jointIndex = 0; jointIndex < joints.Length; jointIndex++)
        {
            _skeleton.SkinMatrices[jointIndex] = joints[jointIndex].InverseBindPose * joints[jointIndex].WorldMatrix;
        }
    }

    public void ResetAnimation()
    {
        _timer = 0f;
        _currentFrame = 0;
    }

    private void UpdateLoop(AnimationClip clip)
    {
        if (_timer > clip.FramesTimer[_currentFrame])
        {
            bool isLastFrame = clip.FramesTimer.Length - 1 == _currentFrame;
            
            if(_currentLoopMode is LoopMode.REPEAT || _currentLoopMode is LoopMode.ONCE && !isLastFrame)
                _currentFrame++;
        }

        if (_currentFrame < clip.JoinByFrameTransform.Length) return;

        if (_currentLoopMode is LoopMode.REPEAT)
        {
            ResetAnimation();
        }

        if (_currentLoopMode is LoopMode.PING_PONG)
        {
            // TODO: Logic of ping pong animation
        }
    }
}
