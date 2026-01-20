using glTFLoader;
using glTFLoader.Schema;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;

namespace UmbrellaToolsKit.Animation3D;

public class Mesh
{
    private ModelVertexType[] _vertices;
    private Vector4[] _weights;
    private Vector4[] _joints;
    private short[] _indices;
    private Vector2[] _textureCoord;
    private Vector3[] _normals;
    private Vector3[] _meshVertices;
    private Matrix[] _inverseBindMatrix;
    private int[] _jointsIndexs;

    private Skeleton _skeleton;
    private Gltf _gltf;

    public ModelVertexType[] Vertices => _vertices;
    public Vector4[] Weights => _weights;
    public Vector4[] Joints => _joints;
    public short[] Indices => _indices;
    public Vector2[] TextureCoord => _textureCoord;
    public Vector3[] Normals => _normals;
    public Matrix[] InverseBindMatrix => _inverseBindMatrix;
    public Skeleton Skeleton => _skeleton;

    public Mesh(string pathModel)
    {
        Load(pathModel);
    }

    public Mesh(){ }

    public void Load(string pathModel)
    {
        _gltf = Interface.LoadModel(pathModel);
        LoadMesh();
        LoadJoints();
        LoadAnimations();
    }

    private void LoadJoints()
    {
        byte[][] uriBytesList;
        var weights = new List<Vector4>();
        var joints = new List<Vector4>();
        var indices = new List<short>();
        var normals = new List<Vector3>();
        var textureCoords = new List<Vector2>();
        uriBytesList = GetBytesList();

        for (int meshesIndex = 0; meshesIndex < _gltf.Meshes.Length; meshesIndex++)
        {
            var primitives = _gltf.Meshes[meshesIndex].Primitives;
            int accessorLenght = _gltf.Accessors.Length;
            for (int accessorIndex = 0; accessorIndex < accessorLenght; accessorIndex++)
            {
                var accessor = _gltf.Accessors[accessorIndex];

                int bufferIndex = accessor.BufferView.Value;
                var bufferView = _gltf.BufferViews[bufferIndex];
                byte[] uriBytes = uriBytesList[bufferView.Buffer];

                //Joints 
                if (primitives[meshesIndex].Attributes.ContainsKey("JOINTS_0") && primitives[meshesIndex].Attributes["JOINTS_0"] == accessorIndex && accessor.Type == Accessor.TypeEnum.VEC4)
                {
                    var listJoints = _gltf.Skins[0].Joints;
                    for (int n = bufferView.ByteOffset; n < bufferView.ByteOffset + bufferView.ByteLength; n++)
                    {
                        int x = uriBytes[n];
                        n++;
                        int y = uriBytes[n];
                        n++;
                        int z = uriBytes[n];
                        n++;
                        int w = uriBytes[n];
                        joints.Add(new Vector4(x, y, z, w));
                    }
                }

                //Weights 
                if (primitives[meshesIndex].Attributes.ContainsKey("WEIGHTS_0") && primitives[meshesIndex].Attributes["WEIGHTS_0"] == accessorIndex && accessor.Type == Accessor.TypeEnum.VEC4)
                {
                    for (int n = bufferView.ByteOffset; n < bufferView.ByteOffset + bufferView.ByteLength; n += 4)
                    {

                        float x = BitConverter.ToSingle(uriBytes, n);
                        n += 4;
                        float y = BitConverter.ToSingle(uriBytes, n);
                        n += 4;
                        float z = BitConverter.ToSingle(uriBytes, n);
                        n += 4;
                        float w = BitConverter.ToSingle(uriBytes, n);
                        var weight = new Vector4(x, y, z, w);
                        weights.Add(weight);
                    }
                }

                // Normals
                if (primitives[meshesIndex].Attributes.ContainsKey("NORMAL") && primitives[meshesIndex].Attributes["NORMAL"] == accessorIndex && accessor.Type == Accessor.TypeEnum.VEC3)
                {
                    normals.AddRange(GetVector3List(bufferView, uriBytes));
                }

                //Texture Coords
                if (primitives[meshesIndex].Attributes.ContainsKey("NORMAL") && primitives[meshesIndex].Attributes.ContainsKey("TEXCOORD_0") && primitives[meshesIndex].Attributes["TEXCOORD_0"] == accessorIndex && accessor.Type == Accessor.TypeEnum.VEC2)
                {
                    for (int n = bufferView.ByteOffset; n < bufferView.ByteOffset + bufferView.ByteLength; n += 4)
                    {
                        float x = BitConverter.ToSingle(uriBytes, n);
                        n += 4;
                        float y = BitConverter.ToSingle(uriBytes, n);

                        textureCoords.Add(new Vector2(x, y));
                    }
                }

                // Indicies
                if (accessor.ComponentType == Accessor.ComponentTypeEnum.UNSIGNED_SHORT)
                {
                    for (int n = bufferView.ByteOffset; n < bufferView.ByteOffset + bufferView.ByteLength; n += 2)
                    {
                        short TriangleItem = BitConverter.ToInt16(uriBytes, n);
                        indices.Add((short)TriangleItem);
                    }
                }
            }

            SetInverseBindMatrix(uriBytesList);

        }

        _weights = weights.ToArray();
        _joints = joints.ToArray();
        _indices = indices.ToArray();
        _normals = normals.ToArray();
        _textureCoord = textureCoords.ToArray();
    }

