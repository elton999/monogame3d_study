using Microsoft.Xna.Framework;
using System;
using System.Linq;

namespace _3dAnimation;

public class Animator
{
    private string _animationName;
    private float _timer;
    private int _currentFrame;
    private Skeleton _skeleton;

    public Animator(Skeleton skeleton) => _skeleton = skeleton;

    public void PlayAnimation(string animationName)
    {
        _timer = 0f;
        _currentFrame = 0;
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

        if (_timer > clip.FramesTimer[_currentFrame])
            _currentFrame++;

        if (_currentFrame >= clip.JoinByFrameTransform.Length)
        { 
            _currentFrame = 0;
            _timer = 0f;
        }

        // 1. Sempre volte ao bind pose antes de aplicar animação
        foreach (var joint in joints)
            joint.ResetToBindPose();

        // 2. Aplica animação LOCAL (sem world ainda)
        var frame = clip.JoinByFrameTransform[_currentFrame];

        if (frame != null)
        {
            for (int jointIndex = 0; jointIndex < joints.Length; jointIndex++)
            {
                if (frame.TryGetValue(jointIndex, out var animTransform))
                    joints[jointIndex].ApplyTransformAnimation(animTransform);
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
}
