using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Game3D
{
    public  class Camera
    {
        public float CAM_HEIGHT = 12;
        public float HEAD_OFFSET = 12;
        public const float FAR_PLANE = 2000;

        public Vector3 pos, target;
        public Matrix view, proj, view_proj;
        public Vector3 up;
        float current_angle = 45.0f;
        float angle_velocity;
        float radius = 100.0f;
        Vector3 unit_direction;

        public Camera(GraphicsDevice gpu, Vector3 UpDirection)
        {
            up = UpDirection;
            pos = new Vector3(20,12, -90);
            target = Vector3.Zero;

            view = Matrix.CreateLookAt(pos, target, up);
            proj = Matrix.CreatePerspectiveFieldOfView(MathHelper.PiOver4, gpu.Viewport.AspectRatio, 1.0f, FAR_PLANE);
            view_proj = view * proj;
        }

        public void MoveCamera(Vector3 move)
        {
            pos += move;
            view = Matrix.CreateLookAt(pos, target, up);
            view_proj = view * proj;
        }

        public void UpdateTarget(Vector3 new_target)
        {
            target = new_target;
            view = Matrix.CreateLookAt(pos, target, pos);
            view_proj = view * proj;
        }
    }
}
