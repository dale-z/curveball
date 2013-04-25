using GoblinXNA.Graphics.Geometry;
using GoblinXNA.SceneGraph;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Curveball
{
    public class RemotePlayer : PlayerAgent
    {
        // A teammate or opponent controlled by someone on the
        // other side of the network.
        // Data to update its paddle should be retrieved from
        // the game server.
        public RemotePlayer(Level level)
            : base(level)
        {
            // Blue...

            Node = new SynchronizedGeometryNode();
            Node.Model = new Box(30, 30, 30);
            Node.Material.Ambient = new Vector4(0.0f, 0.0f, 1.0f, 1.0f);
            Node.Material.Diffuse = new Vector4(0.0f, 0.0f, 1.0f, 1.0f);
            Node.Material.Specular = new Vector4(0.0f, 0.0f, 1.0f, 1.0f);
            Node.Material.SpecularPower = 100.0f;
        }

        public override void Update()
        {
            throw new NotImplementedException();
        }
    }
}
