﻿using GLFW;
using static OpenGL.GL;
using System;


class Line
{
    private static uint vao = 0, vbo = 0, program = 0;
    private static float[] vertices = null;
    private static int locationColor = 0;
    private static int Width = 0, Height = 0;

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

            glBindVertexArray(vao);
            glBindBuffer(GL_ARRAY_BUFFER, vbo);

            CreateVertices();

            locationColor = glGetUniformLocation(program, "myColor");
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

        glLineWidth(lineWidth);

        glDrawArrays(GL_LINES, 0, 2);
    }

    public void Draw2(float x1, float y1, float x2, float y2, float lineWidth = 1.0f)
    {
        vertices[0] = x1;
        vertices[1] = y1;
        vertices[2] = x2;
        vertices[3] = y2;

        CreateVertices();

        glLineWidth(lineWidth);

        glDrawArrays(GL_LINES, 0, 2);
    }

    public void SetColor(float r, float g, float b, float a)
    {
        setColor(locationColor, r, g, b, a);
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

    private static void setColor(int location, float r, float g, float b, float a)
    {
        glUniform4f(location, r, g, b, a);
    }
};
