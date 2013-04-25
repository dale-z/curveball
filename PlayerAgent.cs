using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;
using GoblinXNA.SceneGraph;

namespace Curveball
{
    public abstract class PlayerAgent
    {
        public PlayerAgent(Level level)
        {
            _level = level;
        }

        public SynchronizedGeometryNode Node
        {
            get;
            protected set;
        }

        public abstract void Update();

        public int Score
        {
            get;
            set;
        }

        private Level _level;
    }
}
