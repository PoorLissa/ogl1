using GLFW;
using static OpenGL.GL;
using System;


// Original source      : https://www.youtube.com/watch?v=LcHCygwIgLo
// Hello Triangle       : https://learnopengl.com/Getting-started/Hello-Triangle
// Glfw manual          : https://www.glfw.org/docs/3.3/index.html
// Read later           : https://www.khronos.org/opengl/wiki/Common_Mistakes
// GLSL optimization    : https://stackoverflow.com/questions/22379978/are-glsl-compilers-well-optimized
// GLFW Users Guide     : https://www.glfw.org/GLFWUsersGuide277.pdf


class pt
{
    public float x, y;
    private float dx, dy;

    public pt(Random r)
    {
        x = 0;
        y = 0;

        dx = (float)((r.NextDouble() * 2) - 1.0f) / 100;
        dy = (float)((r.NextDouble() * 2) - 1.0f) / 100;
    }

    public void Move()
    {
        x += dx;
        y += dy;

        float max = 0.9f;

        if (x <= -max || x >= max)
            dx *= -1;

        if (y <= -max || y >= max)
            dy *= -1;
    }
};

class myObj
{
    float r = 0, g = 0, b = 0, a = 0;

    pt p1 = null, p2 = null, p3 = null;

    public myObj(Random r)
    {
        p1 = new pt(r);
        p2 = new pt(r);
        p3 = new pt(r);
    }

    public void Move()
    {
        p1.Move();
        p2.Move();
        p3.Move();
    }

    public void changeColor(Random rand)
    {
        r = (float)rand.NextDouble();
        g = (float)rand.NextDouble();
        b = (float)rand.NextDouble();
        a = (float)rand.NextDouble()/20;
    }

    public void Draw(Triangle t)
    {
        t.SetColor(r, g, b, a);
        t.Draw(p1.x, p1.y, p2.x, p2.y, p3.x, p3.y, false);
    }
}

class myObj2
{
    float x, y, r, time, dt;

    public myObj2(int w, int h, Random rand)
    {
        x = rand.Next(w);
        y = rand.Next(h);
        r = rand.Next(111);
        time = 0;
        dt = 0.0001f * (rand.Next(100)+1);
    }

    public void Move()
    {
        r += (float)(10 * Math.Sin(time));
        time += dt;
    }

    public void Draw(myEllipse el)
    {
        el.SetColor(0.9f, 033f, 0.66f, 0.33f);
        //el.setLineThickness(3);
        el.resetLineThickness();
        el.Draw((int)(x-r), (int)(y-r), (int)(r+r), (int)(r+r), false);
    }
};



class Program
{
    private static Random rand = null;

    // -------------------------------------------------------------------------------------------------------------------

    private static void processInput(Window window)
    {
        if (Glfw.GetKey(window, GLFW.Keys.Escape) == GLFW.InputState.Press)
            Glfw.SetWindowShouldClose(window, true);
    }

    // -------------------------------------------------------------------------------------------------------------------

