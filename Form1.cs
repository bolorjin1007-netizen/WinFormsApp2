
using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Windows.Forms;

namespace WinFormsApp2
{
    public partial class Form1 : Form
    {
        private Panel canvas;
        private Panel toolbar;
        private Bitmap drawingBitmap;
        private Graphics drawingGraphics;

        private bool draw = false;
        private Point startPoint;
        private Point currentPoint;

        private Color brushColor = Color.Black;
        private float brushSize = 4;

        private ComboBox shapeBox;
        private Button colorButton;

        public Form1()
        {
            InitializeComponent();
            CreatePaintApp();
        }

        private void CreatePaintApp()
        {
            Text = "Mini Paint";
            Size = new Size(1000, 700);
            StartPosition = FormStartPosition.CenterScreen;

            toolbar = new Panel();
            toolbar.Dock = DockStyle.Top;
            toolbar.Height = 65;
            toolbar.BackColor = Color.LightGray;
            Controls.Add(toolbar);

            colorButton = new Button();
            colorButton.Text = "Өнгө";
            colorButton.Location = new Point(10, 15);
            colorButton.Size = new Size(75, 35);
            colorButton.Click += ColorButton_Click;
            toolbar.Controls.Add(colorButton);

            Label shapeLabel = new Label();
            shapeLabel.Text = "Дүрс:";
            shapeLabel.Location = new Point(95, 24);
            shapeLabel.AutoSize = true;
            toolbar.Controls.Add(shapeLabel);

            shapeBox = new ComboBox();
            shapeBox.Location = new Point(135, 18);
            shapeBox.Size = new Size(140, 30);
            shapeBox.DropDownStyle = ComboBoxStyle.DropDownList;

            shapeBox.Items.AddRange(new object[]
            {
                "Чөлөөт зураг",
                "Шулуун",
                "Тэгш өнцөгт",
                "Дугуй",
                "Гурвалжин",
                "Сум"
            });

            shapeBox.SelectedIndex = 0;
            toolbar.Controls.Add(shapeBox);

            Label sizeLabel = new Label();
            sizeLabel.Text = "Зураас:";
            sizeLabel.Location = new Point(290, 24);
            sizeLabel.AutoSize = true;
            toolbar.Controls.Add(sizeLabel);

            NumericUpDown sizeBox = new NumericUpDown();
            sizeBox.Location = new Point(345, 18);
            sizeBox.Size = new Size(60, 30);
            sizeBox.Minimum = 1;
            sizeBox.Maximum = 30;
            sizeBox.Value = 4;
            sizeBox.ValueChanged += (s, e) =>
            {
                brushSize = (float)sizeBox.Value;
            };
            toolbar.Controls.Add(sizeBox);

            Button eraserButton = new Button();
            eraserButton.Text = "Баллуур";
            eraserButton.Location = new Point(415, 15);
            eraserButton.Size = new Size(80, 35);
            eraserButton.Click += (s, e) =>
            {
                brushColor = Color.White;
                colorButton.BackColor = Color.White;
                shapeBox.SelectedIndex = 0;
            };
            toolbar.Controls.Add(eraserButton);

            Button clearButton = new Button();
            clearButton.Text = "Цэвэрлэх";
            clearButton.Location = new Point(505, 15);
            clearButton.Size = new Size(80, 35);
            clearButton.Click += (s, e) => ClearCanvas();
            toolbar.Controls.Add(clearButton);

            Button saveButton = new Button();
            saveButton.Text = "Хадгалах";
            saveButton.Location = new Point(595, 15);
            saveButton.Size = new Size(85, 35);
            saveButton.Click += (s, e) => SaveDrawing();
            toolbar.Controls.Add(saveButton);

            canvas = new Panel();
            canvas.Dock = DockStyle.Fill;
            canvas.BackColor = Color.White;
            canvas.Cursor = Cursors.Cross;

            canvas.Paint += Canvas_Paint;
            canvas.MouseDown += Canvas_MouseDown;
            canvas.MouseMove += Canvas_MouseMove;
            canvas.MouseUp += Canvas_MouseUp;

            Controls.Add(canvas);
            toolbar.BringToFront();

            drawingBitmap = new Bitmap(1000, 700);
            drawingGraphics = Graphics.FromImage(drawingBitmap);
            drawingGraphics.Clear(Color.White);
            drawingGraphics.SmoothingMode = SmoothingMode.AntiAlias;
        }

        private void ColorButton_Click(object sender, EventArgs e)
        {
            using (ColorDialog dialog = new ColorDialog())
            {
                dialog.Color = brushColor;

                if (dialog.ShowDialog() == DialogResult.OK)
                {
                    brushColor = dialog.Color;
                    colorButton.BackColor = brushColor;
                }
            }
        }

        private void Canvas_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button != MouseButtons.Left)
                return;

            draw = true;
            startPoint = e.Location;
            currentPoint = e.Location;

            if (shapeBox.SelectedIndex == 0)
            {
                using (Pen pen = CreatePen())
                {
                    drawingGraphics.DrawLine(
                        pen, startPoint, currentPoint);
                }
            }

