using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Curveball
{
    public enum PlayerAgentType : byte
    {
        Main, Ai, Wall
    }

    public enum Role : byte
    {
        Team1, Team2, Server
    }

    // An instance of 'LevelInfo' is supposed to be created at the end
    // of the menu, and:
    // 1. Used to initialize a local scene.
    // 2. Send to the server to initialize the central scene.
    public class LevelInfo
    {
        // This field decides the position of the camera, which paddle to control
        // and what it should do to update the scene.
        // Player1/2: Send the ground array transformation to the server.
        //    Server: Accept ground array transformation to update the paddles.
        //            Update the physics of the ball. Update powerups.
        public Role CurrentRole
        {
            get;
            set;
        }

        public List<PlayerAgentType> Team1PlayerTypes
        {
            get;
            private set;
        }

        public List<PlayerAgentType> Team2PlayerTypes
        {
            get;
            private set;
        }

        // Other fields can also be added, e.g. ball physics parameters.

        public LevelInfo(Role role)
        {
            CurrentRole = role;
            Team1PlayerTypes = new List<PlayerAgentType>();
            Team2PlayerTypes = new List<PlayerAgentType>();
        }
    }
}
