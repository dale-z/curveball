using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Curveball
{
    public class RemotePlayer : PlayerAgent
    {
        // A teammate or opponent controlled by someone on the
        // other side of the network.
        // Data to update its paddle should be retrieved from
        // the game server.
        public RemotePlayer(Level level)
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
