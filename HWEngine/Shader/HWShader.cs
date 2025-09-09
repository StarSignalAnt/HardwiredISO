using System;
using System.IO;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;

public class HWShader : IDisposable
{
    public int Handle { get; private set; }

    public HWShader(string vertexPath, string fragmentPath)
    {
        // Load shader source code
        string vertexSource = File.ReadAllText(vertexPath);
        string fragmentSource = File.ReadAllText(fragmentPath);

        // Create shaders
        int vertexShader = GL.CreateShader(ShaderType.VertexShader);
        int fragmentShader = GL.CreateShader(ShaderType.FragmentShader);

        // Compile vertex shader
        GL.ShaderSource(vertexShader, vertexSource);
        GL.CompileShader(vertexShader);
        CheckShaderCompile(vertexShader, "VERTEX");

        // Compile fragment shader
        GL.ShaderSource(fragmentShader, fragmentSource);
        GL.CompileShader(fragmentShader);
        CheckShaderCompile(fragmentShader, "FRAGMENT");

        // Create shader program
        Handle = GL.CreateProgram();
        GL.AttachShader(Handle, vertexShader);
        GL.AttachShader(Handle, fragmentShader);

        // Link program
        GL.LinkProgram(Handle);
        CheckProgramLink(Handle);

        // Shaders no longer needed after linking
        GL.DetachShader(Handle, vertexShader);
        GL.DetachShader(Handle, fragmentShader);
        GL.DeleteShader(vertexShader);
        GL.DeleteShader(fragmentShader);
    }

    public void Use() => GL.UseProgram(Handle);

    public virtual void Bind() => Use();

    private void CheckShaderCompile(int shader, string type)
    {
        GL.GetShader(shader, ShaderParameter.CompileStatus, out int success);
        if (success == 0)
        {
            string infoLog = GL.GetShaderInfoLog(shader);
            Console.WriteLine($"ERROR::SHADER_COMPILATION_ERROR of type: {type}\n{infoLog}\n");
        }
    }

    private void CheckProgramLink(int program)
    {
        GL.GetProgram(program, GetProgramParameterName.LinkStatus, out int success);
        if (success == 0)
        {
            string infoLog = GL.GetProgramInfoLog(program);
            Console.WriteLine($"ERROR::PROGRAM_LINKING_ERROR:\n{infoLog}\n");
        }
    }

    // ---------------- Uniform setters ----------------

    public void SetInt(string name, int value)
    {
        int location = GL.GetUniformLocation(Handle, name);
        if (location != -1) GL.Uniform1(location, value);
    }

    public void SetFloat(string name, float value)
    {
        int location = GL.GetUniformLocation(Handle, name);
        if (location != -1) GL.Uniform1(location, value);
    }

    public void SetVector2(string name, Vector2 value)
    {
        int location = GL.GetUniformLocation(Handle, name);
        if (location != -1) GL.Uniform2(location, value);
    }

    public void SetVector3(string name, Vector3 value)
    {
        int location = GL.GetUniformLocation(Handle, name);
        if (location != -1) GL.Uniform3(location, value);
    }

    public void SetVector4(string name, Vector4 value)
    {
        int location = GL.GetUniformLocation(Handle, name);
        if (location != -1) GL.Uniform4(location, value);
    }

    public void SetMatrix4(string name, Matrix4 value, bool transpose = false)
    {
        int location = GL.GetUniformLocation(Handle, name);
        if (location != -1) GL.UniformMatrix4(location, transpose, ref value);
    }

    public void Dispose()
    {
        GL.DeleteProgram(Handle);
    }
}
