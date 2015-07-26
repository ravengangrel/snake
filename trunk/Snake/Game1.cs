using System;
using System.Collections.Generic;


using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Snake
{
    /// <summary>
    /// This is the main type for your game.
    /// </summary>
    public class Game1 : Game
    {
        enum Direction
        {
            Invalid,
            UP,
            DOWN,
            LEFT,
            RIGHT
        }

        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;

        Random rng;
        
        Texture2D manzana;
        Texture2D cabeza;
        Texture2D cuerpo;

        List<Point> snake = new List<Point>();
        Point fruit;

        int timeBase = 150;
        int snakeLength = 1;
        int increment = 1;
        Direction dir;
        GameTime lastTime;

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            graphics.IsFullScreen = false;
            graphics.PreferredBackBufferWidth = 640;
            graphics.PreferredBackBufferHeight = 640;
            graphics.ApplyChanges();
            
            Content.RootDirectory = "Content";
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
            rng = new Random();

            CreateSnakeAtRandomPos();

            CreateFruitAtRandomPos();
            base.Initialize();
        }

        protected void CreateSnakeAtRandomPos()
        {
            int hInit = rng.Next(0, 19);
            int vInit = rng.Next(0, 19);
            snake.Add(new Point(hInit, vInit));
        }

        protected void CreateFruitAtRandomPos()
        {
            do
            {
                fruit.X = rng.Next(0, 19);
                fruit.Y = rng.Next(0, 19);
            } while (IsFruitUnderSnake() || FruitToSnakeDistance() < 5);
        }

        protected int FruitToSnakeDistance()
        {
            return (int) Math.Floor((Math.Sqrt(Math.Pow(fruit.X - snake[0].X, 2) + Math.Pow(fruit.Y - snake[0].Y, 2))));
        }
        protected bool IsFruitUnderSnake()
        {
            foreach (var pt in snake)
                if (fruit.X == pt.X && fruit.Y == pt.Y)
                    return true;

            return false;
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);
            
            cabeza = Content.Load<Texture2D>("cabeza.png");
            manzana = Content.Load<Texture2D>("manzana");
            cuerpo = Content.Load<Texture2D>("cuerpo");
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// game-specific content.
        /// </summary>
        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
            cabeza.Dispose();
            manzana.Dispose();
            cuerpo.Dispose();
        }

        double elapsed = 0;
        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            if (Keyboard.GetState().IsKeyDown(Keys.W))
            {
                dir = Direction.UP;
            }
            else if (Keyboard.GetState().IsKeyDown(Keys.S))
            {
                dir = Direction.DOWN;
            }
            else if (Keyboard.GetState().IsKeyDown(Keys.A))
            {
                dir = Direction.LEFT;
            }
            else if (Keyboard.GetState().IsKeyDown(Keys.D))
            {
                dir = Direction.RIGHT;
            }
            else if (Keyboard.GetState().IsKeyDown(Keys.Q))
            {
                Exit();
            }

            elapsed += gameTime.ElapsedGameTime.TotalMilliseconds;
            if (elapsed > timeBase)
            {
                long numRounds = (long)Math.Floor((elapsed) / timeBase);
                for (int i = 0; i < numRounds; i++)
                {
                    Point newPos;
                    if (dir == Direction.UP)
                        newPos = new Point(snake[0].X, snake[0].Y - 1);
                    else if (dir == Direction.DOWN)
                        newPos = new Point(snake[0].X, snake[0].Y + 1);
                    else if (dir == Direction.LEFT)
                        newPos = new Point(snake[0].X - 1, snake[0].Y);
                    else if (dir == Direction.RIGHT)
                        newPos = new Point(snake[0].X + 1, snake[0].Y);
                    else
                        newPos = snake[0];
                    if (newPos.X >= 20)
                        newPos.X = newPos.X % 20;
                    if (newPos.Y >= 20)
                        newPos.Y = newPos.Y % 20;
                    if (newPos.X < 0)
                        newPos.X = newPos.X + 20;
                    if (newPos.Y < 0)
                        newPos.Y = newPos.Y + 20;


                    if (IsFruitUnderSnake())
                    {
                        snakeLength += increment;

                        if (timeBase - (increment * 10) < 75)
                            timeBase = 75;
                        else
                            timeBase -= (increment * 10);

                        if (increment < 5)
                            increment++;

                        CreateFruitAtRandomPos();
                    }

                    snake.Insert(0, newPos);
                    while (snake.Count > snakeLength)
                        snake.RemoveAt(snake.Count - 1);
               

                    elapsed -= timeBase;
                }
            }

            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);
            
            spriteBatch.Begin();
            spriteBatch.Draw(manzana, new Rectangle(32 * fruit.X, 32 * fruit.Y, 32, 32), Color.White);

            for (int i = 0; i < snake.Count; i++)
                spriteBatch.Draw(i == 0 ? cabeza : cuerpo, new Rectangle(32 * snake[i].X, 32 * snake[i].Y, 32, 32), Color.White);
            
            spriteBatch.End();
            // TODO: Add your drawing code here

            base.Draw(gameTime);
        }
    }
}
