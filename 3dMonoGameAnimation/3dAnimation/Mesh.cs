using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.IO;
using glTFLoader;
using Microsoft.Xna.Framework;

namespace _3dAnimation;
public class Mesh
{
    private VertexPositionColor[] _vertices;
    private glTFLoader.Schema.Gltf _gltf;

    public VertexPositionColor[] Vertices => _vertices;

    public Mesh(string pathModel)
    {
        Load(pathModel);
    }

    public Mesh(){ }

    public void Load(string pathModel)
    {
        _gltf = Interface.LoadModel(pathModel);
        LoadMesh();
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

    private void LoadMesh()
    {
        byte[][] uriBytesList;
        var vertices = new List<Vector3>();
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
