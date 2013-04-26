
using System;
using Microsoft.Xna.Framework;
using GoblinXNA.Network;
using GoblinXNA.Helpers;

namespace Tutorial8___Optical_Marker_Tracking___PhoneLib
{
    class NetworkExchangeWrapper : INetworkObject
    {

        //public delegate void ShootFunction(Vector3 paddle, Vector3 wall);
        public delegate void ShootFunction(Matrix m);

        #region Member Fields

        private bool readyToSend;
        private bool hold;
        private int sendFrequencyInHertz;

        private bool reliable;
        private bool inOrder;


        private Vector3 paddleCoord;
        private Vector3 wallCoord;
        private ShootFunction callbackFunc;
        private Matrix globalTransMat;

        #endregion

        #region Constructors

        public NetworkExchangeWrapper()
        {
            readyToSend = false;
            hold = false;
            sendFrequencyInHertz = 0;

            reliable = true;
            inOrder = true;
        }

        #endregion

        #region Properties
        public String Identifier
        {
            get { return "NetworkExchangeWrapper"; }
        }

        public bool ReadyToSend
        {
            get { return readyToSend; }
            set { readyToSend = value; }
        }

        public bool Hold
        {
            get { return hold; }
            set { hold = value; }
        }

        public int SendFrequencyInHertz
        {
            get { return sendFrequencyInHertz; }
            set { sendFrequencyInHertz = value; }
        }

        public bool Reliable
        {
            get { return reliable; }
            set { reliable = value; }
        }

        public bool Ordered
        {
            get { return inOrder; }
            set { inOrder = value; }
        }

        public ShootFunction CallbackFunc
        {
            get { return callbackFunc; }
            set { callbackFunc = value; }
        }

        public Vector3 PaddleCord
        {
            get { return paddleCoord; }
            set { paddleCoord = value; }
        }

        public Vector3 WallCord
        {
            get { return wallCoord; }
            set { wallCoord = value; }
        }

        public Matrix GlobalTransMat
        {
            get { return globalTransMat; }
            set { globalTransMat = value; }
        }
        #endregion

        #region Public Methods
        public byte[] GetMessage()
        {
            // 1 byte: pressedButton
            // 12 bytes: near point (3 floats)
            // 12 bytes: far point (3 floats)
            /*byte[] data = new byte[1 + 12 + 12];

            data[0] = (byte)pressedButton;
            ByteHelper.FillByteArray(ref data, 1, BitConverter.GetBytes(paddleCoord.X));
            ByteHelper.FillByteArray(ref data, 5, BitConverter.GetBytes(paddleCoord.Y));
            ByteHelper.FillByteArray(ref data, 9, BitConverter.GetBytes(paddleCoord.Z));
            ByteHelper.FillByteArray(ref data, 13, BitConverter.GetBytes(wallCoord.X));
            ByteHelper.FillByteArray(ref data, 17, BitConverter.GetBytes(wallCoord.Y));
            ByteHelper.FillByteArray(ref data, 21, BitConverter.GetBytes(wallCoord.Z));*/

            byte[] data = new byte[28];
            ByteHelper.FillByteArray(ref data, 0, MatrixHelper.ConvertToOptimizedBytes(globalTransMat));

            return data;
        }

        public void InterpretMessage(byte[] msg, int startIndex, int length)
        {
            /*pressedButton = (int)msg[startIndex];

            paddleCoord.X = ByteHelper.ConvertToFloat(msg, startIndex + 1);
            paddleCoord.Y = ByteHelper.ConvertToFloat(msg, startIndex + 5);
            paddleCoord.Z = ByteHelper.ConvertToFloat(msg, startIndex + 9);

            wallCoord.X = ByteHelper.ConvertToFloat(msg, startIndex + 13);
            wallCoord.Y = ByteHelper.ConvertToFloat(msg, startIndex + 17);
            wallCoord.Z = ByteHelper.ConvertToFloat(msg, startIndex + 21);

            */

            globalTransMat = MatrixHelper.ConvertFromOptimizedBytes(msg, startIndex, length);

            if (callbackFunc != null)
                callbackFunc(globalTransMat);
        }

        #endregion
    }
}
