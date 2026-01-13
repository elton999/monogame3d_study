using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace _3dAnimation
{
    public class Game1 : Game
    {
        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;
        private Model _model;
        private Mesh _mesh;
        private RenderLine _line;

        public Game1()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
        }

        protected override void Initialize()
        {
            base.Initialize();
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);
            var effect = Content.Load<Effect>("basicAnimationShader");
            var texture = Content.Load<Texture>("WomanTex");
            _mesh = new Mesh(@"Content/Woman.gltf");
            _mesh.Skeleton.ComputeBindPose();
            _model = new Model(_graphics.GraphicsDevice, _mesh, effect);
            _model.SetTexture(texture);
            _line = new RenderLine(GraphicsDevice);

            Console.WriteLine(_mesh.Skeleton.ToString());
        }

        protected override void Update(GameTime gameTime)
        {
            _mesh.Skeleton.Update();
            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            _model.Draw();

            Matrix worldScale = Matrix.CreateScale(0.01f);
            Matrix view = Matrix.CreateLookAt(new Vector3(0, 3, 20), new Vector3(0, 3, 0), new Vector3(0, 1, 0));
            Matrix projection = Matrix.CreatePerspectiveFieldOfView(MathHelper.ToRadians(45), 800f / 480f, 0.01f, 100f);

            foreach (var joint in _mesh.Skeleton.Joints)
            {
                if (!joint.HasParent)
                    continue;


                Vector3 a = Vector3.Transform(
                    joint.WorldMatrix.Translation,
                    worldScale
                );

                Vector3 b = Vector3.Transform(
                    joint.Parent.WorldMatrix.Translation,
                    worldScale
                );

                _line.DrawLine(GraphicsDevice, a, b, view, projection, Color.Green);
            }
        }
    }
}
