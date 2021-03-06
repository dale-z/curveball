/************************************************************************************ 
 * Copyright (c) 2008-2012, Columbia University
 * All rights reserved.
 *
 * Redistribution and use in source and binary forms, with or without
 * modification, are permitted provided that the following conditions are met:
 *     * Redistributions of source code must retain the above copyright
 *       notice, this list of conditions and the following disclaimer.
 *     * Redistributions in binary form must reproduce the above copyright
 *       notice, this list of conditions and the following disclaimer in the
 *       documentation and/or other materials provided with the distribution.
 *     * Neither the name of the Columbia University nor the
 *       names of its contributors may be used to endorse or promote products
 *       derived from this software without specific prior written permission.
 *
 * THIS SOFTWARE IS PROVIDED BY COLUMBIA UNIVERSITY ''AS IS'' AND ANY
 * EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED
 * WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE
 * DISCLAIMED. IN NO EVENT SHALL <copyright holder> BE LIABLE FOR ANY
 * DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES
 * (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES;
 * LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND
 * ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT
 * (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF THIS
 * SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.
 * 
 * 
 * ===================================================================================
 * Author: Ohan Oda (ohan@cs.columbia.edu)
 * 
 *************************************************************************************/

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
using GoblinXNA.Physics.Matali;
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
using Color = Microsoft.Xna.Framework.Color;
using Matrix = Microsoft.Xna.Framework.Matrix;
using Model = GoblinXNA.Graphics.Model;

namespace Tutorial16___Multiple_Viewport___PhoneLib
{
    /// <summary>
    /// This tutorial demonstrates how to create and render multiple viewports using Goblin XNA, one
    /// in AR mode, and another in VR mode.
    /// </summary>
    public class Tutorial16_Phone
    {
        Scene scene;
        MarkerNode groundMarkerNode;
        bool useStaticImage = false;
        bool useSingleMarker = false;
        bool betterFPS = false; // has trade-off of worse tracking if set to true

        Viewport viewport;

        GeometryNode markerBoardGeom;
        TransformNode overlayRoot;
        GeometryNode vrCameraRepNode;
        TransformNode vrCameraRepTransNode;

        CameraNode arCameraNode;
        CameraNode vrCameraNode;

        RenderTarget2D arViewRenderTarget;
        RenderTarget2D vrViewRenderTarget;
        Rectangle arViewRect;
        Rectangle vrViewRect;

        Texture2D videoTexture;

        public const float MarkerSize = 80.0f;

        // Curveball: The state of the game.
        enum GameState
        {
            Menu, Level
        }
        GameState _state;

        // The current level of the game. Its lifetime is shorter than a Tutorial16_Phone
        // instance holding it, since a Tutorial16_Phone instance contains resources that
        // should be shared across instances of 'Level', such as the ground array transformation.
        Level _level;

        public Tutorial16_Phone()
        {
            // no contents
        }

        public Texture2D VideoBackground
        {
            get { return videoTexture; }
            set { videoTexture = value; }
        }

        public void Initialize(IGraphicsDeviceService service, ContentManager content, VideoBrush videoBrush)
        {
            // Center the XNA view and set the XNA viewport size to be the size of the video resolution
            viewport = new Viewport(80, 0, 640, 480);
            viewport.MaxDepth = service.GraphicsDevice.Viewport.MaxDepth;
            viewport.MinDepth = service.GraphicsDevice.Viewport.MinDepth;
            service.GraphicsDevice.Viewport = viewport;

            // Initialize the GoblinXNA framework
            State.InitGoblin(service, content, "");

            // Initialize the scene graph
            scene = new Scene();

            // Set up cameras for both the AR and VR scenes
            CreateCameras();

            // Setup two viewports, one displasy the AR scene, the other displays the VR scene
            SetupViewport();

            // Set up optical marker tracking
            SetupMarkerTracking(videoBrush);

            // Create a geometry representing a camera in the VR scene
            CreateVirtualCameraRepresentation();

            // Create the ground that represents the physical ground marker array
            CreateMarkerBoard();

            // Create 3D objects
            InitializeOverlay();

            State.ShowFPS = true;

            scene.PhysicsEngine = new MataliPhysics();
            ((MataliPhysics)scene.PhysicsEngine).SimulationTimeStep = 1 / 30f;
            scene.PhysicsEngine.Gravity = 10;

            // Curveball: Initialize a test level here.

            // Create a 'LevelInfo' instance describing the level settings.
            LevelInfo info = new LevelInfo(Role.Server);
            info.Team1PlayerTypes.Add(PlayerAgentType.Ai);
            info.Team2PlayerTypes.Add(PlayerAgentType.Wall);
            info.Team2PlayerTypes.Add(PlayerAgentType.Main);

            // Create a level with the settings, and start it.
            StartLevel(new Level(this, info));
        }

