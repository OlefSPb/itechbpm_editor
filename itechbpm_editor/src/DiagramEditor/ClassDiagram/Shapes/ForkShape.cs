using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using System.Collections.Generic;
using NClass.Core;
using NClass.DiagramEditor.ClassDiagram.Dialogs;
using NClass.DiagramEditor.ClassDiagram.ContextMenus;
using NClass.DiagramEditor.ClassDiagram.Editors;
using NClass.Translations;

namespace NClass.DiagramEditor.ClassDiagram.Shapes
{
    internal sealed class ForkShape : Shape
    {
        const int PaddingSize = 10;
        const int DefaultWidth = 50;
        const int DefaultHeight = 50;
        

        static CommentEditor editor = new CommentEditor();
        static Pen borderPen = new Pen(Color.Black);
        static SolidBrush backgroundBrush = new SolidBrush(Color.White);
        static SolidBrush textBrush = new SolidBrush(Color.Black);
        static StringFormat format = new StringFormat(StringFormat.GenericTypographic);

        Comment comment;
        bool editorShowed = false;

        static ForkShape()
		{
			format.Trimming = StringTrimming.EllipsisWord;
			format.FormatFlags = StringFormatFlags.LineLimit;
		}

        /// <exception cref="ArgumentNullException">
		/// <paramref name="comment"/> is null.
		/// </exception>
        internal ForkShape(Comment comment) : base(comment)
		{
			this.comment = comment;
		}

        public override IEntity Entity
        {
            get { return comment; }
        }

        public Comment Comment
        {
            get { return comment; }
        }

        public string Text
        {
            get { return comment.Text; }
            set { comment.Text = value; }
        }

        protected override Size DefaultSize
        {
            get
            {
                return new Size(DefaultWidth, DefaultHeight);
            }
        }

        protected override bool CloneEntity(Diagram diagram)
        {
            return diagram.InsertComment(Comment.Clone());
        }
        
        public static Rhombus GetOutline(Style style)
        {
            //Rhombus[0] = new Point(0,DefaultHeight/2);
            //Rhombus[1] = new Point(DefaultWidth, DefaultHeight / 2);
            //Rhombus[2] = new Point(DefaultWidth/2,0);
            //Rhombus[3] = new Point(DefaultWidth/2,DefaultHeight);
            return new Rhombus(50, 50, DefaultWidth);
            //Matrix myMatrix = new Matrix();
            //myMatrix.Rotate(45, MatrixOrder.Append);
            //Rectangle g = new Rectangle(0, 0, DefaultWidth, DefaultHeight);
            //return new Rectangle(0, 0, DefaultWidth, DefaultHeight);
        }

        protected override int GetBorderWidth(Style style)
        {
            return style.CommentBorderWidth;
        }

        internal Rectangle GetTextRectangle()
        {
            return new Rectangle(
                Left + PaddingSize, Top + PaddingSize,
                Width - 2 * PaddingSize, Height - 2 * PaddingSize
            );
        }

        protected override void OnMove(MoveEventArgs e)
        {
            base.OnMove(e);
            HideEditor();
        }

        protected override void OnResize(ResizeEventArgs e)
        {
            base.OnResize(e);
            if (editorShowed)
            {
                editor.Relocate(this);
                if (!editor.Focused)
                    editor.Focus();
            }
        }

        protected override void OnMouseDown(AbsoluteMouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                IsActive = true;
            }
            base.OnMouseDown(e);
        }

        protected override void OnDoubleClick(AbsoluteMouseEventArgs e)
        {
            if (Contains(e.Location) && e.Button == MouseButtons.Left)
                ShowEditor();
        }

        protected internal override void ShowEditor()
        {
            if (!editorShowed)
            {
                editor.Relocate(this);
                editor.Init(this);
                ShowWindow(editor);
                editor.Focus();
                editorShowed = true;
            }
        }

        protected internal override void HideEditor()
        {
            if (editorShowed)
            {
                HideWindow(editor);
                editorShowed = false;
            }
        }

        protected internal override void MoveWindow()
        {
            HideEditor();
        }

        internal void EditText()
        {
            using (EditCommentDialog dialog = new EditCommentDialog(Text))
            {
                if (dialog.ShowDialog() == DialogResult.OK)
                    Text = dialog.InputText;
            }
        }

        protected internal override IEnumerable<ToolStripItem> GetContextMenuItems(Diagram diagram)
        {
            return CommentShapeContextMenu.Default.GetMenuItems(diagram);
        }

        private void DrawSurface(IGraphics g, bool onScreen, Style style)
        {
            // Update graphical objects
            backgroundBrush.Color = style.CommentBackColor;
            borderPen.Color = style.CommentBorderColor;
            borderPen.Width = style.CommentBorderWidth;
            if (style.IsCommentBorderDashed)
                borderPen.DashPattern = borderDashPattern;
            else
                borderPen.DashStyle = DashStyle.Solid;

            // Create shape pattern
            GraphicsPath path = new GraphicsPath();
            path.AddLine(Left, Top, Right - PaddingSize, Top);
            path.AddLine(Right, Top + PaddingSize, Right, Bottom);
            path.AddLine(Right, Bottom, Left, Bottom);
            path.CloseFigure();

            // Draw shadow first
            if ((!onScreen || !IsSelected) && !style.ShadowOffset.IsEmpty)
            {
                shadowBrush.Color = style.ShadowColor;
                g.TranslateTransform(style.ShadowOffset.Width, style.ShadowOffset.Height);
                g.FillPath(shadowBrush, path);
                g.TranslateTransform(-style.ShadowOffset.Width, -style.ShadowOffset.Height);
            }

            // Draw borders & background
            g.FillPath(backgroundBrush, path);
            g.DrawPath(borderPen, path);

            // Draw earmark
            path.Reset();
            path.AddLine(Right - PaddingSize, Top, Right - PaddingSize, Top + PaddingSize);
            path.AddLine(Right - PaddingSize, Top + PaddingSize, Right, Top + PaddingSize);
            g.DrawPath(borderPen, path);

            path.Dispose();
        }

        private void DrawText(IGraphics g, bool onScreen, Style style)
        {
            Rectangle textBounds = GetTextRectangle();

            if (string.IsNullOrEmpty(Text) && onScreen)
            {
                textBrush.Color = Color.FromArgb(128, style.CommentTextColor);
                g.DrawString(Strings.DoubleClickToEdit,
                    style.CommentFont, textBrush, textBounds, format);
            }
            else
            {
                textBrush.Color = style.CommentTextColor;
                g.DrawString(Text, style.CommentFont, textBrush, textBounds, format);
            }
        }

        public override void Draw(IGraphics g, bool onScreen, Style style)
        {
            DrawSurface(g, onScreen, style);
            DrawText(g, onScreen, style);
        }

        protected override float GetRequiredWidth(Graphics g, Style style)
        {
            return Width;
        }

        protected override int GetRequiredHeight()
        {
            return Height;
        }

        public override string ToString()
        {
            return Strings.Comment;
        }
    }
}
