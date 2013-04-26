using Curveball;
using GoblinXNA;
using GoblinXNA.Device.Capture;
using GoblinXNA.Device.Generic;
using GoblinXNA.Device.Util;
using GoblinXNA.Device.Vision;
using GoblinXNA.Device.Vision.Marker;
using GoblinXNA.Graphics;
using GoblinXNA.Graphics.Geometry;
using GoblinXNA.Helpers;
using GoblinXNA.SceneGraph;
using GoblinXNA.UI;
using GoblinXNA.UI.UI2D;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Media;
using Tutorial16___Multiple_Viewport;
using Tutorial16___Multiple_Viewport___PhoneLib;
using Color = Microsoft.Xna.Framework.Color;
using Matrix = Microsoft.Xna.Framework.Matrix;
using Model = GoblinXNA.Graphics.Model;

namespace Curveball
{
    // The viewport and ground marker node come from the container
    // and are shared by levels attached to it.
    // This class only contains level specific scene elements.

    // An instance of this class is supposed to be created with
    // information from the menu as well as built-in settings, all stored in a
    // 'LevelInfo' object.
    public class Level
    {
        // Who is the winnder.
        public enum LevelResult
        {
            Team1, Team2, Na
        };

        // The container of this level instance.
        private Tutorial16_Phone _container;
        // The level info used to initialize this level.
        private LevelInfo _info;

        // Player's team. A team contains one or multiple players.
        private List<PlayerAgent> _team1;
        // Overall transform for Team 1 so that its players are on the right side of the tunnel.
        private TransformNode _transTeam1Node;
        // The main player on this team if one exists.
        private MainPlayer _player1;

        // Opponent's team. A team contains one or multiple players.
        private List<PlayerAgent> _team2;
        // Overall transform for Team 2 so that its players are on the right side of the tunnel.
        private TransformNode _transTeam2Node;
        // The main player on this team if one exists.
        private MainPlayer _player2;

        // The tunnel. Contains its node and related data.
        TransformNode _transTunnelNode;
        private Tunnel _tunnel;

        // The ball. Contains its node and related data.
        TransformNode _transBallNode;
        private Ball _ball;

        // The powerups. Each of them contains its node and related data.
        private List<Powerup> _powerups;

        // The root node that contains everying for this level.
        // It should be attached to the root node of the scene
        // in a 'Tutorial16_Phone' instance (the container).
        private TransformNode _levelRootNode;

        // Names for those 'SynchronizedGeometryNode'.
        private const string BallName = "Ball";
        private const string TunnelName = "Tunnel";
        private const string Team1NamePrefix = "Player1_";
        private const string Team2NamePrefix = "Player2_";

        public Level(Tutorial16_Phone container, LevelInfo info)
        {
            _container = container;
            _info = info;

            _team1 = new List<PlayerAgent>();
            _team2 = new List<PlayerAgent>();

            _powerups = new List<Powerup>();

            _levelRootNode = new TransformNode();

            Initialize();
        }

        private void Initialize()
        {
            // Initialize physics.
            InitializePhysics();

            // Add lights.
            AddLights();

            // Add ball.
            AddBall();

            // Add tunnel.
            AddTunnel();

            // Add players.

            // Team 1.
            _transTeam1Node = new TransformNode();
            _levelRootNode.AddChild(_transTeam1Node);
            _transTeam1Node.Translation = new Vector3(0.0f, -Tunnel.Length / 2.0f, 0.0f);
            AddPlayers(_info.Team1PlayerTypes, _team1, Role.Team1, Team1NamePrefix);

            // Team 2.
            _transTeam2Node = new TransformNode();
            _levelRootNode.AddChild(_transTeam2Node);
            _transTeam2Node.Translation = new Vector3(0.0f, Tunnel.Length / 2.0f, 0.0f);
            AddPlayers(_info.Team2PlayerTypes, _team2, Role.Team2, Team2NamePrefix);
        }

        private void InitializePhysics()
        {
            // Shall we do it here or in 'Tutorial16_Phone'?
        }

        private void AddLights()
        {
            // Create a directional light source
            LightSource lightSource = new LightSource();
            lightSource.Direction = new Vector3(1, -0.75f, -0.5f);
            lightSource.Diffuse = Color.White.ToVector4();
            lightSource.Specular = new Vector4(0.6f, 0.6f, 0.6f, 1);

            // Create a light node to hold the light source
            LightNode lightNode = new LightNode();
            lightNode.AmbientLightColor = new Vector4(0.2f, 0.2f, 0.2f, 1);
            lightNode.LightSource = lightSource;
            _levelRootNode.AddChild(lightNode);
        }

