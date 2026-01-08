using glTFLoader;
using glTFLoader.Schema;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.IO;

namespace _3dAnimation;
public class Mesh
{
    private VertexPositionColor[] _vertices;
    private Vector4[] _weights;
    private Vector4[] _joints;
    private short[] _indices;
    private Gltf _gltf;

    public VertexPositionColor[] Vertices => _vertices;
    public Vector4[] Weights => _weights;
    public Vector4[] Joints => _joints;
    public short[] Indices => _indices;

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

        try
        {
           
        }
        catch (FileNotFoundException)
        {
            Console.WriteLine($"Error: The file '{pathModel}' was not found.");
        }
        catch (Exception e)
        {
            Console.WriteLine($"An error occurred: {e.Message}");
        }
    }

    private void LoadJoints()
    {
        byte[][] uriBytesList;
        var weights = new List<Vector4>();
        var joints = new List<Vector4>();
        var indices = new List<short>();
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
                    joints.AddRange(GetVector4List(bufferView, uriBytes));
                }

                //Weights 
                if (primitives[meshesIndex].Attributes.ContainsKey("WEIGHTS_0") && primitives[meshesIndex].Attributes["WEIGHTS_0"] == accessorIndex && accessor.Type == Accessor.TypeEnum.VEC4)
                {
                    weights.AddRange(GetVector4List(bufferView, uriBytes));
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
        }

        _weights = weights.ToArray();
        _joints = joints.ToArray();
        _indices = indices.ToArray();
    }

    private void LoadAnimations()
    {
        var joints = new Joint[_gltf.Nodes.Length];
        for(int nodeIndex = 0; nodeIndex < _gltf.Nodes.Length; nodeIndex++)
        {
            var node = _gltf.Nodes[nodeIndex];
            
            if (joints[nodeIndex] == null)
                joints[nodeIndex] = new Joint(node.Name);

            joints[nodeIndex].SetName(node.Name);

            if (node.Children == null) continue;

            for (int childrenIndex = 0; childrenIndex < node.Children.Length; childrenIndex++)
            {
                int index = node.Children[childrenIndex];
                if (joints[index] == null)
                    joints[index] = new Joint();

                joints[index].SetParent(joints[nodeIndex]);
            }
        }

        var skeleton = new Skeleton(joints);
        Console.WriteLine(skeleton.ToString());
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

        var verticesTemp = new List<VertexPositionColor>();
        foreach (var vertex in vertices)
        {
            verticesTemp.Add(new VertexPositionColor(vertex, Color.White));
        }
        _vertices = verticesTemp.ToArray();
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

    private List<Vector4> GetVector4List(BufferView bufferView, byte[] uriBytes)
    {
        var vectorListResult = new List<Vector4>();
        var listJoints = _gltf.Skins[0].Joints;
        for (int n = bufferView.ByteOffset; n < bufferView.ByteOffset + bufferView.ByteLength; n++)
        {
            float x = uriBytes[n];
            n++;
            float y = uriBytes[n];
            n++;
            float z = uriBytes[n];
            n++;
            float w = uriBytes[n];
            var tempVector = new Vector4(x, y, z, w);
            vectorListResult.Add(tempVector);
        }

        return vectorListResult;
    }
}
