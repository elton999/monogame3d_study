using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Game3D
{
    public class Line
    {
        private Vector3 _start;
        private Vector3 _end;

        private Matrix _world;

        private VertexBuffer _vertexBuffer;
        private BasicEffect _basicEffect;

        private Color _color;

        public Line(Vector3 start, Vector3 end, GraphicsDevice graphicsDevice)
        {
            _color = Color.White;
            Init(start, end, graphicsDevice);
        }

        public Line(Vector3 start, Vector3 end, GraphicsDevice graphicsDevice, Color color)
        {
            _color = color;
            Init(start, end, graphicsDevice);
        }

        private void Init(Vector3 start, Vector3 end, GraphicsDevice graphicsDevice)
        {
            _start = start;
            _end = end;
            _world = Matrix.CreateTranslation(Vector3.Zero);
            SetVertex(graphicsDevice);
            _basicEffect = new BasicEffect(graphicsDevice);
        }

        public void SetWorld(Matrix world) => _world = world;

        public void Draw(GraphicsDevice graphicsDevice, Matrix projection, Matrix view)
        {
            var state = new DepthStencilState();
            state.DepthBufferEnable = false;

            graphicsDevice.DepthStencilState = state;

            _basicEffect.World = _world;
            _basicEffect.View = view;
            _basicEffect.Projection = projection;
            _basicEffect.VertexColorEnabled = true;

            graphicsDevice.SetVertexBuffer(_vertexBuffer);

            RasterizerState rasterizerState = new RasterizerState();
            rasterizerState.CullMode = CullMode.None;
            graphicsDevice.RasterizerState = rasterizerState;

            foreach (EffectPass pass in _basicEffect.CurrentTechnique.Passes)
            {
                pass.Apply();
                graphicsDevice.DrawPrimitives(PrimitiveType.LineStrip, 0, 1);
            }
            state = new DepthStencilState();
            state.DepthBufferEnable = true;

            graphicsDevice.DepthStencilState = state;
        }

        private void SetVertex(GraphicsDevice graphicsDevice)
        {
            VertexPositionColor[] vertices = new VertexPositionColor[2];
            vertices[0] = new VertexPositionColor(_start, _color);
            vertices[1] = new VertexPositionColor(_end, _color);

            _vertexBuffer = new VertexBuffer(graphicsDevice, typeof(VertexPositionColor), 2, BufferUsage.WriteOnly);
            _vertexBuffer.SetData(vertices);
        }
    }
}
