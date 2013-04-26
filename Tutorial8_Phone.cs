/************************************************************************************ 
 
 
 *************************************************************************************/

// Uncomment this line if you want to use the pattern-based marker tracking
//#define USE_PATTERN_MARKER

using System;
using System.Collections.Generic;
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
using GoblinXNA.Network;
using GoblinXNA.UI.UI2D;
#if WINDOWS
using GoblinXNA.Physics.Newton1;
#else
//using GoblinXNA.Physics.Matali;
#endif

namespace Tutorial8___Optical_Marker_Tracking___PhoneLib
{
    public class Tutorial8_Phone
    {
        SpriteFont sampleFont;
        Scene scene;
        MarkerNode groundMarkerNode;
        bool useStaticImage = false;
        bool useSingleMarker = false;
        bool betterFPS = true; // has trade-off of worse tracking if set to true

        Viewport viewport;//

        private NetworkExchangeWrapper networkExchange;
        private SynchronizedGeometryNode paddleNode;

        //Scene Object variables
        private float SPHERE_RADIUS;
        private float WALL_WIDTH;
        private float WALL_HEIGHT;
        private float WALL_DEPTH;
        private float PADDLE_HEIGHT;
        private float PADDLE_WIDTH;
        private float PADDLE_DEPTH;
        private String IP_ADDRESS;


#if USE_PATTERN_MARKER
        float markerSize = 32.4f;
#else
        float markerSize = 57f;
#endif

        public Tutorial8_Phone()
        {
            // no contents

            SPHERE_RADIUS = 3;
            WALL_WIDTH = 35;
            PADDLE_WIDTH = 12;
            WALL_HEIGHT = 25;
            PADDLE_HEIGHT = 18;
            WALL_DEPTH = 5;
            PADDLE_DEPTH = 2;
            IP_ADDRESS = "207.10.141.174";

        }

        public Texture2D VideoBackground
        {
            get { return scene.BackgroundTexture; }
            set { scene.BackgroundTexture = value; }
        }

        public void Initialize(IGraphicsDeviceService service, ContentManager content, VideoBrush videoBrush)
        {
            viewport = new Viewport(80, 0, 640, 480);
            viewport.MaxDepth = service.GraphicsDevice.Viewport.MaxDepth;
            viewport.MinDepth = service.GraphicsDevice.Viewport.MinDepth;
            service.GraphicsDevice.Viewport = viewport;

            // Initialize the GoblinXNA framework
            State.InitGoblin(service, content, "");

            LoadContent(content);

            //State.ThreadOption = (ushort)ThreadOptions.MarkerTracking;

            // Initialize the scene graph
            scene = new Scene();
            scene.BackgroundColor = Color.Black;

            State.EnableNetworking = true;
            State.IsServer = false;

            // Set up the lights used in the scene
            CreateLights();

            CreateCamera();

            SetupMarkerTracking(videoBrush);

            CreateObjects();

            State.ShowNotifications = true;
            Notifier.Font = sampleFont;
            Notifier.FadeOutTime = 2000;
            State.ShowFPS = true;

            ///////////////////////
       
            networkExchange = new NetworkExchangeWrapper();
       
            networkExchange.CallbackFunc = ShootBox;

            // Create a network handler for handling the network transfers
            INetworkHandler networkHandler = null;

            IClient client = new SocketClient(IP_ADDRESS, 14242);

                // If the server is not running when client is started, then wait for the
                // server to start up.
                client.WaitForServer = true;
                client.ConnectionTrialTimeOut = 60 * 1000; // 1 minute timeout

                client.ServerConnected += new HandleServerConnection(ServerConnected);
                client.ServerDisconnected += new HandleServerDisconnection(ServerDisconnected);

                networkHandler = new SocketNetworkHandler();
                networkHandler.NetworkClient = client;
            
            // Assign the network handler used for this scene
            scene.NetworkHandler = networkHandler;

            scene.NetworkHandler.AddNetworkObject(networkExchange);

        }

        private void ServerDisconnected()
        {
            Notifier.AddMessage("Disconnected from the server");
        }

        private void ServerConnected()
        {
            Notifier.AddMessage("Connected to the server");
        }

