using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Tutorial16___Multiple_Viewport___PhoneLib;

namespace Curveball
{
    // The viewport and ground marker node comes from the container.
    // This class only contains level specific scene elements.

    // An instance of this class is supposed to be created with
    // information from the menu as well as built-in settings.
    public class Level
    {
        public Level(Tutorial16_Phone container)
        {
            _container = container;

            _players = new List<PlayerAgent>();
            _opponents= new List<PlayerAgent>();

            _ball = new Ball(this);
            _tunnel = new Tunnel(this);

            _powerups = new List<Powerup>();

            throw new NotImplementedException();
        }

        // Where the non-physics logic goes...
        public void Update()
        {
            // Update the players and opponents.
            foreach (PlayerAgent player in _players)
            {
                player.Update();
            }
            foreach (PlayerAgent opponent in _opponents)
            {
                opponent.Update();
            }

            // The update of the ball and the tunnel consists of
            // of two parts: physics and non-physics.
            // The former is handled by GoblinXNA, and the latter
            // will be handled by the update functions of
            // powerups that have effects on them.
            // So they are essentially inactive objects here.

            // Update the powerups: Appear, vanish, affecting other
            // objects in the level, etc.
            foreach (Powerup powerup in _powerups)
            {
                powerup.Update();
            }
            // Also add or remove powerups when needed.

            throw new NotImplementedException();
        }

        // TODO
        // Switch the state of the container to
        // the menu.
        public void GotoMenu()
        {
            // What to do here:
            // Halt the container update.
            // Unmount itself. (Call this.Unmount().) 
            // Inintialize the menu. (Reset or set hi-score.)
            // Mount the menu.
            // Set the state of the container.
            // Resume container update.
        }

        public void Mount()
        {
            // Attach its elements to the scene graph in the container.

            throw new NotImplementedException();
        }

        public void Unmount()
        {
            // Detach its elements from the scene graph in the container.

            throw new NotImplementedException();
        }

        // Player's team. A team contains one or multiple players.
        private List<PlayerAgent> _players;

        // Opponent's team. A team contains one or multiple players.
        private List<PlayerAgent> _opponents;

        // The tunnel. Represent it using a node?
        private Tunnel _tunnel;

        // The ball. Represent it using a node?
        private Ball _ball;

        // A list of active powerups.
        private List<Powerup> _powerups;

        private Tutorial16_Phone _container;

        // TODO
        // Move the level specific part of the scene graph from the
        // container to this class.
    }
}