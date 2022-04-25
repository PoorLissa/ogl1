using GLFW;
using static OpenGL.GL;
using System;


public class myEllipse
{
    private static uint vao = 0, vbo = 0, ebo = 0, program = 0;
    private static uint[] indicesOutline = null;
    private static uint[] indicesFill = null;
    private static float[] vertices = null;
    private static int locationColor = 0;
    private static int Width = 0, Height = 0;

    public myEllipse(int width, int height)
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

            vertices = new float[18];

            indicesOutline = new uint[]
            {
                0, 1,
                1, 2,
                2, 3,
                3, 4,
                4, 5,
                5, 0
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

            // Ebo is used with glDrawElements (with the array of indices).
            // glDrawArrays does not ned this
            __glGenBuffers();
            glBindBuffer(GL_ELEMENT_ARRAY_BUFFER, ebo);

            // Define a simple triangle
            CreateVertices(false);

            locationColor = glGetUniformLocation(program, "myColor");
        }
    }

    public void Draw(int x, int y, int r)
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
                //glDrawArrays(GL_TRIANGLES, 0, 3);
                glDrawElements(GL_LINES, 18, GL_UNSIGNED_INT, NULL);
            }
        }

        // Recalc int coordinates into floats
        float fx = 2.0f * x / (Width + 1) - 1.0f;
        float fy = 1.0f - 2.0f * y / Height;

        float fr = 0.5f;
        float fry = (fr * Height) / Height;
        float frx = (fr * Height) / Width;
        //float half = (r * Height) / Width;

        vertices[0] = fx-frx;
        vertices[1] = fy;
        vertices[2] = 0;
        vertices[3] = fx-frx/2;
        vertices[4] = fy+fry;
        vertices[5] = 0;
        vertices[6] = fx+frx/2;
        vertices[7] = fy+fry;
        vertices[8] = 0;
        vertices[9] = fx+frx;
        vertices[10] = fy;
        vertices[11] = 0;
        vertices[12] = fx+frx/2;
        vertices[13] = fy-fry;
        vertices[14] = 0;
        vertices[15] = fx-frx/2;
        vertices[16] = fy-fry;
        vertices[17] = 0;

        CreateVertices(false);

        __draw(false);
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
