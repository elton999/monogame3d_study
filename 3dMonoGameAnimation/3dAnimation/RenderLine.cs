using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
namespace _3dAnimation;

public class RenderLine
{
    private BasicEffect _lineEffect;

    public RenderLine(GraphicsDevice graphicsDevice)
    {
        _lineEffect = new BasicEffect(graphicsDevice);
        _lineEffect.VertexColorEnabled = true;
    }

    public void DrawLine( GraphicsDevice graphicsDevice, Vector3 start, Vector3 end, Matrix view, Matrix projection, Color color)
    {
        var vertices = new VertexPositionColor[]
        {
        new VertexPositionColor(start, color),
        new VertexPositionColor(end, color),
        };

        _lineEffect.World = Matrix.Identity;
        _lineEffect.View = view;
        _lineEffect.Projection = projection;

        foreach (var pass in _lineEffect.CurrentTechnique.Passes)
        {
            pass.Apply();
            graphicsDevice.DrawUserPrimitives(PrimitiveType.LineList, vertices, 0, 1);
        }
    }
}
