using System;
using System.Collections.Generic;
using System.Linq;
using System.Drawing;
using System.Globalization;

namespace NClass.DiagramEditor.ClassDiagram.Shapes
{
    public class Rhombus
    {

        public static readonly Rhombus Empty = new Rhombus(0,0,0,0);

        private int x;
        private int y;
        private int width;
        private int height;
        private Point[] apex = new Point[5];

        public Rhombus(int x, int y, int width, int height)
        {
            this.x = x;
            this.y = y;
            this.width = width;
            this.height = height;
            apex[0] = new Point(10, 10);
            apex[1] = new Point(10, 10);
            apex[2] = new Point(10, 10);
            apex[3] = new Point(10, 10);
            //apex[4] = new Point(10, 10);
        }

        public Rhombus(int x, int y, int width)
        {
            this.x = x;
            this.y = y;
            this.width = width;
            apex[0] = new Point(this.x, this.y);
            apex[1] = new Point(this.x + width / 2, this.y + width / 2);
            apex[2] = new Point(this.x+width, this.y);
            apex[3] = new Point(this.x + width / 2, this.y - width / 2);
            apex[4] = new Point(this.x, this.y);
        }

        public Rhombus(Point location, Size size)
        {
            this.x = location.X;
            this.y = location.Y;
            this.width = size.Width;
            this.height = size.Height;
        }

        public static Rhombus FromLTRB(int left, int top, int right, int bottom)
        {
            return new Rhombus(left,
                                 top,
                                 right - left,
                                 bottom - top);
        }
     
        public Point Location
        {
            get
            {
                return new Point(X, Y);
            }
            set
            {
                X = value.X;
                Y = value.Y;
            }
        }

        public Size Size
        {
            get
            {
                return new Size(Width, Height);
            }
            set
            {
                this.Width = value.Width;
                this.Height = value.Height;
            }
        }

        public int X
        {
            get
            {
                return x;
            }
            set
            {
                x = value;
            }
        }

        public int Y
        {
            get
            {
                return y;
            }
            set
            {
                y = value;
            }
        }

        public int Width
        {
            get
            {
                return width;
            }
            set
            {
                width = value;
            }
        }

        public int Height
        {
            get
            {
                return height;
            }
            set
            {
                height = value;
            }
        }

        public Point[] Apex
        {
            get
            {
                return apex;
            }
        }

        public int Left
        {
            get
            {
                return X;
            }
        }

        public int Top
        {
            get
            {
                return Y;
            }
        }

        public int Right
        {
            get
            {
                return X + Width;
            }
        }

        public int Bottom
        {
            get
            {
                return Y + Height;
            }
        }

        public bool IsEmpty
        {
            get
            {
                return height == 0 && width == 0 && x == 0 && y == 0;
                // C++ uses this definition:
                // return(Width <= 0 )|| (Height <= 0);
            }
        }

        public override bool Equals(object obj)
        {
            if (!(obj is Rhombus))
                return false;

            Rhombus comp = (Rhombus)obj;

            return (comp.X == this.X) &&
            (comp.Y == this.Y) &&
            (comp.Width == this.Width) &&
            (comp.Height == this.Height);
        }

        public static bool operator ==(Rhombus left, Rhombus right)
        {
            return (left.X == right.X
                    && left.Y == right.Y
                    && left.Width == right.Width
                    && left.Height == right.Height);
        }

        public static bool operator !=(Rhombus left, Rhombus right)
        {
            return !(left == right);
        }

        public static Rhombus Ceiling(RectangleF value)
        {
            return new Rhombus((int)Math.Ceiling(value.X),
                                 (int)Math.Ceiling(value.Y),
                                 (int)Math.Ceiling(value.Width),
                                 (int)Math.Ceiling(value.Height));
        }

        public static Rhombus Truncate(RectangleF value)
        {
            return new Rhombus((int)value.X,
                                 (int)value.Y,
                                 (int)value.Width,
                                 (int)value.Height);
        }

        public static Rhombus Round(RectangleF value)
        {
            return new Rhombus((int)Math.Round(value.X),
                                 (int)Math.Round(value.Y),
                                 (int)Math.Round(value.Width),
                                 (int)Math.Round(value.Height));
        }

        public bool Contains(int x, int y)
        {
            return this.X <= x &&
            x < this.X + this.Width &&
            this.Y <= y &&
            y < this.Y + this.Height;
        }

        public bool Contains(Point pt)
        {
            return Contains(pt.X, pt.Y);
        }

        public bool Contains(Rhombus rect)
        {
            return (this.X <= rect.X) &&
            ((rect.X + rect.Width) <= (this.X + this.Width)) &&
            (this.Y <= rect.Y) &&
            ((rect.Y + rect.Height) <= (this.Y + this.Height));
        }

        public override int GetHashCode()
        {
            return unchecked((int)((UInt32)X ^
                        (((UInt32)Y << 13) | ((UInt32)Y >> 19)) ^
                        (((UInt32)Width << 26) | ((UInt32)Width >> 6)) ^
                        (((UInt32)Height << 7) | ((UInt32)Height >> 25))));
        }

        public void Inflate(int width, int height)
        {
            this.X -= width;
            this.Y -= height;
            this.Width += 2 * width;
            this.Height += 2 * height;
        }

        public void Inflate(Size size)
        {

            Inflate(size.Width, size.Height);
        }

        public static Rhombus Inflate(Rhombus rect, int x, int y)
        {
            Rhombus r = rect;
            r.Inflate(x, y);
            return r;
        }

        public void Intersect(Rhombus rect)
        {
            Rhombus result = Rhombus.Intersect(rect, this);

            this.X = result.X;
            this.Y = result.Y;
            this.Width = result.Width;
            this.Height = result.Height;
        }

        public static Rhombus Intersect(Rhombus a, Rhombus b)
        {
            int x1 = Math.Max(a.X, b.X);
            int x2 = Math.Min(a.X + a.Width, b.X + b.Width);
            int y1 = Math.Max(a.Y, b.Y);
            int y2 = Math.Min(a.Y + a.Height, b.Y + b.Height);

            if (x2 >= x1
                && y2 >= y1)
            {

                return new Rhombus(x1, y1, x2 - x1, y2 - y1);
            }
            return Rhombus.Empty;
        }

        public bool IntersectsWith(Rhombus rect)
        {
            return (rect.X < this.X + this.Width) &&
            (this.X < (rect.X + rect.Width)) &&
            (rect.Y < this.Y + this.Height) &&
            (this.Y < rect.Y + rect.Height);
        }

        public static Rhombus Union(Rhombus a, Rhombus b)
        {
            int x1 = Math.Min(a.X, b.X);
            int x2 = Math.Max(a.X + a.Width, b.X + b.Width);
            int y1 = Math.Min(a.Y, b.Y);
            int y2 = Math.Max(a.Y + a.Height, b.Y + b.Height);

            return new Rhombus(x1, y1, x2 - x1, y2 - y1);
        }

        public void Offset(Point pos)
        {
            Offset(pos.X, pos.Y);
        }

        public void Offset(int x, int y)
        {
            this.X += x;
            this.Y += y;
        }

        public override string ToString()
        {
            return "{X=" + X.ToString(CultureInfo.CurrentCulture) + ",Y=" + Y.ToString(CultureInfo.CurrentCulture) +
            ",Width=" + Width.ToString(CultureInfo.CurrentCulture) +
            ",Height=" + Height.ToString(CultureInfo.CurrentCulture) + "}";
        }
    }
}
