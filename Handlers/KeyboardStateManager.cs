using System;
using Microsoft.Xna.Framework.Input;

namespace MongameComparativeProject.Controls
{
    public class KeyboardStateManager
    {
        //the point of this class is to prevent the continuous movement of the player and
        //restrict them to on movement increment of 64 pixels per key press

        static KeyboardState currentKeyState;               //the key currently pressed
        static KeyboardState previousKeyState;              //the last key pressed

        public static KeyboardState GetState()
        {
            /*
             *  Method:         GetState()
             *  
             *  Method Return:  KeyboardState
             *  
             *  Parameters:     None
             *  
             *  Synopsis:       Gets the key currently being pressed 
             *  
             *  Modifications:  Date:       Name:           Modification:
             *                  05/11/2022  Jared Shaddick  Initial Setup
            */
            previousKeyState = currentKeyState;
            currentKeyState = Keyboard.GetState();
            return currentKeyState;
        }

        public static bool IsPressed(Keys key)          //checks if a key is pressed
        {
            return currentKeyState.IsKeyDown(key);
        }

        public static bool HasNotBeenPressed(Keys key)      //checks if the key hasn't been pressed
        {
            return currentKeyState.IsKeyDown(key) && !previousKeyState.IsKeyDown(key);
        }
    }
}
