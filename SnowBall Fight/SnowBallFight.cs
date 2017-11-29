////////////////////////////////////////////////////////////////
// Copyright 2013, CompuScholar, Inc.
//
// This source code is for use by the students and teachers who 
// have purchased the corresponding TeenCoder or KidCoder product.
// It may not be transmitted to other parties for any reason
// without the written consent of CompuScholar, Inc.
// This source is provided as-is for educational purposes only.
// CompuScholar, Inc. makes no warranty and assumes
// no liability regarding the functionality of this program.
//
////////////////////////////////////////////////////////////////

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
using Microsoft.Xna.Framework.Net;
using Microsoft.Xna.Framework.Storage;
using SpriteLibrary;


namespace SnowBall_Fight
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class SnowBallFight : Microsoft.Xna.Framework.Game
    {
        // define the maximum possible speed for the snowball
        const float MAX_SPEED = 10;

        // content and graphcics-related objects
        private GraphicsDeviceManager graphics;
        private SpriteBatch spriteBatch;

        // Textures used in this game
        Texture2D arrowTexture;
        Texture2D groundTexture;
        Texture2D player1Texture;
        Texture2D player2Texture;
        Texture2D snowballTexture;
        Texture2D snowfortTexture;
        
        // the members below are part of the game state
        // and are provided as part of the Activity Starter.
        private KeyboardState oldKeyboardState;

        private GamePadState oldGamePadState1;
        private GamePadState oldGamePadState2;

        // this variable shows how fast the snowball was thrown
        private float thrownSpeed = 0;

        // sprites for all of the game objects
        private Sprite snowball;

        private Sprite snowFort1;
        private Sprite snowFort2;

        private Sprite ground;

        private Sprite player1;
        private Sprite player2;

        private Sprite directionArrow;

        // this variable keeps track of the current player turn
        private int currentPlayer = 1;

        // this variable shows whether or not the game is over
        private bool gameOver = false;

        // font and string for message display
        private SpriteFont gameFont;
        private String displayMessage = "";

        private bool isPlayer1Throwing = false;
        private bool isPlayer2Throwing = false;

        private bool isPlayer1Hit = false;
        private bool isPlayer2Hit = false;

        // This method is provided fully complete as part of the activity starter.
        public SnowBallFight()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        // This method is provided fully complete as part of the activity starter.
        protected override void Initialize()
        {
            // call base.Initialize() first to get LoadContent called
            // (and therefore all textures initialized) prior to starting game!
            base.Initialize();

            // now that we have loaded the textures, create sprites and assign images to sprites
            initializeSprites();

            // now that all of the sprites are initialized, start the game
            startGame();


        }

        // This method is provided fully complete as part of the activity starter.
        private void startGame()
        {
            // reset to a new game by clearing the gameOver flag and message
            gameOver = false;
            displayMessage = "";

            // start again with player 1
            setPlayer(1);
        }

        // This method is provided fully complete as part of the activity starter.
        private void stopGame(String msg)
        {
            // update the display message and game over flag
            displayMessage = msg;
            gameOver = true;

            // hide the snow ball
            snowball.IsAlive = false;
        }

        // This method is provided fully complete as part of the activity starter.
        private void initializeSprites()
        {
            // create a new snowball sprite, set the texture, and hide it initially
            snowball = new Sprite();
            snowball.SetTexture(snowballTexture);
            snowball.IsAlive = false;
            snowball.MaxSpeed = MAX_SPEED;  // can't throw any faster than this!

            // initialize and position the ground sprite
            ground = new Sprite();
            ground.SetTexture(groundTexture);
            ground.UpperLeft = new Vector2(0, GraphicsDevice.Viewport.Height - 50);

            // initialize and position the two snow forts
            snowFort1 = new Sprite();
            snowFort1.SetTexture(snowfortTexture);
            snowFort1.UpperLeft = new Vector2(150, ground.UpperLeft.Y - snowFort1.GetHeight());

            snowFort2 = new Sprite();
            snowFort2.SetTexture(snowfortTexture);
            snowFort2.UpperLeft = new Vector2(GraphicsDevice.Viewport.Width - 150 - snowFort2.GetWidth(), ground.UpperLeft.Y - snowFort2.GetHeight());

            // initialize and position the two player sprites
            player1 = new Sprite();
            player1.SetTexture(player1Texture, 7);
            player1.ContinuousAnimation = false;    // this is not a continuous animation loop!
            player1.AnimationInterval = 200;        // advance current frame each 200 ms when animating
            player1.UpperLeft = new Vector2(20, GraphicsDevice.Viewport.Height - ground.GetHeight()- player1.GetHeight() + 5);

            player2 = new Sprite();
            player2.SetTexture(player2Texture, 7);
            player2.ContinuousAnimation = false;    // this is not a continuous animation loop!
            player2.AnimationInterval = 200;        // advance current frame each 200 ms when animating
            player2.UpperLeft = new Vector2(GraphicsDevice.Viewport.Width - 25 - player2.GetWidth(), GraphicsDevice.Viewport.Height - ground.GetHeight() - player2.GetHeight()+ 5);

            // initialize the arrow texture 
            // (all other arrrow sprite parameters will be set when we change players)
            directionArrow = new Sprite();
            directionArrow.SetTexture(arrowTexture);
        }

        // This method is provided fully complete as part of the activity starter.
        private void changePlayer()
        {
            // switch to player 2 if player 1, or vice versa
            if (currentPlayer == 1)
                setPlayer(2);
            else
                setPlayer(1);
        }

        // This method is provided fully complete as part of the activity starter.
        private void setPlayer(int newPlayer)
        {
            // if we are changing to player 1's turn
            if (newPlayer == 1)
            {
                // initialize arrow for player 1
                directionArrow.UpperLeft = new Vector2(player1.UpperLeft.X + player1.GetWidth(), player1.UpperLeft.Y);
                directionArrow.RotationAngle = 0.0f;
                directionArrow.Origin = new Vector2(0, directionArrow.GetHeight() / 2);
                directionArrow.Scale.X = 0.5f;

                player2.setCurrentFrame(3);
                player1.setCurrentFrame(0);

            }
            else // we are changing to player 2's turn
            {
                // initialize arrow for player 2
                directionArrow.UpperLeft = new Vector2(player2.UpperLeft.X, player2.UpperLeft.Y);
                directionArrow.RotationAngle = 180.0f;
                directionArrow.Origin = new Vector2(0, directionArrow.GetHeight() / 2);
                directionArrow.Scale.X = 0.5f;

                player1.setCurrentFrame(3);
                player2.setCurrentFrame(0);
        	}

            // update current player
            currentPlayer = newPlayer;
            
            // reset current thrown speed
            thrownSpeed = 0;
        }


        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        // This method is provided fully complete as part of the activity starter.
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);


            gameFont = Content.Load<SpriteFont>("GameFont");

            // load all of the textures used in the game
            arrowTexture = Content.Load<Texture2D>("Images\\Arrow");
            groundTexture = Content.Load<Texture2D>("Images\\Ground");
            player1Texture = Content.Load<Texture2D>("Images\\Reindeer1_Strip");
            player2Texture = Content.Load<Texture2D>("Images\\Reindeer2_Strip");
            snowballTexture = Content.Load<Texture2D>("Images\\SnowBall");
            snowfortTexture = Content.Load<Texture2D>("Images\\SnowFort");

        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// all content.
        /// </summary>
        // This method is provided fully complete as part of the activity starter.
        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        // This method is provided fully complete as part of the activity starter.
        protected override void Update(GameTime gameTime)
        {
            // Allows the game to exit
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed)
                this.Exit();

            // has some animation short already started
            if (isPlayer1Throwing)
            {
                player1.Animate(gameTime);   // advance to next frame if it's time
                if (!player1.IsAnimating())  // see if animation short has completed
                {
                    snowball.IsAlive = true;
                    isPlayer1Throwing = false;
                }
            }
            else if (isPlayer2Throwing)
            {
                player2.Animate(gameTime);   // advance to next frame if it's time
                if (!player2.IsAnimating())  // see if animation short has completed
                {
                    snowball.IsAlive = true;
                    isPlayer2Throwing = false;
                }
            }
            else if (isPlayer1Hit)
            {
                player1.Animate(gameTime);   // advance to next frame if it's time
                if (!player1.IsAnimating())  // see if animation short has completed
                {
                    isPlayer1Hit = false;
                }
            }
            else if (isPlayer2Hit)
            {
                player2.Animate(gameTime);   // advance to next frame if it's time
                if (!player2.IsAnimating())  // see if animation short has completed
                {
                    isPlayer2Hit = false;
                }
            }
            
            // handle all user input
            handleKeyPress();

            //Optionally handle Xbox gamepad input
            handleXboxGamepads();

            // move the snowball if in flight
            moveSnowball();

            // check to see if the snowball hits anything
            checkCollisions();

            base.Update(gameTime);
        }

        // The student will complete part of this function during an activity
        private void handleKeyPress()
        {

            // ****************************************************************************************
            // This part of the function provided complete as part of the starter activity

            // save old key state
            KeyboardState currentKeyboard = Keyboard.GetState();

            if (oldKeyboardState == null)
                oldKeyboardState = currentKeyboard;

            KeyboardState KeyState = Keyboard.GetState();

            if (currentPlayer == 1)
            {
                GamePadState ReturnState = GamePad.GetState(PlayerIndex.One);
                if (ReturnState.IsConnected)
                    return;

                if (KeyState.IsKeyDown(Keys.Enter))
                    if (snowball.IsAlive == false)
                        if (thrownSpeed <= MAX_SPEED)
                        {
                            thrownSpeed += 0.2f;

                            directionArrow.Scale.X += 0.05f;
                        }
                if (KeyState.IsKeyUp(Keys.Enter) && oldKeyboardState.IsKeyDown(Keys.Enter))
                    if (gameOver == true)
                        startGame();
                    else if (snowball.IsAlive == false)
                        throwSnowball();

                if (currentKeyboard.IsKeyDown(Keys.Left))
                {
                    directionArrow.RotationAngle += 1;
                }

                if (currentKeyboard.IsKeyDown(Keys.Right))
                {
                    directionArrow.RotationAngle -= 1;
                }
            }

            if (currentPlayer == 2)
            {
                GamePadState ReturnState = GamePad.GetState(PlayerIndex.Two);
                if (ReturnState.IsConnected)
                    return;

                if (KeyState.IsKeyDown(Keys.Space))
                    if (snowball.IsAlive == false)
                        if (thrownSpeed <= MAX_SPEED)
                        {
                            thrownSpeed += 0.2f;

                            directionArrow.Scale.X += 0.05f;
                        }

                if (KeyState.IsKeyUp(Keys.Space) && oldKeyboardState.IsKeyDown(Keys.Space))
                    if (gameOver == true)
                        startGame();
                    else if (snowball.IsAlive == false)
                        throwSnowball();

                if (currentKeyboard.IsKeyDown(Keys.A))
                {
                    directionArrow.RotationAngle += 1;
                }

                if (currentKeyboard.IsKeyDown(Keys.D))
                {
                    directionArrow.RotationAngle -= 1;
                }
            }
             // if right arrow is held down, make angle smaller
            

            // if left arrow is held down, make angle bigger
            

            // keep angle with [0,90] degrees for player 1
            if (currentPlayer == 1)
            {
                if (directionArrow.RotationAngle < 0)
                    directionArrow.RotationAngle = 0;
                if (directionArrow.RotationAngle > 90)
                    directionArrow.RotationAngle = 90;
            }
            // keep angle with [90,180] degrees for player 2
            if (currentPlayer == 2)
            {
                if (directionArrow.RotationAngle < 90)
                    directionArrow.RotationAngle = 90;
                if (directionArrow.RotationAngle > 180)
                    directionArrow.RotationAngle = 180;
            }

            // ****************************************************************************************
            // Student will complete this section as part of the activity

            
            // ****************************************************************************************

            oldKeyboardState = currentKeyboard;
            
        }

        
        // OPTIONAL: The student may complete part of this function during an activity, if they are using Xbox gamepads
        // If the student is not using gamepads, they do NOT have to complete this activity
        private void handleXboxGamepads()
        {
            // ****************************************************************************************
            // This part of the function provided complete as part of the starter activity

            //save the old gamepad states 
            GamePadState currentGamePad;
            GamePadState oldGamePad;

            // If we are using gamepads, retreive the current state for the gamepad
            // and set the correct old gamepad state
            if (currentPlayer == 1)
            {
                // get the state of player 1's gamepad
                currentGamePad = GamePad.GetState(PlayerIndex.One);
                oldGamePad = oldGamePadState1;
            }
            else
            {
                // get the state of player 2's gamepad
                currentGamePad = GamePad.GetState(PlayerIndex.Two);
                oldGamePad = oldGamePadState2;
            }

            if (oldGamePad == null)
                oldGamePad = currentGamePad;

            if (currentGamePad.IsButtonDown(Buttons.A))
                if (snowball.IsAlive == false)
                    if (thrownSpeed <= MAX_SPEED)
                    {
                        thrownSpeed += 0.2f;

                        directionArrow.Scale.X += 0.05f;
                    }

            if (currentGamePad.IsButtonUp(Buttons.A) && oldGamePad.IsButtonDown(Buttons.A))
                if (gameOver == true)
                    startGame();
                else if (snowball.IsAlive == false)
                    throwSnowball();
            if (currentGamePad.ThumbSticks.Left.X < 0)
            {
                directionArrow.RotationAngle += 1;
            }

            if (currentGamePad.ThumbSticks.Left.X > 0)
            {
                directionArrow.RotationAngle -= 1;
            }

            // keep angle with [0,90] degrees for player 1
            if (currentPlayer == 1)
            {
                if (directionArrow.RotationAngle < 0)
                    directionArrow.RotationAngle = 0;
                if (directionArrow.RotationAngle > 90)
                    directionArrow.RotationAngle = 90;
            }
            // keep angle with [90,180] degrees for player 2
            if (currentPlayer == 2)
            {
                if (directionArrow.RotationAngle < 90)
                    directionArrow.RotationAngle = 90;
                if (directionArrow.RotationAngle > 180)
                    directionArrow.RotationAngle = 180;
            }

            

            //*****************************************************************************************************
            // OPTIONAL: Student will complete this section as part of the activity if using Xbox Gamepad controllers

            

            //*****************************************************************************************************

            if (currentPlayer == 1)
                oldGamePadState1 = currentGamePad;
            else
                oldGamePadState2 = currentGamePad;
        }

        // This method is provided fully complete as part of the activity starter.
        private void animateThrow()
        {
            // begin short throwing animation
            if (currentPlayer == 1)
            {
                isPlayer1Throwing = true;
                player1.StartAnimationShort(0, 2, 0);   // begin animation short
            }
            else
            {
                isPlayer2Throwing = true;
                player2.StartAnimationShort(0, 2, 0);   // begin animation short
            }
        }

        // The student will complete this function as part of an activity
        private void throwSnowball()
        {

            if (currentPlayer == 1)
            {
                snowball.UpperLeft.X = player1.UpperLeft.X + 75;
                snowball.UpperLeft.Y = player1.UpperLeft.Y;
            }
            else
            {
                snowball.UpperLeft.X = player2.UpperLeft.X;
                snowball.UpperLeft.Y = player2.UpperLeft.Y;
            }

            snowball.SetSpeedAndDirection(thrownSpeed, directionArrow.RotationAngle);
            animateThrow();

        }

       
        // The student will complete this function as part of an activity
        private void moveSnowball()
        {

            if (snowball.IsAlive)
            {
                snowball.Accelerate(0.0f, 0.1f);
                snowball.MoveAndVanish(GraphicsDevice.Viewport.Width, GraphicsDevice.Viewport.Height);
                if (snowball.IsAlive == false)
                    changePlayer();
            }

        }

        // This method is provided fully complete as part of the activity starter.
        private void checkCollisions()
        {
            // see if snowball has hit player1
            if (snowball.IsCollided(player1))
            {
                // begin animation short showing hit player
                isPlayer1Hit = true;
                player1.StartAnimationShort(4, 6, 6);

                // game is over
                stopGame("Player 2 wins!");
            }

            // see if snowball has hit player2
            if (snowball.IsCollided(player2))
            {
                // begin animation short showing hit player
                isPlayer2Hit = true;
                player2.StartAnimationShort(4, 6, 6);

                // game is over
                stopGame("Player 1 wins!");
            }

            // see if snowball has hit ground
            if (snowball.IsCollided(ground))
            {
                // hide snowball and move to next player's turn
                snowball.IsAlive = false;
                changePlayer();
            }

            // see if snowball has hit snow fort 1
            if (snowball.IsCollided(snowFort1))
            {
                // hide snowball and move to next player's turn
                snowball.IsAlive = false;
                changePlayer();
            }

            // see if snowball has hit snow fort 2
            if (snowball.IsCollided(snowFort2))
            {
                // hide snowball and move to next player's turn
                snowball.IsAlive = false;
                changePlayer();
            }
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        // This method is provided fully complete as part of the activity starter.
        protected override void Draw(GameTime gameTime)
        {
            // set the sky to a nice blue
            GraphicsDevice.Clear(Color.LightBlue);

            spriteBatch.Begin();

            // Draw the ground, snow forts, person, snowball, and arrow
            ground.Draw(spriteBatch);
            
            snowFort1.Draw(spriteBatch);
            snowFort2.Draw(spriteBatch);

            player1.Draw(spriteBatch);
            player2.Draw(spriteBatch);

            // if game is over
            if (gameOver)
            {
                // show the game-over message
                spriteBatch.DrawString(gameFont, displayMessage, new Vector2(50, 100), Color.Black);
            }
            else
            {
                // draw the snowball if it's alive
                snowball.Draw(spriteBatch);

                // draw the current direction arrow
                directionArrow.Draw(spriteBatch);
            }

            spriteBatch.End();

            base.Draw(gameTime);
        }


    }
}
