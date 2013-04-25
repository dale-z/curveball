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
    public class Tunnel
    {
        public Tunnel(Level level)
        {
            _level = level;

            // TODO Set its node (model and transformation).
            Node = new SynchronizedGeometryNode();
            Node.Model = new Box(Width, Length, Height);
            Node.Material.Ambient = new Vector4(1.0f, 1.0f, 1.0f, 0.6f);
            Node.Material.Diffuse = new Vector4(1.0f, 1.0f, 1.0f, 0.6f);
            Node.Material.Specular = new Vector4(1.0f, 1.0f, 1.0f, 0.6f);
            Node.Material.SpecularPower = 100.0f;

            Node.Physics.Collidable = true;
            Node.Physics.Interactable = true;
            Node.AddToPhysicsEngine = true;
            Node.Physics.Shape = GoblinXNA.Physics.ShapeType.Box;
            Node.Physics.Mass = 10;
        }

        public SynchronizedGeometryNode Node
        {
            get;
            set;
        }

        public const float Length = 100.0f;
        public const float Width = 100.0f;
        public const float Height = 50.0f;

        private Level _level;
    }
}
