using OpenTK.Mathematics;
using System;
using System.Collections.Generic;
using System.Text;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
using HWEngine.Texture;
namespace HWEngine.Draw
{

    public enum BlendMode
    {
        Solid,Alpha,Additive
    }
    public class Drawer : IDisposable
    {

        private readonly int _vao;
        private readonly int _vbo;
        private readonly int _ebo;
        private readonly HWShader _shader;
        private readonly HWShader _imgShader;
        private Matrix4 _projection;


        private readonly uint[] _indices = { 0, 1, 2, 2, 3, 0 };
        private int _screenWidth;
        private int _screenHeight;
        public Drawer(int screenWidth,int screenHeight)
        {
            _screenWidth = screenWidth;
            _screenHeight = screenHeight;
            _projection = Matrix4.CreateOrthographicOffCenter(0, screenWidth, screenHeight, 0, -1f, 1f);

            _vao = GL.GenVertexArray();
            _vbo = GL.GenBuffer();
            _ebo = GL.GenBuffer();

            GL.BindVertexArray(_vao);

            // VBO for 4 vertices (x, y, u, v)
            GL.BindBuffer(BufferTarget.ArrayBuffer, _vbo);
            GL.BufferData(BufferTarget.ArrayBuffer, 4 * 4 * sizeof(float), IntPtr.Zero, BufferUsageHint.DynamicDraw);

            // EBO
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, _ebo);
            GL.BufferData(BufferTarget.ElementArrayBuffer, _indices.Length * sizeof(uint), _indices, BufferUsageHint.StaticDraw);

            // Vertex attribs: position (0) and texcoord (1)
            GL.EnableVertexAttribArray(0);
            GL.VertexAttribPointer(0, 2, VertexAttribPointerType.Float, false, 4 * sizeof(float), 0);

            GL.EnableVertexAttribArray(1);
            GL.VertexAttribPointer(1, 2, VertexAttribPointerType.Float, false, 4 * sizeof(float), 2 * sizeof(float));

            GL.BindVertexArray(0);

            _shader = new HWShader("Engine/Shader/Drawer2D/draw2DV.glsl", "Engine/Shader/Drawer2D/draw2DF.glsl");
            _imgShader = new HWShader("Engine/Shader/Drawer2D/drawImage2DV.glsl", "Engine/Shader/Drawer2D/drawImage2DF.glsl");
        }




        public void Image(Texture2D image,Vector2 position,Vector2 size,BlendMode blend,Vector4? color)
        {

            GL.Viewport(0, 0, _screenWidth, _screenHeight);
            float left = position.X;
            float right = left + size.X;
            float top = position.Y;
            float bottom = top + size.Y;

            // Texture coords: top-left (0,0), bottom-right (1,1)
            float[] vertices = {
            left,  top,    0f, 1f,
            right, top,    1f, 1f,
            right, bottom, 1f, 0f,
            left,  bottom, 0f, 0f
        };

            GL.BindBuffer(BufferTarget.ArrayBuffer, _vbo);
            GL.BufferSubData(BufferTarget.ArrayBuffer, IntPtr.Zero, vertices.Length * sizeof(float), vertices);

            _imgShader.Use();
            _imgShader.SetMatrix4("uProjection", _projection);
            _imgShader.SetVector4("uColor", color ?? new Vector4(1f, 1f, 1f, 1f));

            // Bind texture
            //      GL.ActiveTexture(TextureUnit.Texture0);
            //    GL.BindTexture(TextureTarget.Texture2D, image.Handle);
            image.Bind(0);
            _imgShader.SetInt("uTexture", 0);
            switch(blend)
            {
                case BlendMode.Solid:
                    GL.Disable(EnableCap.Blend);
                    break;
                case BlendMode.Alpha:
                    GL.Enable(EnableCap.Blend);
                    GL.BlendFunc(BlendingFactor.SrcAlpha, BlendingFactor.OneMinusSrcAlpha);
                    break;
                case BlendMode.Additive:
                    GL.Enable(EnableCap.Blend);
                    GL.BlendFunc(BlendingFactor.SrcAlpha, BlendingFactor.One);
                    break;
            }


            GL.BindVertexArray(_vao);
            GL.DrawElements(PrimitiveType.Triangles, _indices.Length, DrawElementsType.UnsignedInt, 0);
            GL.BindVertexArray(0);

        }

        public void Rect(Vector2 position, Vector2 size,BlendMode? blend, Vector4? color = null)
        {

            GL.Viewport(0,0, _screenWidth, _screenHeight);
            float left = position.X;
            float right = left + size.X;
            float top = position.Y;
            float bottom = top + size.Y;

            // Full VBO: x, y, u, v (u/v are 0 for simple color)
            float[] vertices = {
            left,  top,    0f, 0f,
            right, top,    0f, 0f,
            right, bottom, 0f, 0f,
            left,  bottom, 0f, 0f
        };


            switch (blend)
            {
                case BlendMode.Solid:
                    GL.Disable(EnableCap.Blend);
                    break;
                case BlendMode.Alpha:
                    GL.Enable(EnableCap.Blend);
                    GL.BlendFunc(BlendingFactor.SrcAlpha, BlendingFactor.OneMinusSrcAlpha);
                    break;
                case BlendMode.Additive:
                    GL.Enable(EnableCap.Blend);
                    GL.BlendFunc(BlendingFactor.SrcAlpha, BlendingFactor.One);
                    break;
            }

            GL.BindBuffer(BufferTarget.ArrayBuffer, _vbo);
            GL.BufferSubData(BufferTarget.ArrayBuffer, IntPtr.Zero, vertices.Length * sizeof(float), vertices);

            _shader.Use();
            _shader.SetMatrix4("uProjection", _projection);
            _shader.SetVector4("uColor", color ?? new Vector4(1f, 1f, 1f, 1f));

            GL.BindVertexArray(_vao);
            GL.DrawElements(PrimitiveType.Triangles, _indices.Length, DrawElementsType.UnsignedInt, 0);
            GL.BindVertexArray(0);
        }

        public void Dispose()
        {
            GL.DeleteBuffer(_vbo);
            GL.DeleteBuffer(_ebo);
            GL.DeleteVertexArray(_vao);
            _shader.Dispose();
        }


    }

}
