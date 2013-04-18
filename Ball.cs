using GoblinXNA.Graphics;
using GoblinXNA.SceneGraph;
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

            throw new NotImplementedException();
        }

        public TransformNode Transform
        {
            get;
            set;
        }

        public IModel Model
        {
            get;
            set;
        }

        private Level _level;
    }
}
