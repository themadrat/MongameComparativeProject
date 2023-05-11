using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using Microsoft.Xna.Framework.Input;
using MongameComparativeProject.Controls;
using System.IO;
namespace MongameComparativeProject.GameStates
{
    public class GameState : State
    {
        private readonly SpriteFont font; //font for in-game text

        private KeyboardState keyboardState; //class variable that will be used to prevent continuous movement by holding down keys

        protected Texture2D chknSprite;
        protected Texture2D[] tileArray;

        private Vector2 chknPosition;
        private Vector2 previousChknPosition;
        private Vector2[] barrierPositions;
        private Vector2[] goalPositions;

        private Rectangle[] vehiclePositions;
        private Rectangle chknRectangle;

        protected Texture2D chknCoopTile;
        protected Texture2D upperSidewalkTile;
        protected Texture2D lowerSidewalkTile;
        protected Texture2D roadTile;
        protected Texture2D grassTile;
        protected Texture2D barricadeTile;
        protected Texture2D tractorTrailerSprite;
        protected Texture2D tractorTrailerSpriteLeft;

        private int leftVehicleSpeed = -1;
        private int rightVehicleSpeed = 1;
        private int vehicleSpeedMultiplier = 1;
        private int maxSpeedMultiplier = 16;

        private bool playerMoved = false;
        private bool playerDied = false;
        private bool coopReached = false;

        private static byte chknMoveSpeed = 64;

        private short lives;
        private short maxLives = 100;

        private ulong score;
        private ulong lvlUpScore = 1000;
        private ulong maxLvlUpMultiplier = 497;
        private ulong lvlUpMultiplier = 1;
        private ulong maxScore = 18000000000000000000;
        private string highScoreString;

        string[,] tileMap;

        private int indexX;
        private int indexY;

        private string tileMapData;

        public GameState(ContentManager content, Game1 game, GraphicsDevice graphicsDevice) : base(content, game, graphicsDevice)
        {
            highScoreString = "";
            getHighScore();
            lives = 3;
            score = 0;

            font = content.Load<SpriteFont>("Fonts/Font");

            chknSprite = content.Load<Texture2D>("Assets/DuckSprite64");
            chknPosition = new Vector2(576, 704);
            chknRectangle = new Rectangle((int)chknPosition.X, (int)chknPosition.Y, 64, 64);
            previousChknPosition = chknPosition;

            upperSidewalkTile = content.Load<Texture2D>("Assets/UpperSidewalkTile64");
            lowerSidewalkTile = content.Load<Texture2D>("Assets/LowerSidewalkTile64");
            chknCoopTile = content.Load<Texture2D>("Assets/ChknCoopTile64");
            roadTile = content.Load<Texture2D>("Assets/Road");
            grassTile = content.Load<Texture2D>("Assets/GrassTile64");
            barricadeTile = content.Load<Texture2D>("Assets/barrier");

            tractorTrailerSprite = content.Load<Texture2D>("Assets/18WheelerSprite64");
            tractorTrailerSpriteLeft = content.Load<Texture2D>("Assets/18WheelerSprite64Left");


            TileArray();

            GoalArray();

            BarrierArray();

            VehiclePositionArrays();

            tileMap = LoadTileMapFile();
        }

        public override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            spriteBatch.Begin();

            Vector2 tilePositon;
            Texture2D tileTexture;
            int tileNum;

            indexY = 0;
            indexX = 0;
            while (indexY <= 11)
            {
                indexX = 0;
                while (indexX <= 11)
                {
                    tilePositon = TilePos(indexX, indexY);
                    tileNum = int.Parse(tileMap[indexX, indexY]);
                    tileTexture = getTexture(tileArray, tileNum);
                    spriteBatch.Draw(tileTexture, tilePositon, Color.White);
                    indexX++;
                }
                indexY++;
            }

