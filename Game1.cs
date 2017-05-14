using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;

namespace _3DBubble
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class Game1 : Microsoft.Xna.Framework.Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;

        enum GameState
        {
            Start,
            Level_Startup,
            Level_Run,
            Level_Pass,
            Level_Fail
        };

        GameState currentGameState;

        SpriteFont tips;
        SpriteFont text;
        int score;
        string stringScore;

        Camera camera;
        Model blueBallModel;
        Model cyanBallModel;
        Model greenBallModel;
        Model purpleBallModel;
        Model redBallModel;
        Model yellowBallModel;

        Model rampModel;
        Model ceilingModel;
        Model arrowModel;

        // Texture info
        Texture2D texture;
        Texture2D gameStartup;
        Texture2D star;
        Texture2D background;

        SlopeObject slope;
        ArrowObject arrow;
        CeilingObject ceiling;
        CeilingObject baseline;
        

        _3DBall[][] ballArray;
        List<_3DBall> dropballList;

        //Ball2D ballOfMoving;
        _3DBall ballOfReadyToShoot;
        _3DBall ballOfNextReadyToShoot;

        int counterDown;
        float ceilingHeight;
        int maxAllowedRow;
        int arrowAngle;
        int ballMoveDirectionAngle;

        int counterForDeleteBallNumber = 0;                      //for counte the same color ball numbers
        int counterForDropableBallNumber = 0;

        Boolean ballMovable;
        Boolean canProduceNewBall;
        Boolean hasBallDropable;
        Boolean canDisplayBallPop;
        Boolean canMoveOfBallOfNextReadyToShoot;

        int pressCounter = 0;        
        Vector2 warning = new Vector2(2.0f, 0);
        Vector2 noWarning = new Vector2(0, 0);        
        Boolean releaseWarning;

        float intervalOfFire = 12000f;
        float timerOfFire = 0f;

        Vector3 ballOfNextReadyToShootVector = new Vector3(-0.35f, 0.06f, 0.55f);
        Vector3 ballOfReadyToShootVector = new Vector3(0.0f, 0.06f, 0.45f);

        float finalVelocity;


        Song song;

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";

            maxAllowedRow = 10;
            arrowAngle = 0;
            counterDown = 6;
            ballMovable = false;
            canProduceNewBall = false;
            hasBallDropable = false;
            releaseWarning = false;
            canDisplayBallPop = false;
            canMoveOfBallOfNextReadyToShoot = false;
            ceilingHeight = -0.6f;
            score = 0;
            timerOfFire = 0f;

            finalVelocity = 0.0002f;

        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            // TODO: Add your initialization logic here
            graphics.PreferredBackBufferWidth = 912;
            graphics.PreferredBackBufferHeight = 800;
            graphics.ApplyChanges();


            // Initialize camera
            camera = new Camera(this, new Vector3(0, -(float)Math.Sin(Math.PI / 6) * 0.6f + 0.1f, (float)Math.Cos(Math.PI / 6) * 0.6f + 1.2f),
                new Vector3(0, (float)Math.Sin(Math.PI / 6) * 0.6f, -(float)Math.Cos(Math.PI / 6) * 0.6f), Vector3.Up);

            //camera = new Camera(this, new Vector3(0, -0.5f, 2.0f), new Vector3(0, 0.8f, -1.0f), Vector3.Up);  //good camera
            //camera = new Camera(this, new Vector3(0, 2, 1f), Vector3.Zero, Vector3.Up);
            Components.Add(camera);

            ballArray = new _3DBall[11][];
            for (int i = 0; i < 11; i++)
            {
                ballArray[i] = new _3DBall[8];
            }

            dropballList = new List<_3DBall>();

            currentGameState = GameState.Start;

            base.Initialize();
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);

            tips = Content.Load<SpriteFont>(@"Fonts\Tips");
            text = Content.Load<SpriteFont>(@"Fonts\text");

            ceilingModel = Content.Load<Model>(@"Models\ceiling");
            arrowModel = Content.Load<Model>(@"Models\arrow");
            rampModel = Content.Load<Model>(@"Models\ramp");
            slope = new SlopeObject(rampModel);
            arrow = new ArrowObject(arrowModel);
            ceiling = new CeilingObject(ceilingModel);
            baseline = new CeilingObject(ceilingModel);
            baseline.setPosition();

            greenBallModel = Content.Load<Model>(@"Models\greenBall");
            blueBallModel = Content.Load<Model>(@"Models\blueBall");
            cyanBallModel = Content.Load<Model>(@"Models\cyanBall");
            purpleBallModel = Content.Load<Model>(@"Models\purpleBall");
            redBallModel = Content.Load<Model>(@"Models\redBall");
            yellowBallModel = Content.Load<Model>(@"Models\yellowBall");

            // Load texture
            //texture = Content.Load<Texture2D>(@"Textures\droppingBackground");
            texture = Content.Load<Texture2D>(@"Textures\texture");
            gameStartup = Content.Load<Texture2D>(@"Textures\gameStartup");
            star = Content.Load<Texture2D>(@"Textures\star");
            background = Content.Load<Texture2D>(@"Textures\background2");


            song = Content.Load<Song>(@"Sound/theme");
            MediaPlayer.Play(song);
            MediaPlayer.IsRepeating = true;
            // TODO: use this.Content to load your game content here
            

        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// all content.
        /// </summary>
        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            // Allows the game to exit
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed)
                this.Exit();

            // TODO: Add your update logic here
            KeyboardState keyboardState = Keyboard.GetState();

            if (keyboardState.IsKeyDown(Keys.Escape)) // exit
            {
                this.Exit();
            }

            if (currentGameState == GameState.Start)
            {
                if (keyboardState.IsKeyDown(Keys.Enter))
                {
                    currentGameState = GameState.Level_Startup;
                }
            }

            if (currentGameState == GameState.Level_Startup)
            {
                InitialBallDisplayLevelOne();
            }

            if (currentGameState == GameState.Level_Run)
            {
                if (keyboardState.IsKeyDown(Keys.Right))
                {
                    if (arrowAngle < 85)
                    {
                        arrowAngle = arrowAngle + 2;
                    }
                    if (arrowAngle > 85)
                    {
                        arrowAngle = 85;
                    }
                    arrow.updatePosition(arrowAngle);
                }

                if (keyboardState.IsKeyDown(Keys.Left))
                {
                    if (arrowAngle > -85)
                    {
                        arrowAngle = arrowAngle - 2;
                    }
                    if (arrowAngle < -85)
                    {
                        arrowAngle = -85;
                    }
                    arrow.updatePosition(arrowAngle);
                }


                if (ballMovable == false)
                {
                    if (keyboardState.IsKeyDown(Keys.Up))
                    {
                        timerOfFire = 0f;
                        pressCounter++;
                        ballMovable = true;
                        ballMoveDirectionAngle = arrowAngle;
                        canMoveOfBallOfNextReadyToShoot = true;
                        counterDown = 6;
                        ballOfReadyToShoot.movingUnitDirection = new Vector3((float)Math.Sin(arrowAngle * Math.PI / 180), 0, -(float)Math.Cos(arrowAngle * Math.PI / 180));
                        ballOfReadyToShoot.verticalVelocity = (float)Math.Sqrt(Math.Pow(finalVelocity, 2) - 2 * (-ballOfReadyToShoot.gravity) * (float)Math.Cos(arrowAngle * Math.PI / 180) * (ballOfReadyToShootVector.Z - ceilingHeight));
                        ballOfReadyToShoot.horizontalVelocity = ballOfReadyToShoot.verticalVelocity;
              
                    }
                }

                PlayGame(gameTime);
            }

            if ((currentGameState == GameState.Level_Run) || (currentGameState == GameState.Level_Pass))
            {
                displayScoreFormat();
            }

            if (currentGameState == GameState.Level_Pass)
            {
                dropBall();
            }
            

            base.Update(gameTime);
        }

        


        private void PlayGame(GameTime gameTime)
        {
            if (canMoveOfBallOfNextReadyToShoot == true)
            {
                double moveDistance = Math.Sqrt(Math.Pow((ballOfReadyToShootVector.X - ballOfNextReadyToShootVector.X), 2) 
                    + Math.Pow((ballOfReadyToShootVector.Z - ballOfNextReadyToShootVector.Z), 2));
                double moveAngle = Math.Atan((ballOfReadyToShootVector.Z - ballOfNextReadyToShootVector.Z) 
                    / (ballOfReadyToShootVector.X - ballOfNextReadyToShootVector.X));

                ballOfNextReadyToShoot.movingUnitDirection = new Vector3((float)Math.Sin(Math.PI / 2 - moveAngle * Math.PI / 180), 0, -(float)Math.Cos(Math.PI / 2 - moveAngle * Math.PI / 180));
                if (ballOfNextReadyToShoot.position.Z > ballOfReadyToShootVector.Z)
                {
                    ballOfNextReadyToShoot.pitchAngle += (float)Math.PI / 90;
                    ballOfNextReadyToShoot.position += new Vector3((float)(moveDistance / 10 * Math.Cos(moveAngle)), 0, (float)(moveDistance / 10 * Math.Sin(moveAngle)));
                }
                else
                {
                    ballOfNextReadyToShoot.position = ballOfReadyToShootVector;
                }

            }
            
            ////////////////////////////////////////////////////////////////// To Modify           
            for (int i = 0; i < 8; i++)
            {
                if (ballArray[maxAllowedRow][i] != null)
                {
                    currentGameState = GameState.Level_Fail;
                    return;
                }
            }
            ////////////////////////////////////////////////////////////////// 

            if (canProduceNewBall)
            {
                ballMovable = false;
                ballOfReadyToShoot = ballOfNextReadyToShoot;
                produceNewBall();
                ballOfNextReadyToShoot.position = ballOfNextReadyToShootVector;
                canProduceNewBall = false;
                canMoveOfBallOfNextReadyToShoot = false;
            }


            if (ballMovable == true)
            {
                if ((ballOfReadyToShoot.getWorldPosition().Translation.X < -0.35) || (ballOfReadyToShoot.getWorldPosition().Translation.X > 0.35))
                {
                    ballMoveDirectionAngle = ballMoveDirectionAngle * -1;
                }
                ///////////////////////////// new added
                ballOfReadyToShoot.movingUnitDirection = new Vector3((float)Math.Sin(ballMoveDirectionAngle * Math.PI / 180), 0, -(float)Math.Cos(ballMoveDirectionAngle * Math.PI / 180));
                ////////////////////////////
                if (hasCollisionWithOthers() == false)      //here, we decide whether the moving ball will collide with same color balls
                {
                    ballOfReadyToShoot.moveOnScreen(ballMoveDirectionAngle);
                }
                else                                            //Now there is a collision
                {
                    canProduceNewBall = true;
                    for (int i = this.maxAllowedRow; i >= 0; i--)       //set all balls are unvisited
                    {
                        for (int j = 0; j < 8; j++)
                        {
                            if (ballArray[i][j] != null)
                            {
                                ballArray[i][j].hasVisited = false;
                                ballArray[i][j].needDeleted = false;
                            }
                        }
                    }
                    ballOfReadyToShoot.hasVisited = true;
                    counterForDeleteBallNumber = 0;
                    /*
                    if (ballOfReadyToShoot.arrayPositionX == -1)
                    {
                        if ((currentGameState == GameState.Level_Run))
                             currentGameState = GameState.Level_Fail;
                        return;
                    }
                     */
                    if (ballOfReadyToShoot.arrayPositionX == maxAllowedRow)
                    {
                        if ((currentGameState == GameState.Level_Run))
                            currentGameState = GameState.Level_Fail;
                        return;
                    }
                    computeSameColorBallNumber(ballOfReadyToShoot);
                    if (counterForDeleteBallNumber >= 2)
                    {
                        canDisplayBallPop = true;
                        score += (counterForDeleteBallNumber + 1) * 10;
                        ballArray[ballOfReadyToShoot.arrayPositionX][ballOfReadyToShoot.arrayPositionY].needDeleted = true;
                        int minDropRow = deleteSameColorBallAndScore();
                        readyToDropSomeBall(minDropRow);
                        counterForDropableBallNumber = 0;
                        for (int i = 0; i < maxAllowedRow; i++)
                        {
                            for (int j = 0; j < 8; j++)
                            {
                                if ((ballArray[i][j] != null) && (ballArray[i][j].canDrop == true) && (ballArray[i][j].needDeleted == false))
                                {
                                    counterForDropableBallNumber++;

                                }
                            }
                        }
                        hasBallDropable = false;
                        if (counterForDropableBallNumber > 0)
                        {
                            hasBallDropable = true;
                            score += (int)Math.Pow(2, counterForDropableBallNumber) * 10;
                        }
                    }
                    else
                    {
                        for (int i = 0; i < maxAllowedRow; i++)
                        {
                            for (int j = 0; j < 8; j++)
                            {
                                if ((ballArray[i][j] != null) && (ballArray[i][j].needDeleted == true))
                                {
                                    ballArray[i][j].needDeleted = false;

                                }
                            }
                        }
                    }


                    ceilingDropWay(gameTime);
                }
            }  //end ball move
            else
            {
                AutoFireWay(gameTime);
            }
            

            if (canDisplayBallPop == true)          //here, we will timer and ready to display the ball pop animation
            {                
                for (int i = 0; i <= maxAllowedRow; i++)
                {
                    for (int j = 0; j < 8; j++)
                    {
                        if ((ballArray[i][j] != null) && (ballArray[i][j].needDeleted == true))
                        {                            
                            ballArray[i][j].scale -= 0.1f; 

                            if (ballArray[i][j].scale <= 0.1f)
                            {
                                ballArray[i][j] = null;
                                canDisplayBallPop = false;
                            }                                                       
                        }
                    }
                }
                
            }

            for (int i = 0; i <= maxAllowedRow; i++)          // delete the non-seen balls
            {
                for (int j = 0; j < 8; j++)
                {
                   
                    if ((ballArray[i][j] != null) && (ballArray[i][j].position.Z >= 2.6f))
                    {
                        ballArray[i][j] = null;
                    }
                     
                }
            }
            
            

            if (hasBallDropable == true)
            {
                dropBall();
            }
           
            Boolean hasExistingBall = false;
            for (int i = 0; i <= maxAllowedRow; i++)
            {
                for (int j = 0; j < 8; j++)
                {
                    if ((ballArray[i][j] != null))
                    {
                        hasExistingBall = true;
                        break;
                    }
                }
            }           
            if (hasExistingBall == false)
            {
                ballOfReadyToShoot = ballOfNextReadyToShoot; 
            }
      
            if ((hasExistingBall == false) && (currentGameState == GameState.Level_Run))
            {
                currentGameState = GameState.Level_Pass;
            }
          
            
        }


        private Boolean hasCollisionWithOthers()
        {
            //First we consider that the ball will collide with other balls
            for (int i = maxAllowedRow; i >= 0; i--)
            {
                for (int j = 0; j < 8; j++)
                {
                    if (ballArray[i][j] != null)
                    {
                        if (((ballOfReadyToShoot.position.X - ballArray[i][j].position.X) * (ballOfReadyToShoot.position.X - ballArray[i][j].position.X))
                                    + ((ballOfReadyToShoot.position.Z - ballArray[i][j].position.Z) * (ballOfReadyToShoot.position.Z - ballArray[i][j].position.Z))
                                    <= (2 * 0.05) * (2 * 0.05))
                        {                            
                            // if ballOfReadyToShoot come from right top of ballArray[i][j]
                            if ((ballOfReadyToShoot.position.X >= ballArray[i][j].position.X) && (ballOfReadyToShoot.position.Z < ballArray[i][j].position.Z))
                            {
                                if (i > 0)
                                {
                                    if (i % 2 == 0)     // even row
                                    {
                                        if (ballArray[i - 1][j + 1] == null) // if no ball exists on right top, put it there
                                        {
                                            ballOfReadyToShoot.position = new Vector3(ballArray[i][j].position.X + 0.05f, ballArray[i][j].position.Y, ballArray[i][j].position.Z - 0.1f);
                                            ballArray[i - 1][j + 1] = ballOfReadyToShoot;
                                            ballOfReadyToShoot.arrayPositionX = i - 1;
                                            ballOfReadyToShoot.arrayPositionY = j + 1;
                                        }
                                        else // if already another ball exists on right top, put it right
                                        {
                                            ballOfReadyToShoot.position = new Vector3(ballArray[i][j].position.X + 0.1f, ballArray[i][j].position.Y, ballArray[i][j].position.Z);
                                            ballArray[i][j + 1] = ballOfReadyToShoot;
                                            ballOfReadyToShoot.arrayPositionX = i;
                                            ballOfReadyToShoot.arrayPositionY = j + 1;
                                        }
                                    }
                                    else // odd row
                                    {
                                        if (ballArray[i - 1][j] == null) // if no ball exists on right top, put it there
                                        {
                                            ballOfReadyToShoot.position = new Vector3(ballArray[i][j].position.X + 0.05f, ballArray[i][j].position.Y, ballArray[i][j].position.Z - 0.1f);
                                            ballArray[i - 1][j] = ballOfReadyToShoot;
                                            ballOfReadyToShoot.arrayPositionX = i - 1;
                                            ballOfReadyToShoot.arrayPositionY = j;
                                        }
                                        else // if already another ball exists on right bottom, put it right
                                        {
                                            ballOfReadyToShoot.position = new Vector3(ballArray[i][j].position.X + 0.1f, ballArray[i][j].position.Y, ballArray[i][j].position.Z);
                                            ballArray[i][j + 1] = ballOfReadyToShoot;
                                            ballOfReadyToShoot.arrayPositionX = i;
                                            ballOfReadyToShoot.arrayPositionY = j + 1;
                                        }
                                    }
                                }
                                return true;
                            }
                            
                            // if ballOfReadyToShoot come from right of ballArray[i][j]                           
                            if ((ballOfReadyToShoot.position.X > ballArray[i][j].position.X) && (ballOfReadyToShoot.position.Z == ballArray[i][j].position.Z))
                            {
                                ballOfReadyToShoot.position = new Vector3(ballArray[i][j].position.X + 0.1f, ballArray[i][j].position.Y, ballArray[i][j].position.Z);
                                ballArray[i][j + 1] = ballOfReadyToShoot;
                                ballOfReadyToShoot.arrayPositionX = i;
                                ballOfReadyToShoot.arrayPositionY = j + 1;
                                return true;
                            }
                            
                            // if ballOfReadyToShoot come from right bottom of ballArray[i][j]
                            if ((ballOfReadyToShoot.position.X >= ballArray[i][j].position.X) && (ballOfReadyToShoot.position.Z > ballArray[i][j].position.Z))
                            {
                                if (i < maxAllowedRow)
                                {
                                    if ((i % 2 == 0) && (j < 7))     // even row, not border
                                    {
                                        if (ballArray[i + 1][j + 1] == null) // if no ball exists on right bottom, put it there
                                        {
                                            ballOfReadyToShoot.position = new Vector3(ballArray[i][j].position.X + 0.05f, ballArray[i][j].position.Y, ballArray[i][j].position.Z + 0.1f);
                                            ballArray[i + 1][j + 1] = ballOfReadyToShoot;
                                            ballOfReadyToShoot.arrayPositionX = i + 1;
                                            ballOfReadyToShoot.arrayPositionY = j + 1;
                                        }
                                        else // if already another ball exists on right bottom, put it right
                                        {
                                            ballOfReadyToShoot.position = new Vector3(ballArray[i][j].position.X + 0.1f, ballArray[i][j].position.Y, ballArray[i][j].position.Z);
                                            ballArray[i][j + 1] = ballOfReadyToShoot;
                                            ballOfReadyToShoot.arrayPositionX = i;
                                            ballOfReadyToShoot.arrayPositionY = j + 1;
                                        }
                                    }
                                    else if ((i % 2 == 0) && (j == 7))    // even row, right border
                                    {
                                        ballOfReadyToShoot.position = new Vector3(ballArray[i][j].position.X - 0.05f, ballArray[i][j].position.Y, ballArray[i][j].position.Z + 0.1f);
                                        ballArray[i + 1][j] = ballOfReadyToShoot;
                                        ballOfReadyToShoot.arrayPositionX = i + 1;
                                        ballOfReadyToShoot.arrayPositionY = j;
                                    }
                                    else // odd row
                                    {
                                        if (ballArray[i + 1][j] == null) // if no ball exists on right bottom, put it there
                                        {
                                            ballOfReadyToShoot.position = new Vector3(ballArray[i][j].position.X + 0.05f, ballArray[i][j].position.Y, ballArray[i][j].position.Z + 0.1f);
                                            ballArray[i + 1][j] = ballOfReadyToShoot;
                                            ballOfReadyToShoot.arrayPositionX = i + 1;
                                            ballOfReadyToShoot.arrayPositionY = j;
                                        }
                                        else // if already another ball exists on right bottom, put it right
                                        {
                                            ballOfReadyToShoot.position = new Vector3(ballArray[i][j].position.X + 0.1f, ballArray[i][j].position.Y, ballArray[i][j].position.Z);
                                            ballArray[i][j + 1] = ballOfReadyToShoot;
                                            ballOfReadyToShoot.arrayPositionX = i;
                                            ballOfReadyToShoot.arrayPositionY = j + 1;
                                        }
                                    }

                                }
                                return true;
                            }

                            // if ballOfReadyToShoot come from left top of ballArray[i][j]                           
                            if ((ballOfReadyToShoot.position.X < ballArray[i][j].position.X) && (ballOfReadyToShoot.position.Z < ballArray[i][j].position.Z))
                            {
                                if (i > 0)
                                {
                                    if (i % 2 == 0) // even row
                                    {
                                        if (ballArray[i - 1][j] == null) // if no ball exists on left top, put it there
                                        {
                                            ballOfReadyToShoot.position = new Vector3(ballArray[i][j].position.X - 0.05f, ballArray[i][j].position.Y, ballArray[i][j].position.Z - 0.1f);
                                            ballArray[i - 1][j] = ballOfReadyToShoot;
                                            ballOfReadyToShoot.arrayPositionX = i - 1;
                                            ballOfReadyToShoot.arrayPositionY = j;
                                        }
                                        else // if already another ball exists on left top, put it left
                                        {
                                            ballOfReadyToShoot.position = new Vector3(ballArray[i][j].position.X - 0.1f, ballArray[i][j].position.Y, ballArray[i][j].position.Z);
                                            ballArray[i][j - 1] = ballOfReadyToShoot;
                                            ballOfReadyToShoot.arrayPositionX = i;
                                            ballOfReadyToShoot.arrayPositionY = j - 1;
                                        }
                                    }
                                    else // odd row
                                    {
                                        if (ballArray[i - 1][j - 1] == null) // if no ball exists on left top, put it there
                                        {
                                            ballOfReadyToShoot.position = new Vector3(ballArray[i][j].position.X - 0.05f, ballArray[i][j].position.Y, ballArray[i][j].position.Z - 0.1f);
                                            ballArray[i - 1][j - 1] = ballOfReadyToShoot;
                                            ballOfReadyToShoot.arrayPositionX = i - 1;
                                            ballOfReadyToShoot.arrayPositionY = j - 1;
                                        }
                                        else // if already another ball exists on left top, put it left
                                        {
                                            ballOfReadyToShoot.position = new Vector3(ballArray[i][j].position.X - 0.1f, ballArray[i][j].position.Y, ballArray[i][j].position.Z);
                                            ballArray[i][j - 1] = ballOfReadyToShoot;
                                            ballOfReadyToShoot.arrayPositionX = i;
                                            ballOfReadyToShoot.arrayPositionY = j - 1;
                                        }
                                    }
                                }
                                return true;
                            }

                            // if ballOfReadyToShoot come from left of ballArray[i][j] 
                            if ((ballOfReadyToShoot.position.X <= ballArray[i][j].position.X) && (ballOfReadyToShoot.position.Z == ballArray[i][j].position.Z))
                            {
                                ballOfReadyToShoot.position = new Vector3(ballArray[i][j].position.X - 0.1f, ballArray[i][j].position.Y, ballArray[i][j].position.Z);
                                ballArray[i][j - 1] = ballOfReadyToShoot;
                                ballOfReadyToShoot.arrayPositionX = i;
                                ballOfReadyToShoot.arrayPositionY = j - 1;
                                return true;
                            }
                            
                            // if ballOfReadyToShoot come from left bottom of ballArray[i][j]                           
                            if ((ballOfReadyToShoot.position.X < ballArray[i][j].position.X) && (ballOfReadyToShoot.position.Z > ballArray[i][j].position.Z))
                            {
                                if (i < maxAllowedRow)
                                {
                                    if ((i % 2 == 0) && (j > 0)) // even row, not border
                                    {
                                        if (ballArray[i + 1][j] == null) // if no ball exists on left bottom, put it there
                                        {
                                            ballOfReadyToShoot.position = new Vector3(ballArray[i][j].position.X - 0.05f, ballArray[i][j].position.Y, ballArray[i][j].position.Z + 0.1f);
                                            ballArray[i + 1][j] = ballOfReadyToShoot;
                                            ballOfReadyToShoot.arrayPositionX = i + 1;
                                            ballOfReadyToShoot.arrayPositionY = j;
                                        }
                                        else // if already another ball exists on left bottom, put it left
                                        {
                                            ballOfReadyToShoot.position = new Vector3(ballArray[i][j].position.X - 0.1f, ballArray[i][j].position.Y, ballArray[i][j].position.Z);
                                            ballArray[i][j - 1] = ballOfReadyToShoot;
                                            ballOfReadyToShoot.arrayPositionX = i;
                                            ballOfReadyToShoot.arrayPositionY = j - 1;
                                        }
                                    }
                                    else if ((i % 2 == 0) && (j == 0)) // even row, left border
                                    {
                                        ballOfReadyToShoot.position = new Vector3(ballArray[i][j].position.X + 0.05f, ballArray[i][j].position.Y, ballArray[i][j].position.Z + 0.1f);
                                        ballArray[i + 1][j + 1] = ballOfReadyToShoot;
                                        ballOfReadyToShoot.arrayPositionX = i + 1;
                                        ballOfReadyToShoot.arrayPositionY = j + 1;
                                    }
                                    else // odd row
                                    {
                                        if (ballArray[i + 1][j - 1] == null) // if no ball exists on left bottom, put it there
                                        {
                                            ballOfReadyToShoot.position = new Vector3(ballArray[i][j].position.X - 0.05f, ballArray[i][j].position.Y, ballArray[i][j].position.Z + 0.1f);
                                            ballArray[i + 1][j - 1] = ballOfReadyToShoot;
                                            ballOfReadyToShoot.arrayPositionX = i + 1;
                                            ballOfReadyToShoot.arrayPositionY = j - 1;
                                        }
                                        else // if already another ball exists on left bottom, put it left
                                        {
                                            ballOfReadyToShoot.position = new Vector3(ballArray[i][j].position.X - 0.1f, ballArray[i][j].position.Y, ballArray[i][j].position.Z);
                                            ballArray[i][j - 1] = ballOfReadyToShoot;
                                            ballOfReadyToShoot.arrayPositionX = i;
                                            ballOfReadyToShoot.arrayPositionY = j - 1;
                                        }
                                    }
                                }
                                return true;
                            }
                        }
                    }
                }
            }  //end first situation
            
            //The ball maybe touch the ceiling
            if (ballOfReadyToShoot.position.Z < ceiling.position.Z + 0.05f)
            {
               
                if (ballOfReadyToShoot.position.X <= -0.35f)             // first column
                {
                    ballOfReadyToShoot.position = new Vector3(-0.35f, 0.06f, ceiling.position.Z + 0.05f);                    
                    ballArray[0][0] = ballOfReadyToShoot;
                    ballOfReadyToShoot.arrayPositionX = 0;
                    ballOfReadyToShoot.arrayPositionY = 0;
                }
                else if (ballOfReadyToShoot.position.X <= -0.25f)     // second column
                {
                    ballOfReadyToShoot.position = new Vector3(-0.25f, 0.06f, ceiling.position.Z + 0.05f);   
                    ballArray[0][1] = ballOfReadyToShoot;
                    ballOfReadyToShoot.arrayPositionX = 0;
                    ballOfReadyToShoot.arrayPositionY = 1;
                }
                else if (ballOfReadyToShoot.position.X <= -0.15f)   // third column
                {
                    ballOfReadyToShoot.position = new Vector3(-0.15f, 0.06f, ceiling.position.Z + 0.05f);   
                    ballArray[0][2] = ballOfReadyToShoot;
                    ballOfReadyToShoot.arrayPositionX = 0;
                    ballOfReadyToShoot.arrayPositionY = 2;
                }
                else if (ballOfReadyToShoot.position.X <= -0.05f) // fourth column
                {
                    ballOfReadyToShoot.position = new Vector3(-0.05f, 0.06f, ceiling.position.Z + 0.05f);   
                    ballArray[0][3] = ballOfReadyToShoot;
                    ballOfReadyToShoot.arrayPositionX = 0;
                    ballOfReadyToShoot.arrayPositionY = 3;
                }
                else if (ballOfReadyToShoot.position.X <= 0.05f) // fifth column
                {
                    ballOfReadyToShoot.position = new Vector3(0.05f, 0.06f, ceiling.position.Z + 0.05f);   
                    ballArray[0][4] = ballOfReadyToShoot;
                    ballOfReadyToShoot.arrayPositionX = 0;
                    ballOfReadyToShoot.arrayPositionY = 4;
                }
                else if (ballOfReadyToShoot.position.X <= 0.15f) // sixth column
                {
                    ballOfReadyToShoot.position = new Vector3(0.15f, 0.06f, ceiling.position.Z + 0.05f);   
                    ballArray[0][5] = ballOfReadyToShoot;
                    ballOfReadyToShoot.arrayPositionX = 0;
                    ballOfReadyToShoot.arrayPositionY = 5;
                }
                else if (ballOfReadyToShoot.position.X <= 0.25f) // seventh column
                {
                    ballOfReadyToShoot.position = new Vector3(0.25f, 0.06f, ceiling.position.Z + 0.05f);   
                    ballArray[0][6] = ballOfReadyToShoot;
                    ballOfReadyToShoot.arrayPositionX = 0;
                    ballOfReadyToShoot.arrayPositionY = 6;
                }
                else										// eighth column
                {
                    ballOfReadyToShoot.position = new Vector3(0.35f, 0.06f, ceiling.position.Z + 0.05f);   
                    ballArray[0][7] = ballOfReadyToShoot;
                    ballOfReadyToShoot.arrayPositionX = 0;
                    ballOfReadyToShoot.arrayPositionY = 7;
                }

                return true;
            
            }
            

            return false;
        }

 
        private void produceNewBall()
        {
            Boolean hasGreenBall = false;
            Boolean hasBlueBall = false;
            Boolean hasRedBall = false;
            Boolean hasYellowBall = false;
            Boolean hasCyanBall = false;                
            Boolean hasPurpleBall = false;                 
            int colorCounter = 0;

            for (int i = 0; i <= maxAllowedRow; i++)
            {
                for (int j = 0; j < 8; j++)
                {
                    if (ballArray[i][j] != null)
                    {
                        if ((ballArray[i][j].getBallColor() == 1) && (hasGreenBall == false))
                        {
                            hasGreenBall = true;
                            colorCounter++;
                        }
                        else if ((ballArray[i][j].getBallColor() == 2) && (hasBlueBall == false))
                        {
                            hasBlueBall = true;
                            colorCounter++;
                        }
                        else if ((ballArray[i][j].getBallColor() == 3) && (hasRedBall == false))
                        {
                            hasRedBall = true;
                            colorCounter++;
                        }
                        else if ((ballArray[i][j].getBallColor() == 4) && (hasYellowBall == false))
                        {
                            hasYellowBall = true;
                            colorCounter++;
                        }
                        else if ((ballArray[i][j].getBallColor() == 5) && (hasCyanBall == false))          
                        {
                            hasCyanBall = true;
                            colorCounter++;
                        }
                        else if ((ballArray[i][j].getBallColor() == 6) && (hasPurpleBall == false))          
                        {
                            hasPurpleBall = true;
                            colorCounter++;
                        }
                    }
                }
            }
           
            // End checking how many kinds of color ball remaining
            Boolean produceNewBallSucceed = true;


            Random random = new Random();
            _3DBall newBall = null;
            do
            {
                produceNewBallSucceed = true;
                newBall = new _3DBall(random.Next(6) + 1, -1, -1);
                if ((newBall.getBallColor() == 1) && (hasGreenBall == false))
                {
                    produceNewBallSucceed = false;
                }
                else if ((newBall.getBallColor() == 2) && (hasBlueBall == false))
                {
                    produceNewBallSucceed = false;
                }
                else if ((newBall.getBallColor() == 3) && (hasRedBall == false))
                {
                    produceNewBallSucceed = false;
                }
                else if ((newBall.getBallColor() == 4) && (hasYellowBall == false))
                {
                    produceNewBallSucceed = false;
                }
                else if ((newBall.getBallColor() == 5) && (hasCyanBall == false))          
                {
                    produceNewBallSucceed = false;
                }
                else if ((newBall.getBallColor() == 6) && (hasPurpleBall == false))           
                {
                    produceNewBallSucceed = false;
                }

            } while (produceNewBallSucceed == false);

            ballOfNextReadyToShoot = newBall;

            int ballColor = ballOfNextReadyToShoot.getBallColor();
            switch (ballColor)
            {
                case 1:
                    ballOfNextReadyToShoot.setModel(greenBallModel);
                    break;
                case 2:
                    ballOfNextReadyToShoot.setModel(blueBallModel);
                    break;
                case 3:
                    ballOfNextReadyToShoot.setModel(redBallModel);
                    break;
                case 4:
                    ballOfNextReadyToShoot.setModel(yellowBallModel);
                    break;
                case 5:
                    ballOfNextReadyToShoot.setModel(cyanBallModel);
                    break;
                case 6:
                    ballOfNextReadyToShoot.setModel(purpleBallModel);
                    break;
            }
            ballOfNextReadyToShoot.translation(new Vector3(-0.35f, 0.06f, 0.55f));
        }

        private void dropBall()
        {
            for (int i = 0; i <= maxAllowedRow; i++)
            {
                for (int j = 0; j < 8; j++)
                {
                    if ((ballArray[i][j] != null) && (ballArray[i][j].canDrop == true) && (ballArray[i][j].needDeleted == false))
                    {
                        //ballArray[i][j].position += new Vector3(0, 0, +0.01f);
                        dropballList.Add(ballArray[i][j]);
                        ballArray[i][j] = null;
                    }
                }
            }
            
            // drop hanging balls
            for (int i = 0; i < dropballList.Count; ++i)
            {
                _3DBall ball = dropballList[i];
                ball.verticalVelocity += ball.gravity; // compute velocity
                ball.pitchAngle += (float)Math.PI / 90 * ball.verticalVelocity * 100;
                ball.position += new Vector3(0, 0, ball.verticalVelocity); // drop
                if (ball.position.Z >= 0.7) // out of screen
                {
                    dropballList.Remove(ball); // remove from list
                }
            }
         

        }

        private void AutoFireWay(GameTime gameTime)
        {
           timerOfFire += (float)gameTime.ElapsedGameTime.TotalMilliseconds;

            if (timerOfFire > intervalOfFire - 5000)
            {
                counterDown = 5;
            }
            if (timerOfFire > intervalOfFire - 4000)
            {
                counterDown = 4;
            }
            if (timerOfFire > intervalOfFire - 3000)
            {
                counterDown = 3;
            }
            if (timerOfFire > intervalOfFire - 2000)
            {
                counterDown = 2;
            }
            if (timerOfFire > intervalOfFire - 1000)
            {
                counterDown = 1;
            }


            if (timerOfFire > intervalOfFire)
            {
                ballMovable = true;
                ballMoveDirectionAngle = arrowAngle;                
                timerOfFire = 0f;
                produceNewBall();
                counterDown = 6;                
                pressCounter++;               
                canMoveOfBallOfNextReadyToShoot = true;                
                ballOfReadyToShoot.movingUnitDirection = new Vector3((float)Math.Sin(arrowAngle * Math.PI / 180), 0, -(float)Math.Cos(arrowAngle * Math.PI / 180));
                ballOfReadyToShoot.verticalVelocity = (float)Math.Sqrt(Math.Pow(finalVelocity, 2) - 2 * (-ballOfReadyToShoot.gravity) * (float)Math.Cos(arrowAngle * Math.PI / 180) * (ballOfReadyToShootVector.Z - ceilingHeight));
                ballOfReadyToShoot.horizontalVelocity = ballOfReadyToShoot.verticalVelocity;
            }

        }


        private bool checkDropCondition(_3DBall currentBubble)
        {
            int row = currentBubble.arrayPositionX;
            int column = currentBubble.arrayPositionY;

            if (currentBubble.needDeleted == true) //  return dierectly if the ball is need to be deleted
            {
                return true;
            }
            if (row == 0) // if the first row touch the ceiling
            {
                ballArray[row][column].canDrop = false;
                return false;
            }
            else if (row % 2 == 0) // check the even number row but not the first row touch the ceiling
            {
                if (row >= 1)  // check left top
                {
                    if (ballArray[row - 1][column] != null)
                    {
                        if ((ballArray[row - 1][column].needDeleted == false) && (ballArray[row - 1][column].canDrop == false))
                        {
                            ballArray[row][column].canDrop = false;
                            return false;
                        }
                    }
                }
                if ((row >= 1) && (column <= 6)) // check right top
                {
                    if (ballArray[row - 1][column + 1] != null)
                    {
                        if ((ballArray[row - 1][column + 1].needDeleted == false) && (ballArray[row - 1][column + 1].canDrop == false))
                        {
                            ballArray[row][column].canDrop = false;
                            return false;
                        }
                    }
                }
                if (column >= 1) // check left neighbor
                {
                    if (ballArray[row][column - 1] != null)
                    {
                        if ((ballArray[row][column - 1].needDeleted == false) && (ballArray[row][column - 1].canDrop == false))
                        {
                            ballArray[row][column].canDrop = false;
                            return false;
                        }
                        else
                        {
                            if (ballArray[row][column - 1].hasVisited == false) // if not checked
                            {
                                ballArray[row][column - 1].hasVisited = true;
                                if (checkDropCondition(ballArray[row][column - 1]) == false)
                                {
                                    ballArray[row][column].canDrop = false;
                                    return false;
                                }
                            }
                        }
                    }
                }
                if (column <= 6) // check right neighbor
                {
                    if (ballArray[row][column + 1] != null)
                    {
                        if ((ballArray[row][column + 1].needDeleted == false) && (ballArray[row][column + 1].canDrop == false))
                        {
                            ballArray[row][column].canDrop = false;
                            return false;
                        }
                        else
                        {
                            if (ballArray[row][column + 1].hasVisited == false) // if not checked
                            {
                                ballArray[row][column + 1].hasVisited = true;
                                if (checkDropCondition(ballArray[row][column + 1]) == false)
                                {
                                    ballArray[row][column].canDrop = false;
                                    return false;
                                }
                            }
                        }
                    }
                }

                /////////////////////////////////////////////////////
                if (row < maxAllowedRow)  // check left bottom
                {
                    if (ballArray[row + 1][column] != null)
                    {
                        if ((ballArray[row + 1][column].needDeleted == false) && (ballArray[row + 1][column].canDrop == false)) // if not to pop or drop
                        {
                            ballArray[row][column].canDrop = false;
                            return false;
                        }
                    }
                }
                if ((row < maxAllowedRow) && (column <= 6)) // check right bottom
                {
                    if (ballArray[row + 1][column + 1] != null)
                    {
                        if ((ballArray[row + 1][column + 1].needDeleted == false) && (ballArray[row + 1][column + 1].canDrop == false)) // if not to pop or drop
                        {
                            ballArray[row][column].canDrop = false;
                            return false;
                        }
                    }
                }
                ////////////////////////////////////////////////////


            }
            else // check odd number row
            {
                if ((row >= 1) && (column >= 1)) // check left top
                {
                    if (ballArray[row - 1][column - 1] != null)
                    {
                        if ((ballArray[row - 1][column - 1].needDeleted == false) && (ballArray[row - 1][column - 1].canDrop == false))
                        {
                            ballArray[row][column].canDrop = false;
                            return false;
                        }
                    }
                }
                if (row >= 1) // check right top
                {
                    if (ballArray[row - 1][column] != null)
                    {
                        if ((ballArray[row - 1][column].needDeleted == false) && (ballArray[row - 1][column].canDrop == false))
                        {
                            ballArray[row][column].canDrop = false;
                            return false; ;
                        }
                    }
                }
                if (column >= 1) // check left neighbor
                {
                    if (ballArray[row][column - 1] != null)
                    {
                        if ((ballArray[row][column - 1].needDeleted == false) && (ballArray[row][column - 1].canDrop == false))
                        {
                            ballArray[row][column].canDrop = false;
                            return false;
                        }
                        else
                        {
                            if (ballArray[row][column - 1].hasVisited == false) // if not checked
                            {
                                ballArray[row][column - 1].hasVisited = true;
                                if (checkDropCondition(ballArray[row][column - 1]) == false)
                                {
                                    ballArray[row][column].canDrop = false;
                                    return false;
                                }
                            }
                        }
                    }
                }
                if (column <= 6) // check right neighbor
                {
                    if (ballArray[row][column + 1] != null)
                    {
                        if ((ballArray[row][column + 1].needDeleted == false) && (ballArray[row][column + 1].canDrop == false))
                        {
                            ballArray[row][column].canDrop = false;
                            return false;
                        }
                        else
                        {
                            if (ballArray[row][column + 1].hasVisited == false) // if not checked
                            {
                                ballArray[row][column + 1].hasVisited = true;
                                if (checkDropCondition(ballArray[row][column + 1]) == false)
                                {
                                    ballArray[row][column].canDrop = false;
                                    return false;
                                }
                            }
                        }
                    }
                }

                ///////////////////////////////////////////
                if ((row < maxAllowedRow) && (column >= 1)) // check left bottom
                {
                    if (ballArray[row + 1][column - 1] != null)
                    {
                        if ((ballArray[row + 1][column - 1].needDeleted == false) && (ballArray[row + 1][column - 1].canDrop == false)) // if not to pop or drop
                        {
                            ballArray[row][column].canDrop = false;
                            return false;
                        }
                    }
                }
                if (row < maxAllowedRow) // check right bottom
                {
                    if (ballArray[row + 1][column] != null)
                    {
                        if ((ballArray[row + 1][column].needDeleted == false) && (ballArray[row + 1][column].canDrop == false)) // if not to pop or drop
                        {
                            ballArray[row][column].canDrop = false;
                            return false; ;
                        }
                    }
                }
                //////////////////////////////////////////

            }
            return true;
        }

        private void readyToDropSomeBall(int minDropRow)
        {
            //First set all balls from minDropRow to max row as drop as true
            for (int i = minDropRow; i <= maxAllowedRow; i++)       //set all balls are unvisited
            {
                for (int j = 0; j < 8; j++)
                {
                    if (ballArray[i][j] != null)
                    {
                        ballArray[i][j].canDrop = true;
                        ballArray[i][j].hasVisited = false;
                    }
                }
            }

            // check if drop condition satisfied
            for (int i = minDropRow; i <= maxAllowedRow; i++)
            {
                for (int j = 0; j < 8; j++)
                {
                    if (ballArray[i][j] != null)
                    {
                        if (ballArray[i][j].needDeleted == false)
                        {
                            checkDropCondition(ballArray[i][j]); // check if drop condition satisfied
                        }
                    }
                }
            }

        }

        private int deleteSameColorBallAndScore()
        {
            int minDropRow = 10;

            for (int i = this.maxAllowedRow; i >= 0; i--)       //set all balls are unvisited
            {
                for (int j = 0; j < 8; j++)
                {
                    if ((ballArray[i][j] != null) && (ballArray[i][j].needDeleted == true))
                    {
                        if (i < minDropRow)
                        {
                            minDropRow = i;
                        }
                        //ballArray[i][j] = null;  now temo no delete the ball
                    }
                }
            }

            return minDropRow;
        }

        private void ceilingDropWay(GameTime gameTime)
        {

            Boolean hasGreenBall = false;
            Boolean hasBlueBall = false;
            Boolean hasRedBall = false;
            Boolean hasYellowBall = false;
            Boolean hasCyanBall = false;                     
            Boolean hasPurpleBall = false;                      
            int colorCounter = 0;

            for (int i = 0; i <= maxAllowedRow; i++)
            {
                for (int j = 0; j < 8; j++)
                {
                    if (ballArray[i][j] != null)
                    {
                        if ((ballArray[i][j].getBallColor() == 1) && (hasGreenBall == false))
                        {
                            hasGreenBall = true;
                            colorCounter++;
                        }
                        else if ((ballArray[i][j].getBallColor() == 2) && (hasBlueBall == false))
                        {
                            hasBlueBall = true;
                            colorCounter++;
                        }
                        else if ((ballArray[i][j].getBallColor() == 3) && (hasRedBall == false))
                        {
                            hasRedBall = true;
                            colorCounter++;
                        }
                        else if ((ballArray[i][j].getBallColor() == 4) && (hasYellowBall == false))
                        {
                            hasYellowBall = true;
                            colorCounter++;
                        }
                        else if ((ballArray[i][j].getBallColor() == 5) && (hasCyanBall == false))    
                        {
                            hasCyanBall = true;
                            colorCounter++;
                        }
                        else if ((ballArray[i][j].getBallColor() == 6) && (hasPurpleBall == false))        
                        {
                            hasPurpleBall = true;
                            colorCounter++;
                        }
                    }
                }
            }


            if ((colorCounter == 1) && (pressCounter >= 8))
            {
                for (int i = 0; i <= this.maxAllowedRow; i++)
                {
                    for (int j = 0; j < 8; j++)
                    {
                        if (ballArray[i][j] != null)
                        {
                            ballArray[i][j].position += new Vector3(0,0,0.1f); 
                        }
                    }
                }

                releaseWarning = false;
                maxAllowedRow--;
                pressCounter = 0;
                ceiling.ceilingDown();
            }
            else if ((colorCounter == 1) && (pressCounter >= 6))
            {
                releaseWarning = true;
            }



            if ((colorCounter == 2) && (pressCounter >= 8))
            {
                for (int i = 0; i <= this.maxAllowedRow; i++)
                {
                    for (int j = 0; j < 8; j++)
                    {
                        if (ballArray[i][j] != null)
                        {
                            ballArray[i][j].position += new Vector3(0, 0, 0.1f);
                        }
                    }
                }

                releaseWarning = false;
                maxAllowedRow--;
                pressCounter = 0;
                ceiling.ceilingDown();
            }
            else if ((colorCounter == 2) && (pressCounter >= 6))
            {
                releaseWarning = true;
            }


            if ((colorCounter == 3) && (pressCounter >= 8))
            {
                for (int i = 0; i <= this.maxAllowedRow; i++)
                {
                    for (int j = 0; j < 8; j++)
                    {
                        if (ballArray[i][j] != null)
                        {
                            ballArray[i][j].position += new Vector3(0, 0, 0.1f);
                        }
                    }
                }

                releaseWarning = false;
                maxAllowedRow--;
                pressCounter = 0;
                ceiling.ceilingDown();
            }
            else if ((colorCounter == 3) && (pressCounter >= 6))
            {
                releaseWarning = true;
            }

            if ((colorCounter == 4) && (pressCounter >= 9))
            {
                for (int i = 0; i <= this.maxAllowedRow; i++)
                {
                    for (int j = 0; j < 8; j++)
                    {
                        if (ballArray[i][j] != null)
                        {
                            ballArray[i][j].position += new Vector3(0, 0, 0.1f);
                        }
                    }
                }

                releaseWarning = false;
                maxAllowedRow--;
                pressCounter = 0;
                ceiling.ceilingDown();
            }
            else if ((colorCounter == 4) && (pressCounter >= 7))
            {
                releaseWarning = true;
            }

            if ((colorCounter == 5) && (pressCounter >= 9))
            {
                for (int i = 0; i <= this.maxAllowedRow; i++)
                {
                    for (int j = 0; j < 8; j++)
                    {
                        if (ballArray[i][j] != null)
                        {
                            ballArray[i][j].position += new Vector3(0, 0, 0.1f);
                        }
                    }
                }

                releaseWarning = false;
                maxAllowedRow--;
                pressCounter = 0;
                ceiling.ceilingDown();
            }
            else if ((colorCounter == 5) && (pressCounter >= 7))
            {
                releaseWarning = true;
            }


            if ((colorCounter == 6) && (pressCounter >= 10))
            {
                for (int i = 0; i <= this.maxAllowedRow; i++)
                {
                    for (int j = 0; j < 8; j++)
                    {
                        if (ballArray[i][j] != null)
                        {
                            ballArray[i][j].position += new Vector3(0, 0, 0.1f);
                        }
                    }
                }

                releaseWarning = false;
                maxAllowedRow--;
                pressCounter = 0;
                ceiling.ceilingDown();
            }
            else if ((colorCounter == 6) && (pressCounter >= 8))
            {
                releaseWarning = true;
            }

        }

        private void computeSameColorBallNumber(_3DBall currentCheckingBall)
        {
            int row = currentCheckingBall.arrayPositionX;
            int column = currentCheckingBall.arrayPositionY;

            if (row % 2 == 0)      //check the even number row
            {
                if (column <= 6)   // check right neighbor
                {
                    if (ballArray[row][column + 1] != null)
                    {
                        if (ballArray[row][column + 1].hasVisited == false)
                        {
                            ballArray[row][column + 1].hasVisited = true;
                            if (ballArray[row][column + 1].getBallColor() == currentCheckingBall.getBallColor())
                            {
                                this.counterForDeleteBallNumber++;
                                ballArray[row][column + 1].needDeleted = true;
                                computeSameColorBallNumber(ballArray[row][column + 1]);
                            }
                        }
                    }
                }   //// check right neighbor end

                if ((row < maxAllowedRow) && (column <= 6))  //check right bottom
                {
                    if (ballArray[row + 1][column + 1] != null)
                    {
                        if (ballArray[row + 1][column + 1].hasVisited == false)
                        {
                            ballArray[row + 1][column + 1].hasVisited = true;
                            if (ballArray[row + 1][column + 1].getBallColor() == currentCheckingBall.getBallColor())
                            {
                                counterForDeleteBallNumber++;
                                ballArray[row + 1][column + 1].needDeleted = true;
                                computeSameColorBallNumber(ballArray[row + 1][column + 1]);
                            }
                        }
                    }
                }    //check right bottom neighbor end


                if (row < maxAllowedRow)                       ////check left bottom
                {
                    if (ballArray[row + 1][column] != null)    
                    {
                        if (ballArray[row + 1][column].hasVisited == false)
                        {
                            ballArray[row + 1][column].hasVisited = true;
                            if (ballArray[row + 1][column].getBallColor() == currentCheckingBall.getBallColor())
                            {
                                counterForDeleteBallNumber++;
                                ballArray[row + 1][column].needDeleted = true;
                                computeSameColorBallNumber(ballArray[row + 1][column]);
                            }
                        }
                    }   //check left bottom end
                }

                if (column >= 1)    // check left neighbor
                {
                    if (ballArray[row][column - 1] != null)
                    {
                        if (ballArray[row][column - 1].hasVisited == false)
                        {
                            ballArray[row][column - 1].hasVisited = true;
                            if (ballArray[row][column - 1].getBallColor() == currentCheckingBall.getBallColor())
                            {

                                counterForDeleteBallNumber++;
                                ballArray[row][column - 1].needDeleted = true;
                                computeSameColorBallNumber(ballArray[row][column - 1]);
                            }
                        }
                    }
                }      // check left neighbor end


                if (row >= 1)               //check left top
                {
                    if (ballArray[row - 1][column] != null)
                    {
                        if (ballArray[row - 1][column].hasVisited == false)
                        {
                            ballArray[row - 1][column].hasVisited = true;
                            if (ballArray[row - 1][column].getBallColor() == currentCheckingBall.getBallColor())
                            {

                                counterForDeleteBallNumber++;
                                ballArray[row - 1][column].needDeleted = true;
                                computeSameColorBallNumber(ballArray[row - 1][column]);
                            }
                        }
                    }
                }      //check left top  end



                if ((row >= 1) && (column <= 6))     //check right top
                {
                    if (ballArray[row - 1][column + 1] != null)
                    {
                        if (ballArray[row - 1][column + 1].hasVisited == false)
                        {
                            ballArray[row - 1][column + 1].hasVisited = true;
                            if (ballArray[row - 1][column + 1].getBallColor() == currentCheckingBall.getBallColor())
                            {

                                counterForDeleteBallNumber++;
                                ballArray[row - 1][column + 1].needDeleted = true;
                                computeSameColorBallNumber(ballArray[row - 1][column + 1]);
                            }
                        }
                    }
                }     //check right top end


            }    //check the even number row  end
            else // check odd number row
            {
                if (column <= 6)    // check right neighbor
                {
                    if (ballArray[row][column + 1] != null)
                    {
                        if (ballArray[row][column + 1].hasVisited == false)
                        {
                            ballArray[row][column + 1].hasVisited = true;
                            if (ballArray[row][column + 1].getBallColor() == currentCheckingBall.getBallColor())
                            {
                                counterForDeleteBallNumber++;
                                ballArray[row][column + 1].needDeleted = true;
                                computeSameColorBallNumber(ballArray[row][column + 1]);
                            }
                        }
                    }
                }   // check right neighbor  end

                if (row < maxAllowedRow)  //check right bottom
                {
                    if (ballArray[row + 1][column] != null)
                    {
                        if (ballArray[row + 1][column].hasVisited == false)
                        {
                            ballArray[row + 1][column].hasVisited = true;
                            if (ballArray[row + 1][column].getBallColor() == currentCheckingBall.getBallColor())
                            {
                                counterForDeleteBallNumber++;
                                ballArray[row + 1][column].needDeleted = true;
                                computeSameColorBallNumber(ballArray[row + 1][column]);
                            }
                        }
                    }
                }    //check right bottom end


                if ((row < maxAllowedRow) && (column >= 1))   //check left bottom
                {
                    if (ballArray[row + 1][column - 1] != null)
                    {
                        if (ballArray[row + 1][column - 1].hasVisited == false)
                        {
                            ballArray[row + 1][column - 1].hasVisited = true;
                            if (ballArray[row + 1][column - 1].getBallColor() == currentCheckingBall.getBallColor())
                            {
                                counterForDeleteBallNumber++;
                                ballArray[row + 1][column - 1].needDeleted = true;
                                computeSameColorBallNumber(ballArray[row + 1][column - 1]);
                            }
                        }
                    }
                }    //check left bottom end

                if (column >= 1) // check left neighbor
                {
                    if (ballArray[row][column - 1] != null)
                    {
                        if (ballArray[row][column - 1].hasVisited == false)
                        {
                            ballArray[row][column - 1].hasVisited = true;
                            if (ballArray[row][column - 1].getBallColor() == currentCheckingBall.getBallColor())
                            {
                                counterForDeleteBallNumber++;
                                ballArray[row][column - 1].needDeleted = true;
                                computeSameColorBallNumber(ballArray[row][column - 1]);
                            }
                        }
                    }
                }     // check left neighbor  end


                if ((row >= 1) && (column >= 1)) //check left top
                {
                    if (ballArray[row - 1][column - 1] != null)
                    {
                        if (ballArray[row - 1][column - 1].hasVisited == false)
                        {
                            ballArray[row - 1][column - 1].hasVisited = true;
                            if (ballArray[row - 1][column - 1].getBallColor() == currentCheckingBall.getBallColor())
                            {
                                counterForDeleteBallNumber++;
                                ballArray[row - 1][column - 1].needDeleted = true;
                                computeSameColorBallNumber(ballArray[row - 1][column - 1]);
                            }
                        }
                    }
                }   //check left top end
                if (row >= 1)
                {
                    if (ballArray[row - 1][column] != null)    //check right top
                    {
                        if (ballArray[row - 1][column].hasVisited == false)
                        {
                            ballArray[row - 1][column].hasVisited = true;
                            if (ballArray[row - 1][column].getBallColor() == currentCheckingBall.getBallColor())
                            {
                                counterForDeleteBallNumber++;
                                ballArray[row - 1][column].needDeleted = true;
                                computeSameColorBallNumber(ballArray[row - 1][column]);
                            }
                        }
                    }    //check right top end
                }

            }   // check odd number row end 

        }


        private void  displayScoreFormat()
        {
            if(score == 0)
            {
                stringScore = "0 0 0 0 0 0";                
            }
            else if(score<100)
            {
                stringScore = " 0 0 0 0 " + score/10 + " 0";
            }
            else if (score < 1000)
            {
                stringScore = " 0 0 0 " + score / 100 + " " + score % 100 / 10  + " 0";
            }
            else if (score < 10000)
            {
                int fourthBit = score / 1000;
                int thirdBit = (score - 1000 * fourthBit) / 100;
                int secondBit = (score - 1000 * fourthBit - 100 * thirdBit) / 10;

                stringScore = " 0 0 " + fourthBit + " " + thirdBit + " " + secondBit + " 0";
            }
            else if (score < 100000)
            {
                int fifthBit = score / 10000;
                int fourthBit = (score - 10000 * fifthBit) / 1000;
                int thirdBit = (score - 10000 * fifthBit - 1000 * fourthBit) / 100;
                int secondBit = (score - 10000 * fifthBit - 1000 * fourthBit - 100 * thirdBit) / 10;

                stringScore = " 0 " + fifthBit + " " + fourthBit + " " + thirdBit + " " + secondBit + " 0";
            }
            else
            {
                int sixthBit = score / 100000;
                int fifthBit = (score - 100000 * sixthBit) / 10000;
                int fourthBit = (score - 100000 * sixthBit - 10000 * fifthBit) / 1000;
                int thirdBit = (score - 100000 * sixthBit - 10000 * fifthBit - 1000 * fourthBit) / 100;
                int secondBit = (score - 100000 * sixthBit- 10000 * fifthBit - 1000 * fourthBit - 100 * thirdBit) / 10;

                stringScore = " " + sixthBit + " " + fifthBit + " " + fourthBit + " " + thirdBit + " " + secondBit + " 0";
            }
           
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);





            if (currentGameState == GameState.Start)
            {
                spriteBatch.Begin();
                spriteBatch.Draw(gameStartup,
                                 new Vector2(0, 0),            // The location (in screen coordinates) to draw the sprite. 
                                 null,
                                 Color.White,
                                 0f,
                                 new Vector2(0, 0),                     //The sprite origin; the default is (0,0) which represents the upper-left corner.
                                 (float)0.96f,                 //scale
                                 SpriteEffects.None,
                                 0);

                spriteBatch.DrawString(text,                //print score
                                       "Press the Enter key to start the game",
                                       new Vector2(150, 720),
                                       Color.DeepSkyBlue, 0,
                                       Vector2.Zero,
                                       1.5f,
                                       SpriteEffects.None,
                                       1);

                spriteBatch.DrawString(tips,                //print score
                                       "Puzzle  \nBubble",
                                       new Vector2(590, 70),
                                       Color.SeaGreen, 0,
                                       Vector2.Zero,
                                       3f,
                                       SpriteEffects.None,
                                       1);
                spriteBatch.End();
            }


            if ((currentGameState == GameState.Level_Run) || (currentGameState == GameState.Level_Pass))
            {

                spriteBatch.Begin();
                spriteBatch.Draw(background,
                                     new Vector2(0, 0),            // The location (in screen coordinates) to draw the sprite. 
                                     null,
                                     Color.White,
                                     0f,
                                     new Vector2(0, 0),                     //The sprite origin; the default is (0,0) which represents the upper-left corner.
                                     (float)1.0f,                 //scale
                                     SpriteEffects.None,
                                     0);
                spriteBatch.End();
                // TODO: Add your drawing code here
                Matrix[] transformsSlope = new Matrix[slope.model.Bones.Count];
                slope.model.CopyAbsoluteBoneTransformsTo(transformsSlope);

                foreach (ModelMesh mesh in slope.model.Meshes)
                {
                    foreach (BasicEffect be in mesh.Effects)
                    {
                        be.EnableDefaultLighting();
                        be.Projection = camera.projection;
                        be.View = camera.view;
                        be.World = slope.getPosition();// *mesh.ParentBone.Transform;  
                        //be.Texture = texture;
                        //be.TextureEnabled = true;
                    }
                    mesh.Draw();
                }

            
                transformsSlope = new Matrix[ceiling.model.Bones.Count];
                ceiling.model.CopyAbsoluteBoneTransformsTo(transformsSlope);

                foreach (ModelMesh mesh in ceiling.model.Meshes)
                {
                    foreach (BasicEffect be in mesh.Effects)
                    {
                        be.EnableDefaultLighting();
                        be.Projection = camera.projection;
                        be.View = camera.view;
                        be.World = Matrix.Identity * mesh.ParentBone.Transform * Matrix.CreateTranslation(ceiling.position) * Matrix.CreateRotationX(30 * MathHelper.Pi / 180); ;                    
                    
                    }
                    mesh.Draw();
                }


                transformsSlope = new Matrix[baseline.model.Bones.Count];
                baseline.model.CopyAbsoluteBoneTransformsTo(transformsSlope);

                foreach (ModelMesh mesh in baseline.model.Meshes)
                {
                    foreach (BasicEffect be in mesh.Effects)
                    {
                        be.EnableDefaultLighting();
                        be.Projection = camera.projection;
                        be.View = camera.view;
                        be.World = Matrix.Identity * mesh.ParentBone.Transform * Matrix.CreateTranslation(baseline.position) * Matrix.CreateRotationX(30 * MathHelper.Pi / 180); ;

                    }
                    mesh.Draw();
                }
          

                Matrix[] transforms = null;

                for (int i = 0; i < maxAllowedRow; i++)
                {
                    for (int j = 0; j < 8; j++)
                    {
                        if (ballArray[i][j] != null)
                        {
                            transforms = new Matrix[ballArray[i][j].model.Bones.Count];
                            ballArray[i][j].model.CopyAbsoluteBoneTransformsTo(transforms);

                            foreach (ModelMesh mesh in ballArray[i][j].model.Meshes)
                            {
                                foreach (BasicEffect be in mesh.Effects)
                                {
                                    be.EnableDefaultLighting();
                                    be.Projection = camera.projection;
                                    be.View = camera.view;
                                    be.World = ballArray[i][j].getWorldPosition() * mesh.ParentBone.Transform;
                                    be.Texture = texture;
                                    be.TextureEnabled = true;
                                }
                                mesh.Draw();
                            }
                        }
                    }
                }


                // draw drop balls
                foreach (_3DBall ball in this.dropballList)
                {
                    transforms = new Matrix[ball.model.Bones.Count];
                    ball.model.CopyAbsoluteBoneTransformsTo(transforms);
                    foreach (ModelMesh mesh in ball.model.Meshes)
                    {
                        foreach (BasicEffect be in mesh.Effects)
                        {
                            be.EnableDefaultLighting();
                            be.Projection = camera.projection;
                            be.View = camera.view;
                            be.World = ball.getWorldPosition() * mesh.ParentBone.Transform;                            
                            be.Texture = texture;
                            be.TextureEnabled = true;
                        }
                        mesh.Draw();
                    }
                }



           
                // TODO: Add your drawing code here
                Matrix[] transformsReadyToShoot = new Matrix[ballOfReadyToShoot.model.Bones.Count];
                ballOfReadyToShoot.model.CopyAbsoluteBoneTransformsTo(transformsSlope);

                foreach (ModelMesh mesh in ballOfReadyToShoot.model.Meshes)
                {
                    foreach (BasicEffect be in mesh.Effects)
                    {
                        be.EnableDefaultLighting();
                        be.Projection = camera.projection;
                        be.View = camera.view;
                        be.World = ballOfReadyToShoot.getWorldPosition();// *mesh.ParentBone.Transform;
                    
                    }
                    mesh.Draw();
                }


                Matrix[] transformsNextReadyToShoot = new Matrix[ballOfNextReadyToShoot.model.Bones.Count];
                ballOfNextReadyToShoot.model.CopyAbsoluteBoneTransformsTo(transformsSlope);

                foreach (ModelMesh mesh in ballOfNextReadyToShoot.model.Meshes)
                {
                    foreach (BasicEffect be in mesh.Effects)
                    {
                        be.EnableDefaultLighting();
                        be.Projection = camera.projection;
                        be.View = camera.view;
                        be.World = ballOfNextReadyToShoot.getWorldPosition();// *mesh.ParentBone.Transform;
                    }
                    mesh.Draw();
                }

                Matrix[] transformsArrow = new Matrix[arrow.model.Bones.Count];
                arrow.model.CopyAbsoluteBoneTransformsTo(transformsArrow);

                foreach (ModelMesh mesh in arrow.model.Meshes)
                {
                    foreach (BasicEffect be in mesh.Effects)
                    {
                        be.EnableDefaultLighting();
                        be.Projection = camera.projection;
                        be.View = camera.view;
                        be.World = arrow.getPosition();// *mesh.ParentBone.Transform;                    
                    }
                    mesh.Draw();
                }

                spriteBatch.Begin();


                if (counterDown <= 5)
                {
                    spriteBatch.DrawString(tips,
                                           "Hurry!",
                                           new Vector2(graphics.PreferredBackBufferWidth / 2 - 250, graphics.PreferredBackBufferHeight - 150),
                                           Color.DeepPink,
                                           0,
                                           Vector2.Zero,
                                           1.5f,
                                           SpriteEffects.None,
                                           1);

                    spriteBatch.Draw(star,
                                     new Vector2(graphics.PreferredBackBufferWidth / 2 - 250,
                                     graphics.PreferredBackBufferHeight - 110),
                                     null,
                                     Color.White,
                                     0f,
                                     Vector2.Zero,
                                     1.0f,
                                     SpriteEffects.None,
                                     0);

                    spriteBatch.DrawString(tips,
                                           "" + counterDown,
                                           new Vector2(graphics.PreferredBackBufferWidth / 2 - 210, graphics.PreferredBackBufferHeight - 80),
                                           Color.DarkBlue,
                                           0,
                                           Vector2.Zero,
                                           1.5f,
                                           SpriteEffects.None,
                                           1);
                }
                /*
                spriteBatch.DrawString(tips,"Angle is: " + arrowAngle,
                                               new Vector2(10, 10),
                                               Color.DeepPink,
                                               0,
                                               Vector2.Zero,
                                               1.5f,
                                               SpriteEffects.None,
                                               1);
                */
                int numberOfBall = 0;
                int numberOfDeleteBall = 0;
                int numberOfDropBall = 0;
                int numberOfVisitedBall = 0;

                for (int i = 0; i < 10; i++)
                {
                    for (int j = 0; j < 8; j++)
                    {
                        if (ballArray[i][j] != null)
                        {
                            numberOfBall++;
                            if (ballArray[i][j].canDrop == true)
                            {
                                numberOfDropBall++;
                            }

                            if (ballArray[i][j].hasVisited == true)
                            {
                                numberOfVisitedBall++;
                            }

                            if (ballArray[i][j].needDeleted == true)
                            {
                                numberOfDeleteBall++;
                            }

                        }
                    }
                }
               
              
                spriteBatch.DrawString(tips, "Score: " + stringScore,
                                               new Vector2(500, 10),
                                               Color.DeepPink,
                                               0,
                                               Vector2.Zero,
                                               1.5f,
                                               SpriteEffects.None,
                                               1);

                spriteBatch.End();
            }

            if (currentGameState == GameState.Level_Pass)
            {
                spriteBatch.Begin();
                spriteBatch.DrawString(text,                //print score
                                       "   Round Clear\nScore :" + stringScore,
                                       new Vector2(200, 300),
                                       Color.DarkRed, 0,
                                       Vector2.Zero,
                                       3.0f,
                                       SpriteEffects.None,
                                       1);
                spriteBatch.DrawString(tips,                //print score
                                       "       You are victor",
                                       new Vector2(250, 450),
                                       Color.Blue, 0,
                                       Vector2.Zero,
                                       1.3f,
                                       SpriteEffects.None,
                                       1);
                spriteBatch.End();
            }

            if (currentGameState == GameState.Level_Fail)
            {
                spriteBatch.Begin();
                spriteBatch.DrawString(tips,                //print score
                                       "Game Over, You score is " + stringScore,
                                       new Vector2(220, 450),
                                       Color.Aquamarine, 0,
                                       Vector2.Zero,
                                       1.3f,
                                       SpriteEffects.None,
                                       1);

                
                spriteBatch.End();
            }

            base.Draw(gameTime);
        }

        private void InitialBallDisplayLevelOne()
        {
            //1.Green  2.Blue  3.Red  4.Yellow  5.Cyan  6.Purple

            //Create balls in the first row          
            ballArray[0][3] = new _3DBall(3, 0, 3);              //red ball
            ballArray[0][3].setModel(redBallModel);
            ballArray[0][3].translation(new Vector3(-0.05f, 0.06f, -0.55f));

            ballArray[0][4] = new _3DBall(3, 0, 4);              //red ball
            ballArray[0][4].setModel(redBallModel);
            ballArray[0][4].translation(new Vector3(0.05f, 0.06f, -0.55f));
            
            //Create balls in the second row
            ballArray[1][3] = new _3DBall(5, 1, 3);
            ballArray[1][3].setModel(cyanBallModel);
            ballArray[1][3].translation(new Vector3(-0.1f, 0.06f, -0.45f));

            ballArray[1][4] = new _3DBall(3, 1, 4);
            ballArray[1][4].setModel(redBallModel);
            ballArray[1][4].translation(new Vector3(0.0f, 0.06f, -0.45f));

            
            ballArray[1][5] = new _3DBall(2, 1, 5);
            ballArray[1][5].setModel(blueBallModel);
            ballArray[1][5].translation(new Vector3(0.1f, 0.06f, -0.45f));


            //Create balls in the third row
            //1.Green  2.Blue  3.Red  4.Yellow  5.Cyan  6.Purple

            ballArray[2][2] = new _3DBall(6, 2, 2);
            ballArray[2][2].setModel(purpleBallModel);
            ballArray[2][2].translation(new Vector3(-0.15f, 0.06f, -0.35f));


            ballArray[2][3] = new _3DBall(5, 2, 3);
            ballArray[2][3].setModel(cyanBallModel);
            ballArray[2][3].translation(new Vector3(-0.05f, 0.06f, -0.35f));


            ballArray[2][4] = new _3DBall(2, 2, 4);
            ballArray[2][4].setModel(blueBallModel);
            ballArray[2][4].translation(new Vector3(0.05f, 0.06f, -0.35f));


            ballArray[2][5] = new _3DBall(4, 2, 5);
            ballArray[2][5].setModel(yellowBallModel);
            ballArray[2][5].translation(new Vector3(0.15f, 0.06f, -0.35f));

            //1.Green  2.Blue  3.Red  4.Yellow  5.Cyan  6.Purple
            ballArray[3][2] = new _3DBall(1, 3, 2);
            ballArray[3][2].setModel(greenBallModel);
            ballArray[3][2].translation(new Vector3(-0.2f, 0.06f, -0.25f));


            ballArray[3][3] = new _3DBall(6, 3, 3);
            ballArray[3][3].setModel(purpleBallModel);
            ballArray[3][3].translation(new Vector3(-0.1f, 0.06f, -0.25f));


            ballArray[3][5] = new _3DBall(4, 3, 5);
            ballArray[3][5].setModel(yellowBallModel);
            ballArray[3][5].translation(new Vector3(0.1f, 0.06f, -0.25f));


            ballArray[3][6] = new _3DBall(3, 3, 6);
            ballArray[3][6].setModel(redBallModel);
            ballArray[3][6].translation(new Vector3(0.2f, 0.06f, -0.25f));

            //1.Green  2.Blue  3.Red  4.Yellow  5.Cyan  6.Purple
            //Create balls in the fifth arrow

            ballArray[4][0] = new _3DBall(4, 4, 0);
            ballArray[4][0].setModel(yellowBallModel);
            ballArray[4][0].translation(new Vector3(-0.35f, 0.06f, -0.15f));

            ballArray[4][1] = new _3DBall(2, 4, 1);
            ballArray[4][1].setModel(blueBallModel);
            ballArray[4][1].translation(new Vector3(-0.25f, 0.06f, -0.15f));

            ballArray[4][2] = new _3DBall(1, 4, 2);
            ballArray[4][2].setModel(greenBallModel);
            ballArray[4][2].translation(new Vector3(-0.15f, 0.06f, -0.15f));


            ballArray[4][5] = new _3DBall(3, 4, 5);
            ballArray[4][5].setModel(redBallModel);
            ballArray[4][5].translation(new Vector3(0.15f, 0.06f, -0.15f));

            ballArray[4][6] = new _3DBall(5, 4, 6);
            ballArray[4][6].setModel(cyanBallModel);
            ballArray[4][6].translation(new Vector3(0.25f, 0.06f, -0.15f));

            ballArray[4][7] = new _3DBall(6, 4, 7);
            ballArray[4][7].setModel(purpleBallModel);
            ballArray[4][7].translation(new Vector3(0.35f, 0.06f, -0.15f));

            //1.Green  2.Blue  3.Red  4.Yellow  5.Cyan  6.Purple
            //Create balls in the sixth arrow

            ballArray[5][1] = new _3DBall(4, 5, 1);
            ballArray[5][1].setModel(yellowBallModel);        //4---yellow
            ballArray[5][1].translation(new Vector3(-0.3f, 0.06f, -0.05f));


            ballArray[5][2] = new _3DBall(2, 5, 2);
            ballArray[5][2].setModel(blueBallModel);          //2---blue
            ballArray[5][2].translation(new Vector3(-0.2f, 0.06f, -0.05f));


            ballArray[5][6] = new _3DBall(5, 5, 6);
            ballArray[5][6].setModel(cyanBallModel);
            ballArray[5][6].translation(new Vector3(0.2f, 0.06f, -0.05f));


            ballArray[5][7] = new _3DBall(6, 5, 7);
            ballArray[5][7].setModel(purpleBallModel);
            ballArray[5][7].translation(new Vector3(0.3f, 0.06f, -0.05f));
            

            //Draw readyToFireBall and NextReadyToFireBall
            //1.Green  2.Blue  3.Red  4.Yellow  5.Cyan  6.Purple

            Random random = new Random();        
            //ballOfReadyToShoot = new _3DBall(random.Next(6) + 1, -1, -1);
            ballOfReadyToShoot = new _3DBall(2 + 1, -1, -1);
            int ballColor = ballOfReadyToShoot.getBallColor();
            switch(ballColor)
            {
                case 1:
                    ballOfReadyToShoot.setModel(greenBallModel);
                    break;
                case 2:
                    ballOfReadyToShoot.setModel(blueBallModel);
                    break;
                case 3:
                    ballOfReadyToShoot.setModel(redBallModel);
                    break;
                case 4:
                    ballOfReadyToShoot.setModel(yellowBallModel);
                    break;
                case 5:
                    ballOfReadyToShoot.setModel(cyanBallModel);
                    break;
                case 6:
                    ballOfReadyToShoot.setModel(purpleBallModel);
                    break;
            }
            ballOfReadyToShoot.translation(new Vector3(0.0f, 0.06f, 0.45f));

            ballOfNextReadyToShoot = new _3DBall(random.Next(6) + 1, -1, -1);
            ballColor = ballOfNextReadyToShoot.getBallColor();
            switch (ballColor)
            {
                case 1:
                    ballOfNextReadyToShoot.setModel(greenBallModel);
                    break;
                case 2:
                    ballOfNextReadyToShoot.setModel(blueBallModel);
                    break;
                case 3:
                    ballOfNextReadyToShoot.setModel(redBallModel);
                    break;
                case 4:
                    ballOfNextReadyToShoot.setModel(yellowBallModel);
                    break;
                case 5:
                    ballOfNextReadyToShoot.setModel(cyanBallModel);
                    break;
                case 6:
                    ballOfNextReadyToShoot.setModel(purpleBallModel);
                    break;
            }
            ballOfNextReadyToShoot.translation(new Vector3(-0.35f, 0.06f, 0.55f));

            currentGameState = GameState.Level_Run;

        }
    }
}
