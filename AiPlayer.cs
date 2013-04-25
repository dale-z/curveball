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
       public AiPlayer(Level level)
            : base(level)
        {
            // Red...

            Node = new SynchronizedGeometryNode();
            Node.Model = new Box(30, 30, 30);
            Node.Material.Ambient = new Vector4(1.0f, 0.0f, 0.0f, 1.0f);
            Node.Material.Diffuse = new Vector4(1.0f, 0.0f, 0.0f, 1.0f);
            Node.Material.Specular = new Vector4(1.0f, 0.0f, 0.0f, 1.0f);
            Node.Material.SpecularPower = 100.0f;
        }

       public override void Update()
       {
           throw new NotImplementedException();
       }
    }
}