            spriteBatch.Draw(tractorTrailerSprite, vehiclePositions[0], Color.White);
            spriteBatch.Draw(tractorTrailerSprite, vehiclePositions[1], Color.White);
            spriteBatch.Draw(tractorTrailerSprite, vehiclePositions[2], Color.White);
            spriteBatch.Draw(tractorTrailerSprite, vehiclePositions[3], Color.White);
            spriteBatch.Draw(tractorTrailerSpriteLeft, vehiclePositions[4], Color.White);
            spriteBatch.Draw(tractorTrailerSpriteLeft, vehiclePositions[5], Color.White);
            spriteBatch.Draw(tractorTrailerSpriteLeft, vehiclePositions[6], Color.White);
            spriteBatch.Draw(tractorTrailerSprite, vehiclePositions[7], Color.White);
            spriteBatch.Draw(tractorTrailerSprite, vehiclePositions[8], Color.White);
            spriteBatch.Draw(tractorTrailerSpriteLeft, vehiclePositions[9], Color.White);
            spriteBatch.Draw(tractorTrailerSpriteLeft, vehiclePositions[10], Color.White);
            spriteBatch.Draw(tractorTrailerSpriteLeft, vehiclePositions[11], Color.White);
            spriteBatch.Draw(tractorTrailerSpriteLeft, vehiclePositions[12], Color.White);
            spriteBatch.Draw(tractorTrailerSprite, vehiclePositions[13], Color.White);
            spriteBatch.Draw(tractorTrailerSprite, vehiclePositions[14], Color.White);

            spriteBatch.Draw(chknSprite, chknRectangle, Color.White);

            spriteBatch.DrawString(font, "score:\n" + score.ToString(), new Vector2((192 / 2) - 25, 250), Color.Red);
            spriteBatch.DrawString(font, "high score:\n" + highScoreString.ToString(), new Vector2((192 / 2) - 25, 100), Color.Red);
            spriteBatch.DrawString(font, "lives: " + lives.ToString(), new Vector2(1024 + (192 / 2), 250), Color.Red);

            spriteBatch.End();

        }

        private Texture2D getTexture(Texture2D[] tile, int tileNum)
        {
            Texture2D theTile = tile[tileNum];
            return theTile;
        }

        private Vector2 TilePos(int X, int Y)
        {
            Vector2 positionOfTile = new Vector2(256 +(X * 64), Y * 64);

            return positionOfTile;
        }

        public override void Update(GameTime gameTime)
        {
            vehicleSpeedHandling();

            keyboardState = KeyboardStateManager.GetState();

            if (keyboardState.GetPressedKeyCount() > 0)
            {
                HandleInput();
                EdgeOfMapHandling();
                RoadBarrierCollisionHandling();
            }

            CarMovementHandler();

            CarCollisions();

            ScoreHandling();

            LifeHandling();
        }

        #region FileLoading
        private string[,] LoadTileMapFile()
        {
            StreamReader mapReader = File.OpenText("Content/TileMap.txt");

            tileMapData = mapReader.ReadToEnd();
            mapReader.Close();
            tileMap = new string[12, 12];


            foreach (var row in tileMapData.Split('\n'))
            {
                if (indexY <= 11)
                {
                    indexX = 0;
                    foreach (var col in row.Trim().Split(" "))
                    {

                        if (indexX <= 11)
                        {
                            tileMap[indexX, indexY] = col.Trim();

                            indexX++;
                        }
                    }
                    indexY++;
                }


            }
            return tileMap;
        }

        private void getHighScore()
        {
            StreamReader scoreReader = File.OpenText(@"Content/HighScore.txt");
            highScoreString = scoreReader.ReadLine();
            scoreReader.Close();
        }

        #endregion

