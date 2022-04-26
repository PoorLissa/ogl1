using GLFW;
using static OpenGL.GL;
using System;


public class myEllipse : myPrimitive
{
    private static uint vao = 0, vbo = 0, ebo = 0, program = 0;
    private static uint[] indicesFill = null;
    private static float[] vertices = null;
    private static int locationColor = 0, locationCenter = 0, locationScrSize = 0;

    public myEllipse()
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
            vertices = new float[12];

            indicesFill = new uint[]
            {
                0, 1, 3,   // first triangle
                1, 2, 3    // second triangle
            };

            vao = glGenVertexArray();
            vbo = glGenBuffer();

            CreateProgram();
            locationColor   = glGetUniformLocation(program, "myColor");
            locationCenter  = glGetUniformLocation(program, "myCenter");
            locationScrSize = glGetUniformLocation(program, "myScrSize");

            glBindVertexArray(vao);
            glBindBuffer(GL_ARRAY_BUFFER, vbo);

            __glGenBuffers();
            glBindBuffer(GL_ELEMENT_ARRAY_BUFFER, ebo);
        }
    }

    public void Draw(int x, int y, int w, int h, bool doFill = false)
    {
        static unsafe void __draw()
        {
            glPolygonMode(GL_FRONT_AND_BACK, GL_FILL);
            glDrawElements(GL_TRIANGLES, 6, GL_UNSIGNED_INT, NULL);
        }

        float fx, fy;

        // Recalc int coordinates into floats
        fx = 2.0f * x / (Width) - 1.0f;
        fy = 1.0f - 2.0f * y / Height;
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

        CreateVertices();

        glUseProgram(program);
        setColor(locationColor, _r, _g, _b, _a);

        glUniform2f(locationCenter, x + w/2, y + h/2);
        updUniformScreenSize(locationScrSize, Width, Height);

        int lalala = glGetUniformLocation(program, "lalala");

        float zxc1 = 2.0f * (w/2) / Width;

        float zxc3 = (float)(Math.Sqrt(zxc1));

        //glUniform2f(lalala, 0.0125f, h/2);
        glUniform2f(lalala, zxc1, h/2);

        __draw();
    }

    private static void CreateProgram()
    {
        var vertex = myOGL.CreateShaderEx(GL_VERTEX_SHADER, "layout (location = 0) in vec3 pos; out vec2 zzz; uniform vec2 myCenter; uniform ivec2 myScrSize;",
            main: @"zzz = vec2(pos.x, myScrSize.y * pos.y / myScrSize.x);
                    gl_Position = vec4(pos, 1.0);"
        );

        var fragment = myOGL.CreateShaderEx(GL_FRAGMENT_SHADER, "in vec2 zzz; out vec4 result; uniform vec4 myColor; uniform vec2 lalala;",
                main: @"
                        float dist = zzz.x * zzz.x + zzz.y * zzz.y;
                        if(dist < 0.0125)
                        {
                            result = myColor;
                        }
                        else
                        {
                            result = vec4(0, 0, 0, 0);
                        }"
        );

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
            glBufferData(GL_ARRAY_BUFFER, sizeof(float) * vertices.Length, v, GL_DYNAMIC_DRAW);
        }

        fixed (uint* i = &indicesFill[0])
        {
            glBufferData(GL_ELEMENT_ARRAY_BUFFER, sizeof(uint) * indicesFill.Length, i, GL_DYNAMIC_DRAW);
        }

        glVertexAttribPointer(0, 3, GL_FLOAT, false, 3 * sizeof(float), NULL);
        glEnableVertexAttribArray(0);
    }
};
