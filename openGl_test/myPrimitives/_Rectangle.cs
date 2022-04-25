using GLFW;
using static OpenGL.GL;
using System;


public class myRectangle
{
    private static uint vao = 0, vbo = 0, ebo = 0, program = 0;
    private static uint[] indicesOutline = null;
    private static uint[] indicesFill = null;
    private static float[] vertices = null;
    private static int locationColor = 0;
    private static int Width = 0, Height = 0;

    public myRectangle(int width, int height)
    {
        static unsafe void __glGenBuffers()
        {
            fixed (uint* e = &ebo)
            {
                glGenBuffers(1, e);
            }
        }

        if (vertices == null)
        {
            Width = width;
            Height = height;

            vertices = new float[12];

            indicesOutline = new uint[]
            {
                0, 1,
                1, 2,
                2, 3,
                3, 0
            };

            indicesFill = new uint[]
            {
                0, 1, 3,   // first triangle
                1, 2, 3    // second triangle
            };

            vao = glGenVertexArray();
            vbo = glGenBuffer();

            CreateProgram();

            glBindVertexArray(vao);
            glBindBuffer(GL_ARRAY_BUFFER, vbo);

            __glGenBuffers();
            glBindBuffer(GL_ELEMENT_ARRAY_BUFFER, ebo);

            CreateVertices(false);

            locationColor = glGetUniformLocation(program, "myColor");
        }
    }

    public void Draw(int x, int y, int w, int h, bool doFill = false)
    {
        static unsafe void __draw(bool doFill)
        {
            if (doFill)
            {
                glPolygonMode(GL_FRONT_AND_BACK, GL_FILL);
                glDrawElements(GL_TRIANGLES, 6, GL_UNSIGNED_INT, NULL);
            }
            else
            {
                glDrawElements(GL_LINES, 8, GL_UNSIGNED_INT, NULL);
            }
        }

        // Recalc int coordinates into floats
        // Shifting Width a bit to eliminate incomplete left bottom angle
        float fx = 2.0f * x / (Width + 1) - 1.0f;
        float fy = 1.0f - 2.0f * y / Height;
        vertices[06] = fx;
        vertices[09] = fx;
        vertices[01] = fy;
        vertices[10] = fy;

        fx = 2.0f * (x + w) / Width - 1.0f;
        vertices[0] = fx;
        vertices[3] = fx;

        fy = 1.0f - 2.0f * (y + h) / Height;
        vertices[4] = fy;
        vertices[7] = fy;

        CreateVertices(doFill);

        __draw(doFill);
    }

    public void SetColor(float r, float g, float b, float a)
    {
        setColor(locationColor, r, g, b, a);
    }

    private static void CreateProgram()
    {
        var vertex = myOGL.CreateShaderEx(GL_VERTEX_SHADER, "layout (location = 0) in vec3 pos;", main: "gl_Position = vec4(pos, 1.0);");
        var fragment = myOGL.CreateShaderEx(GL_FRAGMENT_SHADER, "out vec4 result; uniform vec4 myColor;", main: "result = myColor;");

        program = glCreateProgram();
        glAttachShader(program, vertex);
        glAttachShader(program, fragment);

        glLinkProgram(program);

        glDeleteShader(vertex);
        glDeleteShader(fragment);

        glUseProgram(program);
    }

    private static unsafe void CreateVertices(bool doFill)
    {
        fixed (float* v = &vertices[0])
        {
            glBufferData(GL_ARRAY_BUFFER, sizeof(float) * vertices.Length, v, GL_DYNAMIC_DRAW);
        }

        if (doFill)
        {
            fixed (uint* i = &indicesFill[0])
            {
                glBufferData(GL_ELEMENT_ARRAY_BUFFER, sizeof(uint) * indicesFill.Length, i, GL_DYNAMIC_DRAW);
            }
        }
        else
        {
            fixed (uint* i = &indicesOutline[0])
            {
                glBufferData(GL_ELEMENT_ARRAY_BUFFER, sizeof(uint) * indicesOutline.Length, i, GL_DYNAMIC_DRAW);
            }
        }

        glVertexAttribPointer(0, 3, GL_FLOAT, false, 3 * sizeof(float), NULL);
        glEnableVertexAttribArray(0);
    }

    private static void setColor(int location, float r, float g, float b, float a)
    {
        glUniform4f(location, r, g, b, a);
    }
};
