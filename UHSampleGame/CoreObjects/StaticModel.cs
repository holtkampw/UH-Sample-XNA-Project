#region Using Statements
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using UHSampleGame.CameraManagement;
using UHSampleGame.ScreenManagement;
#endregion

namespace UHSampleGame.CoreObjects
{
    /// <summary>
    /// Enum describes the various possible techniques
    /// that can be chosen to implement instancing.
    /// </summary>
    public enum InstancingTechnique
    {
        HardwareInstancing,
        NoInstancing,
        NoInstancingOrStateBatching
    }

    public class StaticModel : GameObject
    {
        #region Class Variables
        //Model model;
        protected float scale;
        CameraManager cameraManager;
        protected static Model model;

        //Instancing Stuff
        DynamicVertexBuffer vertexBuffer;
        public static InstancingTechnique instancingTechnique = InstancingTechnique.HardwareInstancing;

        // To store instance transform matrices in a vertex buffer, we use this custom
        // vertex type which encodes 4x4 matrices as a set of four Vector4 values.
        static VertexDeclaration instanceVertexDeclaration = new VertexDeclaration
        (
            new VertexElement(0, VertexElementFormat.Vector4, VertexElementUsage.BlendWeight, 0),
            new VertexElement(16, VertexElementFormat.Vector4, VertexElementUsage.BlendWeight, 1),
            new VertexElement(32, VertexElementFormat.Vector4, VertexElementUsage.BlendWeight, 2),
            new VertexElement(48, VertexElementFormat.Vector4, VertexElementUsage.BlendWeight, 3)
        );

        //Model Stuff
        Matrix view;
        Matrix[] transforms;
        Matrix[] boneTransforms;
        Matrix rotationMatrixX;
        Matrix rotationMatrixY;
        Matrix rotationMatrixZ;
        #endregion

        #region Initialization
        /// <summary>
        /// Default Constructor to setup Model
        /// </summary>
        public StaticModel()
        {
            model = null;
        }

        /// <summary>
        /// Constructor consisting of a given model
        /// </summary>
        /// <param name="model">Model for use</param>
        public StaticModel(Model newModel)
        {
            if(model == null)
                model = newModel;
            SetupModel();
            SetupCamera();
        }

        /// <summary>
        /// Adds a model to the Static Model and performs setup
        /// </summary>
        /// <param name="model">Model for this instance</param>
        public void SetupModel(Model newModel)
        {
            if(model == null)
                model = newModel;
            SetupModel();
            SetupCamera();
        }

        protected void SetupModel()
        {
            //set scale
            scale = 1.0f;

            //save bones
            boneTransforms = new Matrix[model.Bones.Count];
            model.CopyAbsoluteBoneTransformsTo(boneTransforms);
            
            //setup transforms
            transforms = new Matrix[1];
            transforms[0] = Matrix.CreateScale(scale) * Matrix.CreateTranslation(position);

            //give default rotation
            rotationMatrixX = Matrix.CreateRotationX(0.0f);
            rotationMatrixY = Matrix.CreateRotationY(0.0f);
            rotationMatrixZ = Matrix.CreateRotationZ(0.0f);

            //give default position
            position = Vector3.Zero;
        }

        /// <summary>
        /// Sets up default camera information
        /// </summary>
        protected void SetupCamera()
        {
            cameraManager = (CameraManager)ScreenManager.Game.Services.GetService(typeof(CameraManager));
            view = Matrix.CreateTranslation(0,0,0) * Matrix.CreateScale(scale) * cameraManager.ViewMatrix;
        }
        #endregion

        #region Properties
        public float Scale
        {
            get { return scale; }
            set
            {
                scale = value;
                SetupCamera();
            }
        }
        #endregion

        #region Manipulation
        public void RotateX(float rotation)
        {
            rotationMatrixX = Matrix.CreateRotationX(rotation);
        }

        public void RotateY(float rotation)
        {
            rotationMatrixY = Matrix.CreateRotationY(rotation);
        }

