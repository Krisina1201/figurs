using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Shapes;
using Avalonia.Input;
using Avalonia.Media;
using Avalonia.Interactivity;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace figurs.Views
{
    public partial class MainWindow : Window
    {
        private readonly Canvas _canvas;
        private readonly List<Shape> _shapes = new List<Shape>();
        private Shape? _draggedShape;
        private Point _dragStart;
        private TextBlock? _messageTextBlock;

        private static readonly Random _random = new Random();

        public MainWindow()
        {
            InitializeComponent();
            _canvas = this.FindControl<Canvas>("DrawingCanvas");

            _canvas.PointerPressed += OnPointerPressed;
            _canvas.PointerMoved += OnPointerMoved;
            _canvas.PointerReleased += OnPointerReleased;

            _canvas.Loaded += OnCanvasLoaded;
        }

        private void OnCanvasLoaded(object? sender, RoutedEventArgs e)
        {
            _canvas.Loaded -= OnCanvasLoaded;


            AddRandomShape(CreateSquare());
            AddRandomShape(CreatePentagon());
            AddRandomShape(CreateHexagon());
            AddRandomShape(CreateOctagon());
            AddRandomShape(CreateCircle());
            AddRandomShape(CreateHeptagon());
            AddRandomShape(CreateTriangle());
        }

        private void AddRandomShape(Shape shape)
        {

            var randomPosition = GenerateRandomPosition(shape);
            Canvas.SetLeft(shape, randomPosition.X);
            Canvas.SetTop(shape, randomPosition.Y);
            AddShape(shape);
        }

        private Rectangle CreateSquare()
        {

            return new Rectangle
            {
                Width = 80,
                Height = 80,
                Fill = Brushes.Blue
            };
        }

        private Ellipse CreateCircle()
        {
            return new Ellipse
            {
                Width = 80,
                Height = 80,
                Fill = Brushes.Yellow
            };
        }

        private Polygon CreateTriangle()
        {
            return new Polygon
            {
                Points = new Avalonia.Collections.AvaloniaList<Point>
        {
            new Point(40, 0),
            new Point(80, 80),
            new Point(0, 80)
        },
                Fill = Brushes.Orange
            };
        }

        private Polygon CreateHeptagon()
        {
            return new Polygon
            {
                Points = new Avalonia.Collections.AvaloniaList<Point>
        {
            new Point(40, 0),
            new Point(80, 20),
            new Point(70, 50),
            new Point(40, 70),
            new Point(10, 50),
            new Point(0, 20),
            new Point(20, 40)
        },
                Fill = Brushes.Brown
            };
        }


        private Polygon CreatePentagon()
        {

            return new Polygon
            {
                Points = new Avalonia.Collections.AvaloniaList<Point>
                {
                    new Point(40, 0),
                    new Point(80, 30),
                    new Point(65, 80),
                    new Point(15, 80),
                    new Point(0, 30)
                },
                Fill = Brushes.Green
            };
        }

        private Polygon CreateHexagon()
        {

            return new Polygon
            {
                Points = new Avalonia.Collections.AvaloniaList<Point>
                {
                    new Point(40, 0),
                    new Point(80, 20),
                    new Point(80, 60),
                    new Point(40, 80),
                    new Point(0, 60),
                    new Point(0, 20)
                },
                Fill = Brushes.Red
            };
        }

        private Polygon CreateOctagon()
        {

            return new Polygon
            {
                Points = new Avalonia.Collections.AvaloniaList<Point>
                {
                    new Point(30, 0),
                    new Point(50, 0),
                    new Point(80, 30),
                    new Point(80, 50),
                    new Point(50, 80),
                    new Point(30, 80),
                    new Point(0, 50),
                    new Point(0, 30)
                },
                Fill = Brushes.Purple
            };
        }

        private void AddShape(Shape shape)
        {
            // ��������� ������ � ������ � �� �����
            _shapes.Add(shape);
            _canvas.Children.Add(shape);
        }

        private void ClearShapes()
        {

            _shapes.Clear();
            _canvas.Children.Clear();
        }

        private void OnPointerPressed(object? sender, PointerPressedEventArgs e)
        {
            var point = e.GetPosition(_canvas);
            _draggedShape = null;

            // ���������, �� ����� ������ ����� ������������
            foreach (var shape in _shapes)
            {
                if (shape.Bounds.Contains(point))
                {
                    _draggedShape = shape;
                    _dragStart = point;
                    break;
                }
            }

            if (_draggedShape == null)
            {
                ShowMessage("�� �����!");
            }
            else
            {
                string shapeName = _draggedShape is Rectangle ? "�������" :
                                   _draggedShape is Ellipse ? "����" :
                                   _draggedShape is Polygon polygon ? (polygon.Points.Count == 3 ? "�����������" :
                                   polygon.Points.Count == 5 ? "������������" :
                                   polygon.Points.Count == 6 ? "�������������" :
                                   polygon.Points.Count == 7 ? "������������" : "��������������") :
                                   "������";

                ShowMessage($"�� ����� � {shapeName}");
            }
        }


        private void OnPointerMoved(object? sender, PointerEventArgs e)
        {
            if (_draggedShape != null)
            {
                // ���������� ������ ����� �� ���������� ����
                var currentPoint = e.GetPosition(_canvas);
                var delta = currentPoint - _dragStart;

                var newLeft = Canvas.GetLeft(_draggedShape) + delta.X;
                var newTop = Canvas.GetTop(_draggedShape) + delta.Y;

                // �������� ������ ������, ����� ������ �� �������� �� ��� �������
                if (newLeft < 0) newLeft = 0;
                if (newTop < 0) newTop = 0;
                if (newLeft + _draggedShape.Bounds.Width > _canvas.Bounds.Width) newLeft = _canvas.Bounds.Width - _draggedShape.Bounds.Width;
                if (newTop + _draggedShape.Bounds.Height > _canvas.Bounds.Height) newTop = _canvas.Bounds.Height - _draggedShape.Bounds.Height;

                Canvas.SetLeft(_draggedShape, newLeft);
                Canvas.SetTop(_draggedShape, newTop);

                _dragStart = currentPoint;
            }
        }

        private void OnPointerReleased(object? sender, PointerReleasedEventArgs e)
        {
            _draggedShape = null;
        }

        private async void ShowMessage(string message)
        {

            if (_messageTextBlock != null)
            {
                _canvas.Children.Remove(_messageTextBlock);
                _messageTextBlock = null;
            }


            _messageTextBlock = new TextBlock
            {
                Text = message,
                FontSize = 40,
                Foreground = Brushes.Black,
                HorizontalAlignment = Avalonia.Layout.HorizontalAlignment.Center,
                VerticalAlignment = Avalonia.Layout.VerticalAlignment.Top,
                TextAlignment = Avalonia.Media.TextAlignment.Center,
                Margin = new Thickness(0, 30, 0, 0)
            };

            // ����������� �� ������ ���� ���������
            Canvas.SetLeft(_messageTextBlock, (_canvas.Bounds.Width - _messageTextBlock.Bounds.Width) / 2);
            Canvas.SetTop(_messageTextBlock, 0);

            _canvas.Children.Add(_messageTextBlock);

            // ������� ��������� ����� 3 ���
            await Task.Delay(3000);
            if (_messageTextBlock != null)
            {
                _canvas.Children.Remove(_messageTextBlock);
                _messageTextBlock = null;
            }
        }

        private Point GenerateRandomPosition(Shape shape)
        {
            // ��������� ��������� ������ � �������� ������ �� ������� ������
            double x = _random.NextDouble() * (_canvas.Bounds.Width - shape.Bounds.Width);
            double y = _random.NextDouble() * (_canvas.Bounds.Height - shape.Bounds.Height);
            return new Point(x, y);
        }


        private void OnDrawSquareClick(object? sender, RoutedEventArgs e)
        {
            ClearShapes();

            var square = new Rectangle
            {
                Width = 80,
                Height = 80,
                Fill = Brushes.Blue
            };

            Canvas.SetLeft(square, (DrawingCanvas.Bounds.Width - square.Width) / 2);
            Canvas.SetTop(square, (DrawingCanvas.Bounds.Height - square.Height) / 2);

            AddShape(square);
        }

        private void OnDrawCircleClick(object? sender, RoutedEventArgs e)
        {
            ClearShapes();

            var circle = CreateCircle();

            Canvas.SetLeft(circle, (DrawingCanvas.Bounds.Width - circle.Width) / 2);
            Canvas.SetTop(circle, (DrawingCanvas.Bounds.Height - circle.Height) / 2);

            AddShape(circle);
        }

        private void OnDrawTriangleClick(object? sender, RoutedEventArgs e)
        {
            ClearShapes();

            var triangle = CreateTriangle();

            Canvas.SetLeft(triangle, (DrawingCanvas.Bounds.Width - 80) / 2);
            Canvas.SetTop(triangle, (DrawingCanvas.Bounds.Height - 80) / 2);

            AddShape(triangle);
        }

        private void OnDrawHeptagonClick(object? sender, RoutedEventArgs e)
        {
            ClearShapes();

            var heptagon = CreateHeptagon();

            Canvas.SetLeft(heptagon, (DrawingCanvas.Bounds.Width - 80) / 2);
            Canvas.SetTop(heptagon, (DrawingCanvas.Bounds.Height - 80) / 2);

            AddShape(heptagon);
        }


        private void OnDrawPentagonClick(object? sender, RoutedEventArgs e)
        {
            ClearShapes();

            var pentagon = new Polygon
            {
                Points = new Avalonia.Collections.AvaloniaList<Point>
                {
                    new Point(40, 0),
                    new Point(80, 30),
                    new Point(65, 80),
                    new Point(15, 80),
                    new Point(0, 30)
                },
                Fill = Brushes.Green
            };

            Canvas.SetLeft(pentagon, (DrawingCanvas.Bounds.Width - 80) / 2);
            Canvas.SetTop(pentagon, (DrawingCanvas.Bounds.Height - 80) / 2);

            AddShape(pentagon);
        }

        private void OnDrawHexagonClick(object? sender, RoutedEventArgs e)
        {
            ClearShapes();

            var hexagon = new Polygon
            {
                Points = new Avalonia.Collections.AvaloniaList<Point>
                {
                    new Point(40, 0),
                    new Point(80, 20),
                    new Point(80, 60),
                    new Point(40, 80),
                    new Point(0, 60),
                    new Point(0, 20)
                },
                Fill = Brushes.Red
            };

            Canvas.SetLeft(hexagon, (DrawingCanvas.Bounds.Width - 80) / 2);
            Canvas.SetTop(hexagon, (DrawingCanvas.Bounds.Height - 80) / 2);

            AddShape(hexagon);
        }

        private void OnDrawOctagonClick(object? sender, RoutedEventArgs e)
        {
            ClearShapes();

            var octagon = new Polygon
            {
                Points = new Avalonia.Collections.AvaloniaList<Point>
                {
                    new Point(30, 0),
                    new Point(50, 0),
                    new Point(80, 30),
                    new Point(80, 50),
                    new Point(50, 80),
                    new Point(30, 80),
                    new Point(0, 50),
                },
                Fill = Brushes.Purple
            };

            Canvas.SetLeft(octagon, (DrawingCanvas.Bounds.Width - 80) / 2);
            Canvas.SetTop(octagon, (DrawingCanvas.Bounds.Height - 80) / 2);

            AddShape(octagon);
        }
    }
}