using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace ParticleStormDLL.VertexDeclarations
{
    public struct VertexInstance : IVertexType
    {
        private Vector2 instanceIndex;
        public Vector2 InstanceIndex
        {
            get { return instanceIndex; }
            set { instanceIndex = value; }
        }

        public readonly static VertexDeclaration VertexDeclaration = new VertexDeclaration
        (
            new VertexElement(0, VertexElementFormat.Vector2, VertexElementUsage.BlendWeight, 0)
        );

        public VertexInstance(Vector2 index)
        {
            instanceIndex = index;
        }

        public VertexInstance(int index, int size)
        {
            float u = (float)(index % size) / (float)(size - 1);
            float v = (int)(index / size) / (float)(size - 1);
            instanceIndex = new Vector2(u, v);
        }

        VertexDeclaration IVertexType.VertexDeclaration
        {
            get { return VertexDeclaration; }
        }
    }
}