        #region StatsHandling
        private void vehicleSpeedHandling()
        {
            vehiclePositions[0].X += rightVehicleSpeed * vehicleSpeedMultiplier;
            vehiclePositions[1].X += rightVehicleSpeed * vehicleSpeedMultiplier;
            vehiclePositions[2].X += rightVehicleSpeed * vehicleSpeedMultiplier;
            vehiclePositions[3].X += rightVehicleSpeed * vehicleSpeedMultiplier;
            vehiclePositions[4].X += leftVehicleSpeed * vehicleSpeedMultiplier;
            vehiclePositions[5].X += leftVehicleSpeed * vehicleSpeedMultiplier;
            vehiclePositions[6].X += leftVehicleSpeed * vehicleSpeedMultiplier;
            vehiclePositions[7].X += rightVehicleSpeed * vehicleSpeedMultiplier;
            vehiclePositions[8].X += rightVehicleSpeed * vehicleSpeedMultiplier;
            vehiclePositions[9].X += leftVehicleSpeed * vehicleSpeedMultiplier;
            vehiclePositions[10].X += leftVehicleSpeed * vehicleSpeedMultiplier;
            vehiclePositions[11].X += leftVehicleSpeed * vehicleSpeedMultiplier;
            vehiclePositions[12].X += leftVehicleSpeed * vehicleSpeedMultiplier;
            vehiclePositions[13].X += rightVehicleSpeed * vehicleSpeedMultiplier;
            vehiclePositions[14].X += rightVehicleSpeed * vehicleSpeedMultiplier;
        }
        private void ScoreHandling()
        {
            if (playerMoved == true && playerDied == false && chknPosition != previousChknPosition && score != maxScore)
            {
                score += 25;
                playerMoved = false;
                previousChknPosition.X = chknPosition.X;
                previousChknPosition.Y = chknPosition.Y;
            }

            if (chknPosition.X == goalPositions[0].X && chknPosition.Y == goalPositions[0].Y &&
                score < maxScore)
            {
                coopReached = true;
                score += 100;
                chknPosition.X = 576;
                chknPosition.Y = 704;

            }

            else if (chknPosition.X == goalPositions[1].X && chknPosition.Y == goalPositions[1].Y &&
                score < maxScore)
            {
                coopReached = true;
                score += 100;
                chknPosition.X = 576;
                chknPosition.Y = 704;
            }

            else if (chknPosition.X == goalPositions[2].X && chknPosition.Y == goalPositions[2].Y &&
                score < maxScore)
            {
                coopReached = true;
                score += 100;
                chknPosition.X = 576;
                chknPosition.Y = 704;
            }

            else if (chknPosition.X == goalPositions[3].X && chknPosition.Y == goalPositions[3].Y &&
                score < maxScore)
            {
                coopReached = true;
                score += 100;
                chknPosition.X = 576;
                chknPosition.Y = 704;
            }

            if (vehicleSpeedMultiplier < maxSpeedMultiplier && coopReached)
            {
                vehicleSpeedMultiplier++;
                coopReached = false;
            }

            chknRectangle.X = (int)chknPosition.X;
            chknRectangle.Y = (int)chknPosition.Y;
        }

        private void LifeHandling()
        {
            if (playerDied == true)
            {
                lives--;
                chknPosition.X = 576;
                chknPosition.Y = 704;
                chknRectangle.X = (int)chknPosition.X;
                chknRectangle.Y = (int)chknPosition.Y;
                playerDied = false;
            }

            if (lives < maxLives && lvlUpMultiplier != maxLvlUpMultiplier && score >= lvlUpScore * lvlUpMultiplier)
            {
                lives++;
                lvlUpMultiplier++;
            }

            if (lives <= 0)
            {
                if ((ulong)int.Parse(highScoreString) < score)
                {
                    highScoreString = score.ToString();
                    FileStream fileStream = File.Open("Content/HighScore.txt", FileMode.OpenOrCreate, FileAccess.Write);
                    StreamWriter scoreWriter = new StreamWriter(fileStream);
                    scoreWriter.WriteLine(highScoreString);
                    scoreWriter.Close();
                }
                game.ChangeState(new GameOverState(content, game, graphicsDevice));
            }
        }

        #endregion

        #region Arrays
        private void VehiclePositionArrays()
        {
            vehiclePositions = new Rectangle[15];

            vehiclePositions[0] = new Rectangle(256, 640, 64, 64);
            vehiclePositions[1] = new Rectangle(640, 640, 64, 64);
            vehiclePositions[2] = new Rectangle(768, 640, 64, 64);
            vehiclePositions[3] = new Rectangle(448, 640, 64, 64);
            vehiclePositions[4] = new Rectangle(384, 576, 64, 64);
            vehiclePositions[5] = new Rectangle(576, 576, 64, 64);
            vehiclePositions[6] = new Rectangle(960, 576, 64, 64);
            vehiclePositions[7] = new Rectangle(448, 448, 64, 64);
            vehiclePositions[8] = new Rectangle(768, 448, 64, 64);
            vehiclePositions[9] = new Rectangle(256, 384, 64, 64);
            vehiclePositions[10] = new Rectangle(704, 384, 64, 64);
            vehiclePositions[11] = new Rectangle(192, 256, 64, 64);
            vehiclePositions[12] = new Rectangle(512, 256, 64, 64);
            vehiclePositions[13] = new Rectangle(640, 192, 64, 64);
            vehiclePositions[14] = new Rectangle(960, 192, 64, 64);
        }

