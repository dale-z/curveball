using GoblinXNA.Graphics.Geometry;
using GoblinXNA.SceneGraph;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Curveball
{
    public class AiPlayer : PlayerAgent
    {
        // An AI player is supposed to update its paddle
        // in an autonomous way.
       public AiPlayer(Level level, string name)
            : base(level)
        {
            // Red...

            Node = new SynchronizedGeometryNode(name);
            Node.Model = new Box(30, 30, 30);
            Node.Material.Ambient = new Vector4(1.0f, 0.0f, 0.0f, 1.0f);
            Node.Material.Diffuse = new Vector4(1.0f, 0.0f, 0.0f, 1.0f);
            Node.Material.Specular = new Vector4(1.0f, 0.0f, 0.0f, 1.0f);
            Node.Material.SpecularPower = 100.0f;

            // TODO Add physics for the paddle.
        }

       public override void Update()
       {
           // TODO
           // Update its paddle movement autonomously. Information about the
           // environent can be accessed from '_level'.

           throw new NotImplementedException();
       }
    }
}
