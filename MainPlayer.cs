using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Curveball
{
    public class MainPlayer : PlayerAgent
    {
        // This is the player conntrolled by the camera.
        // It should be able to update its paddle according to
        // the ground node of the scene.
        public MainPlayer(Level level)
            : base(level)
        {
            throw new NotImplementedException();
        }

        public override Paddle Update()
        {
            throw new NotImplementedException();
        }
    }
}
