using GoblinXNA.Graphics.Geometry;
using GoblinXNA.SceneGraph;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Curveball
{
    public class WallPlayer : PlayerAgent
    {
        public const float WallThickness = 10.0f;

        // A static player used mainly as an obstable to
        // bounce the ball back. Typically used in the practice
        // mode.
        public WallPlayer(Level level, string name)
            : base(level)
        {
            Node = new SynchronizedGeometryNode(name);
            Node.Model = new Box(Tunnel.Width, WallThickness, Tunnel.Height);
            Node.Material.Ambient = new Vector4(1.0f, 1.0f, 1.0f, 1.0f);
            Node.Material.Diffuse = new Vector4(1.0f, 1.0f, 1.0f, 1.0f);
            Node.Material.Specular = new Vector4(1.0f, 1.0f, 1.0f, 1.0f);
            Node.Material.SpecularPower = 100.0f;

            // TODO Add physics for the wall.
        }

        public override void Update()
        {
            // Do nothing. It's a wall!
        }
    }
}
