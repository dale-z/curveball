using GoblinXNA.Graphics;
using GoblinXNA.Graphics.Geometry;
using GoblinXNA.SceneGraph;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Curveball
{
    public class Ball
    {
        public Ball(Level level)
        {
            _level = level;

            // TODO Set its node (model and transformation).
            Node = new SynchronizedGeometryNode();

            Node.Model = new Sphere(5, 32, 32);
            Node.Material.Ambient = new Vector4(0.5f, 0.5f, 0.5f, 1.0f);
            Node.Material.Diffuse = new Vector4(0.5f, 0.5f, 0.5f, 1.0f);
            Node.Material.Specular = new Vector4(0.5f, 0.5f, 0.5f, 1.0f);
            Node.Material.SpecularPower = 100.0f;
        }

        public SynchronizedGeometryNode Node
        {
            get;
            set;
        }

        private Level _level;
    }
}
