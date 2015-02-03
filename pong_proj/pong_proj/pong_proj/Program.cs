using System;

namespace pong_proj
{
#if WINDOWS || XBOX
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        static void Main(string[] args)
        {
            using (Space_Pong game = new Space_Pong())
            {
                game.Run();
            }
        }
    }
#endif
}

