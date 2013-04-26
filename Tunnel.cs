using GoblinXNA.Graphics;
using GoblinXNA.Graphics.Geometry;
using GoblinXNA.SceneGraph;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Tutorial16___Multiple_Viewport___PhoneLib;

namespace Curveball
{
    public class Tunnel
    {
        public Tunnel(Level level, string name)
        {
            _level = level;

            // TODO Set its node (model and transformation).
            Node = new SynchronizedGeometryNode(name);
            Node.Model = new Box(Width, Length, Height);
            Node.Material.Ambient = new Vector4(1.0f, 1.0f, 1.0f, 0.6f);
            Node.Material.Diffuse = new Vector4(1.0f, 1.0f, 1.0f, 0.6f);
            Node.Material.Specular = new Vector4(1.0f, 1.0f, 1.0f, 0.6f);
            Node.Material.SpecularPower = 100.0f;

            // TODO Add physics for the tunnel.

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

        public const float Length = Tutorial16_Phone.MarkerSize * 2;
        public const float Width = Tutorial16_Phone.MarkerSize * 2;
        public const float Height = Tutorial16_Phone.MarkerSize;

        private Level _level;
    }
}
