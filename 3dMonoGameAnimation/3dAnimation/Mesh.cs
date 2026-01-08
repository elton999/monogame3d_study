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
        LoadAnimation();

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

    private void LoadAnimation()
    {
        byte[][] uriBytesList;
        var weights = new List<Vector4>();
        var joints = new List<Vector4>();
        var indices = new List<short>();

        uriBytesList = new byte[_gltf.Buffers.Length][];
        for (int bytesListIndex = 0; bytesListIndex < uriBytesList.Length; bytesListIndex++)
        {
            uriBytesList[bytesListIndex] = Convert.FromBase64String(_gltf.Buffers[bytesListIndex].Uri.Replace("data:application/octet-stream;base64,", ""));
        }

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
                if (primitives[meshesIndex].Attributes.ContainsKey("JOINTS_0") && primitives[meshesIndex].Attributes["JOINTS_0"] == accessorIndex && accessor.Type == glTFLoader.Schema.Accessor.TypeEnum.VEC4)
                {
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
                        joints.Add(new Vector4(x, y, z, w));
                    }
                }

                //Weights 
                if (primitives[meshesIndex].Attributes.ContainsKey("WEIGHTS_0") && primitives[meshesIndex].Attributes["WEIGHTS_0"] == accessorIndex && accessor.Type == glTFLoader.Schema.Accessor.TypeEnum.VEC4)
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
                        weights.Add(new Vector4(x, y, z, w));
                    }
                }

                // Indicies
                if (accessor.ComponentType == glTFLoader.Schema.Accessor.ComponentTypeEnum.UNSIGNED_SHORT)
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
        _indices = indices.ToArray();
        _joints = joints.ToArray();
    }

    private void LoadMesh()
    {
        byte[][] uriBytesList;
        var vertices = new List<Vector3>();

        uriBytesList = new byte[_gltf.Buffers.Length][];
        for (int bytesListIndex = 0; bytesListIndex < uriBytesList.Length; bytesListIndex++)
        {
            uriBytesList[bytesListIndex] = Convert.FromBase64String(_gltf.Buffers[bytesListIndex].Uri.Replace("data:application/octet-stream;base64,", ""));
        }

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

                if (primitives[meshesIndex].Attributes["POSITION"] == accessorIndex && accessor.Type == glTFLoader.Schema.Accessor.TypeEnum.VEC3)
                {
                    float[] ScalingFactorForVariables = new float[3] { 1.0f, 1.0f, 1.0f };

                    for (int n = bufferView.ByteOffset; n < bufferView.ByteOffset + bufferView.ByteLength; n += 4)
                    {
                        float x = BitConverter.ToSingle(uriBytes, n) / ScalingFactorForVariables[0];
                        n += 4;
                        float y = BitConverter.ToSingle(uriBytes, n) / ScalingFactorForVariables[1];
                        n += 4;
                        float z = BitConverter.ToSingle(uriBytes, n) / ScalingFactorForVariables[2];

                        vertices.Add(new Vector3(x, y, z));
                    }
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
}
