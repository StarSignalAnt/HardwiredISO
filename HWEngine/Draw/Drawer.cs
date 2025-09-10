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
        Solid, Alpha, Additive
    }

    public class DrawCall
    {
        public Vector2 Position;
        public Vector2 Size;
        public Texture2D Texture;
        public Vector4 Color;
        public BlendMode BlendMode;
        public bool IsRect;
        public Vector2 TexCoordMin;
        public Vector2 TexCoordMax;

        public DrawCall()
        {
            // Default values
            Position = Vector2.Zero;
            Size = Vector2.One;
            Texture = null;
            Color = new Vector4(1f, 1f, 1f, 1f);
            BlendMode = BlendMode.Solid;
            IsRect = true;
            TexCoordMin = Vector2.Zero;
            TexCoordMax = Vector2.Zero;
        }
    }
    public class DrawList
    {
        private List<DrawCall> _calls = new List<DrawCall>();
        public void AddCall(DrawCall call) => _calls.Add(call);
        public void Clear() => _calls.Clear();
        public List<DrawCall> GetCalls()
        {
            // No sorting is performed, so the calls are returned in the order they were added.
            return _calls;
        }
    }

    public struct Vertex
    {
        public Vector2 Position;
        public Vector2 TexCoord;
        public Vector4 Color;

        public static int SizeInBytes()
        {
            return Vector2.SizeInBytes + Vector2.SizeInBytes + Vector4.SizeInBytes;
        }
    }


    public class Drawer : IDisposable
    {
        private readonly int _vao;
        private readonly int _vbo;
        private readonly int _ebo;
        private readonly HWShader _shader;
        private readonly HWShader _imgShader;
        private readonly Matrix4 _projection;
        private readonly DrawList _drawList = new DrawList();
        private readonly int _screenWidth;
        private readonly int _screenHeight;

        private const int MAX_QUADS = 10000;
        private const int QUAD_VERTEX_COUNT = 4;

        private readonly Vertex[] _vertices = new Vertex[MAX_QUADS * QUAD_VERTEX_COUNT];
        private int _quadCount = 0;

        public Drawer(int screenWidth, int screenHeight)
        {
            _screenWidth = screenWidth;
            _screenHeight = screenHeight;
            _projection = Matrix4.CreateOrthographicOffCenter(0, screenWidth, screenHeight, 0, -1f, 1f);

            _vao = GL.GenVertexArray();
            _vbo = GL.GenBuffer();
            _ebo = GL.GenBuffer();

            GL.BindVertexArray(_vao);

            GL.BindBuffer(BufferTarget.ArrayBuffer, _vbo);
            GL.BufferData(BufferTarget.ArrayBuffer, _vertices.Length * Vertex.SizeInBytes(), IntPtr.Zero, BufferUsageHint.DynamicDraw);

            uint[] indices = new uint[MAX_QUADS * 6];
            for (int i = 0; i < MAX_QUADS; i++)
            {
                indices[i * 6 + 0] = (uint)(i * 4 + 0);
                indices[i * 6 + 1] = (uint)(i * 4 + 1);
                indices[i * 6 + 2] = (uint)(i * 4 + 2);
                indices[i * 6 + 3] = (uint)(i * 4 + 2);
                indices[i * 6 + 4] = (uint)(i * 4 + 3);
                indices[i * 6 + 5] = (uint)(i * 4 + 0);
            }
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, _ebo);
            GL.BufferData(BufferTarget.ElementArrayBuffer, indices.Length * sizeof(uint), indices, BufferUsageHint.StaticDraw);

            GL.EnableVertexAttribArray(0);
            GL.VertexAttribPointer(0, 2, VertexAttribPointerType.Float, false, Vertex.SizeInBytes(), 0);

            GL.EnableVertexAttribArray(1);
            GL.VertexAttribPointer(1, 2, VertexAttribPointerType.Float, false, Vertex.SizeInBytes(), Vector2.SizeInBytes);

            GL.EnableVertexAttribArray(2);
            GL.VertexAttribPointer(2, 4, VertexAttribPointerType.Float, false, Vertex.SizeInBytes(), Vector2.SizeInBytes * 2);

            GL.BindVertexArray(0);

            _shader = new HWShader("Engine/Shader/Drawer2D/draw2DV.glsl", "Engine/Shader/Drawer2D/draw2DF.glsl");
            _imgShader = new HWShader("Engine/Shader/Drawer2D/drawImage2DV.glsl", "Engine/Shader/Drawer2D/drawImage2DF.glsl");
        }

        public void Image(Texture2D image, Vector2 position, Vector2 size, BlendMode blend, Vector4? color = null)
        {
            var call = new DrawCall
            {
                Position = position,
                Size = size,
                Texture = image,
                Color = color ?? new Vector4(1f, 1f, 1f, 1f),
                BlendMode = blend,
                IsRect = false,
                TexCoordMin = new Vector2(0, 0),
                TexCoordMax = new Vector2(1, 1)
            };
            _drawList.AddCall(call);
        }

        public void Rect(Vector2 position, Vector2 size, BlendMode? blend, Vector4? color = null)
        {
            var call = new DrawCall
            {
                Position = position,
                Size = size,
                Color = color ?? new Vector4(1f, 1f, 1f, 1f),
                BlendMode = blend ?? BlendMode.Solid,
                IsRect = true,
            };
            _drawList.AddCall(call);
        }

        public void Flush()
        {
            GL.Viewport(0, 0, _screenWidth, _screenHeight);
            var calls = _drawList.GetCalls();
            Texture2D currentTexture = null;
            BlendMode currentBlendMode = BlendMode.Solid;

            for (int i = 0; i < calls.Count; i++)
            {
                var call = calls[i];

                if (call.Texture != currentTexture || call.BlendMode != currentBlendMode || _quadCount >= MAX_QUADS)
                {
                    if (_quadCount > 0)
                    {
                        RenderBatch(currentTexture, currentBlendMode);
                    }
                    currentTexture = call.Texture;
                    currentBlendMode = call.BlendMode;
                }

                if (_quadCount >= MAX_QUADS)
                {
                    Console.WriteLine("Warning: Max quads exceeded. Some draw calls were dropped.");
                    break;
                }

                AddQuadToBatch(call);
            }

            if (_quadCount > 0)
            {
                RenderBatch(currentTexture, currentBlendMode);
            }

            _drawList.Clear();
        }

        private void AddQuadToBatch(DrawCall call)
        {
            Vector2[] positions = new Vector2[]
            {
                new Vector2(call.Position.X, call.Position.Y),
                new Vector2(call.Position.X + call.Size.X, call.Position.Y),
                new Vector2(call.Position.X + call.Size.X, call.Position.Y + call.Size.Y),
                new Vector2(call.Position.X, call.Position.Y + call.Size.Y)
            };

            Vector2[] texCoords = call.IsRect ?
            new Vector2[]
            {
                Vector2.Zero, new Vector2(1, 0), new Vector2(1, 1), new Vector2(0, 1)
            }
            :
            new Vector2[]
            {
                new Vector2(0, 0), new Vector2(1, 0), new Vector2(1, 1), new Vector2(0, 1)
            };

            for (int i = 0; i < QUAD_VERTEX_COUNT; i++)
            {
                _vertices[_quadCount * QUAD_VERTEX_COUNT + i] = new Vertex
                {
                    Position = positions[i],
                    TexCoord = texCoords[i],
                    Color = call.Color
                };
            }

            _quadCount++;
        }

        private void RenderBatch(Texture2D texture, BlendMode blendMode)
        {
            HWShader shaderToUse = texture == null ? _shader : _imgShader;
            shaderToUse.Use();
            shaderToUse.SetMatrix4("uProjection", _projection);

            switch (blendMode)
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

            if (texture != null)
            {
                texture.Bind(0);
                shaderToUse.SetInt("uTexture", 0);
            }

            GL.BindBuffer(BufferTarget.ArrayBuffer, _vbo);
            GL.BufferSubData(BufferTarget.ArrayBuffer, IntPtr.Zero, _quadCount * QUAD_VERTEX_COUNT * Vertex.SizeInBytes(), _vertices);

            GL.BindVertexArray(_vao);
            GL.DrawElements(PrimitiveType.Triangles, _quadCount * 6, DrawElementsType.UnsignedInt, 0);

            _quadCount = 0;
            GL.BindVertexArray(0);
        }

        public void Dispose()
        {
            GL.DeleteBuffer(_vbo);
            GL.DeleteBuffer(_ebo);
            GL.DeleteVertexArray(_vao);
            _shader.Dispose();
            _imgShader.Dispose();
        }
    }
}