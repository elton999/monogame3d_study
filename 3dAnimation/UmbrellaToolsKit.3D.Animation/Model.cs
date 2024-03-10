using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace UmbrellaToolsKit.Animation3D
{
    public class Model
    {
        private Mesh _mesh;

        private VertexBuffer _vertexBuffer;
        private IndexBuffer _indexBuffer;
        
        private Matrix _modelWorld;
        private Texture _texture;

        private Effect basicEffect;
        private Vector3 _lightPosition;

        public Model(Mesh mesh, GraphicsDevice graphicsDevice)
        {
            _mesh = mesh;

            LoadModel(graphicsDevice);
        }

        public void SetLightPosition(Vector3 lightPosition) => _lightPosition = lightPosition;
        public void SetWorld(Matrix world) => _modelWorld = world;
        public void SetEffect(Effect effect) => basicEffect = effect;
        public void SetTexture(Texture texture) => _texture = texture;

        public void Draw(GraphicsDevice graphicsDevice, Matrix projection, Matrix view)
        {
            basicEffect.Parameters["World"].SetValue(_modelWorld);
            basicEffect.Parameters["View"].SetValue(view);
            basicEffect.Parameters["Projection"].SetValue(projection);
            basicEffect.Parameters["lightPosition"].SetValue(_lightPosition);
            basicEffect.Parameters["SpriteTexture"].SetValue(_texture);

            graphicsDevice.BlendState = BlendState.Opaque;
            graphicsDevice.DepthStencilState = DepthStencilState.Default;
            graphicsDevice.SamplerStates[0] = SamplerState.LinearWrap;
            graphicsDevice.RasterizerState = RasterizerState.CullNone;
            
            graphicsDevice.SetVertexBuffer(_vertexBuffer);
            graphicsDevice.Indices = _indexBuffer;

            foreach (EffectPass pass in basicEffect.CurrentTechnique.Passes)
            {
                pass.Apply();
                graphicsDevice.DrawIndexedPrimitives(PrimitiveType.TriangleList, 0, 0, _mesh.Vertices.Length, 0, _mesh.Indices.Length / 3);
            }
        }

        private void LoadModel(GraphicsDevice graphicsDevice)
        {
            ModelVertexType[] vertices = new ModelVertexType[_mesh.Vertices.Length];
            Console.WriteLine($"vertex lenght: {_mesh.Vertices.Length}");
            Console.WriteLine($"joints lenght: {_mesh.Joints.Length}");
            Console.WriteLine($"weight lenght: {_mesh.Weights.Length}");
            for (int i = 0; i < _mesh.Vertices.Length; i++)
            {
                vertices[i] = new ModelVertexType(_mesh.Vertices[i], Color.White, _mesh.Normals[i], _mesh.TexCoords[i], _mesh.Joints[i], _mesh.Weights[i]);
                //Console.WriteLine(vertices[i].ToString());
            }

            _vertexBuffer = new VertexBuffer(graphicsDevice, typeof(ModelVertexType), _mesh.Vertices.Length, BufferUsage.WriteOnly);
            _vertexBuffer.SetData<ModelVertexType>(vertices);

            _indexBuffer = new IndexBuffer(graphicsDevice, typeof(short), _mesh.Indices.Length, BufferUsage.WriteOnly);
            _indexBuffer.SetData(_mesh.Indices);
        }
    }
}