        private void CreateCamera()
        {
            // Create a camera 
            Camera camera = new Camera();
            // Put the camera at the origin
            camera.Translation = new Vector3(0, 0, 0);
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

        private void CreateLights()
        {
            // Create a directional light source
            LightSource lightSource = new LightSource();
            lightSource.Direction = new Vector3(1, -1, -1);
            lightSource.Diffuse = Color.White.ToVector4();
            lightSource.Specular = new Vector4(0.6f, 0.6f, 0.6f, 1);

            // Create a light node to hold the light source
            LightNode lightNode = new LightNode();
            lightNode.AmbientLightColor = new Vector4(0.2f, 0.2f, 0.2f, 1);
            lightNode.LightSource = lightSource;

            scene.RootNode.AddChild(lightNode);
        }

        private void SetupMarkerTracking(VideoBrush videoBrush)
        {
            IVideoCapture captureDevice = null;

            if (useStaticImage)
            {
                captureDevice = new NullCapture();
                captureDevice.InitVideoCapture(0, FrameRate._30Hz, Resolution._320x240,
                    ImageFormat.B8G8R8A8_32, false);
                if (useSingleMarker)
                    ((NullCapture)captureDevice).StaticImageFile = "MarkerImageHiro.jpg";
                else
                    ((NullCapture)captureDevice).StaticImageFile = "MarkerImage_320x240";

                scene.ShowCameraImage = true;
            }
            else
            {
                captureDevice = new PhoneCameraCapture(videoBrush);
                captureDevice.InitVideoCapture(0, FrameRate._30Hz, Resolution._640x480,
                    ImageFormat.B8G8R8A8_32, false);
                ((PhoneCameraCapture)captureDevice).UseLuminance = true;

                if (betterFPS)
                    captureDevice.MarkerTrackingImageResizer = new HalfResizer();
            }

            // Add this video capture device to the scene so that it can be used for
            // the marker tracker
            scene.AddVideoCaptureDevice(captureDevice);

#if USE_PATTERN_MARKER
            NyARToolkitTracker tracker = new NyARToolkitTracker();
#else
            NyARToolkitIdTracker tracker = new NyARToolkitIdTracker();
#endif

            if (captureDevice.MarkerTrackingImageResizer != null)
                tracker.InitTracker((int)(captureDevice.Width * captureDevice.MarkerTrackingImageResizer.ScalingFactor),
                    (int)(captureDevice.Height * captureDevice.MarkerTrackingImageResizer.ScalingFactor),
                    "camera_para.dat");
            else
                tracker.InitTracker(captureDevice.Width, captureDevice.Height, "camera_para.dat");

            // Set the marker tracker to use for our scene
            scene.MarkerTracker = tracker;
        }

        private void CreateObjects()
        {
            // Create a marker node to track a ground marker array.
#if USE_PATTERN_MARKER
            if(useSingleMarker)
                groundMarkerNode = new MarkerNode(scene.MarkerTracker, "patt.hiro", 16, 16, markerSize, 0.7f);
            else
                groundMarkerNode = new MarkerNode(scene.MarkerTracker, "NyARToolkitGroundArray.xml", 
                    NyARToolkitTracker.ComputationMethod.Average);
#else
            groundMarkerNode = new MarkerNode(scene.MarkerTracker, "NyARToolkitIDGroundArray.xml",
                NyARToolkitTracker.ComputationMethod.Average);
#endif
            scene.RootNode.AddChild(groundMarkerNode);

            //toolbarMarkerNode = new MarkerNode(scene.MarkerTracker, "NyARToolkitIDToolbar1.xml",
              //  NyARToolkitTracker.ComputationMethod.Average);
            //scene.RootNode.AddChild(toolbarMarkerNode);

            SynchronizedGeometryNode wall = new SynchronizedGeometryNode("wall");
            wall.Model = new Box(WALL_WIDTH, WALL_HEIGHT, WALL_DEPTH);
            wall.Model.ShowBoundingBox = true;

            Material wallMaterial = new Material();
            wallMaterial.Diffuse = Color.Gray.ToVector4();
            wallMaterial.Ambient = Color.Blue.ToVector4();
            wallMaterial.Emissive = Color.Green.ToVector4();
            wall.Material = wallMaterial;

            TransformNode wallTrans = new TransformNode();
            wallTrans.Translation = new Vector3(0, 0, -20);
            wall.Physics.Collidable = true;
            wall.Physics.Interactable = true;
            wall.AddToPhysicsEngine = true;
            wall.Physics.Shape = GoblinXNA.Physics.ShapeType.Box;
            wall.Physics.Mass = 0;

            //scene.RootNode.AddChild(wall);
            groundMarkerNode.AddChild(wallTrans);
            wallTrans.AddChild(wall);
            /////////////////////////////////////////////////////
            

            SynchronizedGeometryNode ball = new SynchronizedGeometryNode("ball");
            ball.Model = new Sphere(SPHERE_RADIUS, 20, 20);
            ball.Model.ShowBoundingBox = true;

            Material ballMaterial = new Material();
            ballMaterial.Diffuse = Color.Yellow.ToVector4();
            //ballMaterial.Ambient = Color.Maroon.ToVector4();
            //ballMaterial.Emissive = Color.LightCoral.ToVector4();
            ball.Material = ballMaterial;

            TransformNode ballTrans = new TransformNode();
            ballTrans.Translation = new Vector3(0, 0, 200);

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

            /////////////////////////////////////////////////////////////////

            paddleNode = new SynchronizedGeometryNode("paddle");
            paddleNode.Model = new Box(PADDLE_HEIGHT,PADDLE_WIDTH,PADDLE_DEPTH);

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

        private void LoadContent(ContentManager content)
        {
            sampleFont = content.Load<SpriteFont>("Sample");
        }

        public void Dispose()
        {
            scene.Dispose();
        }

        public void Update(TimeSpan elapsedTime, bool isActive)
        {
            scene.Update(elapsedTime, false, isActive);
        }

        public void Draw(TimeSpan elapsedTime)
        {
            State.Device.Viewport = viewport;

            if (groundMarkerNode.MarkerFound)
            {
          // //  //   //paddleNode.Physics.PhysicsWorldTransform = Matrix.CreateTranslation(0, 0, -50) * Matrix.Invert(groundMarkerNode.WorldTransformation);

                networkExchange.ReadyToSend = true;
                //networkExchange.PressedButton = 0;
                //networkExchange.PaddleCord = new Vector3(-3, 0, 0); //paddleNode.Physics.PhysicsWorldTransform.Translation;
                //networkExchange.WallCord = groundMarkerNode.WorldTransformation.Translation;
                networkExchange.GlobalTransMat = groundMarkerNode.WorldTransformation;

            }

            scene.Draw(elapsedTime, false);
        }

        private void ShootBox(Matrix matrix)
        {
            //Console.Out.WriteLine("X: " + matrix.Translation.X);
            //Console.Out.WriteLine("paddle = " + paddle);
            //Console.Out.WriteLine("wall = " + wall);

        }


    }
}
