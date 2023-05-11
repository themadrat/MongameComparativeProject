using MongameComparativeProject;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using MongameComparativeProject.Controls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MongameComparativeProject.GameStates
{
    public class MenuState : State
    {
        //The main menu of the game

        private readonly SpriteFont font; //the font to be used for text

        private List<Component> buttonComponents; //this list will house the buttons for starting and exiting the game

        public MenuState(ContentManager content, Game1 game, GraphicsDevice graphicsDevice) : base(content, game, graphicsDevice)
        {
            font = content.Load<SpriteFont>("Fonts/Font");                      //loads the font
            var buttonTexture = content.Load<Texture2D>("Assets/Button2");      //loads the button texture
            var buttonFont = content.Load<SpriteFont>("Fonts/Font");            //loads the font for the buttons
            var startButton = new Button(buttonTexture, buttonFont)             //start button
            {
                Position = new Vector2(640 - 100, 350),                         //button position
                Text = "Start Game",                                            //button text
            };

            startButton.Click += StartGameButton_Click;                         //Triggers the method that starts the game

            var quitButton = new Button(buttonTexture, buttonFont)              //quit button
            {
                Position = new Vector2(640 - 100 , 450 + 50),                   //button position
                Text = "Exit Game",                                             //button text
            };

            quitButton.Click += QuitButton_Click;                               //Triggers the method that closes the game

            buttonComponents = new List<Component>()                            //instantiates the list that will hold the buttons
            {
                startButton,
                quitButton,
            };
        }

        public void StartGameButton_Click(object sender, EventArgs e)
        {
            /*
             *  Method:             StartGameButton_Click()
             *  
             *  Method Return:      Void
             *  
             *  Method Parameters:  object sender, EventArgs e
             *  
             *  Synopsis:           When triggered by the user clicking the start button
             *                      the application will transition from the menu state
             *                      to the game state
             * 
             *  Modifications:      Date:       Name:           Modification:
             *                      05/11/2022  Jared Shaddick  Initial Setup
            */
            game.ChangeState(new GameState(content, game, graphicsDevice)); //changes from the menu state to the game state
        }

        public void QuitButton_Click(object sender, EventArgs e)
        {
            /*
             *  Method:             QuitButton_Click()
             *  
             *  Method Return:      Void
             *  
             *  Method Parameters:  object sender, EventArgs e
             *  
             *  Synopsis:           When triggered by the user clicking the quit button
             *                      the game will close
             * 
             *  Modifications:      Date:       Name:           Modification:
             *                      05/11/2022  Jared Shaddick  Initial Setup
            */
            game.Exit(); //closes the game
        }

        public override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            spriteBatch.Begin(); //starts the spritebatch

            spriteBatch.DrawString(font, "The Chicken's Road Crossing", new Vector2(450, 200), Color.Red); //draws the title text

            foreach(var component in buttonComponents)  //draws the buttons in the components list
            {
                component.Draw(gameTime, spriteBatch);  //draws buttons
            }

            spriteBatch.End();  //ends the spritebatch
        }

        public override void Update(GameTime gameTime)
        {
            foreach (var component in buttonComponents)
                component.Update(gameTime);     //updates the buttons
        }
    }
}
