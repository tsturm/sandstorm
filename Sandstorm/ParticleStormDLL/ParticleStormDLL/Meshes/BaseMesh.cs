using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace ParticleStormDLL.Meshes
{
    public abstract class BaseMesh
    {
        public Matrix Transform { get; set; }
        public GraphicsDevice GraphicsDevice { get; set; }
        public VertexBuffer VertexBuffer { get; set; }
        public IndexBuffer IndexBuffer { get; set; }
        public int VertexCount { get; private set; }
        public int PrimitiveCount { get; private set; }

        protected abstract void GenerateVertexBuffer();
        protected abstract void GenerateIndexBuffer();

        public BaseMesh(GraphicsDevice graphicsDevice)
        {
            GraphicsDevice = graphicsDevice;
            Transform = Matrix.Identity;
            GenerateVertexBuffer();
            GenerateIndexBuffer();
            VertexCount = VertexBuffer.VertexCount;
            PrimitiveCount = IndexBuffer.IndexCount / 3;
        }
    }
}