        private void AddBall()
        {
            // Add ball and set ball initial velocity.
            _ball = new Ball(this, BallName);

            _transBallNode = new TransformNode();

            // TODO Set the transformation of the ball.

            _transBallNode.AddChild(_ball.Node);
            _levelRootNode.AddChild(_transBallNode);

            // TODO Set physics for the ball here.
        }

        private void AddTunnel()
        {
            // Add tunnel.
            _tunnel = new Tunnel(this, TunnelName);

            _transTunnelNode = new TransformNode();

            // TODO Set the transformation of the tunnel.

            _transTunnelNode.AddChild(_tunnel.Node);
            _levelRootNode.AddChild(_transTunnelNode);

            // TODO Set physics for the tunnel here.
        }

        private void AddPlayers(List<PlayerAgentType> types, List<PlayerAgent> team, Role role, string namePrefix)
        {
            foreach (PlayerAgentType type in types)
            {
                PlayerAgent player = null;
                switch (type)
                {
                    case PlayerAgentType.Ai:
                        player = new AiPlayer(this, namePrefix + team.Count);
                        break;

                    case PlayerAgentType.Main:
                        player = new MainPlayer(this, namePrefix + team.Count);
                        break;

                    case PlayerAgentType.Wall:
                        player = new WallPlayer(this, namePrefix + team.Count);
                        break;

                    default:
                        throw new NotSupportedException();
                }

                team.Add(player);

                switch (role)
                {
                    case Role.Team1:
                        // Initialize the transformation.
                        _transTeam1Node.AddChild(player.Node);
                        break;

                    case Role.Team2:
                        // Initialize the transformation.
                        _transTeam2Node.AddChild(player.Node);
                        break;

                    default:
                        throw new NotSupportedException();
                }
            }
        }

        public void Mount()
        {
            // Attach its elements to the scene graph in the container.
            _container.OverlayRoot.AddChild(_levelRootNode);
        }

        public void Unmount()
        {
            // Detach its elements from the scene graph in the container.
            _container.OverlayRoot.RemoveChild(_levelRootNode);
        }

        // Where the non-physics logic goes...
        public void Update(TimeSpan elapsedTime, bool isActive)
        {
            // The following part is not finished.
            // Return here to avoid 'NotImplementedExeption' during testing.
            return;

            switch (_info.CurrentRole)
            {
                case Role.Team1:
                    UpdateTeam1(elapsedTime, isActive);
                    break;

                case Role.Team2:
                    UpdateTeam2(elapsedTime, isActive);
                    break;

                case Role.Server:
                    UpdateServer(elapsedTime, isActive);
                    break;

                default:
                    throw new NotSupportedException();
            }
        }

        public void UpdateTeam1(TimeSpan elapsedTime, bool isActive)
        {
            // TODO
            // Just transmit its ground array transformation
            // back to the server.
            throw new NotImplementedException();
        }

        public void UpdateTeam2(TimeSpan elapsedTime, bool isActive)
        {
            // TODO
            // Just transmit its ground array transformation
            // back to the server.
            throw new NotImplementedException();
        }

        public void UpdateServer(TimeSpan elapsedTime, bool isActive)
        {
            // TODO
            // 1. Update movement of paddles according to the.
            // 2. Update physics.

            // Update the players and opponents.
            foreach (PlayerAgent player in _team1)
            {
                player.Update();
            }
            foreach (PlayerAgent opponent in _team2)
            {
                opponent.Update();
            }

            // TODO Generate new powerups when appropriate.

            // Update powerups.
            foreach (Powerup powerup in _powerups)
            {
                powerup.Update();
            }

            // Remove expired powerups.
            for (int i = _powerups.Count; i > 0; --i)
            {
                if (_powerups[i].TTL == 0) _powerups.RemoveAt(i);
            }

            // TODO Call scene.Update() here.

            throw new NotImplementedException();
        }

        public LevelResult GetResult()
        {
            float borderOffsetTeam1 = -Tunnel.Length / 2;
            float borderOffsetTeam2 = Tunnel.Length / 2;

            // Determine if the ball has gone past the border of either side.

            if (_ball.Node.WorldTransformation.Translation.Y < borderOffsetTeam1)
            {
                return LevelResult.Team2;
            }
            else if (_ball.Node.WorldTransformation.Translation.Y > borderOffsetTeam2)
            {
                return LevelResult.Team1;
            }
            else
            {
                return LevelResult.Na;
            }
        }

        public TransformNode LevelRootNode
        {
            get
            {
                return _levelRootNode;
            }
        }

        public Tutorial16_Phone Container
        {
            get
            {
                return _container;
            }
        }
    }
}