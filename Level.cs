using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Media;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Color = Microsoft.Xna.Framework.Color;
using Matrix = Microsoft.Xna.Framework.Matrix;

using GoblinXNA;
using GoblinXNA.Graphics;
using GoblinXNA.SceneGraph;
using Model = GoblinXNA.Graphics.Model;
using GoblinXNA.Graphics.Geometry;
using GoblinXNA.Device.Generic;
using GoblinXNA.Device.Capture;
using GoblinXNA.Device.Vision;
using GoblinXNA.Device.Vision.Marker;
using GoblinXNA.Device.Util;
using GoblinXNA.Helpers;
using GoblinXNA.UI;
using GoblinXNA.UI.UI2D;

using Tutorial16___Multiple_Viewport;
using Curveball;
using Tutorial16___Multiple_Viewport___PhoneLib;

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
        public enum LevelResult
        {
            Team1, Team2, Na
        };

        private Tutorial16_Phone _container;
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
        private Tunnel _tunnel;

        // The ball. Contains its node and related data.
        private Ball _ball;
        TransformNode _transBallNode;

        // The powerups. Each of them contains its node and related data.
        private List<Powerup> _powerups;

        // The root node that contains everying for this level.
        // It should be attached to the root node of the scene
        // in a 'Tutorial16_Phone' instance (the container).
        private TransformNode _levelRootNode;

        // The node for the aircraft from the original tutorial. For testing only.
        private GeometryNode _overlayNode;

        public Level(Tutorial16_Phone container, LevelInfo info)
        {
            _container = container;
            _info = info;

            _team1 = new List<PlayerAgent>();
            _team2 = new List<PlayerAgent>();

            _ball = new Ball(this);
            _tunnel = new Tunnel(this);

            _powerups = new List<Powerup>();

            _levelRootNode = new TransformNode();

            // For testing, use:
            // CreateObjects();

            // After 'Initialize' has been implemented, use:
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
            AddPlayers(_info.Team1PlayerTypes, _team1, Role.Team1);

            // Team 2.
            _transTeam2Node = new TransformNode();
            _levelRootNode.AddChild(_transTeam2Node);
            _transTeam2Node.Translation = new Vector3(0.0f, Tunnel.Length / 2.0f, 0.0f);
            AddPlayers(_info.Team2PlayerTypes, _team2, Role.Team2);

            // For test purposes:
            // CreateObjects();
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
            _ball = new Ball(this);

            _transBallNode = new TransformNode();
            _transBallNode.AddChild(_ball.Node);
            _levelRootNode.AddChild(_transBallNode);
            // TODO Set the transformation of the ball.
            // TODO Set physics for the ball there.
        }

        private void AddTunnel()
        {
            // Add tunnel.
            _tunnel = new Tunnel(this);

            TransformNode transNode = new TransformNode();

            _levelRootNode.AddChild(_tunnel.Node);
            // TODO Set the transformation of the tunnel.
        }

        private void AddPlayers(List<PlayerAgentType> types, List<PlayerAgent> team, Role role)
        {
            foreach (PlayerAgentType type in types)
            {
                PlayerAgent player = null;
                switch (type)
                {
                    case PlayerAgentType.Ai:
                        player = new AiPlayer(this);
                        break;

                    case PlayerAgentType.Main:
                        player = new MainPlayer(this);
                        break;

                    case PlayerAgentType.Wall:
                        player = new WallPlayer(this);
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

        private void CreateObjects()
        {
            ModelLoader loader = new ModelLoader();

            _overlayNode = new GeometryNode("Overlay");
            _overlayNode.Model = (Model)loader.Load("", "p1_wedge");
            ((Model)_overlayNode.Model).UseInternalMaterials = true;

            // Get the dimension of the model
            Vector3 dimension = Vector3Helper.GetDimensions(_overlayNode.Model.MinimumBoundingBox);
            // Scale the model to fit to the size of 5 markers
            float scale = 100 / Math.Max(dimension.X, dimension.Z);

            _levelRootNode = new TransformNode()
            {
                Translation = new Vector3(0, 0, dimension.Y * scale / 2),
                Rotation = Quaternion.CreateFromAxisAngle(Vector3.UnitX, MathHelper.ToRadians(90)) *
                    Quaternion.CreateFromAxisAngle(Vector3.UnitY, MathHelper.ToRadians(90)),
                Scale = new Vector3(scale, scale, scale)
            };

            _levelRootNode.AddChild(_overlayNode);
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
        public void Update()
        {
            Vector3 trans = _transBallNode.Translation;
            _transBallNode.Translation = new Vector3(trans.X, trans.Y - 0.3f, trans.Z);

            return;

            switch (_info.CurrentRole)
            {
                case Role.Team1:
                    UpdateTeam1();
                    break;

                case Role.Team2:
                    UpdateTeam2();
                    break;

                case Role.Server:
                    UpdateServer();
                    break;

                default:
                    throw new NotSupportedException();
            }
        }

        public void UpdateTeam1()
        {
            // TODO Just transmit its ground array transformation to the server?
            throw new NotImplementedException();
        }

        public void UpdateTeam2()
        {
            // TODO Just transmit its ground array transformation to the server?
            throw new NotImplementedException();
        }

        public void UpdateServer()
        {
            // Update the players and opponents.
            foreach (PlayerAgent player in _team1)
            {
                player.Update();
            }
            foreach (PlayerAgent opponent in _team2)
            {
                opponent.Update();
            }

            // The update of the ball and the tunnel consists of
            // of two parts: physics and non-physics.
            // The former is handled by GoblinXNA, and the latter
            // will be handled by the update functions of
            // powerups that have effects on them.
            // So they are essentially inactive objects here.


            // TODO Generate new powerups.

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

            throw new NotImplementedException();
        }

        public LevelResult GetResult()
        {
            float borderOffsetTeam1 = -Tunnel.Length / 2;
            float borderOffsetTeam2 = Tunnel.Length / 2;

            // Determine if the ball has gone past the border of some side.

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
    }
}