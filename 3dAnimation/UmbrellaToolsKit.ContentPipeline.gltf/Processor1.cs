using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework.Content.Pipeline;

using TInput = glTFLoader.Schema.Gltf;
using TOutput = UmbrellaToolsKit.Animation3D.Mesh;

namespace UmbrellaToolsKit.ContentPipeline.gltf
{
    [ContentProcessor(DisplayName = "Processor1")]
    internal class Processor1 : ContentProcessor<TInput, TOutput>
    {
        public override TOutput Process(TInput input, ContentProcessorContext context)
        {
            ValidateFile(input);
            Animation3D.Mesh mesh = LoadMesh(input);
            return mesh;
        }

        public Animation3D.Mesh LoadMesh(glTFLoader.Schema.Gltf gltf)
        {
            Animation3D.Mesh meshR = new Animation3D.Mesh();

            
            byte[] uriBytes = Convert.FromBase64String(gltf.Buffers[0].Uri.Replace("data:application/octet-stream;base64,", ""));
            var accessor = gltf.Accessors[0];
            // vertices
            for(int n = gltf.BufferViews[0].ByteOffset; n < gltf.BufferViews[0].ByteOffset + gltf.BufferViews[0].ByteLength; n+=4)
            {
                float x = BitConverter.ToSingle(uriBytes, n);
                n += 4;
                float y = BitConverter.ToSingle(uriBytes, n);
                n += 4;
                float z = BitConverter.ToSingle(uriBytes, n);

                meshR.Vertices.Add(new Microsoft.Xna.Framework.Vector3(x, y, z));
            }

            for(int n = gltf.BufferViews[3].ByteOffset; n < gltf.BufferViews[3].ByteOffset + gltf.BufferViews[3].ByteLength; n += 2)
            {
                UInt16 TriangleItem = BitConverter.ToUInt16(uriBytes, n);
                meshR.Indices.Add((int)TriangleItem);
            }

            return meshR;
        }

        public static void MeshFromAttributes()
        {

        }

        public static void ValidateFile(glTFLoader.Schema.Gltf gltf)
        {
            if (gltf.Buffers.Length == 0)
                throw new InvalidContentException($"Could not load buffers");
        }
    }
}