        private void BarrierArray()
        {
            barrierPositions = new Vector2[22];

            barrierPositions[0].X = 256;
            barrierPositions[0].Y = 512;

            barrierPositions[1].X = 384;
            barrierPositions[1].Y = 512;

            barrierPositions[2].X = 640;
            barrierPositions[2].Y = 512;

            barrierPositions[3].X = 768;
            barrierPositions[3].Y = 512;

            barrierPositions[4].X = 896;
            barrierPositions[4].Y = 512;

            barrierPositions[5].X = 960;
            barrierPositions[5].Y = 512;

            barrierPositions[6].X = 256;
            barrierPositions[6].Y = 320;

            barrierPositions[7].X = 320;
            barrierPositions[7].Y = 320;

            barrierPositions[8].X = 448;
            barrierPositions[8].Y = 320;

            barrierPositions[9].X = 512;
            barrierPositions[9].Y = 320;

            barrierPositions[10].X = 704;
            barrierPositions[10].Y = 320;

            barrierPositions[11].X = 768;
            barrierPositions[11].Y = 320;

            barrierPositions[12].X = 896;
            barrierPositions[12].Y = 320;

            barrierPositions[13].X = 960;
            barrierPositions[13].Y = 320;

            barrierPositions[14].X = 256;
            barrierPositions[14].Y = 0;

            barrierPositions[15].X = 320;
            barrierPositions[15].Y = 0;

            barrierPositions[16].X = 448;
            barrierPositions[16].Y = 0;

            barrierPositions[17].X = 512;
            barrierPositions[17].Y = 0;

            barrierPositions[18].X = 704;
            barrierPositions[18].Y = 0;

            barrierPositions[19].X = 768;
            barrierPositions[19].Y = 0;

            barrierPositions[20].X = 896;
            barrierPositions[20].Y = 0;

            barrierPositions[21].X = 960;
            barrierPositions[21].Y = 0;
        }

        private void GoalArray()
        {
            goalPositions = new Vector2[4];

            goalPositions[0].X = 384;
            goalPositions[0].Y = 0;

            goalPositions[1].X = 576;
            goalPositions[1].Y = 0;

            goalPositions[2].X = 640;
            goalPositions[2].Y = 0;

            goalPositions[3].X = 832;
            goalPositions[3].Y = 0;
        }

        private void TileArray()
        {
            tileArray = new Texture2D[6];

            tileArray[0] = upperSidewalkTile;
            tileArray[1] = lowerSidewalkTile;
            tileArray[2] = chknCoopTile;
            tileArray[3] = roadTile;
            tileArray[4] = grassTile;
            tileArray[5] = barricadeTile;
        }
        #endregion

        #region ControlAndEdgeHandling
        protected void HandleInput()
        {
            if (Keyboard.GetState().IsKeyDown(Keys.Escape))
                game.Exit();

            if (keyboardState.IsKeyDown(Keys.W) && KeyboardStateManager.HasNotBeenPressed(Keys.W))
            {
                previousChknPosition.X = chknPosition.X;
                previousChknPosition.Y = chknPosition.Y;
                chknPosition.Y -= chknMoveSpeed; //move up
                playerMoved = true;
            };

            if (keyboardState.IsKeyDown(Keys.S) && KeyboardStateManager.HasNotBeenPressed(Keys.S))
            {
                previousChknPosition.X = chknPosition.X;
                previousChknPosition.Y = chknPosition.Y;
                chknPosition.Y += chknMoveSpeed; //move down
                playerMoved = true;

            }

            if (keyboardState.IsKeyDown(Keys.A) && KeyboardStateManager.HasNotBeenPressed(Keys.A))
            {
                previousChknPosition.X = chknPosition.X;
                previousChknPosition.Y = chknPosition.Y;
                chknPosition.X -= chknMoveSpeed; //move left
                playerMoved = true;
            }

            if (keyboardState.IsKeyDown(Keys.D) && KeyboardStateManager.HasNotBeenPressed(Keys.D))
            {
                previousChknPosition.X = chknPosition.X;
                previousChknPosition.Y = chknPosition.Y;
                chknPosition.X += chknMoveSpeed; //move right
                playerMoved = true;

            }
            chknRectangle.X = (int)chknPosition.X;
            chknRectangle.Y = (int)chknPosition.Y;
        }

