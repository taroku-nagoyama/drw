using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace DRW.Controls
{
    public class SkyControl : UserControl
    {
        const int shadeY = 20;
        readonly Pen MyPenTrans;
        readonly Pen MyPenBlack, MyPenRed;
        readonly Brush solidNight, solidDay, gradNight2Day, gradDay2Night;
        readonly Random rand = new Random();

        public Point? MousePos { get; private set; }

        public SkyControl()
        {
            MyPenTrans = new Pen(Brushes.Transparent, 0);
            MyPenBlack = new Pen(Brushes.Black, 1);
            MyPenRed = new Pen(Brushes.Red, 1);
            solidNight = Brushes.DarkCyan;
            solidDay = Brushes.White;
            gradNight2Day = new LinearGradientBrush(Colors.DarkCyan, Colors.White, 0);
            gradDay2Night = new LinearGradientBrush(Colors.White, Colors.DarkCyan, 0);

            this.MinWidth = 10;
            this.MinHeight = 50;

            MouseEnter += SkyControl_MouseEnter;
            MouseMove += SkyControl_MouseMove;
            MouseDown += SkyControl_MouseDown;
            MouseLeave += SkyControl_MouseLeave;
        }

        private void SkyControl_MouseEnter(object sender, System.Windows.Input.MouseEventArgs e)
            => UpdateMousePos(e.GetPosition(this));

        private void SkyControl_MouseMove(object sender, System.Windows.Input.MouseEventArgs e)
            => UpdateMousePos(e.GetPosition(this));

        private void SkyControl_MouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            UpdateMousePos(e.GetPosition(this));
            if (e.RightButton == System.Windows.Input.MouseButtonState.Pressed)
            {
                if (SkyChanged != null)
                {
                    var m = 24 * 60 * MousePos.Value.X / ActualWidth;
                    SkyChanged.Execute(TimeSpan.FromMinutes(m).Add(TimeSpan.FromSeconds(rand.Next(60))));
                }
            }
        }

        private void SkyControl_MouseLeave(object sender, System.Windows.Input.MouseEventArgs e)
            => UpdateMousePos(null);

        private void UpdateMousePos(Point? mousePos)
        {
            MousePos = mousePos;
            InvalidateVisual();
        }

        protected override void OnRender(DrawingContext g)
        {
            base.OnRender(g);
            g.DrawRectangle(solidNight, MyPenTrans, new Rect(0, shadeY, ActualWidth/6, ActualHeight - shadeY));
            g.DrawRectangle(gradNight2Day, MyPenTrans, new Rect(ActualWidth / 6, shadeY, ActualWidth/6, ActualHeight - shadeY));
            g.DrawRectangle(solidDay, MyPenTrans, new Rect(ActualWidth / 3, shadeY, ActualWidth/2, ActualHeight - shadeY));
            g.DrawRectangle(gradDay2Night, MyPenTrans, new Rect(5 * ActualWidth / 6, shadeY, ActualWidth/6, ActualHeight - shadeY));
            //g.DrawRectangle(solidNight, MyPenTrans, new Rect(5 * ActualWidth / 6, shadeY, ActualWidth / 6, ActualHeight - shadeY));
            g.DrawEllipse(Brushes.Yellow, MyPenTrans, new Point(ActualWidth / 2, shadeY + 10), 20, 20);

            for (int i = 0; i < 24; i++)
            {
                var labelX = i * ActualWidth / 24.0;
                g.DrawText(new FormattedText(
                    $"{i}h",
                    CultureInfo.CurrentCulture,
                    FlowDirection.LeftToRight,
                    //new Typeface(FontFamily, FontStyle, FontWeight, FontStretch),
                    new Typeface("Verdana"),
                    10,
                    Brushes.Black
                    ), new Point(labelX+2, 0));
                g.DrawLine(MyPenBlack, new Point(labelX, 0), new Point(labelX, ActualHeight));
            }

            if (MousePos != null)
            {
                var labelX = MousePos.Value.X;
                g.DrawLine(MyPenRed, new Point(labelX, 0), new Point(labelX, ActualHeight));
            }

            //if (DraggyImageSequence?.CurImageFilenameFull != null)
            //{
            //    var bs = new BitmapImage(new Uri(DraggyImageSequence.CurImageFilenameFull, UriKind.Absolute));
            //    var imageRelativeHeight = bs.Height * ActualWidth / bs.Width;
            //    var yOffset = DraggyImageSequence.VerticalScroll * ActualHeight / 4;
            //    g.DrawImage(bs, new Rect(0, -yOffset, ActualWidth, imageRelativeHeight));
            //}
        }


        public ICommand SkyChanged
        {
            get { return (ICommand)GetValue(SkyChangedProperty); }
            set { SetValue(SkyChangedProperty, value); }
        }

        // Using a DependencyProperty as the backing store for SkyChanged.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty SkyChangedProperty =
            DependencyProperty.Register("SkyChanged", typeof(ICommand), typeof(SkyControl),
                        new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.AffectsRender, OnSkyChangedPropertyChanged));

        private static void OnSkyChangedPropertyChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            var control = sender as SkyControl;
            if (control != null)
                control.OnSkyChangedChanged((ICommand)e.OldValue, (ICommand)e.NewValue);
        }

        private void OnSkyChangedChanged(ICommand oldValue, ICommand newValue) { }


    }
}
