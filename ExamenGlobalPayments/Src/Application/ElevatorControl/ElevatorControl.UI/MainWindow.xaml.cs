using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using ElevatorControl.UI.Model;

namespace ElevatorControl.UI
{
    public partial class MainWindow : Window
    {
        private readonly MainViewModel _vieWModel;
        private readonly Queue<int> _requestQueue = new Queue<int>();
        private bool _isProcessngQueue = false;
        private const double DoorLeftClosedX = 60;
        private const double DoorRightClosedX = 100;
        private const double DoorOffset = 20;

        public MainWindow(MainViewModel vieWModel)
        {
            InitializeComponent();
            _vieWModel = vieWModel;
            UpdateStatusPanel();
        }

        private void EnqueueRequest(int floor)
        {
            _requestQueue.Enqueue(floor);
            if (!_isProcessngQueue)
                _ = ProcessQueueAsync();
        }

        private async Task ProcessQueueAsync()
        {
            _isProcessngQueue = true;
            while (_requestQueue.Count > 0)
            {
                var nextFloor = _requestQueue.Dequeue();
                await MoveToFloorAsync(nextFloor);
            }
            _isProcessngQueue = false;
        }

        private void CallButton_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button btn && int.TryParse(btn.Tag?.ToString(), out int floor))
                EnqueueRequest(floor);
        }

        private void InternalButton_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button btn && int.TryParse(btn.Tag?.ToString(), out int floor))
                EnqueueRequest(floor);
        }

        private async Task MoveToFloorAsync(int targetFloor)
        {
            if (_vieWModel.CurrentFloor == targetFloor) return;

            AnimateDoorsClose();
            await Task.Delay(3500);

            var floorsToTrabel = Math.Abs(_vieWModel.CurrentFloor - targetFloor);
            AnimateElevator(targetFloor);
            await Task.Delay(floorsToTrabel * 1000);

            _vieWModel.CurrentFloor = targetFloor;

            AnimateDoorsOpen();
            await Task.Delay(3500);

            UpdateStatusPanel();
        }

        private void AnimateElevator(int floor)
        {
            double targetY = (5 - floor) * 60;
            var duration = TimeSpan.FromSeconds(Math.Abs(_vieWModel.CurrentFloor - floor));
            var ani = new DoubleAnimation { To = targetY, Duration = duration };

            ElevatorBox.BeginAnimation(Canvas.TopProperty, ani);
            DoorLeft.BeginAnimation(Canvas.TopProperty, ani);
            DoorRight.BeginAnimation(Canvas.TopProperty, ani);
        }

        private void AnimateDoorsClose()
        {
            var leftAnim = new DoubleAnimation { To = DoorLeftClosedX, Duration = TimeSpan.FromSeconds(0.5) };
            var rightAnim = new DoubleAnimation { To = DoorRightClosedX, Duration = TimeSpan.FromSeconds(0.5) };
            DoorLeft.BeginAnimation(Canvas.LeftProperty, leftAnim);
            DoorRight.BeginAnimation(Canvas.LeftProperty, rightAnim);
        }

        private void AnimateDoorsOpen()
        {
            var leftAnim = new DoubleAnimation { To = DoorLeftClosedX - DoorOffset, Duration = TimeSpan.FromSeconds(0.5) };
            var rightAnim = new DoubleAnimation { To = DoorRightClosedX + DoorOffset, Duration = TimeSpan.FromSeconds(0.5) };
            DoorLeft.BeginAnimation(Canvas.LeftProperty, leftAnim);
            DoorRight.BeginAnimation(Canvas.LeftProperty, rightAnim);
        }

        private void UpdateStatusPanel()
        {
            bool doorsOpen = Canvas.GetLeft(DoorLeft) < DoorLeftClosedX;
            for (int floor = 1; floor <= 5; floor++)
            {
                var ellipse = (Ellipse)FindName($"Status{floor}");
                bool isCurrent = floor == _vieWModel.CurrentFloor;
                if (!isCurrent)
                    ellipse.Fill = System.Windows.Media.Brushes.Gray;
                else if (doorsOpen)
                    ellipse.Fill = System.Windows.Media.Brushes.Green;
                else
                    ellipse.Fill = System.Windows.Media.Brushes.Red;
            }
        }
    }
}
