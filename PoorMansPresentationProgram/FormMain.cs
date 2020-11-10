using System.Drawing;
using System.Windows.Forms;

namespace PoorMansPresentationProgram
{
    public partial class FormMain : Form
    {
        private readonly Presentation presentation;

        public FormMain(System.IO.FileInfo file)
        {
            InitializeComponent();

            this.DoubleBuffered = true;
            this.Text = $"PMPP - {file.Name}";

            Width = Screen.PrimaryScreen.Bounds.Width;
            Height = Screen.PrimaryScreen.Bounds.Height;

            presentation = new Presentation(file);
            Disposed += (sender, e) => presentation.Dispose();
        }

        protected override void OnPaintBackground(PaintEventArgs e)
        {
            if (presentation.Master != null)
            {
                e.Graphics.DrawImage(presentation.Master, 0, 0, Width, Height);
            }
            else
            {
                e.Graphics.Clear(Color.White);
            }
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            if (presentation.CurrentSlide != null)
            {
                e.Graphics.DrawImage(presentation.CurrentSlide, 0, 0, Width, Height);
            }
        }

        protected override void OnKeyUp(KeyEventArgs e)
        {
            switch (e.KeyCode)
            {
                case Keys.Enter:
                case Keys.Right:
                case Keys.Space:
                case Keys.BrowserForward:
                    presentation.Next();
                    Invalidate();
                    break;
                case Keys.Left:
                case Keys.Back:
                case Keys.BrowserBack:
                    presentation.Previous();
                    Invalidate();
                    break;
            }

            if (presentation.IsAtEndOfPresentation)
            {
                this.Close();
            }
        }

        protected override void OnMouseClick(MouseEventArgs e)
        {
            switch (e.Button)
            {
                case MouseButtons.Left:
                    presentation.Next();
                    Invalidate();
                    break;
                case MouseButtons.Right:
                    presentation.Previous();
                    Invalidate();
                    break;
            }

            if (presentation.IsAtEndOfPresentation)
            {
                this.Close();
            }
        }
    }
}