            canvas.Invalidate();
        }

        private void Canvas_MouseMove(object sender, MouseEventArgs e)
        {
            if (!draw || e.Button != MouseButtons.Left)
                return;

            currentPoint = e.Location;

            if (shapeBox.SelectedIndex == 0)
            {
                using (Pen pen = CreatePen())
                {
                    drawingGraphics.DrawLine(
                        pen, startPoint, currentPoint);
                }

                startPoint = currentPoint;
            }

            canvas.Invalidate();
        }

        private void Canvas_MouseUp(object sender, MouseEventArgs e)
        {
            if (!draw)
                return;

            currentPoint = e.Location;

            if (shapeBox.SelectedIndex != 0)
            {
                DrawSelectedShape(drawingGraphics,
                    startPoint, currentPoint);
            }

            draw = false;
            canvas.Invalidate();
        }

        private Pen CreatePen()
        {
            Pen pen = new Pen(brushColor, brushSize);
            pen.StartCap = LineCap.Round;
            pen.EndCap = LineCap.Round;
            pen.LineJoin = LineJoin.Round;
            return pen;
        }

        private void DrawSelectedShape(
            Graphics graphics, Point p1, Point p2)
        {
            using (Pen pen = CreatePen())
            {
                int left = Math.Min(p1.X, p2.X);
                int top = Math.Min(p1.Y, p2.Y);
                int width = Math.Abs(p2.X - p1.X);
                int height = Math.Abs(p2.Y - p1.Y);

                Rectangle rect = new Rectangle(
                    left, top, width, height);

                switch (shapeBox.SelectedIndex)
                {
                    case 1:
                        graphics.DrawLine(pen, p1, p2);
                        break;

                    case 2:
                        graphics.DrawRectangle(pen, rect);
                        break;

                    case 3:
                        graphics.DrawEllipse(pen, rect);
                        break;

                    case 4:
                        Point[] triangle =
                        {
                            new Point(left + width / 2, top),
                            new Point(left, top + height),
                            new Point(left + width, top + height)
                        };

                        graphics.DrawPolygon(pen, triangle);
                        break;

                    case 5:
                        if (p1 != p2)
                        {
                            pen.CustomEndCap = new AdjustableArrowCap(
                                5 + brushSize, 7 + brushSize, true);

                            graphics.DrawLine(pen, p1, p2);
                        }
                        break;
                }
            }
        }

        private void Canvas_Paint(object sender, PaintEventArgs e)
        {
            if (drawingBitmap != null)
            {
                e.Graphics.DrawImage(drawingBitmap, 0, 0);
            }

            // Зурж байх үед дүрсийг урьдчилан харуулна
            if (draw && shapeBox.SelectedIndex != 0)
            {
                using (Pen previewPen = CreatePen())
                {
                    previewPen.DashStyle = DashStyle.Dash;

                    // Түр харагдах дүрсийг тусад нь зурна
                    DrawPreview(e.Graphics, previewPen);
                }
            }
        }

        private void DrawPreview(Graphics graphics, Pen pen)
        {
            int left = Math.Min(startPoint.X, currentPoint.X);
            int top = Math.Min(startPoint.Y, currentPoint.Y);
            int width = Math.Abs(currentPoint.X - startPoint.X);
            int height = Math.Abs(currentPoint.Y - startPoint.Y);

            Rectangle rect = new Rectangle(left, top, width, height);

            switch (shapeBox.SelectedIndex)
            {
                case 1:
                    graphics.DrawLine(pen, startPoint, currentPoint);
                    break;

                case 2:
                    graphics.DrawRectangle(pen, rect);
                    break;

                case 3:
                    graphics.DrawEllipse(pen, rect);
                    break;

                case 4:
                    Point[] triangle =
                    {
                        new Point(left + width / 2, top),
                        new Point(left, top + height),
                        new Point(left + width, top + height)
                    };

                    graphics.DrawPolygon(pen, triangle);
                    break;

                case 5:
                    if (startPoint != currentPoint)
                    {
                        using (Pen arrowPen = CreatePen())
                        {
                            arrowPen.DashStyle = DashStyle.Solid;
                            arrowPen.CustomEndCap =
                                new AdjustableArrowCap(8, 10, true);

                            graphics.DrawLine(
                                arrowPen, startPoint, currentPoint);
                        }
                    }
                    break;
            }
        }

        private void ClearCanvas()
        {
            drawingGraphics.Clear(Color.White);
            canvas.Invalidate();
        }

        private void SaveDrawing()
        {
            using (SaveFileDialog dialog = new SaveFileDialog())
            {
                dialog.Filter =
                    "PNG зураг (*.png)|*.png|JPEG зураг (*.jpg)|*.jpg";
                dialog.FileName = "MyDrawing.png";

                if (dialog.ShowDialog() == DialogResult.OK)
                {
                    ImageFormat format = dialog.FilterIndex == 2
                        ? ImageFormat.Jpeg
                        : ImageFormat.Png;

                    drawingBitmap.Save(dialog.FileName, format);

                    MessageBox.Show("Зураг хадгалагдлаа!");
                }
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {
        }

        private void Form1_MouseDown(
            object sender, MouseEventArgs e)
        {
        }

        protected override void OnFormClosed(FormClosedEventArgs e)
        {
            drawingGraphics?.Dispose();
            drawingBitmap?.Dispose();
            base.OnFormClosed(e);
        }
    }
}