        protected void EdgeOfMapHandling()
        {
            if (chknPosition.X < 256)
            {
                chknPosition.X = 256;
                previousChknPosition.X = chknPosition.X;
                previousChknPosition.Y = chknPosition.Y;
            }

            if (chknPosition.X >= 1024)
            {
                chknPosition.X = 1024 - 64;
                previousChknPosition.X = chknPosition.X;
                previousChknPosition.Y = chknPosition.Y;
            }

            if (chknPosition.Y < 0)
            {
                chknPosition.Y = 0;
                previousChknPosition.Y = chknPosition.Y;
                previousChknPosition.X = chknPosition.X;
            }

            if (chknPosition.Y >= 768)
            {
                chknPosition.Y = 768 - 64;
                previousChknPosition.Y = chknPosition.Y;
                previousChknPosition.X = chknPosition.X;
            }
            chknRectangle.X = (int)chknPosition.X;
            chknRectangle.Y = (int)chknPosition.Y;
        }

        private void CarMovementHandler()
        {
            if (vehiclePositions[0].X > 1024)
                vehiclePositions[0].X = 192;

            if (vehiclePositions[1].X > 1024)
                vehiclePositions[1].X = 192;

            if (vehiclePositions[2].X > 1024)
                vehiclePositions[2].X = 192;

            if (vehiclePositions[3].X > 1024)
                vehiclePositions[3].X = 192;

            if (vehiclePositions[4].X < 192)
                vehiclePositions[4].X = 1024;

            if (vehiclePositions[5].X < 192)
                vehiclePositions[5].X = 1024;

            if (vehiclePositions[6].X < 192)
                vehiclePositions[6].X = 1024;

            if (vehiclePositions[7].X > 1024)
                vehiclePositions[7].X = 192;

            if (vehiclePositions[8].X > 1024)
                vehiclePositions[8].X = 192;

            if (vehiclePositions[9].X < 192)
                vehiclePositions[9].X = 1024;

            if (vehiclePositions[10].X < 192)
                vehiclePositions[10].X = 1024;

            if (vehiclePositions[11].X < 192)
                vehiclePositions[11].X = 1024;

            if (vehiclePositions[12].X < 192)
                vehiclePositions[12].X = 1024;

            if (vehiclePositions[13].X > 1024)
                vehiclePositions[13].X = 192;

            if (vehiclePositions[14].X > 1024)
                vehiclePositions[14].X = 192;
        }
        #endregion

