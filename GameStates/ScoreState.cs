using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace MongameComparativeProject.GameStates
{
    public class GameOverState : State
    {
        private readonly SpriteFont font;   //spritefont that will be used to display text
        public GameOverState(ContentManager content, Game1 game, GraphicsDevice graphicsDevice) : base(content, game, graphicsDevice)
        {
            font = content.Load<SpriteFont>("Fonts/BigFont"); //loads and instantiates the spritefont
        }
        public override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            spriteBatch.Begin(); //begins spritebatch

            spriteBatch.DrawString(font, "Game Over\nIf You Beat Your High Score\nIt Will be Displayed\nIn The Next Game\nPress Escape to Exit", new Vector2(200, 200), Color.Red); //draws the game over text

            spriteBatch.End(); //ends spritebatch
        }

        public override void Update(GameTime gameTime)
        {
            if (Keyboard.GetState().IsKeyDown(Keys.Escape)) //conditional that is met by pressing the escape key
                game.Exit(); //closes the game
        }
    }
}
