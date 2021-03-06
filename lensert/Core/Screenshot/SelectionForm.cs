﻿using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Windows.Forms;
using Lensert.Helpers;

namespace Lensert.Core.Screenshot
{
    public sealed partial class SelectionForm : Form
    {
        private const int DIMENSION_TEXT_OFFSET = 2; //TODO: Refactor into settings
        private readonly Rectangle _emtpyRectangle;

        private readonly SolidBrush _rectangleBrush,
            _textBrush;

        private readonly Pen _rectanglePen;

        private Rectangle _selectedArea;

        private Image _shadedScreenshot,
            _cleanScreenshot;

        public SelectionForm()
        {
            InitializeComponent();
            Bounds = Native.UnscaledBounds;
            _emtpyRectangle = new Rectangle(Bounds.Location, Size.Empty);

#if (DEBUG)
            {
                TopMost = false;
            }
#endif
            DoubleBuffered = true;

            _textBrush = new SolidBrush(Color.Red);
            _rectangleBrush = new SolidBrush(Color.FromArgb(120, Color.White)); //This is actually a bug where the transparancykey with the Red does register the mous input 
            _rectanglePen = new Pen(Color.Red); //(fyi, with any other color the mouse would click through it)
        }

        public Image Screenshot
        {
            get { return _cleanScreenshot; }
            set
            {
                _cleanScreenshot?.Dispose();
                _shadedScreenshot?.Dispose();

                _cleanScreenshot = ResizeImage(value, Bounds);

                _shadedScreenshot = new Bitmap(_cleanScreenshot);
                using (var graphics = Graphics.FromImage(_shadedScreenshot))
                    graphics.FillRectangle(_rectangleBrush, 0, 0, Bounds.Width, Bounds.Height);

                BackgroundImage = _shadedScreenshot;
            }
        }

        public Rectangle SelectedArea
        {
            get
            {
                var location = _selectedArea.Location;
                return new Rectangle(location, _selectedArea.Size);
            }
            set
            {
                _selectedArea = value;

                Invalidate();
                Update();
            }
        }

        private void SelectionForm_Load(object sender, EventArgs e)
        {
            _selectedArea = _emtpyRectangle;
        }

        private void SelectionForm_MouseUp(object sender, MouseEventArgs e)
        {
            Close();
        }

        private void SelectionForm_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Escape)
            {
                SelectedArea = _emtpyRectangle;
                Close();
            }
        }

        private void SelectionForm_Paint(object sender, PaintEventArgs e)
        {
            e.Graphics.DrawImage(_cleanScreenshot, _selectedArea, _selectedArea, GraphicsUnit.Pixel);

            var borderRectangle = _selectedArea;
            borderRectangle.Width -= 1;
            borderRectangle.Height -= 1;
            if (Bounds.Width <= borderRectangle.Right)
            {
                var deltaRight = borderRectangle.Right - Bounds.Right;
                borderRectangle.Width -= deltaRight == 0
                    ? 1
                    : deltaRight;
            }

            if (Bounds.Height <= borderRectangle.Bottom)
            {
                var deltaBottom = borderRectangle.Bottom - Bounds.Bottom;
                borderRectangle.Height -= deltaBottom == 0
                    ? 1
                    : deltaBottom; //compensates for out of bounds (only visually, 
            } //screenshot does reach till the end and beyond)
            e.Graphics.DrawRectangle(_rectanglePen, borderRectangle); //Draw the border

            var dimension = $"{_selectedArea.Width}x{_selectedArea.Height}";
            var size = e.Graphics.MeasureString(dimension, Font); //generates dimension string

            float y = _selectedArea.Y + _selectedArea.Height + DIMENSION_TEXT_OFFSET; //spaces the dimension text right bottom corner
            var x = _selectedArea.X + _selectedArea.Width - size.Width; //calculates the x_pos of the dimension 

            var currentScreenBounds = Screen.FromPoint(MousePosition).Bounds;
            if (y + size.Height > currentScreenBounds.Height)
                y -= size.Height + DIMENSION_TEXT_OFFSET * 2;

            e.Graphics.DrawString(dimension, Font, _textBrush, x, y); //draws string
        }

        private static Bitmap ResizeImage(Image image, Rectangle rect)
        {
            var destImage = new Bitmap(rect.Width, rect.Height);

            destImage.SetResolution(image.HorizontalResolution, image.VerticalResolution);

            using (var graphics = Graphics.FromImage(destImage))
            {
                graphics.CompositingMode = CompositingMode.SourceCopy;
                graphics.CompositingQuality = CompositingQuality.HighQuality;
                graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
                graphics.SmoothingMode = SmoothingMode.HighQuality;
                graphics.PixelOffsetMode = PixelOffsetMode.HighQuality;

                using (var wrapMode = new ImageAttributes())
                {
                    wrapMode.SetWrapMode(WrapMode.TileFlipXY);

                    var destRectangle = new Rectangle(0, 0, rect.Width, rect.Height);
                    graphics.DrawImage(image, destRectangle, 0, 0, image.Width, image.Height, GraphicsUnit.Pixel, wrapMode);
                }
            }

            return destImage;
        }
    }
}