        private void CreateCameras()
        {
            // Create a camera for VR scene 
            Camera vrCamera = new Camera();
            vrCamera.Translation = new Vector3(0, -280, 480);
            vrCamera.Rotation = Quaternion.CreateFromAxisAngle(Vector3.UnitX, MathHelper.ToRadians(45));
            vrCamera.FieldOfViewY = MathHelper.ToRadians(60);
            vrCamera.ZNearPlane = 1;
            vrCamera.ZFarPlane = 2000;

            vrCameraNode = new CameraNode(vrCamera);
            scene.RootNode.AddChild(vrCameraNode);

            // Create a camera for AR scene
            Camera arCamera = new Camera();
            arCamera.ZNearPlane = 1;
            arCamera.ZFarPlane = 2000;

            arCameraNode = new CameraNode(arCamera);
            scene.RootNode.AddChild(arCameraNode);

            // Set the AR camera to be the main camera so that at the time of setting up the marker tracker,
            // the marker tracker will assign the right projection matrix to this camera
            scene.CameraNode = arCameraNode;
        }

        private void SetupViewport()
        {
            PresentationParameters pp = State.Device.PresentationParameters;

            // Create a render target to render the AR scene to
            arViewRenderTarget = new RenderTarget2D(State.Device, viewport.Width, viewport.Height, false,
                SurfaceFormat.Color, pp.DepthStencilFormat);

            // Create a render target to render the VR scene to.
            vrViewRenderTarget = new RenderTarget2D(State.Device, viewport.Width * 2 / 5, viewport.Height * 2 / 5, false,
                SurfaceFormat.Color, pp.DepthStencilFormat);

            // Set the AR scene to take the full window size
            arViewRect = new Rectangle(0, 0, viewport.Width, viewport.Height);

            // Set the VR scene to take the 2 / 5 of the window size and positioned at the top right corner
            vrViewRect = new Rectangle(viewport.Width - vrViewRenderTarget.Width, 0,
                vrViewRenderTarget.Width, vrViewRenderTarget.Height);
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

#if USE_PATTERN_MARKER
            if(useSingleMarker)
                groundMarkerNode = new MarkerNode(scene.MarkerTracker, "patt.hiro", 16, 16, markerSize, 0.7f);
            else
                groundMarkerNode = new MarkerNode(scene.MarkerTracker, "NyARToolkitGroundArray.xml", 
                    NyARToolkitTracker.ComputationMethod.Average);
#else
            //groundMarkerNode = new MarkerNode(scene.MarkerTracker, "CurveballArray.xml",
            //    NyARToolkitTracker.ComputationMethod.Average);
            groundMarkerNode = new MarkerNode(scene.MarkerTracker, "NyARToolkitIDGroundArray.xml",
                NyARToolkitTracker.ComputationMethod.Average);
#endif
            scene.RootNode.AddChild(groundMarkerNode);
        }

        private void CreateMarkerBoard()
        {
            markerBoardGeom = new GeometryNode("MarkerBoard")
            {
                Model = new TexturedPlane(340, 200),
                Material =
                {
                    Diffuse = Color.White.ToVector4(),
                    Specular = Color.White.ToVector4(),
                    SpecularPower = 20,
                    Texture = State.Content.Load<Texture2D>("ALVARArray")
                }
            };

            // Rotate the marker board in the VR scene so that it appears Z-up
            TransformNode markerBoardTrans = new TransformNode()
            {
                Rotation = Quaternion.CreateFromAxisAngle(Vector3.UnitX, MathHelper.PiOver2)
            };

            scene.RootNode.AddChild(markerBoardTrans);
            markerBoardTrans.AddChild(markerBoardGeom);
        }

        private void CreateVirtualCameraRepresentation()
        {
            vrCameraRepNode = new GeometryNode("VR Camera")
            {
                Model = new Pyramid(MarkerSize * 4 / 3, MarkerSize, MarkerSize),
                Material =
                {
                    Diffuse = Color.Orange.ToVector4(),
                    Specular = Color.White.ToVector4(),
                    SpecularPower = 20
                }
            };

            vrCameraRepTransNode = new TransformNode();

            TransformNode camOffset = new TransformNode()
            {
                Rotation = Quaternion.CreateFromAxisAngle(Vector3.UnitX, MathHelper.PiOver2)
            };

            scene.RootNode.AddChild(vrCameraRepTransNode);
            vrCameraRepTransNode.AddChild(camOffset);
            camOffset.AddChild(vrCameraRepNode);
        }

