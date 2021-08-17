using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace MyStory.Gfx2d
{
    class Camera : Spatial
    {
        public Camera()
        {
        }

        public void Move(float x, float y)
        {
            SetPosition((int)(Position.X + x), (int)(Position.Y + y));
        }

        public override string ToString()
        {
            return string.Format("Camera: {0}, {1}", Position.X, Position.Y);
        }

        public static Camera Wrap(Camera camera, float x, float y, float width, float height) {
            Camera wrappedCamera = new Camera();

            wrappedCamera.SetPosition((camera.Position.X + x) % (width), (camera.Position.Y + y) % height);
            wrappedCamera.SetPosition(wrappedCamera.Position.X < 0 ? width + wrappedCamera.Position.X : wrappedCamera.Position.X, wrappedCamera.Position.Y < 0 ? height + wrappedCamera.Position.Y : wrappedCamera.Position.Y );
            wrappedCamera.SetScale(camera.Scale);
            wrappedCamera.SetRotation(camera.Rotation);

            return wrappedCamera;
        }
    }
}