    static void Main(string[] args)
    {
        Window window;
        rand = new Random();
        long n = 0;
        int Width = 0, Height = 0;

        // Set context creation hints
        myOGL.PrepareContext();

        // Create a window and shader program
        if (true)
        {
            Width = 1920;
            Height = 1200;
        }

        window = myOGL.CreateWindow(ref Width, ref Height, "my Window", trueFullScreen: false);

        // One time call to let primitives know the screen dimensions
        myPrimitive.setScreenDimensions(Width, Height);

        var t = new Triangle();

        var e = new myEllipse();
        e.SetColor(1.0f, 0.25f, 0.25f, 0.5f);

        var l = new Line();
        l.SetColor(0, 1, 0, 1);

        var r = new myRectangle();
        r.SetColor(1, 1, 0, 0.33f);

        var r2 = new myRectangle();
        r2.SetColor(1, 0, 0, 0.33f);

        var h = new myHexagon();
        h.SetColor(1, 0, 0, 0.33f);

        var list1 = new System.Collections.Generic.List<myObj>();
        for (int i = 0; i < 1111; i++)
        {
            var obj = new myObj(rand);
            obj.changeColor(rand);
            list1.Add(obj);
        }

        var list2 = new System.Collections.Generic.List<myObj2>();
        for (int i = 0; i < 333; i++)
        {
            var obj = new myObj2(Width, Height, rand);
            list2.Add(obj);
        }


        glEnable(GL_BLEND);                                 // Enable blending
        glBlendFunc(GL_SRC_ALPHA, GL_ONE_MINUS_SRC_ALPHA);  // Set blending function


        if (false)
        {
            // https://community.khronos.org/t/how-to-turn-off-antialiasing/38308
            glDisable(GL_DITHER);
            //glDisable(GL_POINT_SMOOTH);
            glDisable(GL_LINE_SMOOTH);
            glDisable(GL_POLYGON_SMOOTH);
            //glHint(GL_POINT_SMOOTH, GL_DONT_CARE);
            glHint(GL_LINE_SMOOTH, GL_DONT_CARE);
            glHint(GL_POLYGON_SMOOTH_HINT, GL_DONT_CARE);
            var GL_MULTISAMPLE_ARB = 0x809D;
            glDisable(GL_MULTISAMPLE_ARB);
        }


        //Glfw.SwapInterval(111);

        float angle1 = 0.0f;
        float angle2 = 0.0f;

        while (!Glfw.WindowShouldClose(window))
        {
            processInput(window);

            // Swap fore/back framebuffers, and poll for operating system events.
            Glfw.SwapBuffers(window);
            Glfw.PollEvents();

            // Clear the framebuffer to defined background color
            glClearColor(0.2f, 0.3f, 0.3f, 1.0f);
            glClear(GL_COLOR_BUFFER_BIT);

            foreach (var item in list1)
            {
                item.Draw(t);
                item.Move();
            }

            foreach (var item in list2)
            {
                item.Draw(e);
                item.Move();
            }

#if false
            for (int i = 0; i < 11; i++)
            {
                int x = rand.Next(Width);
                int y = rand.Next(Height);

                int rr = rand.Next(111);
                e.Draw(x - rr, y - rr, 2 * rr, 2 * rr, false);
                e.Draw(x - rr, y - rr, 2 * rr, 2 * rr, true);
            }
            System.Threading.Thread.Sleep(333);
#endif

            // Ellipse
            int aaa = (int)(575 * Math.Sin(angle1 / 10));
            e.setLineThickness(33);
            e.Draw(Width / 2 - 111 - aaa, Height / 2 - 111 - aaa, 222 + 2*aaa, 222 + 2*aaa, false);

            // Rectangle
            r.SetAngle(angle1);
            r.Draw(Width / 4 - 222, Height / 4 - 222, 444, 444, true);

            // Line
            l.Draw(0, Height, Width, 0);

            // Triangle
            t.SetAngle((angle1+angle2)/6);
            t.SetColor(0.0f, 0.9f, 0.9f, 0.75f);
            t.Draw(Width/3, Height - 200, Width/2, 200, 2*Width/3, Height - 200, false);

            // Hexagon
            h.SetAngle(angle2);
            h.Draw(Width / 4, Height / 4, 100 + (int)(75 * Math.Sin(angle1/10)), true);

            angle1 += 0.125f;
            angle2 += 0.05f;
            continue;

            // Rectangle
            r.SetColor(1, 1, 0, 0.99f);
            r.Draw(Width / 2 + rand.Next(333), Height / 2 + rand.Next(333), 222, 222, false);

            // Line
            l.SetColor(0, 1, 0, 1);
            l.Draw(-1, -1, +1, +1);
            l.Draw(-1, +1, +1, -1);
            l.Draw(-0.5f, 1, -0.5f, -1);

            // Triangle
            t.SetColor(1.0f, 0.5f, 0.5f, 0.5f);
            t.Draw(0.0f, 1.0f, -1.0f, -1.0f, 1.0f, -1.0f, true);
            t.Draw(0.0f, 0.5f, -0.5f, -0.5f, 0.5f, -0.5f, false);

            // Hexagon
            h.SetColor(1, 1, 1, 0.50f);
            h.Draw(Width / 2, Height / 2, 100, true);

            h.SetColor(1, 0, 0, 0.85f);
            h.Draw(Width / 2, Height / 2, 100, false);
        }

        Glfw.Terminate();
    }

    // -------------------------------------------------------------------------------------------------------------------

    private static void SetRandomColor(int location)
    {
        var r = (float)rand.NextDouble();
        var g = (float)rand.NextDouble();
        var b = (float)rand.NextDouble();
        glUniform4f(location, r, g, b, 0.0f);
    }

    // -------------------------------------------------------------------------------------------------------------------

    /// <summary>
    /// Creates an extremely basic shader program that is capable of displaying a triangle on screen.
    /// </summary>
    /// <returns>The created shader program. No error checking is performed for this basic example.</returns>
    private static uint CreateProgram()
    {
        //var vertex = myOGL.CreateShaderEx(GL_VERTEX_SHADER, "layout (location = 0) in vec3 pos;", main: "gl_Position = vec4(pos.x, pos.y, pos.z, 1.0);");
        //var vertex = myOGL.CreateShaderEx(GL_VERTEX_SHADER, "layout (location = 0) in vec3 pos; out vec4 vertexColor; ", main: "gl_Position = vec4(pos, 1.0); vertexColor = vec4(0.5, 0.0, 0.0, 1.0);");

        var vertex = myOGL.CreateShaderEx(GL_VERTEX_SHADER, "layout (location = 0) in vec3 pos;", main: "gl_Position = vec4(pos, 1.0);");

        var fragment = myOGL.CreateShaderEx(GL_FRAGMENT_SHADER, "out vec4 result; uniform vec4 myColor;", main: "result = myColor;");

        var program = glCreateProgram();
        glAttachShader(program, vertex);
        glAttachShader(program, fragment);

        glLinkProgram(program);

        glDeleteShader(vertex);
        glDeleteShader(fragment);

        glUseProgram(program);
        return program;
    }

    // -------------------------------------------------------------------------------------------------------------------

    /// <summary>
    /// Creates a VBO and VAO to store the vertices for a triangle.
    /// </summary>
    /// <param name="vao">The created vertex array object for the triangle.</param>
    /// <param name="vbo">The created vertex buffer object for the triangle.</param>
    private static unsafe void CreateVertices(out uint vao, out uint vbo)
    {
        var vertices = new[] {
            0.0f, 1.0f, 0.0f,
            -1.0f, -1.0f, 0.0f,
            1.0f, -1.0f, 0.0f,
        };

        vao = glGenVertexArray();
        vbo = glGenBuffer();

        glBindVertexArray(vao);

        glBindBuffer(GL_ARRAY_BUFFER, vbo);
        fixed (float* v = &vertices[0])
        {
            glBufferData(GL_ARRAY_BUFFER, sizeof(float) * vertices.Length, v, GL_STATIC_DRAW);
        }

        glVertexAttribPointer(0, 3, GL_FLOAT, false, 3 * sizeof(float), NULL);
        glEnableVertexAttribArray(0);
    }

    // -------------------------------------------------------------------------------------------------------------------
}