    private void SetInverseBindMatrix(byte[][] uriBytesList)
    {
        int accessorIndex = _gltf.Skins[0].InverseBindMatrices.Value;
        var accessor = _gltf.Accessors[accessorIndex];

        var bufferView = _gltf.BufferViews[accessor.BufferView.Value];
        byte[] buffer = uriBytesList[bufferView.Buffer];

        int matrixCount = accessor.Count;
        int stride = bufferView.ByteStride ?? (16 * 4); // 64 bytes

        int offset = bufferView.ByteOffset + accessor.ByteOffset;

        var inverseBind = new Matrix[matrixCount];

        for (int matrixIndex = 0; matrixIndex < matrixCount; matrixIndex++)
        {
            Matrix m = new Matrix();

            int baseOffset = offset + matrixIndex * stride;

            for (int j = 0; j < 16; j++)
            {
                m[j] = BitConverter.ToSingle(buffer, baseOffset + j * 4);
            }

            inverseBind[matrixIndex] = m;
        }

        _inverseBindMatrix = inverseBind;
        _jointsIndexs = _gltf.Skins[0].Joints;
    }

    private void LoadAnimations()
    {
        var skin = _gltf.Skins[0];
        var jointNodes = skin.Joints;

        int jointCount = jointNodes.Length;

        var joints = new Joint[jointCount];

        Dictionary<int, int> nodeToJoint = new();

        for (int jointIndex = 0; jointIndex < jointCount; jointIndex++)
        {
            int nodeIndex = jointNodes[jointIndex];
            nodeToJoint[nodeIndex] = jointIndex;

            var node = _gltf.Nodes[nodeIndex];
            joints[jointIndex] = new Joint(node.Name);
        }

        for (int jointIndex = 0; jointIndex < jointCount; jointIndex++)
        {
            int nodeIndex = jointNodes[jointIndex];
            var node = _gltf.Nodes[nodeIndex];

            if (node.Children == null)
                continue;

            foreach (int childNodeIndex in node.Children)
            {
                if (!nodeToJoint.TryGetValue(childNodeIndex, out int childJointIndex))
                    continue;

                joints[childJointIndex].SetParent(joints[jointIndex]);
            }
        }

        _skeleton = new Skeleton(joints);
        _skeleton.InverseBindMatrix = _inverseBindMatrix;

        var uriBytesList = GetBytesList();
        for (int animationIndex = 0; animationIndex < _gltf.Animations.Length; animationIndex++)
        {
            var animation = _gltf.Animations[animationIndex];
            var clip = new AnimationClip(animation.Name);
            _skeleton.AddAnimationClip(clip);

            foreach (var channel in animation.Channels)
            {
                var sampler = animation.Samplers[channel.Sampler];

                int inputAccessorIndex = sampler.Input;
                int outputAccessorIndex = sampler.Output;
                var animationInterpolation = sampler.Interpolation;

                var inputAccessor = _gltf.Accessors[inputAccessorIndex];
                var outputAccessor = _gltf.Accessors[outputAccessorIndex];

                var inputView = _gltf.BufferViews[inputAccessor.BufferView.Value];
                var outputView = _gltf.BufferViews[outputAccessor.BufferView.Value];

                byte[] inputBuffer = uriBytesList[inputView.Buffer];
                byte[] outputBuffer = uriBytesList[outputView.Buffer];

                int jointIndex = nodeToJoint[(int)channel.Target.Node];

                int frameCount = inputAccessor.Count;
                for (int frameIndex = 0; frameIndex < frameCount; frameIndex++)
                {
                    float time = BitConverter.ToSingle(
                        inputBuffer,
                        inputView.ByteOffset + inputAccessor.ByteOffset + frameIndex * 4
                    );

                    clip.AddFrameTimer(frameIndex, time);
                }

                int elementSize = GetStride(outputAccessor.Type, InterpolationType.LINEAR); // Tamanho de 1 VEC3 (12 bytes)
                int totalFrameStride = GetStride(outputAccessor.Type, (InterpolationType)animationInterpolation);

                for (int frameIndex = 0; frameIndex < frameCount; frameIndex++)
                {
                    int frameOffset = outputView.ByteOffset + outputAccessor.ByteOffset + (frameIndex * totalFrameStride);

                    switch (channel.Target.Path)
                    {
                        case AnimationChannelTarget.PathEnum.translation:
                            {
                                var v = new VectorFrame { Interpolation = (InterpolationType)animationInterpolation };
                                if (v.Interpolation == InterpolationType.CUBICSPLINE)
                                {
                                    v.InTangent = ReadFloatArray(outputBuffer, frameOffset, 3);
                                    v.mValue = ReadFloatArray(outputBuffer, frameOffset + elementSize, 3);
                                    v.OutTangent = ReadFloatArray(outputBuffer, frameOffset + elementSize * 2, 3);
                                }
                                else
                                {
                                    v.mValue = ReadFloatArray(outputBuffer, frameOffset, 3);
                                }
                                clip.AddTranslation(frameIndex, jointIndex, v);
                                break;
                            }

                        case AnimationChannelTarget.PathEnum.rotation:
                            {
                                var q = new QuaternionFrame { Interpolation = (InterpolationType)animationInterpolation };
                                if (q.Interpolation == InterpolationType.CUBICSPLINE)
                                {
                                    q.InTangent = ReadFloatArray(outputBuffer, frameOffset, 4);
                                    q.mValue = ReadFloatArray(outputBuffer, frameOffset + elementSize, 4);
                                    q.OutTangent = ReadFloatArray(outputBuffer, frameOffset + elementSize * 2, 4);
                                }
                                else
                                {
                                    q.mValue = ReadFloatArray(outputBuffer, frameOffset, 4);
                                }
                                clip.AddRotation(frameIndex, jointIndex, q);
                                break;
                            }

                        case AnimationChannelTarget.PathEnum.scale:
                            {
                                var s = new ScalarFrame { Interpolation = (InterpolationType)animationInterpolation };
                                if (s.Interpolation == InterpolationType.CUBICSPLINE)
                                {
                                    s.InTangent = ReadFloatArray(outputBuffer, frameOffset, 3);
                                    s.mValue = ReadFloatArray(outputBuffer, frameOffset + elementSize, 3);
                                    s.OutTangent = ReadFloatArray(outputBuffer, frameOffset + elementSize * 2, 3);
                                }
                                else
                                {
                                    s.mValue = ReadFloatArray(outputBuffer, frameOffset, 3);
                                }
                                clip.AddScalar(frameIndex, jointIndex, s);
                                break;
                            }
                    }
                }
            }
        }

        for (int jointIndex = 0; jointIndex < skin.Joints.Length; jointIndex++)
        {
            int nodeIndex = skin.Joints[jointIndex];
            var node = _gltf.Nodes[nodeIndex];

            Transform result = new Transform();

            if (node.Translation != null)
                result.Translation.mValue = new float[] { node.Translation[0], node.Translation[1], node.Translation[2] };

            if (node.Rotation != null)
                result.Rotation.mValue = new float[] { node.Rotation[0], node.Rotation[1], node.Rotation[2], node.Rotation[3] };

            if (node.Scale != null)
                result.Scalar.mValue = new float[] { node.Scale[0], node.Scale[1], node.Scale[2] };

            _skeleton.Joints[jointIndex].SetTransform(result);
        }


        var verticesTemp = new List<ModelVertexType>();
        int countIndex = 0;
        foreach (var vertex in _meshVertices)
        {
            verticesTemp.Add(new ModelVertexType(vertex, Color.White, Normals[countIndex], TextureCoord[countIndex], Joints[countIndex], Weights[countIndex]));
            countIndex++;
        }
        _vertices = verticesTemp.ToArray();
    }

