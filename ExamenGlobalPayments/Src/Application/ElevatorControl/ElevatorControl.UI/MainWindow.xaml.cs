using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using ElevatorControl.UI.Model;
using ElevatorControl.Application.Services;
using System.Windows.Media;

namespace ElevatorControl.UI
{
    public partial class MainWindow : Window
    {
        private const double DoorLeftClosedX = 60;
        private const double DoorRightClosedX = 100;
        private const double DoorOffset = 20;

        private readonly MainViewModel _viewModel;
        private readonly ElevatorController _controller;

        public MainWindow(MainViewModel viewModel, ElevatorController controller)
        {
            InitializeComponent();
            _viewModel = viewModel;
            _controller = controller;

            _controller.FloorArrived += (s, f) => Dispatcher.Invoke(() => AnimateElevator(f));
            _controller.DoorsStateChanged += (s, o) => Dispatcher.Invoke(() => AnimateDoors(o));
        }

        private void CallButton_Click(object s, RoutedEventArgs e)
        {
            try
            {
                if (s is Button b && int.TryParse(b.Tag.ToString(), out int f))
                    _controller.RequestFloor(f);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al procesar llamada externa: " + ex.Message);
            }
        }

        private void InternalButton_Click(object s, RoutedEventArgs e)
        {
            try
            {
                if (s is Button b && int.TryParse(b.Tag.ToString(), out int f))
                    _controller.RequestFloor(f);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al procesar llamada interna: " + ex.Message);
            }
        }

        private void AnimateElevator(int floor)
        {
            try
            {
                double y = (5 - floor) * 60;
                var dur = TimeSpan.FromMilliseconds(Math.Abs(_viewModel.CurrentFloor - floor) * _controller.FloorTravelTimeMs);
                var ani = new DoubleAnimation(y, dur);
                ElevatorBox.BeginAnimation(Canvas.TopProperty, ani);
                DoorLeft.BeginAnimation(Canvas.TopProperty, ani);
                DoorRight.BeginAnimation(Canvas.TopProperty, ani);
                _viewModel.CurrentFloor = floor;
                UpdateStatusPanel();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al animar elevador: " + ex.Message);
            }
        }

        private void AnimateDoors(bool open)
        {
            try
            {
                double leftX = open ? DoorLeftClosedX - DoorOffset : DoorLeftClosedX;
                double rightX = open ? DoorRightClosedX + DoorOffset : DoorRightClosedX;
                var dur = TimeSpan.FromMilliseconds(_controller.DoorOperationTimeMs);
                DoorLeft.BeginAnimation(Canvas.LeftProperty, new DoubleAnimation(leftX, dur));
                DoorRight.BeginAnimation(Canvas.LeftProperty, new DoubleAnimation(rightX, dur));
                UpdateStatusPanel();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al animar puertas: " + ex.Message);
            }
        }

        private void UpdateStatusPanel()
        {
            for (int f = 1; f <= 5; f++)
            {
                var e = (Ellipse)FindName($"Status{f}");
                e.Fill = (f == _viewModel.CurrentFloor)
                    ? (_controller.DoorsOpen ? Brushes.Green : Brushes.Red)
                    : Brushes.Gray;
            }
        }
    }
}
