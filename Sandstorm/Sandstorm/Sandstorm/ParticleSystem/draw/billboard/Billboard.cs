//-----------------------------------------------------------------------------
// Copyright (c) 2011 dhpoware. All Rights Reserved.
//
// Permission is hereby granted, free of charge, to any person obtaining a
// copy of this software and associated documentation files (the "Software"),
// to deal in the Software without restriction, including without limitation
// the rights to use, copy, modify, merge, publish, distribute, sublicense,
// and/or sell copies of the Software, and to permit persons to whom the
// Software is furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in
// all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS
// OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING
// FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS
// IN THE SOFTWARE.
//-----------------------------------------------------------------------------

using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using Sandstorm.ParticleSystem;
using Sandstorm.ParticleSystem.structs;

namespace Dhpoware
{
    /// <summary>
    /// Custom vertex structure used for billboarding.
    /// </summary>
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

    /// <summary>
    /// A class to manage the resources required to implement billboarding
    /// using shaders. Each billboard consists of 4 vertices and 6 indices.
    /// The billboard's quad is rendered as an indexed triangle list consisting
    /// of 2 triangles. All of the vertices for the billboard's triangles are
    /// positioned at the center of the billboard - as specified in the
    /// Billboard class' constructor. The Billboard's texture coordinate
    /// vertex attribute is a 4 component floating point vector. The first 2
    /// components is the 2D texture coordinates of the color map. The other
    /// 2 components are the x and y offsets from the center of the billboard
    /// that the vertex shader will use to transform each vertex to its
    /// correct position.
    /// </summary>
    public class Billboard
    {
        private VertexBuffer vertexBuffer;
        private IndexBuffer indexBuffer;   
                
        public Billboard(GraphicsDevice graphicsDevice,
                         Particle[] positions,
                         float upperLeftVertexAnimationWeight,
                         float upperRightVertexAnimationWeight,
                         float lowerLeftVertexAnimationWeight,
                         float lowerRightVertexAnimationWeight)
        {
            Create(graphicsDevice,
                   positions,
                   upperLeftVertexAnimationWeight,
                   upperRightVertexAnimationWeight,
                   lowerLeftVertexAnimationWeight,
                   lowerRightVertexAnimationWeight);
        }

        public void Create(GraphicsDevice graphicsDevice,
                           Particle[] positions,
                           float upperLeftVertexAnimationWeight,
                           float upperRightVertexAnimationWeight,
                           float lowerLeftVertexAnimationWeight,
                           float lowerRightVertexAnimationWeight)
        {
            // The billboard quad is made of 2 triangles and is centered about
            // position P.
            //
            //  0---1
            //  |  /|
            //  | P |
            //  |/  |
            //  2---3
            //
            // 1st triangle: 0, 1, 2
            // 2nd triangle: 2, 1, 3
            //
            // The 4 vertices of the billboard are setup to be located at
            // position P.
            //
            // Each vertex is assigned unit offsets from the billboard's center
            // position (i.e., vertex 0's offset is (1, -1).
            //
            // The billboard vertex shader will transform each billboard's
            // vertex to the correct position in world space.
            //
            // Each billboard quad consists of 4 vertices and 6 indices.

            if (positions.Length * 4 > short.MaxValue)
            {
                CreateWith32BitIndices(graphicsDevice,
                                       positions,
                                       upperLeftVertexAnimationWeight,
                                       upperRightVertexAnimationWeight,
                                       lowerLeftVertexAnimationWeight,
                                       lowerRightVertexAnimationWeight);
            }
            else
            {
                CreateWith16BitIndices(graphicsDevice,
                                       positions,
                                       upperLeftVertexAnimationWeight,
                                       upperRightVertexAnimationWeight,
                                       lowerLeftVertexAnimationWeight,
                                       lowerRightVertexAnimationWeight);
            }
        }

        public void Draw(GraphicsDevice graphicsDevice, Effect effect)
        {
            if (graphicsDevice != null && effect != null)
            {
                graphicsDevice.SetVertexBuffer(vertexBuffer);
                graphicsDevice.Indices = indexBuffer;

                foreach (EffectPass pass in effect.CurrentTechnique.Passes)
                {
                    pass.Apply();
                    graphicsDevice.DrawIndexedPrimitives(PrimitiveType.TriangleList,
                                                         0,
                                                         0,
                                                         vertexBuffer.VertexCount,
                                                         0,
                                                         (int)(vertexBuffer.VertexCount * 0.5f));
                }

                graphicsDevice.Indices = null;
                graphicsDevice.SetVertexBuffer(null);
            }
        }

