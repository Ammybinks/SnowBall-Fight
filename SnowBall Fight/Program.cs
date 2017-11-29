using System;

namespace SnowBall_Fight
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        static void Main(string[] args)
        {
            using (SnowBallFight game = new SnowBallFight())
            {
                game.Run();
            }
        }
    }
}

