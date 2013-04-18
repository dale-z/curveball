using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Curveball
{
    public class AiPlayer : PlayerAgent
    {
        // An AI player is supposed to update its paddle
        // in an autonomous way.
       public AiPlayer(Level level)
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
