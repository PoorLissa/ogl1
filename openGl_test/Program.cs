﻿using GLFW;
using static OpenGL.GL;
using System;


// Original source  : https://www.youtube.com/watch?v=LcHCygwIgLo
// Hello Triangle   : https://learnopengl.com/Getting-started/Hello-Triangle
// Glfw manual      : https://www.glfw.org/docs/3.3/index.html

class pt
{
    public float x, y;
    private float dx, dy;

    public pt()
    {
        x = 0;
        y = 0;

        var r = new Random((int)DateTime.Now.Ticks);

        dx = (float)((r.NextDouble() * 2) - 1.0f) / 100;
        dy = (float)((r.NextDouble() * 2) - 1.0f) / 100;
    }

    public void Move()
    {
        x += dx;
        y += dy;

        if (x <= -1 || x >= 1)
            dx *= -1;

        if (y <= -1 || y >= 1)
            dy *= -1;
    }
};

class myObj
{
    float r = 0, g = 0, b = 0, a = 0;

    pt p1 = new pt();
    pt p2 = new pt();
    pt p3 = new pt();

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
        a = (float)rand.NextDouble()/10;
    }

    public void Draw(Triangle t)
    {
        t.SetColor(r, g, b, a);
        t.Draw(p1.x, p1.y, p2.x, p2.y, p3.x, p3.y);
    }
}



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
        rand = new Random();

        // Set context creation hints
        myOGL.PrepareContext();

        // Create a window and shader program
        //var window = myOGL.CreateWindow(1920, 1200, "my Window", trueFullScreen: false);

        var window = myOGL.CreateWindow(0, 0, "my Window", trueFullScreen: false);

        Triangle t = new Triangle();
        t.SetColor(1, 1, 1, 1);
        long n = 0;

        var list = new System.Collections.Generic.List<myObj>();

        for (int i = 0; i < 111; i++)
        {
            var obj = new myObj();
            obj.changeColor(rand);

            list.Add(obj);
        }

        glEnable(GL_BLEND); //Enable blending.
        glBlendFunc(GL_SRC_ALPHA, GL_ONE_MINUS_SRC_ALPHA); //Set blending function.
        glClearColor(0.2f, 0.3f, 0.3f, 1.0f);

        while (!Glfw.WindowShouldClose(window))
        {
            processInput(window);

            // Swap fore/back framebuffers, and poll for operating system events.
            Glfw.SwapBuffers(window);
            Glfw.PollEvents();

            // Clear the framebuffer to defined background color
            glClear(GL_COLOR_BUFFER_BIT);

            if (n++ % 500 == 0)
            {
                /*
                foreach (var item in list)
                    item.changeColor(rand);*/
            }

            foreach (var item in list)
            {
                item.Draw(t);
                item.Move();
            }
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
