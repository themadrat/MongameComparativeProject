using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MongameComparativeProject.GameStates;

namespace MongameComparativeProject
{
    public class Game1 : Game
    {
        private GraphicsDeviceManager graphics;
        private SpriteBatch spriteBatch;

        private State currentState;
        private State nextState;

        public void ChangeState(State state)
        {
            nextState = state;
        }

        public Game1()                                  //sets up the screen dimensions and the root directory
        {
            graphics = new GraphicsDeviceManager(this);
            graphics.PreferredBackBufferWidth = 1280;
            graphics.PreferredBackBufferHeight = 768;
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
        }

        protected override void Initialize()        //initializes the game screen
        {
            IsMouseVisible = true;

            base.Initialize();
        }

        protected override void LoadContent()       //loads the graphicsdevice into the sprite batch and sets the initial state
        {
            spriteBatch = new SpriteBatch(GraphicsDevice);

            currentState = new MenuState(Content, this, graphics.GraphicsDevice);

        }

        protected override void Update(GameTime gameTime)           //manages the changing of states
        {
            if (nextState != null)
            {
                currentState = nextState;

                nextState = null;
            }

            currentState.Update(gameTime);

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)             //draws all content from the current state
        {
            GraphicsDevice.Clear(Color.Black);

            currentState.Draw(gameTime, spriteBatch);

            base.Draw(gameTime);
        }

        
    }
}