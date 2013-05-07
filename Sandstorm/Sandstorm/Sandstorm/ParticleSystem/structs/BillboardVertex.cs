using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;

namespace Sandstorm.ParticleSystem.structs
{
    public struct BillboardVertex : IVertexType
    {
        public static readonly VertexDeclaration VertexDeclaration = new VertexDeclaration
        (
            new VertexElement(0, VertexElementFormat.Vector4, VertexElementUsage.Position, 0),
            new VertexElement(sizeof(float) * 4, VertexElementFormat.Vector4, VertexElementUsage.TextureCoordinate, 0)
        );

        public Vector4 Position;
        public Vector4 TexCoord;

        public BillboardVertex(Vector3 position, Vector2 texCoord, Vector2 offset)
        {
            Position = new Vector4(position, 0.0f);

            // Coordinates for the texture map.
            TexCoord.X = texCoord.X;
            TexCoord.Y = texCoord.Y;

            // The 2D offset vector.
            TexCoord.Z = offset.X;
            TexCoord.W = offset.Y;

        }

        public BillboardVertex(Vector3 position, float animationWeight, Vector2 texCoord, Vector2 offset)
        {
            Position = new Vector4(position, animationWeight);

            // Coordinates for the texture map.
            TexCoord.X = texCoord.X;
            TexCoord.Y = texCoord.Y;

            // The 2D offset vector.
            TexCoord.Z = offset.X;
            TexCoord.W = offset.Y;
        }

        VertexDeclaration IVertexType.VertexDeclaration
        {
            get { return VertexDeclaration; }
        }
    }
}
