/************************************************************************************ 


 *************************************************************************************/

#define USE_SOCKET_NETWORK

using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

using GoblinXNA;
using GoblinXNA.SceneGraph;
using GoblinXNA.Helpers;
using GoblinXNA.Graphics;
using GoblinXNA.Graphics.Geometry;
using GoblinXNA.Device.Generic;
using GoblinXNA.Device;
using Tutorial10;
using Model = GoblinXNA.Graphics.Model;
using GoblinXNA.Network;
using GoblinXNA.Physics;
using GoblinXNA.UI;
#if WINDOWS
using GoblinXNA.Physics.Newton1;
#else
using GoblinXNA.Physics.Matali;
#endif

namespace Tutorial10___Networking
{
 
    public class Tutorial10 : Microsoft.Xna.Framework.Game
    {
        GraphicsDeviceManager graphics;

        // A Goblin XNA scene graph
        Scene scene;

        // A network object which transmits information

        private NetworkExchangeWrapper networkExchange;
        private SynchronizedGeometryNode paddleNode;
        private SynchronizedGeometryNode ball;

        // Indicates whether this is a server
        bool isServer;

        //Scene Object variables
        private float SPHERE_RADIUS;
        private float WALL_WIDTH;
        private float WALL_HEIGHT;
        private float WALL_DEPTH;
        private float PADDLE_HEIGHT;
        private float PADDLE_WIDTH;
        private float PADDLE_DEPTH;

        public Tutorial10()
            : this(false)
        {
        }

        public Tutorial10(bool isServer)
        {
            this.isServer = isServer;
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";


            SPHERE_RADIUS = 3;
            WALL_WIDTH = 35;
            PADDLE_WIDTH = 12;
            WALL_HEIGHT = 25;
            PADDLE_HEIGHT = 18;
            WALL_DEPTH = 5;
            PADDLE_DEPTH = 2;

#if WINDOWS_PHONE
            // Extend battery life under lock.
            InactiveSleepTime = TimeSpan.FromSeconds(1);

            graphics.IsFullScreen = true;
#endif
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            base.Initialize();
#if WINDOWS
            this.IsMouseVisible = true;
#endif

            // Initialize the GoblinXNA framework
            State.InitGoblin(graphics, Content, "");

            // Initialize the scene graph
            scene = new Scene();

            State.EnableNetworking = true;
            State.IsServer = isServer;

            State.ShowNotifications = true;
            State.ShowFPS = true;
            Notifier.FadeOutTime = 2000;

            // Set up the lights used in the scene
            CreateLights();

            // Set up the camera, which defines the eye location and viewing frustum
            CreateCamera();

            // Create 3D objects
            CreateObject();

            // Create a network object that contains mouse press information to be
            // transmitted over network
           // mouseNetworkObj = new MouseNetworkObject();
            networkExchange = new NetworkExchangeWrapper();

            // When a mouse press event is sent from the other side, then call "ShootBox"
            // function
            //mouseNetworkObj.CallbackFunc = ShootBox;
            networkExchange.CallbackFunc = ShootBox;

            // Create a network handler for handling the network transfers
            INetworkHandler networkHandler = null;

#if USE_SOCKET_NETWORK || WINDOWS_PHONE
            networkHandler = new SocketNetworkHandler();
#else
            networkHandler = new NetworkHandler();
#endif

            if (true)//(State.IsServer)
            {
                IServer server = null;
#if WINDOWS
#if USE_SOCKET_NETWORK
                server = new SocketServer(14242);
#else
                server = new LidgrenServer("Tutorial8_Phone", 14242);
#endif
                State.NumberOfClientsToWait = 1;
                scene.PhysicsEngine = new NewtonPhysics();
#else

                scene.PhysicsEngine = new MataliPhysics();
                ((MataliPhysics)scene.PhysicsEngine).SimulationTimeStep = 1 / 30f;
#endif

                scene.PhysicsEngine.Gravity = 0;
                server.ClientConnected += new HandleClientConnection(ClientConnected);
                server.ClientDisconnected += new HandleClientDisconnection(ClientDisconnected);
                networkHandler.NetworkServer = server;
            }

            // Assign the network handler used for this scene
            scene.NetworkHandler = networkHandler;

            // Add the mouse network object to the scene graph, so it'll be sent over network
            // whenever ReadyToSend is set to true.
            scene.NetworkHandler.AddNetworkObject(networkExchange);

        }

