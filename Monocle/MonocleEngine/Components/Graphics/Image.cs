using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Monocle
{
    public class Image : GraphicsComponent
    {
        public Texture Texture { get; protected set; }
        public Rectangle ClipRect;
        protected Texture2D outlineTexture;

        public Image(Texture texture, Rectangle? clipRect = null)
            : this(texture, clipRect, false)
        {

        }

        public Image(Subtexture subTexture, Rectangle? clipRect = null)
            : this(subTexture, clipRect, false)
        {

        }

        internal Image(Texture texture, Rectangle? clipRect, bool active)
            : base(active)
        {
            Texture = texture;
            ClipRect = clipRect ?? texture.Rect;
        }

        internal Image(Subtexture subTexture, Rectangle? clipRect, bool active)
            : base(active)
        {
            Texture = subTexture.Texture;

            if (clipRect.HasValue)
                ClipRect = subTexture.GetAbsoluteClipRect(clipRect.Value);
            else
                ClipRect = subTexture.Rect;
        }

        public override void Render()
        {
            Draw.SpriteBatch.Draw(Texture.Texture2D, RenderPosition, ClipRect, Color, Rotation, Origin, Scale * Zoom, Effects, 0);
        }

        public override int Width
        {
            get { return ClipRect.Width; }
        }

        public override int Height
        {
            get { return ClipRect.Height; }
        }

        public void SwapSubtexture(Subtexture subtexture, Rectangle? clipRect = null)
        {
            Texture = subtexture.Texture;
            ClipRect = clipRect ?? subtexture.Rect;
        }

        private void BakeFilledOutline()
        {
            outlineTexture = new Texture2D(Engine.Instance.GraphicsDevice, Texture.Width, Texture.Height);

            Color[] data = new Color[outlineTexture.Width * outlineTexture.Height];

            Texture.Texture2D.GetData(data);

            for (int i = 0; i < data.Length; i++)
                if (data[i].A > 0) data[i] = Color.White;

            outlineTexture.SetData(data);
        }

        public void DrawFilledOutline(Color color, int offset = 1)
        {
            Vector2 pos = Position;
            Color was = Color;
            Color = color;
            if (outlineTexture == null) BakeFilledOutline();
            for (int i = -1; i < 2; i++)
            {
                for (int j = -1; j < 2; j++)
                {
                    if (i != 0 || j != 0)
                    {
                        Position = pos + new Vector2(i * offset, j * offset);
                        RenderOutline();
                    }
                }
            }

            Position = pos;
            Color = was;
        }

        protected virtual void RenderOutline()
        {
            Draw.SpriteBatch.Draw(outlineTexture, RenderPosition, ClipRect, Color, Rotation, Origin, Scale * Zoom, Effects, 0);
        }
    }
}