        private void InitializeOverlay()
        {
            overlayRoot = new TransformNode();
            scene.RootNode.AddChild(overlayRoot);
        }

        public void Dispose()
        {
            scene.Dispose();
        }

        public void Update(TimeSpan elapsedTime, bool isActive)
        {
            // Curveball:

            // Decide whose logic to run.
            if (_state == GameState.Menu)
            {
                // Currently do nothing.
            }
            else
            {
                // Update non-physics logic.
                _level.Update(elapsedTime, isActive);

                // If a winner is born...
                if (_level.GetResult() != Level.LevelResult.Na)
                {
                    // ... Go back to the menu.
                    GoToMenu();
                }
            }

            // Update the other aspects of the scene as normal.
            scene.Update(elapsedTime, false, isActive);
        }

        public void Draw(TimeSpan elapsedTime)
        {
            // Reset the XNA viewport to our centered and resized viewport
            State.Device.Viewport = viewport;

            // Set the render target for rendering the AR scene
            scene.SceneRenderTarget = arViewRenderTarget;
            scene.BackgroundColor = Color.Black;
            // Set the scene background size to be the size of the AR scene viewport
            scene.BackgroundBound = arViewRect;
            // Set the camera to be the AR camera
            scene.CameraNode = arCameraNode;
            // Associate the overlaid model with the ground marker for rendering it in AR scene
            scene.RootNode.RemoveChild(overlayRoot);
            groundMarkerNode.AddChild(overlayRoot);
            // Don't render the marker board and camera representation
            markerBoardGeom.Enabled = false;
            vrCameraRepNode.Enabled = false;
            // Show the video background
            scene.BackgroundTexture = videoTexture;
            // Render the AR scene
            scene.Draw(elapsedTime, false);

            // Set the render target for rendering the VR scene
            scene.SceneRenderTarget = vrViewRenderTarget;
            scene.BackgroundColor = Color.CornflowerBlue;
            // Set the scene background size to be the size of the VR scene viewport
            scene.BackgroundBound = vrViewRect;
            // Set the camera to be the VR camera
            scene.CameraNode = vrCameraNode;
            // Remove the overlaid model from the ground marker for rendering it in VR scene
            groundMarkerNode.RemoveChild(overlayRoot);
            scene.RootNode.AddChild(overlayRoot);
            // Render the marker board and camera representation in VR scene
            markerBoardGeom.Enabled = true;
            vrCameraRepNode.Enabled = true;
            // Update the transformation of the camera representation in VR scene based on the
            // marker array transformation
            if (groundMarkerNode.MarkerFound)
                vrCameraRepTransNode.WorldTransformation = Matrix.Invert(groundMarkerNode.WorldTransformation);
            // Do not show the video background
            scene.BackgroundTexture = null;
            // Re-traverse the scene graph since we have modified it, and render the VR scene 
            scene.RenderScene(false, true);

            // Adjust the viewport to be centered
            arViewRect.X += viewport.X;
            vrViewRect.X += viewport.X;

            // Set the render target back to the frame buffer
            State.Device.SetRenderTarget(null);
            State.Device.Clear(Color.Black);
            // Render the two textures rendered on the render targets
            State.SharedSpriteBatch.Begin(SpriteSortMode.Immediate, BlendState.Opaque);
            State.SharedSpriteBatch.Draw(arViewRenderTarget, arViewRect, Color.White);
            State.SharedSpriteBatch.Draw(vrViewRenderTarget, vrViewRect, Color.White);
            State.SharedSpriteBatch.End();

            // Reset the adjustments
            arViewRect.X -= viewport.X;
            vrViewRect.X -= viewport.X;
        }

        // Curveball:
        public void GoToMenu()
        {
            // This line crashes GoblinXNA. Waiting for the bug to be resolved.
            // _level.Unmount();

            // Set the state to indicate the menu is running.
            _state = GameState.Menu;
        }

        // Curveball:
        public void StartLevel(Level level)
        {
            // Mount the root node of the level to a subnode of the scene.
            _level = level;
            _level.Mount();

            // Set the state to indicate a level is running.
            _state = GameState.Level;
        }

        public Scene Scene
        {
            get
            {
                return scene;
            }
        }

        public TransformNode OverlayRoot
        {
            get
            {
                return overlayRoot;
            }
        }
    }
}