    private void LoadMesh()
    {
        byte[][] uriBytesList;
        var vertices = new List<Vector3>();

        uriBytesList = GetBytesList();

        for (int meshesIndex = 0; meshesIndex < _gltf.Meshes.Length; meshesIndex++)
        {
            var primitives = _gltf.Meshes[meshesIndex].Primitives;
            int accessorLenght = _gltf.Accessors.Length;

            for (int accessorIndex = 0; accessorIndex < accessorLenght; accessorIndex++)
            {
                var accessor = _gltf.Accessors[accessorIndex];

                int bufferIndex = accessor.BufferView.Value;
                var bufferView = _gltf.BufferViews[bufferIndex];
                byte[] uriBytes = uriBytesList[bufferView.Buffer];

                if (primitives[meshesIndex].Attributes["POSITION"] == accessorIndex && accessor.Type == Accessor.TypeEnum.VEC3)
                {
                    vertices.AddRange(GetVector3List(bufferView, uriBytes));
                }
            }
        }

        _meshVertices = vertices.ToArray();
    }

    private float[] ReadFloatArray(byte[] buffer, int offset, int count)
    {
        float[] result = new float[count];
        for (int i = 0; i < count; i++)
        {
            result[i] = BitConverter.ToSingle(buffer, offset + (i * 4));
        }
        return result;
    }

