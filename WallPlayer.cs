using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Curveball
{
    public class WallPlayer : PlayerAgent
    {
        // A static player used mainly as an obstable to
        // bounce the ball back. Typically used in the practice
        // mode.
        public WallPlayer(Level level)
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