        private void ClientDisconnected(string clientIP, int portNumber)
        {
            Notifier.AddMessage("Disconnected from " + clientIP + " at port " + portNumber);
        }

        private void ClientConnected(string clientIP, int portNumber)
        {
            Notifier.AddMessage("Accepted connection from " + clientIP + " at port " + portNumber);
        }

        private void CreateLights()
        {
            // Create a directional light source
            LightSource lightSource = new LightSource();
            lightSource.Direction = new Vector3(-1, -1, 0);
            lightSource.Diffuse = Color.White.ToVector4();
            lightSource.Specular = Color.White.ToVector4();

            // Create a light node to hold the light source
            LightNode lightNode = new LightNode();
            lightNode.AmbientLightColor = new Vector4(0.2f, 0.2f, 0.2f, 1);
            lightNode.LightSource = lightSource;

            // Add this light node to the root node
            scene.RootNode.AddChild(lightNode);
        }

        private void CreateCamera()
        {
            // Create a camera 
            Camera camera = new Camera();


            camera.Translation = new Vector3(10, 0, 250);//This near-up of Ball

            //Or the whole scene in one:

            //camera.Translation = new Vector3(300, 0, 0);
            //camera.Rotation = new Quaternion(Vector3.UnitY, MathHelper.Pi/2);



            // Set the vertical field of view to be 60 degrees
            camera.FieldOfViewY = MathHelper.ToRadians(60);
            // Set the near clipping plane to be 0.1f unit away from the camera
            camera.ZNearPlane = 0.1f;
            // Set the far clipping plane to be 1000 units away from the camera
            camera.ZFarPlane = 1000;

            // Now assign this camera to a camera node, and add this camera node to our scene graph
            CameraNode cameraNode = new CameraNode(camera);
            scene.RootNode.AddChild(cameraNode);

            // Assign the camera node to be our scene graph's current camera node
            scene.CameraNode = cameraNode;
        }

        private void CreateObject()
        {
            SynchronizedGeometryNode wall = new SynchronizedGeometryNode("wall");
            wall.Model = new Box(WALL_WIDTH, WALL_HEIGHT, WALL_DEPTH);
            wall.Model.ShowBoundingBox = true;

            Material wallMaterial = new Material();
            wallMaterial.Diffuse = Color.Gray.ToVector4();
            wallMaterial.Ambient = Color.Blue.ToVector4();
            wallMaterial.Emissive = Color.Green.ToVector4();
            wall.Material = wallMaterial;

            TransformNode wallTrans = new TransformNode();
            wallTrans.Translation = new Vector3(0,0,-20);
            wall.Physics.Collidable = true;
            wall.Physics.Interactable = true;
            wall.AddToPhysicsEngine = true;
            wall.Physics.Shape = GoblinXNA.Physics.ShapeType.Box;
            wall.Physics.Mass = 0;

            TransformNode groundMarkerNode = new TransformNode();
            scene.RootNode.AddChild(groundMarkerNode);
            groundMarkerNode.AddChild(wallTrans);
            wallTrans.AddChild(wall);

            /////////////////////////////////////////////////////////////////////////

            ball = new SynchronizedGeometryNode("ball");
            ball.Model = new Sphere(SPHERE_RADIUS, 20, 20);
            ball.Model.ShowBoundingBox = true;

            Material ballMaterial = new Material();
            ballMaterial.Diffuse = Color.Yellow.ToVector4();
            //ballMaterial.Ambient = Color.Maroon.ToVector4();
            //ballMaterial.Emissive = Color.LightCoral.ToVector4();
            ball.Material = ballMaterial;

            TransformNode ballTrans = new TransformNode();
            ballTrans.Translation = new Vector3(0,0,200);

            //ball.Physics = new PhysicsObject(ball);
            ball.Physics.Collidable = true;
            ball.Physics.Interactable = true;
            ball.Physics.AngularDamping = Vector3.Zero;
            ball.Physics.LinearDamping = 0f;
        
            ball.Physics.Shape = GoblinXNA.Physics.ShapeType.Sphere;
            ball.Physics.Mass = 10;
            ball.AddToPhysicsEngine = true;

            ballTrans.AddChild(ball);
            groundMarkerNode.AddChild(ballTrans);

            ////////////////////////////////////////////////////////////////////////////

            paddleNode = new SynchronizedGeometryNode("paddle");
            paddleNode.Model = new Box(PADDLE_HEIGHT, PADDLE_WIDTH, PADDLE_DEPTH);
            paddleNode.Model.ShowBoundingBox = true;

            Material paddleMaterial = new Material();
            paddleMaterial.Diffuse = Color.Orange.ToVector4();
            paddleMaterial.Specular = Color.White.ToVector4();
            paddleMaterial.SpecularPower = 10;
            paddleNode.Material = paddleMaterial;

            paddleNode.Physics.Collidable = true;
            paddleNode.Physics.Interactable = true;
            paddleNode.Physics.Shape = GoblinXNA.Physics.ShapeType.Box;
            paddleNode.Physics.Mass = 10;
            paddleNode.AddToPhysicsEngine = true;

            groundMarkerNode.AddChild(paddleNode);
        }