        #region CollisionHandling
        protected void RoadBarrierCollisionHandling()
        {
            if (chknPosition.X == barrierPositions[0].X && chknPosition.Y == barrierPositions[0].Y)
                chknPosition = previousChknPosition;
            
            else if (chknPosition.X == barrierPositions[1].X && chknPosition.Y == barrierPositions[1].Y)
                chknPosition = previousChknPosition;

            else if (chknPosition.X == barrierPositions[2].X && chknPosition.Y == barrierPositions[2].Y)
                chknPosition = previousChknPosition;

            else if (chknPosition.X == barrierPositions[3].X && chknPosition.Y == barrierPositions[3].Y)
                chknPosition = previousChknPosition;

            else if (chknPosition.X == barrierPositions[4].X && chknPosition.Y == barrierPositions[4].Y)
                chknPosition = previousChknPosition;

            else if (chknPosition.X == barrierPositions[5].X && chknPosition.Y == barrierPositions[5].Y)
                chknPosition = previousChknPosition;

            else if (chknPosition.X == barrierPositions[6].X && chknPosition.Y == barrierPositions[6].Y)
                chknPosition = previousChknPosition;

            else if (chknPosition.X == barrierPositions[7].X && chknPosition.Y == barrierPositions[7].Y)
                chknPosition = previousChknPosition;

            else if (chknPosition.X == barrierPositions[8].X && chknPosition.Y == barrierPositions[8].Y)
                chknPosition = previousChknPosition;

            else if (chknPosition.X == barrierPositions[9].X && chknPosition.Y == barrierPositions[9].Y)
                chknPosition = previousChknPosition;

            else if (chknPosition.X == barrierPositions[10].X && chknPosition.Y == barrierPositions[10].Y)
                chknPosition = previousChknPosition;

            else if (chknPosition.X == barrierPositions[11].X && chknPosition.Y == barrierPositions[11].Y)
                chknPosition = previousChknPosition;

            else if (chknPosition.X == barrierPositions[12].X && chknPosition.Y == barrierPositions[12].Y)
                chknPosition = previousChknPosition;

            else if (chknPosition.X == barrierPositions[13].X && chknPosition.Y == barrierPositions[13].Y)
                chknPosition = previousChknPosition;

            else if (chknPosition.X == barrierPositions[14].X && chknPosition.Y == barrierPositions[14].Y)
                chknPosition = previousChknPosition;

            else if (chknPosition.X == barrierPositions[15].X && chknPosition.Y == barrierPositions[15].Y)
                chknPosition = previousChknPosition;

            else if (chknPosition.X == barrierPositions[16].X && chknPosition.Y == barrierPositions[16].Y)
                chknPosition = previousChknPosition;

            else if (chknPosition.X == barrierPositions[17].X && chknPosition.Y == barrierPositions[17].Y)
                chknPosition = previousChknPosition;

            else if (chknPosition.X == barrierPositions[18].X && chknPosition.Y == barrierPositions[18].Y)
                chknPosition = previousChknPosition;

            else if (chknPosition.X == barrierPositions[19].X && chknPosition.Y == barrierPositions[19].Y)
                chknPosition = previousChknPosition;

            else if (chknPosition.X == barrierPositions[20].X && chknPosition.Y == barrierPositions[20].Y)
                chknPosition = previousChknPosition;

            else if (chknPosition.X == barrierPositions[21].X && chknPosition.Y == barrierPositions[21].Y)
                chknPosition = previousChknPosition;
        }

        protected void CarCollisions()
        {
            if (vehiclePositions[0].Intersects(chknRectangle))
            {
                playerDied = true;
            }
            if (vehiclePositions[1].Intersects(chknRectangle))
            {
                playerDied = true;
            }
            if (vehiclePositions[2].Intersects(chknRectangle))
            {
                playerDied = true;
            }
            if (vehiclePositions[3].Intersects(chknRectangle))
            {
                playerDied = true;
            }

            if (vehiclePositions[4].Intersects(chknRectangle))
            {
                playerDied = true;
            }

            if (vehiclePositions[5].Intersects(chknRectangle))
            {
                playerDied = true;
            }

            if (vehiclePositions[6].Intersects(chknRectangle))
            {
                playerDied = true;
            }

            if (vehiclePositions[7].Intersects(chknRectangle))
            {
                playerDied = true;
            }

            if (vehiclePositions[8].Intersects(chknRectangle))
            {
                playerDied = true;
            }
            if (vehiclePositions[9].Intersects(chknRectangle))
            {
                playerDied = true;
            }
            if (vehiclePositions[10].Intersects(chknRectangle))
            {
                playerDied = true;
            }

            if (vehiclePositions[11].Intersects(chknRectangle))
            {
                playerDied = true;
            }

            if (vehiclePositions[12].Intersects(chknRectangle))
            {
                playerDied = true;
            }

            if (vehiclePositions[13].Intersects(chknRectangle))
            {
                playerDied = true;
            }

            if (vehiclePositions[14].Intersects(chknRectangle))
            {
                playerDied = true;
            }
        }
        #endregion
    }
}
