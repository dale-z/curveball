using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;

namespace Curveball
{
    public abstract class PlayerAgent
    {
        public PlayerAgent(Level level)
        {
            _level = level;

            throw new NotImplementedException();
        }

        public abstract Paddle Update();

        public int Score
        {
            get;
            set;
        }

        private Level _level;
    }
}
