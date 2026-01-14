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
            _model = new Model(_graphics.GraphicsDevice, _mesh, effect);
            _model.SetTexture(texture);
            _mesh.Skeleton.ComputeBindPose();
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
        }
    }
}
