using Microsoft.Xna.Framework;

namespace MyStory.Gfx2d
{
    public class Spatial
    {
        public Vector2 Position { get; protected set; }
        public Vector2 Scale { get; protected set; }
        public float Rotation { get; protected set; }
        public Matrix LocalMatrix { get; protected set; }
        public bool IsDirty { get; protected set; }

        public Spatial()
        {
            Scale = Vector2.One;
            Position = Vector2.Zero;
            Rotation = 0;
            IsDirty = true;
        }

        public virtual void SetPosition(Vector2 position)
        {
            Position = position;
            MarkDirty();
        }

        public void SetPosition(float x, float y)
        {
            SetPosition(new Vector2(x, y));
        }

        public virtual void SetScale(Vector2 scale)
        {
            Scale = scale;
            MarkDirty();
        }

        public void SetScale(float x, float y)
        {
            SetScale(new Vector2(x, y));
        }

        public void SetScale(float x)
        {
            SetScale(new Vector2(x, x));
        }

        public virtual void SetRotation(float radians)
        {
            Rotation = radians % MathHelper.TwoPi;
            MarkDirty();
        }

        public Matrix GetLocalMatrix()
        {
            if (IsDirty) Update();
            return LocalMatrix;
        }

        protected void MarkDirty()
        {
            IsDirty = true;
        }

        protected void Update()
        {
            LocalMatrix =
                Matrix.CreateScale(Scale.X, Scale.Y, 1.0f)
                * Matrix.CreateRotationY(Rotation)
                * Matrix.CreateTranslation(-Position.X, -Position.Y, 0.0f);
            IsDirty = false;
        }
    }
}