        private void CreateWith16BitIndices(GraphicsDevice graphicsDevice,
                                            Particle[] positions,
                                            float upperLeftVertexAnimationWeight,
                                            float upperRightVertexAnimationWeight,
                                            float lowerLeftVertexAnimationWeight,
                                            float lowerRightVertexAnimationWeight)
        {
            List<BillboardVertex> vertices = new List<BillboardVertex>();
            List<short> indices = new List<short>();
            short baseIndex = (short)0;

            for (int i = 0; i < positions.Length; ++i)
            {
                Vector3 position = positions[i].getPosition();
                BillboardVertex vertex = new BillboardVertex();

                vertex.Position = new Vector4(position, upperLeftVertexAnimationWeight);
                vertex.TexCoord = new Vector4(0.0f, 0.0f, -1.0f, 1.0f);
                vertices.Add(vertex);

                vertex.Position = new Vector4(position, upperRightVertexAnimationWeight);
                vertex.TexCoord = new Vector4(1.0f, 0.0f, 1.0f, 1.0f);
                vertices.Add(vertex);

                vertex.Position = new Vector4(position, lowerLeftVertexAnimationWeight);
                vertex.TexCoord = new Vector4(0.0f, 1.0f, -1.0f, -1.0f);
                vertices.Add(vertex);

                vertex.Position = new Vector4(position, lowerRightVertexAnimationWeight);
                vertex.TexCoord = new Vector4(1.0f, 1.0f, 1.0f, -1.0f);
                vertices.Add(vertex);

                baseIndex = (short)(i * 4);

                indices.Add((short)(0 + baseIndex));
                indices.Add((short)(1 + baseIndex));
                indices.Add((short)(2 + baseIndex));
                indices.Add((short)(2 + baseIndex));
                indices.Add((short)(1 + baseIndex));
                indices.Add((short)(3 + baseIndex));
            }

            vertexBuffer = new VertexBuffer(graphicsDevice, typeof(BillboardVertex), vertices.Count, BufferUsage.WriteOnly);
            vertexBuffer.SetData(vertices.ToArray());

            indexBuffer = new IndexBuffer(graphicsDevice, IndexElementSize.SixteenBits, indices.Count, BufferUsage.WriteOnly);
            indexBuffer.SetData(indices.ToArray());
        }

        private void CreateWith32BitIndices(GraphicsDevice graphicsDevice,
                                            Particle[] positions,
                                            float upperLeftVertexAnimationWeight,
                                            float upperRightVertexAnimationWeight,
                                            float lowerLeftVertexAnimationWeight,
                                            float lowerRightVertexAnimationWeight)
        {
            List<BillboardVertex> vertices = new List<BillboardVertex>();
            List<int> indices = new List<int>();
            int baseIndex = 0;

            for (int i = 0; i < positions.Length; ++i)
            {
                Vector3 position = positions[i].getPosition();
                BillboardVertex vertex = new BillboardVertex();

                vertex.Position = new Vector4(position, upperLeftVertexAnimationWeight);
                vertex.TexCoord = new Vector4(0.0f, 0.0f, -1.0f, 1.0f);
                vertices.Add(vertex);

                vertex.Position = new Vector4(position, upperRightVertexAnimationWeight);
                vertex.TexCoord = new Vector4(1.0f, 0.0f, 1.0f, 1.0f);
                vertices.Add(vertex);

                vertex.Position = new Vector4(position, lowerLeftVertexAnimationWeight);
                vertex.TexCoord = new Vector4(0.0f, 1.0f, -1.0f, -1.0f);
                vertices.Add(vertex);

                vertex.Position = new Vector4(position, lowerRightVertexAnimationWeight);
                vertex.TexCoord = new Vector4(1.0f, 1.0f, 1.0f, -1.0f);
                vertices.Add(vertex);

                baseIndex = i * 4;

                indices.Add(0 + baseIndex);
                indices.Add(1 + baseIndex);
                indices.Add(2 + baseIndex);
                indices.Add(2 + baseIndex);
                indices.Add(1 + baseIndex);
                indices.Add(3 + baseIndex);
            }

            vertexBuffer = new VertexBuffer(graphicsDevice, typeof(BillboardVertex), vertices.Count, BufferUsage.WriteOnly);
            vertexBuffer.SetData(vertices.ToArray());

            indexBuffer = new IndexBuffer(graphicsDevice, IndexElementSize.ThirtyTwoBits, indices.Count, BufferUsage.WriteOnly);
            indexBuffer.SetData(indices.ToArray());
        }
    }
}