    private byte[][] GetBytesList()
    {
        byte[][] uriBytesList = new byte[_gltf.Buffers.Length][];
        for (int bytesListIndex = 0; bytesListIndex < uriBytesList.Length; bytesListIndex++)
        {
            uriBytesList[bytesListIndex] = Convert.FromBase64String(_gltf.Buffers[bytesListIndex].Uri.Replace("data:application/octet-stream;base64,", ""));
        }

        return uriBytesList;
    }

    private List<Vector3> GetVector3List(BufferView bufferView, byte[] uriBytes, float[] ScalingFactorForVariables = null)
    {
        var vectorListResult = new List<Vector3>();
        if (ScalingFactorForVariables == null)
            ScalingFactorForVariables = new float[3] { 1f, 1f, 1f };

        for (int n = bufferView.ByteOffset; n < bufferView.ByteOffset + bufferView.ByteLength; n += 4)
        {
            float x = BitConverter.ToSingle(uriBytes, n) / ScalingFactorForVariables[0];
            n += 4;
            float y = BitConverter.ToSingle(uriBytes, n) / ScalingFactorForVariables[1];
            n += 4;
            float z = BitConverter.ToSingle(uriBytes, n) / ScalingFactorForVariables[2];

            vectorListResult.Add(new Vector3(x, y, z));
        }

        return vectorListResult;
    }

    private static int GetStride(Accessor.TypeEnum type, InterpolationType interpolation)
    {
        int baseStride = type switch
        {
            Accessor.TypeEnum.SCALAR => 4,
            Accessor.TypeEnum.VEC3 => 12,
            Accessor.TypeEnum.VEC4 => 16,
            _ => throw new NotSupportedException()
        };

        return interpolation == InterpolationType.CUBICSPLINE ? baseStride * 3 : baseStride;
    }
}
