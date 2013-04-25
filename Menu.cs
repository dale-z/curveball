using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Media;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Color = Microsoft.Xna.Framework.Color;
using Matrix = Microsoft.Xna.Framework.Matrix;

using GoblinXNA;
using GoblinXNA.Graphics;
using GoblinXNA.SceneGraph;
using Model = GoblinXNA.Graphics.Model;
using GoblinXNA.Graphics.Geometry;
using GoblinXNA.Device.Generic;
using GoblinXNA.Device.Capture;
using GoblinXNA.Device.Vision;
using GoblinXNA.Device.Vision.Marker;
using GoblinXNA.Device.Util;
using GoblinXNA.Helpers;
using GoblinXNA.UI;
using GoblinXNA.UI.UI2D;

using Tutorial16___Multiple_Viewport;
using Curveball;
using Tutorial16___Multiple_Viewport___PhoneLib;

namespace Curveball
{
    // The viewport and ground marker node comes from the container.
    // This class only contains menu specific scene elements.
    public class Menu
    {
        private Tutorial16_Phone _container;
        GeometryNode overlayNode;

        public Menu(Tutorial16_Phone container)
        {
            _container = container;
        }

        public void GotoLevel()
        {
            // What to do here:
            // Halt the container update.
            // Unmount itself. (Call this.Unmount().)
            // Create a level with the information gathered.
            // Mount the level. (Call level.Mount().)
            // Set the state of the container.
            // Resume container update.
        }

        public void Mount()
        {
            // Attach its elements to the scene graph in the container.

            throw new NotImplementedException();
        }

        public void Unmount()
        {
            // Detach its elements from the scene graph in the container.

            throw new NotImplementedException();
        }

        // Move the menu specific part of the scene graph from the
        // container to this class

        public void Initialize()
        {
            // Attach itself to the scene graph of the container.

            // Create 3D objects
            CreateObjects();
        }

        private void CreateObjects()
        {
            ModelLoader loader = new ModelLoader();

            overlayNode = new GeometryNode("Overlay");
            overlayNode.Model = (Model)loader.Load("", "p1_wedge");
            ((Model)overlayNode.Model).UseInternalMaterials = true;

            // Get the dimension of the model
            Vector3 dimension = Vector3Helper.GetDimensions(overlayNode.Model.MinimumBoundingBox);
            // Scale the model to fit to the size of 5 markers
            float scale = 100 / Math.Max(dimension.X, dimension.Z);

            TransformNode overlayTransNode = new TransformNode()
            {
                Translation = new Vector3(0, 0, dimension.Y * scale / 2),
                Rotation = Quaternion.CreateFromAxisAngle(Vector3.UnitX, MathHelper.ToRadians(90)) *
                    Quaternion.CreateFromAxisAngle(Vector3.UnitY, MathHelper.ToRadians(90)),
                Scale = new Vector3(scale, scale, scale)
            };

            _container.OverlayRoot.AddChild(overlayTransNode);
            overlayTransNode.AddChild(overlayNode);
        }

        public void Update()
        {

        }

        public void Return()
        {
            // Called when returning from level.
        }
    }
}