        public void RotateZ(float rotation)
        {
            rotationMatrixZ = Matrix.CreateRotationZ(rotation);
        }
        #endregion

        #region Update and Draw
        public override void Update(GameTime gameTime)
        {
            //update view matrix
            UpdateView();
            UpdateTransforms();

            base.Update(gameTime);
        }

        public void UpdateView()
        {
            view = cameraManager.ViewMatrix;
        }

        public void UpdateTransforms()
        {
            transforms[0] = Matrix.CreateScale(scale) *
                    Matrix.CreateTranslation(position) *
                    rotationMatrixX *
                    rotationMatrixY *
                    rotationMatrixZ;
        }

        public override void Draw(GameTime gameTime)
        {
            //// Draw all of the meshes for a model
            //foreach (ModelMesh mesh in model.Meshes)
            //{
            //    //each mesh has an effect
            //    foreach (BasicEffect effect in mesh.Effects)
            //    {
            //        effect.EnableDefaultLighting();

            //        //Where will the model be in the world?
            //        effect.World = transforms[mesh.ParentBone.Index];

            //        //How are we viewing it?
            //        effect.View = view;

            //        //Projection information
            //        effect.Projection = cameraManager.ProjectionMatrix;
            //    }
            //    // Draw the mesh, using the effects set above.
            //    mesh.Draw();
            //}



            // Draw all the instances, using the currently selected rendering technique.
            switch (instancingTechnique)
            {
                case InstancingTechnique.HardwareInstancing:
                    DrawModelHardwareInstancing(model, boneTransforms,
                                                transforms, view, cameraManager.ProjectionMatrix);
                    break;

                case InstancingTechnique.NoInstancing:
                    DrawModelNoInstancing(model, boneTransforms,
                                          transforms, view, cameraManager.ProjectionMatrix);
                    break;

                case InstancingTechnique.NoInstancingOrStateBatching:
                    DrawModelNoInstancingOrStateBatching(model, boneTransforms,
                                                         transforms, view, cameraManager.ProjectionMatrix);
                    break;
            }

            base.Draw(gameTime);
        }


        /// <summary>
        /// Efficiently draws several copies of a piece of geometry using hardware instancing.
        /// </summary>
        void DrawModelHardwareInstancing(Model model, Matrix[] modelBones,
                                         Matrix[] instances, Matrix view, Matrix projection)
        {
            //if (instances.Length == 0)
            //    return;

            // If we have more instances than room in our vertex buffer, grow it to the neccessary size.
            if ((vertexBuffer == null) ||
                (instances.Length > vertexBuffer.VertexCount))
            {
                if (vertexBuffer != null)
                    vertexBuffer.Dispose();

                vertexBuffer = new DynamicVertexBuffer(ScreenManager.Game.GraphicsDevice, instanceVertexDeclaration,
                                                               /*instances.Length*/1, BufferUsage.WriteOnly);
            }

            // Transfer the latest instance transform matrices into the instanceVertexBuffer.
            vertexBuffer.SetData(instances, 0, instances.Length, SetDataOptions.Discard);

            foreach (ModelMesh mesh in model.Meshes)
            {
                foreach (ModelMeshPart meshPart in mesh.MeshParts)
                {
                    // Tell the GPU to read from both the model vertex buffer plus our instanceVertexBuffer.
                    ScreenManager.Game.GraphicsDevice.SetVertexBuffers(
                        new VertexBufferBinding(meshPart.VertexBuffer, meshPart.VertexOffset, 0),
                        new VertexBufferBinding(vertexBuffer, 0, 1)
                    );

                    ScreenManager.Game.GraphicsDevice.Indices = meshPart.IndexBuffer;

                    // Set up the instance rendering effect.
                    Effect effect = meshPart.Effect;

                    effect.CurrentTechnique = effect.Techniques["HardwareInstancing"];

                    effect.Parameters["World"].SetValue(modelBones[mesh.ParentBone.Index]);
                    effect.Parameters["View"].SetValue(view);
                    effect.Parameters["Projection"].SetValue(projection);

                    // Draw all the instance copies in a single call.
                    foreach (EffectPass pass in effect.CurrentTechnique.Passes)
                    {
                        pass.Apply();

                        ScreenManager.Game.GraphicsDevice.DrawInstancedPrimitives(PrimitiveType.TriangleList, 0, 0,
                                                               meshPart.NumVertices, meshPart.StartIndex,
                                                               meshPart.PrimitiveCount, instances.Length);
                    }
                }
            }
        }


