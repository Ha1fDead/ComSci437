using Microsoft.Kinect;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace pong_proj
{
    class KinectControls
    {
        Game game;

        enum KinectDraw { Depth, Camera, None }

        KinectSensor kinectSensor;

        //Textures to store video data (color = straight video, depth = depth data)
        Texture2D colorVideo, depthVideo, skeletalVideo;

        List<ColoredPoint> skeletonPoints;

        Skeleton[] skeletons;
        Skeleton trackedSkeletonOne;
        Skeleton trackedSkeletonTwo;

        public bool IsTrackingOne
        {
            get
            {
                return (trackedSkeletonOne != null);
            }
        }

        public bool IsTrackingTwo
        {
            get
            {
                return (trackedSkeletonTwo != null);
            }
        }

        KinectDraw drawState = KinectDraw.None;

        bool drawSkeleton = false;

        public KinectControls(Game game)
        {
            this.game = game;

            skeletons = new Skeleton[6];
            skeletonPoints = new List<ColoredPoint>();

            kinectSensor = KinectSensor.KinectSensors[0];
            if(kinectSensor.IsRunning)
            {
                kinectSensor.Stop();
            }

            //enable video data with color
            kinectSensor.ColorStream.Enable();

            //enable depth data
            kinectSensor.DepthStream.Enable();

            //enable skeletal data
            kinectSensor.SkeletonStream.Enable();

            if (!this.kinectSensor.SkeletonStream.AppChoosesSkeletons)
            {
                this.kinectSensor.SkeletonStream.AppChoosesSkeletons = true; // Ensure AppChoosesSkeletons is set
            }

            //event handler that fires when the kinect sensor has new color frames ready
            //kinectSensor.ColorFrameReady += new EventHandler<ColorImageFrameReadyEventArgs>(kinect_ColorFrameReady);
            kinectSensor.AllFramesReady += new EventHandler<AllFramesReadyEventArgs>(kinect_AllFramesReady);

            //initialize the textures
            colorVideo = new Texture2D(game.GraphicsDevice, kinectSensor.ColorStream.FrameWidth, kinectSensor.ColorStream.FrameHeight);
            depthVideo = new Texture2D(game.GraphicsDevice, kinectSensor.DepthStream.FrameWidth, kinectSensor.DepthStream.FrameHeight);

            kinectSensor.Start();

            kinectSensor.ElevationAngle = 15;
        }

        public void Unload()
        {
            kinectSensor.Stop();
        }

        private void TrackClosestSkeleton()
        {
            if (this.kinectSensor != null && this.kinectSensor.SkeletonStream != null && this.skeletons != null)
            {
                float closestDistance = 10000f; // Start with a far enough distance
                int closestId = 0;
                int secondClosestId = 0;

                foreach (Skeleton skeleton in this.skeletons.Where(s => s.TrackingState != SkeletonTrackingState.NotTracked))
                {
                    if (skeleton.Position.Z < closestDistance)
                    {
                        trackedSkeletonTwo = trackedSkeletonOne;
                        trackedSkeletonOne = skeleton;

                        secondClosestId = closestId;
                        closestId = skeleton.TrackingId;
                        closestDistance = skeleton.Position.Z;
                    }
                }

                if (closestId > 0)
                {
                    if(secondClosestId > 0)
                    {
                        this.kinectSensor.SkeletonStream.ChooseSkeletons(closestId, secondClosestId); // Track the two closest skeletons
                    }
                    else
                    {
                        this.kinectSensor.SkeletonStream.ChooseSkeletons(closestId); // only one close skeleton was found
                    }
                }
                else
                {
                    this.kinectSensor.SkeletonStream.ChooseSkeletons(); // no valid skeletons were found
                }
            }
        }


        void kinect_AllFramesReady(object sender, AllFramesReadyEventArgs e)
        {
            // Color
            using (var frame = e.OpenColorImageFrame())
            {
                //Get raw image
                if (frame != null)
                {
                    //Create array for pixel data and copy it from the image frame
                    Byte[] pixelData = new Byte[frame.PixelDataLength];
                    frame.CopyPixelDataTo(pixelData);

                    //Convert RGBA to BGRA
                    Byte[] bgraPixelData = new Byte[frame.PixelDataLength];
                    for (int i = 0; i < pixelData.Length; i += 4)
                    {
                        bgraPixelData[i] = pixelData[i + 2];
                        bgraPixelData[i + 1] = pixelData[i + 1];
                        bgraPixelData[i + 2] = pixelData[i];
                        bgraPixelData[i + 3] = (Byte)255; //The video comes with 0 alpha so it is transparent
                    }

                    // Create a texture and assign the realigned pixels
                    colorVideo = new Texture2D(game.GraphicsDevice, frame.Width, frame.Height);
                    colorVideo.SetData(bgraPixelData);
                }
            }

            // Depth
            using (var frame = e.OpenDepthImageFrame())
            {
                if (frame != null)
                {
                    short[] pixelData = new short[frame.PixelDataLength];
                    pixelData = new short[frame.PixelDataLength];
                    frame.CopyPixelDataTo(pixelData);

                    depthVideo = new Texture2D(game.GraphicsDevice, frame.Width, frame.Height);

                    depthVideo.SetData(ConvertDepthFrame(pixelData, kinectSensor.DepthStream));
                }
            }

            using (var frame = e.OpenSkeletonFrame())
            {
                if (frame != null)
                {
                    skeletonPoints = new List<ColoredPoint>();
                    frame.CopySkeletonDataTo(this.skeletons);

                    //refresh the closest skeletons
                    this.TrackClosestSkeleton();

                    foreach (var body in skeletons)
                    {
                        if (body != null && body.TrackingState == SkeletonTrackingState.Tracked)
                        {
                            foreach (Joint joint in body.Joints)
                            {
                                SkeletonPoint skeletonPoint = joint.Position;

                                // 2D coordinates in pixels
                                Point point = new Point();

                                if (drawState == KinectDraw.Camera)
                                {
                                    // Skeleton-to-Color mapping
                                    ColorImagePoint colorPoint = kinectSensor.CoordinateMapper.MapSkeletonPointToColorPoint(skeletonPoint, ColorImageFormat.RgbResolution640x480Fps30);

                                    point.X = colorPoint.X;
                                    point.Y = colorPoint.Y;
                                }

                                if (drawState == KinectDraw.Depth) // Remember to change the Image and Canvas size to 320x240.
                                {
                                    // Skeleton-to-Depth mapping
                                    DepthImagePoint depthPoint = kinectSensor.CoordinateMapper.MapSkeletonPointToDepthPoint(skeletonPoint, DepthImageFormat.Resolution640x480Fps30);

                                    point.X = depthPoint.X;
                                    point.Y = depthPoint.Y;
                                }

                                Color colorToAdd;

                                if(trackedSkeletonOne != null)
                                {
                                    if (body.TrackingId == trackedSkeletonOne.TrackingId)
                                    {
                                        colorToAdd = Color.Red;
                                    }
                                    else if(body.TrackingId == trackedSkeletonTwo.TrackingId)
                                    {
                                        colorToAdd = Color.Blue;
                                    }
                                    else
                                    {
                                        //something obnoxious
                                        colorToAdd = Color.HotPink;
                                    }
                                }
                                else
                                {
                                    //something obnoxious
                                    colorToAdd = Color.HotPink;
                                }

                                skeletonPoints.Add(new ColoredPoint(point, colorToAdd));

                            }
                        }
                    }
                }
            }
        }

        public bool IsLeftMoveUp(PlayerIndex playerIndex)
        {
            Skeleton currentSkeleton;
            if(playerIndex == PlayerIndex.One)
            {
                currentSkeleton = trackedSkeletonOne;
            }
            else
            {
                currentSkeleton = trackedSkeletonTwo;
            }

            Joint leftHand = currentSkeleton.Joints.First(x => x.JointType == JointType.HandLeft);

            if(leftHand.Position.Y < 0.0)
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        public bool IsRightMoveUp(PlayerIndex playerIndex)
        {
            Skeleton currentSkeleton;
            if (playerIndex == PlayerIndex.One)
            {
                if(!this.IsTrackingOne)
                {
                    return false;
                }

                currentSkeleton = trackedSkeletonOne;
            }
            else
            {
                if(!this.IsTrackingTwo)
                {
                    return false;
                }

                currentSkeleton = trackedSkeletonTwo;
            }

            Joint rightHand = currentSkeleton.Joints.First(x => x.JointType == JointType.HandRight);

            if (rightHand.Position.Y < 0.0)
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        private byte[] ConvertDepthFrame(short[] depthFrame, DepthImageStream depthStream)
        {
            int RedIndex = 0, GreenIndex = 1, BlueIndex = 2, AlphaIndex = 3;

            byte[] depthFrame32 = new byte[depthStream.FrameWidth * depthStream.FrameHeight * 4];

            for (int i16 = 0, i32 = 0; i16 < depthFrame.Length && i32 < depthFrame32.Length; i16++, i32 += 4)
            {
                int player = depthFrame[i16] & DepthImageFrame.PlayerIndexBitmask;
                int realDepth = depthFrame[i16] >> DepthImageFrame.PlayerIndexBitmaskWidth;

                // transform 13-bit depth information into an 8-bit intensity appropriate
                // for display (we disregard information in most significant bit)
                byte intensity = (byte)(~(realDepth >> 4));

                depthFrame32[i32 + RedIndex] = (byte)(intensity);
                depthFrame32[i32 + GreenIndex] = (byte)(intensity);
                depthFrame32[i32 + BlueIndex] = (byte)(intensity);
                depthFrame32[i32 + AlphaIndex] = 125;
            }
            return depthFrame32;
        }


        public void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).IsButtonDown(Buttons.Y))
            {
                this.drawState = KinectDraw.None;
            }
            if (GamePad.GetState(PlayerIndex.One).IsButtonDown(Buttons.X))
            {
                this.drawSkeleton = true;
            }
            if (GamePad.GetState(PlayerIndex.One).IsButtonDown(Buttons.B))
            {
                this.drawState = KinectDraw.Depth;
            }
            if (GamePad.GetState(PlayerIndex.One).IsButtonDown(Buttons.A))
            {
                this.drawState = KinectDraw.Camera;
            }
        }

        public void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {

            Texture2D drawTex;

            if (drawState == KinectDraw.Camera)
            {
                drawTex = colorVideo;
            }
            else if (drawState == KinectDraw.Depth)
            {
                drawTex = depthVideo;
            }
            else
            {
                return;
            }

            spriteBatch.Draw(drawTex, new Rectangle(0, 0, 640, 480), Color.White);

            if(drawSkeleton)
            {
                foreach (var coloredPoint in skeletonPoints)
                {
                    spriteBatch.Draw(drawTex, new Rectangle(coloredPoint.Point.X, coloredPoint.Point.Y, 10, 10), coloredPoint.Color);
                }
            }
        }

        protected class ColoredPoint
        {
            public Point Point { get; internal set; }
            public Color Color { get; internal set; }

            public ColoredPoint(Point point, Color color)
            {
                this.Point = point;
                this.Color = color;
            }
        }
    }
}
