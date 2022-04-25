using GLFW;
using static OpenGL.GL;
using System;


class Line
{
    private static uint vao = 0, vbo = 0, program = 0;
    private static float[] vertices = null;
    private static int locationColor = 0;
    private static int Width = 0, Height = 0;
    private static float _r, _g, _b, _a;

    public Line(int width, int height)
    {
        if (vertices == null)
        {
            Width = width;
            Height = height;

            vertices = new float[4];

            vao = glGenVertexArray();
            vbo = glGenBuffer();

            CreateProgram();
            locationColor = glGetUniformLocation(program, "myColor");

            glBindVertexArray(vao);
            glBindBuffer(GL_ARRAY_BUFFER, vbo);
        }
    }

    public void Draw(float x1, float y1, float x2, float y2, float lineWidth = 1.0f)
    {
        // Recalc int coordinates into floats
        float fx = 2.0f * x1 / (Width) - 1.0f;
        float fy = 1.0f - 2.0f * y1 / Height;

        vertices[0] = fx;
        vertices[1] = fy;

        fx = 2.0f * x2 / (Width) - 1.0f;
        fy = 1.0f - 2.0f * y2 / Height;
        vertices[2] = fx;
        vertices[3] = fy;

        CreateVertices();

        glUseProgram(program);
        setColor(locationColor, _r, _g, _b, _a);

        glLineWidth(lineWidth);

        glDrawArrays(GL_LINES, 0, 2);
    }

    // Just remember the color value
    public void SetColor(float r, float g, float b, float a)
    {
        _r = r;
        _g = g;
        _b = b;
        _a = a;
    }

    private static void setColor(int location, float r, float g, float b, float a)
    {
        glUniform4f(location, r, g, b, a);
    }

    private static void CreateProgram()
    {
        var vertex = myOGL.CreateShaderEx(GL_VERTEX_SHADER, "layout (location = 0) in vec2 pos;", main: "gl_Position = vec4(pos.x, pos.y, 0.0, 1.0);");
        var fragment = myOGL.CreateShaderEx(GL_FRAGMENT_SHADER, "out vec4 result; uniform vec4 myColor;", main: "result = myColor;");

        program = glCreateProgram();
        glAttachShader(program, vertex);
        glAttachShader(program, fragment);

        glLinkProgram(program);

        glDeleteShader(vertex);
        glDeleteShader(fragment);

        glUseProgram(program);
    }

    private static unsafe void CreateVertices()
    {
        fixed (float* v = &vertices[0])
        {
            glBufferData(GL_ARRAY_BUFFER, sizeof(float) * vertices.Length, v, GL_STATIC_DRAW);
        }

        glVertexAttribPointer(0, 2, GL_FLOAT, false, 2 * sizeof(float), NULL);
        glEnableVertexAttribArray(0);
    }
};