        /// <summary>
        /// Draws several copies of a piece of geometry without using any
        /// special GPU instancing techniques at all. This just does a
        /// regular loop and issues several draw calls one after another.
        /// </summary>
        void DrawModelNoInstancing(Model model, Matrix[] modelBones,
                                   Matrix[] instances, Matrix view, Matrix projection)
        {
            foreach (ModelMesh mesh in model.Meshes)
            {
                foreach (ModelMeshPart meshPart in mesh.MeshParts)
                {
                    ScreenManager.Game.GraphicsDevice.SetVertexBuffer(meshPart.VertexBuffer, meshPart.VertexOffset);
                    ScreenManager.Game.GraphicsDevice.Indices = meshPart.IndexBuffer;

                    // Set up the rendering effect.
                    Effect effect = meshPart.Effect;

                    effect.CurrentTechnique = effect.Techniques["NoInstancing"];

                    effect.Parameters["View"].SetValue(view);
                    effect.Parameters["Projection"].SetValue(projection);

                    EffectParameter transformParameter = effect.Parameters["World"];

                    // Draw a single instance copy each time around this loop.
                    for (int i = 0; i < instances.Length; i++)
                    {
                        transformParameter.SetValue(modelBones[mesh.ParentBone.Index] * instances[i]);

                        foreach (EffectPass pass in effect.CurrentTechnique.Passes)
                        {
                            pass.Apply();

                            ScreenManager.Game.GraphicsDevice.DrawIndexedPrimitives(PrimitiveType.TriangleList, 0, 0,
                                                                 meshPart.NumVertices, meshPart.StartIndex,
                                                                 meshPart.PrimitiveCount);
                        }
                    }
                }
            }
        }


        /// <summary>
        /// This technique is NOT a good idea! It is only included in the sample
        /// for comparison purposes, so you can compare its performance with the
        /// other more sensible approaches. This uses the exact same shader code
        /// as the preceding NoInstancing technique, but with a key difference.
        /// Where the NoInstancing technique worked like this:
        /// 
        ///     SetRenderStates()
        ///     foreach instance
        ///     {
        ///         Update effect with per-instance transform matrix
        ///         DrawIndexedPrimitives()
        ///     }
        /// 
        /// NoInstancingOrStateBatching works like so:
        /// 
        ///     foreach instance
        ///     {
        ///         Set per-instance transform matrix into the effect
        ///         SetRenderStates()
        ///         DrawIndexedPrimitives()
        ///     }
        ///      
        /// As you can see, this is repeatedly setting the same renderstates.
        /// Not so efficient.
        /// 
        /// In other words, the built-in Model.Draw method is pretty inefficient when
        /// it comes to drawing more than one instance! Even without using any fancy
        /// shader techniques, you can get a significant speed boost just by rearranging
        /// your drawing code to work more like the earlier NoInstancing technique.
        /// </summary>
        void DrawModelNoInstancingOrStateBatching(Model model, Matrix[] modelBones,
                                                  Matrix[] instances, Matrix view, Matrix projection)
        {
            for (int i = 0; i < instances.Length; i++)
            {
                foreach (ModelMesh mesh in model.Meshes)
                {
                    foreach (Effect effect in mesh.Effects)
                    {
                        effect.CurrentTechnique = effect.Techniques["NoInstancing"];

                        effect.Parameters["World"].SetValue(modelBones[mesh.ParentBone.Index] * instances[i]);
                        effect.Parameters["View"].SetValue(view);
                        effect.Parameters["Projection"].SetValue(projection);
                    }

                    mesh.Draw();
                }
            }
        }
        #endregion
    }
}
