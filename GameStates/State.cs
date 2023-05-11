using MongameComparativeProject;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MongameComparativeProject.GameStates
{
    public abstract class State
    {
        #region Fields

        protected ContentManager content;

        protected Game1 game;

        protected GraphicsDevice graphicsDevice;

        #endregion

        #region Methods

        public State(ContentManager content, Game1 game, GraphicsDevice graphicsDevice) //constructor
        {
            this.content = content;
            this.game = game;
            this.graphicsDevice = graphicsDevice;
        }

        public abstract void Draw(GameTime gameTime, SpriteBatch spriteBatch);      //the draw method for the different states

        public abstract void Update(GameTime gameTime);                 //update method for the different states

        #endregion
    }
}
