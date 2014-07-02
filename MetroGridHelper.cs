using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using Windows.UI;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Shapes;

namespace Helpers
{
    public static class MetroGridHelper
    {
        private static bool _visible;
        private static double _opacity = 0.6;
        private static Color _color = Colors.Red;
        private static List<Shape> _shapes;
        private static Grid _grid;

        /// <summary>
        /// Gets or sets a value indicating whether the designer grid is
        /// visible on top of the application's frame.
        /// </summary>
        public static bool IsVisible
        {
            get { return _visible; }
            set
            {
                _visible = value;
                UpdateGrid();
            }
        }

        /// <summary>
        /// Gets or sets the color to use for the grid's squares.
        /// </summary>
        public static Color Color
        {
            get { return _color; }
            set
            {
                _color = value;
                UpdateGrid();
            }
        }

        /// <summary>
        /// Gets or sets a value indicating the opacity for the grid's squares.
        /// </summary>
        public static double Opacity
        {
            get { return _opacity; }
            set
            {
                _opacity = value;
                UpdateGrid();
            }
        }

        /// <summary>
        /// Updates the grid (if it already has been created) or initializes it
        /// otherwise.
        /// </summary>
        private static void UpdateGrid()
        {
            if (_shapes != null)
            {
                var brush = new SolidColorBrush(_color);
                foreach (Shape square in _shapes)
                {
                    square.Fill = brush;
                }
                if (_grid != null)
                {
                    _grid.Visibility = _visible ? Visibility.Visible : Visibility.Collapsed;
                    _grid.Opacity = _opacity;
                }
            }
            else
            {
                BuildGrid();
            }
        }

        /// <summary>
        /// Builds the grid.
        /// </summary>
        private static async void BuildGrid()
        {
            _shapes = new List<Shape>();

            var frame = Window.Current.Content as Frame;
            if (frame == null || VisualTreeHelper.GetChildrenCount(frame) == 0)
            {
                await Window.Current.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, BuildGrid);
                return;
            }

            DependencyObject child = VisualTreeHelper.GetChild(frame, 0);
            var childAsBorder = child as Border;
            var childAsGrid = child as Grid;
            if (childAsBorder != null)
            {
                // Not a pretty way to control the root visual, but I did not
                // want to implement using a popup.
                UIElement content = childAsBorder.Child;
                if (content == null)
                {
                    await Window.Current.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, BuildGrid);
                    return;
                }
                childAsBorder.Child = null;
                await Window.Current.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
                {
                    var newGrid = new Grid();
                    childAsBorder.Child =
                        newGrid;
                    newGrid.Children.Add(
                        content);
                    PrepareGrid(frame,
                                newGrid);
                });
            }
            else if (childAsGrid != null)
            {
                PrepareGrid(frame, childAsGrid);
            }
            else
            {
                Debug.WriteLine("Dear developer:");
                Debug.WriteLine("Unfortunately the design overlay feature requires that the root frame visual");
                Debug.WriteLine("be a Border or a Grid. So the overlay grid just isn't going to happen.");
            }
        }

        public static void CreateGrid()
        {
            IsVisible = true;
        }

        /// <summary>
        /// Does the actual work of preparing the grid once the parent frame is
        /// in the visual tree and we have a Grid instance to work with for
        /// placing the chilren.
        /// </summary>
        /// <param name="frame">The phone application frame.</param>
        /// <param name="parent">The parent grid to insert the sub-grid into.</param>
        private static void PrepareGrid(Frame frame, Grid parent)
        {
            var brush = new SolidColorBrush(_color);

            _grid = new Grid { IsHitTestVisible = false };

            // To support both orientations, unfortunately more visuals need to
            // be used. An alternate implementation would be to react to the
            // orientation change event and re-draw/remove squares.
            double width = frame.ActualWidth;
            double height = frame.ActualHeight;
            double max = Math.Max(width, height);

            #if WINDOWS_APP

            const double strokeWidth = 2.0;

            var horizontalLine = new Line
            {
                IsHitTestVisible = false,
                Stroke = brush,
                X1 = 0,
                X2 = max,
                Y1 = 100 + (strokeWidth / 2),
                Y2 = 100 + (strokeWidth / 2),
                StrokeThickness = strokeWidth,
            };
            _grid.Children.Add(horizontalLine);
            _shapes.Add(horizontalLine);
            var horizontalLine2 = new Line
            {
                IsHitTestVisible = false,
                Stroke = brush,
                X1 = 0,
                X2 = max,
                Y1 = 140 + (strokeWidth / 2),
                Y2 = 140 + (strokeWidth / 2),
                StrokeThickness = strokeWidth,
            };
            _grid.Children.Add(horizontalLine2);
            _shapes.Add(horizontalLine2);

            var verticalLine = new Line
            {
                IsHitTestVisible = false,
                Stroke = brush,
                X1 = 120 - (strokeWidth / 2),
                X2 = 120 - (strokeWidth / 2),
                Y1 = 0,
                Y2 = max,
                StrokeThickness = strokeWidth,
            };
            _grid.Children.Add(verticalLine);
            _shapes.Add(verticalLine);

            var horizontalBottomLine = new Line
            {
                IsHitTestVisible = false,
                Stroke = brush,
                X1 = 0,
                X2 = max,
                Y1 = height - 130 + (strokeWidth / 2),
                Y2 = height - 130 + (strokeWidth / 2),
                StrokeThickness = strokeWidth,
            };
            _grid.Children.Add(horizontalBottomLine);
            _shapes.Add(horizontalBottomLine);
            var horizontalBottomLine2 = new Line
            {
                IsHitTestVisible = false,
                Stroke = brush,
                X1 = 0,
                X2 = max,
                Y1 = height - 50 + (strokeWidth / 2),
                Y2 = height - 50 + (strokeWidth / 2),
                StrokeThickness = strokeWidth,
            };
            _grid.Children.Add(horizontalBottomLine2);
            _shapes.Add(horizontalBottomLine2);
            
            #endif

            double tileWidth = 20;
            double tileHeight = 20;

            #if WINDOWS_PHONE_APP

            double x = 19;
            double y = 38;
            double block = 29.5;

            #else

            double x = 120;
            double y = 140;
            double block = 40;

            #endif

            for (; x < /*width*/ max; x += block)
            {
                #if WINDOWS_PHONE_APP
                y = 38;
                #else
                y = 140;
                #endif

                for (; y < /*height*/ max; y += block)
                {
                    var rect = new Rectangle
                    {
                        Width = tileWidth,
                        Height = tileHeight,
                        VerticalAlignment = VerticalAlignment.Top,
                        HorizontalAlignment = HorizontalAlignment.Left,
                        Margin = new Thickness(x, y, 0, 0),
                        IsHitTestVisible = false,
                        Fill = brush,
                    };
                    _grid.Children.Add(rect);
                    _shapes.Add(rect);
                }
            }

            _grid.Visibility = _visible ? Visibility.Visible : Visibility.Collapsed;
            _grid.Opacity = _opacity;

            // For performance reasons a single surface should ideally be used
            // for the grid.
            _grid.CacheMode = new BitmapCache();

            // Places the grid into the visual tree. It is never removed once
            // being added.
            parent.Children.Add(_grid);
        }
    }
}