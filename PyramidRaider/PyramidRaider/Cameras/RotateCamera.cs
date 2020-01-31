using Microsoft.Xna.Framework;
using System;
using PyramidRaider;
#if WINDOWS_PHONE || ANDROID || WINDOWS_APP
using Microsoft.Xna.Framework.Input.Touch;
#endif

namespace Cameras
{
    enum CameraDirection
    {
        North = 0,
        East = 1,
        South = 2,
        West = 3
    }

    enum CameraState { Stand, Clockwise, CounterClockwise }

    class RotateCamera : Camera
    {
        private double _radius;
        private Vector3 _position;
        private Vector3 _target;
        private int _targetAngle;

        public CameraDirection Direction { get; private set; }
        public CameraState State { get; private set; }
        public int Angle { get; set; }
        public int[] DirectionMap { get; private set; }

        public RotateCamera(int halfMazeSize, float aspectRatio)
            : base(aspectRatio)
        {
            Direction = CameraDirection.North;
            ResetCamera(halfMazeSize);
            DirectionMap = new int[4];
            buildDirectionMap();
        }

        public void ResetCamera(int halfMazeSize)
        {
            switch (Direction)
            {
                case CameraDirection.North:
                    _position = new Vector3(-halfMazeSize * 1.3f, halfMazeSize * 3f, halfMazeSize * 3.3f);
                    Angle = 225;
                    break;

                case CameraDirection.East:
                    _position = new Vector3(-halfMazeSize * 1.3f, halfMazeSize * 3f, -halfMazeSize * 1.3f);
                    Angle = 135;
                    break;

                case CameraDirection.South:
                    _position = new Vector3(halfMazeSize * 3.3f, halfMazeSize * 3f, -halfMazeSize * 1.3f);
                    Angle = 45;
                    break;

                case CameraDirection.West:
                    _position = new Vector3(halfMazeSize * 3.3f, halfMazeSize * 3f, halfMazeSize * 3.3f);
                    Angle = 315;
                    break;
            }
            _target = new Vector3(halfMazeSize, halfMazeSize * -0.2f, halfMazeSize);
            _radius = Math.Sqrt(Math.Pow(_position.X - _target.X, 2) + Math.Pow(_position.Z - _target.Z, 2));
            View = Matrix.CreateLookAt(_position, _target, Vector3.Up);
            State = CameraState.Stand;
        }

        public override void Update()
        {
            switch (State)
            {
                case CameraState.Clockwise:
                    if (Angle > _targetAngle) Angle -= 5;
                    else
                    {
                        Angle = _targetAngle;
                        State = CameraState.Stand;
                    }
                    break;

                case CameraState.CounterClockwise:
                    if (Angle < _targetAngle) Angle += 5;
                    else
                    {
                        Angle = _targetAngle;
                        State = CameraState.Stand;
                    }
                    break;

                case CameraState.Stand:
                    return;
            }
            _position.X = _target.X + (int)(_radius * Math.Cos(Angle * Math.PI / 180));
            _position.Z = _target.Z - (int)(_radius * Math.Sin(Angle * Math.PI / 180));
            View = Matrix.CreateLookAt(_position, _target, Vector3.Up);
        }

        private void buildDirectionMap()
        {
            switch (Direction)
            {
                case CameraDirection.North:
                    DirectionMap[0] = 0;
                    DirectionMap[1] = 1;
                    DirectionMap[2] = 2;
                    DirectionMap[3] = 3;
                    break;

                case CameraDirection.East:
                    DirectionMap[0] = 3;
                    DirectionMap[1] = 0;
                    DirectionMap[2] = 1;
                    DirectionMap[3] = 2;
                    break;

                case CameraDirection.South:
                    DirectionMap[0] = 2;
                    DirectionMap[1] = 3;
                    DirectionMap[2] = 0;
                    DirectionMap[3] = 1;
                    break;

                case CameraDirection.West:
                    DirectionMap[0] = 1;
                    DirectionMap[1] = 2;
                    DirectionMap[2] = 3;
                    DirectionMap[3] = 0;
                    break;
            }
        }

        private void beginClockwise()
        {
            if ((int)++Direction == 4) Direction = CameraDirection.North;
            _targetAngle = 225 - (int)Direction * 90;
            if (Direction == CameraDirection.North) Angle = 315;
            State = CameraState.Clockwise;
#if DEBUG
            Main.InsertLog("Camera: Rotate clockwise");
#endif
        }

        private void beginCounterClockwise()
        {
            if ((int)--Direction == -1) Direction = CameraDirection.West;
            _targetAngle = 225 - (int)Direction * 90;
            if (Direction == CameraDirection.West) _targetAngle = 315;
            if (Direction == CameraDirection.South) Angle = -45;
            State = CameraState.CounterClockwise;
#if DEBUG
            Main.InsertLog("Camera: Rotate counter-clockwise");
#endif
        }

        public bool PerformRotation(int x, int y)
        {
            if (State != CameraState.Stand) return false;

#if WINDOWS_PHONE || ANDROID
            while (TouchPanel.IsGestureAvailable)
            {
                GestureSample gesture = TouchPanel.ReadGesture();

                if (gesture.GestureType == GestureType.Flick)
                {
                    bool isClockwise = true;
                    if (Math.Abs(gesture.Delta.X) > Math.Abs(gesture.Delta.Y))
                    {
                        if (gesture.Delta.X < 0) isClockwise = false;
                        if (y < 240) isClockwise = !isClockwise;
                    }
                    else
                    {
                        if (gesture.Delta.Y > 0) isClockwise = false;
                        if (x < 300) isClockwise = !isClockwise;
                    }

                    if (isClockwise) beginClockwise();
                    else beginCounterClockwise();
                    buildDirectionMap();
                    return true;
                }
            }
            return false;
#endif
#if WINDOWS
            if (PlayScene.recRotateCW.Contains(x, y)) beginClockwise();
            else beginCounterClockwise();
            buildDirectionMap();
            return true;
#endif
        }
    }
}
