using GoblinXNA.Graphics.Geometry;
using GoblinXNA.SceneGraph;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Curveball
{
    public class MainPlayer : PlayerAgent
    {
        // This is the player conntrolled by the camera.
        // It should be able to update its paddle according to
        // the ground node of the scene.
        public MainPlayer(Level level)
            : base(level)
        {
            // Green...

            Node = new SynchronizedGeometryNode();
            Node.Model = new Box(30, 30, 30);
            Node.Material.Ambient = new Vector4(0.0f, 1.0f, 0.0f, 1.0f);
            Node.Material.Diffuse = new Vector4(0.0f, 1.0f, 0.0f, 1.0f);
            Node.Material.Specular = new Vector4(0.0f, 1.0f, 0.0f, 1.0f);
            Node.Material.SpecularPower = 100.0f;
        }

        public override void Update()
        {
            throw new NotImplementedException();
        }
    }
}
