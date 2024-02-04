using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace UmbrellaToolsKit.Animation3D
{
    public class Model
    {
        private Mesh _mesh;

        private VertexBuffer _vertexBuffer;
        private IndexBuffer _indexBuffer;
        
        private Matrix _modelProjection;
        private Texture _texture;

        private Effect basicEffect;
        private Vector3 _lightPosition;

        public Model(Mesh mesh, GraphicsDevice graphicsDevice)
        {
            _mesh = mesh;

            LoadModel(graphicsDevice);
        }

        public void SetLightPosition(Vector3 lightPosition) => _lightPosition = lightPosition;
        public void SetProjection(Matrix projection) => _modelProjection = projection;
        public void SetEffect(Effect effect) => basicEffect = effect;
        public void SetTexture(Texture texture) => _texture = texture;

        public void Draw(GraphicsDevice graphicsDevice, Matrix world, Matrix view)
        {
            basicEffect.Parameters["World"].SetValue(world);
            basicEffect.Parameters["View"].SetValue(view);
            basicEffect.Parameters["Projection"].SetValue(_modelProjection);
            basicEffect.Parameters["lightPosition"].SetValue(_lightPosition);
            basicEffect.Parameters["SpriteTexture"].SetValue(_texture);

            graphicsDevice.SetVertexBuffer(_vertexBuffer);
            graphicsDevice.Indices = _indexBuffer;

            RasterizerState rasterizerState = new RasterizerState();
            rasterizerState.CullMode = CullMode.None;
            graphicsDevice.RasterizerState = rasterizerState;

            foreach (EffectPass pass in basicEffect.CurrentTechnique.Passes)
            {
                pass.Apply();
                graphicsDevice.DrawIndexedPrimitives(PrimitiveType.TriangleList, 0, 0, _mesh.Vertices.Length, 0, _mesh.Indices.Length / 3);
            }
        }

        private void LoadModel(GraphicsDevice graphicsDevice)
        {
            VertexPositionColorNormalTexture[] vertices = new VertexPositionColorNormalTexture[_mesh.Vertices.Length];
            for (int i = 0; i < _mesh.Vertices.Length; i++)
            {
                vertices[i] = new VertexPositionColorNormalTexture(_mesh.Vertices[i], Color.White, _mesh.Normals[i], _mesh.TexCoords[i]);
            }
            _vertexBuffer = new VertexBuffer(graphicsDevice, typeof(VertexPositionColorNormalTexture), _mesh.Vertices.Length, BufferUsage.WriteOnly);
            _vertexBuffer.SetData<VertexPositionColorNormalTexture>(vertices);

            _indexBuffer = new IndexBuffer(graphicsDevice, typeof(short), _mesh.Indices.Length, BufferUsage.WriteOnly);
            _indexBuffer.SetData(_mesh.Indices);
        }
    }
}
