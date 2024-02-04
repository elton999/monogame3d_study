using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Game3D
{
    public class Line
    {
        private Vector3 _start;
        private Vector3 _end;

        private VertexBuffer _vertexBuffer;
        private BasicEffect _basicEffect;

        public Line(Vector3 start, Vector3 end, GraphicsDevice graphicsDevice)
        {
            _start = start;
            _end = end;
            SetVertex(graphicsDevice);
            _basicEffect = new BasicEffect(graphicsDevice);
        }

        public void Draw(GraphicsDevice graphicsDevice, Matrix projection, Matrix view)
        {
            _basicEffect.World = Matrix.CreateTranslation(0, 0, 0);
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
                graphicsDevice.DrawPrimitives(PrimitiveType.LineList, 0, 1);
            }
        }

        private void SetVertex(GraphicsDevice graphicsDevice)
        {
            VertexPositionColor[] vertices = new VertexPositionColor[2];
            vertices[0] = new VertexPositionColor(_start, Color.White);
            vertices[1] = new VertexPositionColor(_end, Color.White);

            _vertexBuffer = new VertexBuffer(graphicsDevice, typeof(VertexPositionColor), 2, BufferUsage.WriteOnly);
            _vertexBuffer.SetData(vertices);
        }
    }
}
