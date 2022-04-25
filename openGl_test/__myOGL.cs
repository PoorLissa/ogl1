using System;
using GLFW;
using static OpenGL.GL;
using System.Runtime.InteropServices;   // For dll import

class myOGL
{
    // -------------------------------------------------------------------------------------------------------------------

    public static void PrepareContext()
    {
        // Set some common hints for the OpenGL profile creation
        Glfw.WindowHint(Hint.ClientApi, ClientApi.OpenGL);
        Glfw.WindowHint(Hint.ContextVersionMajor, 3);
        Glfw.WindowHint(Hint.ContextVersionMinor, 3);
        Glfw.WindowHint(Hint.OpenglProfile, Profile.Core);
        Glfw.WindowHint(Hint.Doublebuffer, true);
        Glfw.WindowHint(Hint.Decorated, false);

        Glfw.WindowHint(Hint.Samples, 0);
        //Glfw.WindowHint(Hint.ScaleToMonitor, true);

        //Glfw.WindowHint(Hint.Floating, false);
    }

    // -------------------------------------------------------------------------------------------------------------------

    /// <summary>
    /// Creates and returns a handle to a GLFW window with a current OpenGL context.
    /// </summary>
    /// <param name="width">The width of the client area, in pixels.</param>
    /// <param name="height">The height of the client area, in pixels.</param>
    /// <returns>A handle to the created window.</returns>
    public static Window CreateWindow(ref int width, ref int height, string Title, bool trueFullScreen = false)
    {
        // Create window, make the OpenGL context current on the thread, and import graphics functions

        Window window;

        if (trueFullScreen)
        {
            // Set monitor to real full screen mode (which takes a long time somehow)
            var mode = Glfw.GetVideoMode(Glfw.PrimaryMonitor);
            width  = mode.Width;
            height = mode.Height;
            window = Glfw.CreateWindow(width, height, Title, Glfw.PrimaryMonitor, Window.None);
        }
        else
        {
            if (width == 0 && height == 0)
            {
                // Create window that fully covers the entire monitor at its current resolution
                var mode = Glfw.GetVideoMode(Glfw.PrimaryMonitor);
                width  = mode.Width;
                height = mode.Height;
                window = Glfw.CreateWindow(width, height, Title, Monitor.None, Window.None);
            }
            else
            {
                // Create window with required size
                window = Glfw.CreateWindow(width, height, Title, Monitor.None, Window.None);

                // Center window
                var screen = Glfw.PrimaryMonitor.WorkArea;
                var x = (screen.Width  -  width) / 2;
                var y = (screen.Height - height) / 2;

                Glfw.SetWindowPosition(window, x, y);
            }
        }

        Glfw.MakeContextCurrent(window);
        Import(Glfw.GetProcAddress);

        return window;
    }

    // -------------------------------------------------------------------------------------------------------------------

    /// <summary>
    /// Creates a shader of the specified type from the given source string.
    /// </summary>
    /// <param name="type">An OpenGL enum for the shader type.</param>
    /// <param name="source">The source code of the shader.</param>
    /// <returns>The created shader. No error checking is performed for this basic example.</returns>
    public static uint CreateShader(int type, string source)
    {
        var shader = glCreateShader(type);
        glShaderSource(shader, source);
        glCompileShader(shader);
        return shader;
    }

    // -------------------------------------------------------------------------------------------------------------------

    /// <summary>
    /// Creates a shader of the specified type from the given source string.
    /// </summary>
    /// <param name="type">An OpenGL enum for the shader type.</param>
    /// <param name="source">The source code of the shader.</param>
    /// <returns>The created shader. No error checking is performed for this basic example.</returns>
    public static uint CreateShaderEx(int type, string header, string main)
    {
        string src = "#version 330 core\n";

        src += header;
        src += "void main(){";
        src += main;
        src += "}";

        var shader = glCreateShader(type);
        glShaderSource(shader, src);
        glCompileShader(shader);
        return shader;
    }

    // -------------------------------------------------------------------------------------------------------------------
}
