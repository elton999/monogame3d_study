using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using UmbrellaToolsKit.Animation3D;
using Model = UmbrellaToolsKit.Animation3D.Model;

namespace _3dAnimation
{
    public class Game1 : Game
    {
        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;
        private Model _model;
        private Mesh _mesh;
        private Animator _animator;

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
            _model.SetDebugState(Model.Debug.NONE);

            _model.World = Matrix.CreateTranslation(0, 0, 0) * Matrix.CreateScale(0.01f);
            _model.View = Matrix.CreateLookAt(new Vector3(0, 4, 10), new Vector3(0, 3, 0), new Vector3(0, 1, 0));
            _model.Projection = Matrix.CreatePerspectiveFieldOfView(MathHelper.ToRadians(45), 800f / 480f, 0.01f, 1000f);

            _animator = new Animator(_mesh.Skeleton);
            _animator.PlayAnimation("Walking");
        }

        protected override void Update(GameTime gameTime)
        {
            float deltaTime = (float)gameTime.ElapsedGameTime.TotalSeconds;
            _animator.Update(deltaTime);
            _model.World *= Matrix.CreateRotationY(0.01f);
            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            _model.Draw();
        }
    }
}
