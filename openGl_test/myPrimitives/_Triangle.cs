using GLFW;
using static OpenGL.GL;
using System;


public class Triangle
{
    private static uint vao = 0, vbo = 0, program = 0;
    private static float[] vertices = null;
    private static int locationColor = 0;

    public Triangle()
    {
        if (vertices == null)
        {
            vertices = new float[9];

            vao = glGenVertexArray();
            vbo = glGenBuffer();

            CreateProgram();

            glBindVertexArray(vao);
            glBindBuffer(GL_ARRAY_BUFFER, vbo);

            locationColor = glGetUniformLocation(program, "myColor");
        }
    }

    public void Draw(float x1, float y1, float x2, float y2, float x3, float y3, bool doFill = false)
    {
        vertices[0] = x1;
        vertices[1] = y1;
        vertices[3] = x2;
        vertices[4] = y2;
        vertices[6] = x3;
        vertices[7] = y3;

        CreateVertices();

        // Draw only outline or fill the whole polygon with color
        glPolygonMode(GL_FRONT_AND_BACK, doFill ? GL_FILL : GL_LINE);

        glDrawArrays(GL_TRIANGLES, 0, 3);
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

    private static unsafe void CreateVertices()
    {
        fixed (float* v = &vertices[0])
        {
            // todo: see which one is better or maybe we need a choise here
            //glBufferData(GL_ARRAY_BUFFER, sizeof(float) * vertices.Length, v, GL_STATIC_DRAW);
            glBufferData(GL_ARRAY_BUFFER, sizeof(float) * vertices.Length, v, GL_DYNAMIC_DRAW);
        }

        glVertexAttribPointer(0, 3, GL_FLOAT, false, 3 * sizeof(float), NULL);
        glEnableVertexAttribArray(0);
    }

    private static void setColor(int location, float r, float g, float b, float a)
    {
        glUniform4f(location, r, g, b, a);
    }
};
