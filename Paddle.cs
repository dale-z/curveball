using GoblinXNA.Graphics;
using GoblinXNA.SceneGraph;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Curveball
{
    // This class is different from 'Player' since they are not in the
    // same concept domain.
    public class Paddle
    {
        public Paddle(PlayerAgent player)
        {
            _player = player;

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

        private PlayerAgent _player;
    }
}