        /// <summary>
        /// Load your graphics content.  If loadAllContent is true, you should
        /// load content from both ResourceManagementMode pools.  Otherwise, just
        /// load ResourceManagementMode.Manual content.
        /// </summary>
        /// <param name="loadAllContent">Which type of content to load.</param>
        protected override void LoadContent()
        {
            base.LoadContent();
        }

        /// <summary>
        /// Unload your graphics content.  If unloadAllContent is true, you should
        /// unload content from both ResourceManagementMode pools.  Otherwise, just
        /// unload ResourceManagementMode.Manual content.  Manual content will get
        /// Disposed by the GraphicsDevice during a Reset.
        /// </summary>
        /// <param name="unloadAllContent">Which type of content to unload.</param>
        protected override void UnloadContent()
        {
            Content.Unload();
        }

#if !WINDOWS_PHONE
        protected override void Dispose(bool disposing)
        {
            scene.Dispose();
        }
#endif

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
#if WINDOWS_PHONE
            // Allows the game to exit
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed)
            {
                scene.Dispose();

                this.Exit();
            }
#endif

            scene.Update(gameTime.ElapsedGameTime, gameTime.IsRunningSlowly, this.IsActive);
        }

        /// <summary>
        /// Shoot a box from the clicked mouse location
        /// </summary>
        /// <param name="near"></param>
        /// <param name="far"></param>
        private void ShootBox(Matrix globalTransform)
        {
            //Notifier.AddMessage("paddle = " + paddle);
            //Notifier.AddMessage("wall = " + wall);

           //paddleNode.Physics.PhysicsWorldTransform = Matrix.Invert(Matrix.CreateTranslation(wall));
            
            //paddleNode.Physics.PhysicsWorldTransform = globalTransform.Translation;
            //Notifier.AddMessage("OBJECT WORLD TRANS = "+ globalTransform.Translation);

            paddleNode.Physics.PhysicsWorldTransform = Matrix.Invert(globalTransform);
            //Notifier.AddMessage("Translation = " + globalTransform.Translation);
            //Notifier.AddMessage("Paddle Pos  = " + paddleNode.WorldTransformation.Translation);

            Notifier.AddMessage("Paddle: " + paddleNode.WorldTransformation.Translation);
            Notifier.AddMessage("Ball:" + ball.WorldTransformation.Translation);

        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            scene.Draw(gameTime.ElapsedGameTime, gameTime.IsRunningSlowly);
        }
    }
}
