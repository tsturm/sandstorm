using System;
using SandstormKinect;

namespace Sandstorm
{
#if WINDOWS || XBOX
    static class Program
    {
        static Sandstorm game;

        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        static void Main(string[] args)
        {
            SandstormEditor editor = new SandstormEditor();
            editor.Disposed += new EventHandler(form_Disposed);

            SandstormBeamer beamer = new SandstormBeamer();
            beamer.Disposed += new EventHandler(form_Disposed);


            SandstormKinectThread kinect = new SandstormKinectThread();
            kinect.

            using (game = new Sandstorm(editor, beamer))
            {
                game.Run();
            }
        }

        static void form_Disposed (object sender, EventArgs e)
        {
            game.Exit();
        }
    }
#endif
}

