using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;


namespace UHSampleGame.InstancedModels
{

    public class InstancedModel
    {

        const int maxShaderMatrices = 20;
        VertexBuffer instancedVertexBuffer;
        IndexBuffer instancedIndexBuffer;
        VertexDeclaration instancedVertexDeclaration;
        int originalVertexCount = 0;
        int originalIndexCount = 0;
        int vertexStride = 0;
        int maxInstances = 0;

        Matrix[] tempMatrices = new Matrix[maxShaderMatrices];
        Model originalModel;
        Matrix[] modelBones;
        GraphicsDevice graphicsDevice;

        public InstancedModel(GraphicsDevice graphics)
        {
            graphicsDevice = graphics; 
        }

        public void Setup(Model model)
        {
            originalModel = model;
            modelBones = new Matrix[model.Bones.Count];
            model.CopyAbsoluteBoneTransformsTo(modelBones);

            SetupInstancedVertexData();
        }

        void SetupInstancedVertexData()
        {
            // Read the existing vertex data, then destroy the existing vertex buffer.
            ModelMesh mesh = originalModel.Meshes[0];
            ModelMeshPart part = mesh.MeshParts[0];

            originalVertexCount = part.VertexBuffer.VertexCount;
            VertexDeclaration originalVertexDeclaration = part.VertexBuffer.VertexDeclaration;

            int indexOverflowLimit = ushort.MaxValue / originalVertexCount;
            maxInstances = Math.Min(indexOverflowLimit, maxShaderMatrices);

            byte[] oldVertexData = new byte[originalVertexCount * originalVertexDeclaration.VertexStride];
            part.VertexBuffer.GetData(oldVertexData);

            // Adjust the vertex stride to include our additional index channel.
            int oldVertexStride = part.VertexBuffer.VertexDeclaration.VertexStride;
            vertexStride = oldVertexStride + (sizeof(byte) * 4) + (sizeof(float) * 4); //add Byte4 for BoneIndices and Vector4 for BoneWeights

            // Allocate a temporary array to hold the replicated vertex data.
            byte[] newVertexData = new byte[originalVertexCount * vertexStride * maxInstances];
            int outputPosition = 0;

            // Replicate one copy of the original vertex buffer for each instance.
            for (int instanceIndex = 0; instanceIndex < maxInstances; instanceIndex++)
            {
                int sourcePosition = 0;

                // Convert the instance index from float into an array of raw bits.
                byte[] blendIndices = new byte[4];
                blendIndices[0] = (byte)instanceIndex;
                blendIndices[1] = (byte)instanceIndex;
                blendIndices[2] = (byte)instanceIndex;
                blendIndices[3] = (byte)instanceIndex;

                byte[] blendWeight = BitConverter.GetBytes(1.0f);

                for (int i = 0; i < originalVertexCount; i++)
                {
                    // Copy over the existing data for this vertex.
                    Array.Copy(oldVertexData, sourcePosition,
                               newVertexData, outputPosition, oldVertexStride);

                    outputPosition += oldVertexStride;
                    sourcePosition += oldVertexStride;

                    // Set the value of our new index channel.
                    blendIndices.CopyTo(newVertexData, outputPosition);
                    outputPosition += blendIndices.Length;

                    //copy blend weights
                    blendWeight.CopyTo(newVertexData, outputPosition);
                    outputPosition += blendWeight.Length;
                    blendWeight.CopyTo(newVertexData, outputPosition);
                    outputPosition += blendWeight.Length;
                    blendWeight.CopyTo(newVertexData, outputPosition);
                    outputPosition += blendWeight.Length;
                    blendWeight.CopyTo(newVertexData, outputPosition);
                    outputPosition += blendWeight.Length;
                }
            }

            int instanceIndexOffset = oldVertexStride;
            VertexElement[] extraElements =
            {
                new VertexElement((short)oldVertexStride, VertexElementFormat.Byte4, VertexElementUsage.BlendIndices, 0),
                new VertexElement((short)oldVertexStride + (sizeof(byte) * 4), VertexElementFormat.Vector4, VertexElementUsage.BlendWeight, 0)
            };

            int length = originalVertexDeclaration.GetVertexElements().Length + extraElements.Length;
            VertexElement[] elements = new VertexElement[length];
            originalVertexDeclaration.GetVertexElements().CopyTo(elements, 0);
            extraElements.CopyTo(elements, originalVertexDeclaration.GetVertexElements().Length);

            // Create a new vertex declaration.
            instancedVertexDeclaration = new VertexDeclaration(elements);

            // Create a new vertex buffer, and set the replicated data into it.
            instancedVertexBuffer = new VertexBuffer(graphicsDevice, instancedVertexDeclaration, newVertexData.Length, BufferUsage.None); ///ERROR!
            instancedVertexBuffer.SetData(newVertexData);

            //handle vertex indices
            originalIndexCount = part.IndexBuffer.IndexCount;
            ushort[] oldIndices = new ushort[originalIndexCount];
            part.IndexBuffer.GetData(oldIndices);

            // Allocate a temporary array to hold the replicated index data.
            ushort[] newIndices = new ushort[originalIndexCount * maxInstances];
            outputPosition = 0;

            // Replicate one copy of the original index buffer for each instance.
            for (int instanceIndex = 0; instanceIndex < maxInstances; instanceIndex++)
            {
                int instanceOffset = instanceIndex * originalVertexCount;

                for (int i = 0; i < part.IndexBuffer.IndexCount; i++)
                {
                    newIndices[outputPosition] = (ushort)(oldIndices[i] +
                                                          instanceOffset);
                    outputPosition++;
                }
            }



            // Create a new index buffer, and set the replicated data into it.
            instancedIndexBuffer = new IndexBuffer(graphicsDevice, IndexElementSize.SixteenBits, newIndices.Length, BufferUsage.None);
            instancedIndexBuffer.SetData(newIndices);
        }


        public void Draw(ref Matrix[] transformMatrices, ref SkinnedEffect skinnedEffect)
        {
            int totalInstances = transformMatrices.Length;
            BasicEffect effect = (BasicEffect)originalModel.Meshes[0].MeshParts[0].Effect;
            skinnedEffect.Texture = effect.Texture;

            graphicsDevice.SetVertexBuffer(instancedVertexBuffer);
            graphicsDevice.Indices = instancedIndexBuffer;

            for (int i = 0; i < totalInstances; i += maxInstances)
            {
                // How many instances can we fit into this batch?
                int instanceCount = totalInstances - i;

                if (instanceCount > maxInstances)
                    instanceCount = maxInstances;

                // Upload transform matrices as shader constants.
                Array.Copy(transformMatrices, i, tempMatrices, 0, instanceCount);

                // *transformMatrices[i];
                skinnedEffect.SetBoneTransforms(tempMatrices);

                foreach (EffectPass pass in skinnedEffect.CurrentTechnique.Passes)
                {
                    pass.Apply();

                    graphicsDevice.DrawIndexedPrimitives(PrimitiveType.TriangleList, 0, 0,
                        instanceCount * originalVertexCount, 0, instanceCount * originalIndexCount / 3);
                }
            }
        }
    }
}