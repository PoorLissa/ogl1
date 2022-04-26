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

        float fx, fy, rad = w / 2;

        // Recalc int coordinates into floats
        fx = 2.0f * x / (Width) - 1.0f;
        fy = 1.0f - 2.0f * y / Height;
        vertices[06] = fx;
        vertices[09] = fx;
        vertices[01] = fy;
        vertices[10] = fy;

        // X: -0.1156 ... +0.1156

        fx = 2.0f * (x + w) / Width - 1.0f;
        vertices[0] = fx;
        vertices[3] = fx;

        fy = 1.0f - 2.0f * (y + h) / Height;
        vertices[4] = fy;
        vertices[7] = fy;
        // ------------------ clean it later *********************************

        {
            // Leave coordinates as they are, and recalc them in the shader
            fx = x;
            fy = y;
            vertices[06] = fx;
            vertices[09] = fx;
            vertices[01] = fy;
            vertices[10] = fy;

            fx = x + w;
            vertices[0] = fx;
            vertices[3] = fx;

            fy = y + h;
            vertices[4] = fy;
            vertices[7] = fy;
        }

        CreateVertices();

        glUseProgram(program);
        setColor(locationColor, _r, _g, _b, _a);

        glUniform2f(locationCenter, x + w/2, y + h/2);
        updUniformScreenSize(locationScrSize, Width, Height);

        int RadSq = glGetUniformLocation(program, "RadSq");

        float zxc1 = (float)w / (float)Width;
        float zxc2 = (float)h / (float)Height;

        glUniform2f(RadSq, zxc1 * zxc1, zxc2 * zxc2);

        __draw();
    }

    private static void CreateProgram()
    {
        var vertex = myOGL.CreateShaderEx(GL_VERTEX_SHADER, "layout (location = 0) in vec3 pos; out vec4 zzz; uniform vec2 myCenter; uniform ivec2 myScrSize;",
            main: @"gl_Position = vec4(pos, 1.0);

                    gl_Position.x = 2.0f * gl_Position.x / (myScrSize.x+1) - 1.0f;
                    gl_Position.y = 1.0f - 2.0f * gl_Position.y / myScrSize.y;

                    zzz = vec4(gl_Position.x,
                               myScrSize.y * gl_Position.y / myScrSize.x,
                               2.0f * myCenter.x / (myScrSize.x+1) - 1.0f,
                               1.0f - 2.0f * myCenter.y / myScrSize.y);"
        );

        // still works wrong when moved along y axis
        var fragment = myOGL.CreateShaderEx(GL_FRAGMENT_SHADER, "in vec4 zzz; out vec4 result; uniform vec4 myColor; uniform vec2 RadSq;",
                main: @"

                        float dist = (zzz.x - zzz.z) * (zzz.x - zzz.z) + (zzz.y - zzz.w) * (zzz.y - zzz.w);

                        if (dist < RadSq.x)
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
