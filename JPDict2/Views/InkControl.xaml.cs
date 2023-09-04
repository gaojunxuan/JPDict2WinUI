using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using Windows.UI.Input.Inking;
using Microsoft.UI.Xaml.Shapes;
using Microsoft.UI;
using System.Diagnostics;
using System.Windows.Shapes;
using Line = Microsoft.UI.Xaml.Shapes.Line;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace JPDict2.Views
{
    public sealed partial class InkControl : UserControl
    {
        private bool penDown = false;
        private bool isPenMode = true;
        private Point oldPoint;
        private InkStrokeBuilder inkStrokeBuilder = new();
        private List<Point> currentStrokePoints = new();
        private List<Line> currentLineSegments = new();



        public string Character
        {
            get => (string)GetValue(CharacterProperty);
            set => SetValue(CharacterProperty, value);
        }

        public static readonly DependencyProperty CharacterProperty =
            DependencyProperty.Register("Character", typeof(string), typeof(InkControl), new PropertyMetadata(0));


        public InkControl()
        {
            this.InitializeComponent();
        }

        private void PenMode_Button_Click(object sender, RoutedEventArgs e)
        {
            EraseMode_Button.IsChecked = !EraseMode_Button.IsChecked;
            isPenMode = true;
        }

        private void EraseMode_Button_Click(object sender, RoutedEventArgs e)
        {
            PenMode_Button.IsChecked = !EraseMode_Button.IsChecked;
            isPenMode = false;
        }

        private void ClearCanvas_Button_Click(object sender, RoutedEventArgs e)
        {
            DrawCanvas.Children.Clear();
            if (currentStrokePoints != null) { currentStrokePoints.Clear(); }
            if (currentLineSegments != null) { currentLineSegments.Clear(); }

        }

        private void DrawCanvas_PointerPressed(object sender, PointerRoutedEventArgs e)
        {
            e.Handled = true;

            if (e.Pointer.PointerDeviceType == Microsoft.UI.Input.PointerDeviceType.Mouse ||
                e.Pointer.PointerDeviceType == Microsoft.UI.Input.PointerDeviceType.Pen)
            {
                penDown = true;
                var pnt = e.GetCurrentPoint(DrawCanvas);
                oldPoint = pnt.Position;
            }
            if (!isPenMode) return;

            currentStrokePoints = new List<Point>
            {
                oldPoint
            };
        }

        private void DrawCanvas_PointerMoved(object sender, PointerRoutedEventArgs e)
        {
            e.Handled = true;

            if (!penDown || !isPenMode) return;

            var pnt = e.GetCurrentPoint(DrawCanvas);

            if (pnt.Position.X > 0 && pnt.Position.X < DrawCanvas.Width && pnt.Position.Y > 0 && pnt.Position.Y
                < DrawCanvas.Height)
            {
                Line line = new Line();
                line.X1 = oldPoint.X;
                line.Y1 = oldPoint.Y;
                line.X2 = pnt.Position.X;
                line.Y2 = pnt.Position.Y;
                line.Stroke = new SolidColorBrush(Colors.Black);
                line.StrokeEndLineCap = PenLineCap.Round;
                line.StrokeStartLineCap = PenLineCap.Round;
                line.StrokeLineJoin = PenLineJoin.Round;
                //line.StrokeThickness = 4 * pnt.Properties.Pressure;
                line.StrokeThickness = 8;

                DrawCanvas.Children.Add(line);
                currentLineSegments.Add(line);

                oldPoint = pnt.Position;

                currentStrokePoints.Add(pnt.Position);
            }
            else
            {
                penDown = false;

                if (!isPenMode) return;
                currentStrokePoints.Add(e.GetCurrentPoint(DrawCanvas).Position);
                var inkStroke = inkStrokeBuilder.CreateStroke(currentStrokePoints);
                Bezier(DrawCanvas, inkStroke);
            }
        }

        public void Bezier(Canvas canvas, InkStroke stroke)
        {
            var segments = stroke.GetRenderingSegments();

            PathFigure pthFigure = new PathFigure() { StartPoint = new Point(segments[0].Position.X, segments[0].Position.Y) };

            for (int i = 1; i < segments.Count; i++)
            {
                var segment = segments[i];
                var bezier = new BezierSegment();
                bezier.Point1 = new Point(segment.BezierControlPoint1.X, segment.BezierControlPoint1.Y);
                bezier.Point2 = new Point(segment.BezierControlPoint2.X, segment.BezierControlPoint2.Y);
                bezier.Point3 = new Point(segment.Position.X, segment.Position.Y);
                pthFigure.Segments.Add(bezier);
            }

            PathGeometry pthGeometry = new PathGeometry();

            pthGeometry.Figures.Add(pthFigure);

            Microsoft.UI.Xaml.Shapes.Path path = new Microsoft.UI.Xaml.Shapes.Path();
            //path.Stroke = new SolidColorBrush(stroke.DrawingAttributes.Color);
            //path.StrokeThickness = stroke.DrawingAttributes.Size.Height;
            path.Stroke = new SolidColorBrush(Colors.Black);
            path.StrokeLineJoin = PenLineJoin.Round;
            path.StrokeEndLineCap = PenLineCap.Round;
            path.StrokeStartLineCap = PenLineCap.Round;
            path.StrokeThickness = 8;
            path.Data = pthGeometry;
            path.PointerEntered += Path_PointerEntered;

            canvas.Children.Add(path);

            foreach (var line in currentLineSegments)
            {
                DrawCanvas.Children.Remove(line);
            }
            currentLineSegments.Clear();


        }

        private void Path_PointerEntered(object sender, PointerRoutedEventArgs e)
        {
            if (!isPenMode && penDown)
            {
                var path = (Microsoft.UI.Xaml.Shapes.Path)sender;
                path.PointerEntered -= Path_PointerEntered;
                DrawCanvas.Children.Remove(path);
            }
        }

        private void DrawCanvas_PointerReleased(object sender, PointerRoutedEventArgs e)
        {
            if (penDown)
            {
                penDown = false;

                if (!isPenMode) return;
                currentStrokePoints.Add(e.GetCurrentPoint(DrawCanvas).Position);
                var inkStroke = inkStrokeBuilder.CreateStroke(currentStrokePoints);
                Bezier(DrawCanvas, inkStroke);
            }
        }

        private void DrawCanvas_PointerExited(object sender, PointerRoutedEventArgs e)
        {
            penDown = false;
            DrawCanvas.ReleasePointerCaptures();
        }
    }
}
