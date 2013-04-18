using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GoblinXNA;
using GoblinXNA.Graphics;
using Microsoft.Xna.Framework;
using GoblinXNA.SceneGraph;

namespace Curveball
{
    public abstract class Powerup
    {
        public Powerup(Level level)
        {
            _level = level;

            throw new NotImplementedException();
        }

        // Callback to modify other objects in the game?
        public abstract void Update();

        // Access other objects via '_level'.

        // Position
        public TransformNode Transform
        {
            get;
            set;
        }

        // Model
        public IModel Model
        {
            get;
            set;
        }

        // Remaining time to live (in milliseconds)?
        public int TTL
        {
            get;
            set;
        }

        private Level _level;
    }
}
