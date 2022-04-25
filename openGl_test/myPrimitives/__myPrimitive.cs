using GLFW;
using static OpenGL.GL;
using System;


public class myPrimitive
{
    protected static int Width = -1, Height = -1;
    protected float _r = 0, _g = 0, _b = 0, _a = 0;

    // One time call to let primitives know the screen dimensions
    public static void setScreenDimensions(int width, int height)
    {
        if (Width < 0 && Height < 0)
        {
            Width = width;
            Height = height;
        }
    }

    protected void updUniformScreenSize(int location, int w, int h)
    {
        glUniform2i(location, w, h);
    }

    // Just remember the color value
    public void SetColor(float r, float g, float b, float a)
    {
        _r = r;
        _g = g;
        _b = b;
        _a = a;
    }

    // Update program with a color (the program must be activated via 'glUseProgram' before that)
    public void setColor(int location, float r, float g, float b, float a)
    {
        glUniform4f(location, r, g, b, a);
    }
};
