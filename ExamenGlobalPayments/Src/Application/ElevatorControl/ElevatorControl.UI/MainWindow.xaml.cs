using System;
using System.Collections.Generic;
using System.Linq;
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
        private readonly List<int> _requests = new List<int>();
        private int _direction = 0;
        private bool _processing = false;
        private bool _doorsOpen = false;
        private const double DoorLeftClosedX = 60;
        private const double DoorRightClosedX = 100;
        private const double DoorOffset = 20;
        private const int FloorTravelTimeMs = 1000;

        public MainWindow(MainViewModel vieWModel)
        {
            InitializeComponent();
            _vieWModel = vieWModel;
            UpdateStatusPanel();
        }

        private void CallButton_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button btn && int.TryParse(btn.Tag?.ToString(), out int floor))
                AddRequest(floor);
        }

        private void InternalButton_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button btn && int.TryParse(btn.Tag?.ToString(), out int floor))
                AddRequest(floor);
        }

        private void AddRequest(int floor)
        {
            if (!_requests.Contains(floor))
                _requests.Add(floor);
            if (!_processing)
                _ = ProcessRequestsAsync();
        }

        private async Task ProcessRequestsAsync()
        {
            _processing = true;
            DetermineDirection();
            while (_requests.Count > 0)
            {
                DetermineDirection();
                int target = GetNextStop();
                await MoveToFloorInternal(target);
            }
            _processing = false;
        }

        private void DetermineDirection()
        {
            if (_direction == 0 && _requests.Any())
            {
                _direction = _requests[0] > _vieWModel.CurrentFloor ? 1 : -1;
            }
            else if (!_requests.Any(r => _direction == 1 ? r > _vieWModel.CurrentFloor : r < _vieWModel.CurrentFloor))
            {
                _direction = -_direction;
            }
        }

        private int GetNextStop()
        {
            var inPath = _direction == 1
                ? _requests.Where(r => r > _vieWModel.CurrentFloor).OrderBy(r => r)
                : _requests.Where(r => r < _vieWModel.CurrentFloor).OrderByDescending(r => r);
            if (inPath.Any())
                return inPath.First();
            return _requests.First();
        }

        private async Task MoveToFloorInternal(int targetFloor)
        {
            if (_vieWModel.CurrentFloor != targetFloor)
            {
                if (_doorsOpen)
                {
                    AnimateDoorsClose();
                    _doorsOpen = false;
                    UpdateStatusPanel();
                    await Task.Delay(3500);
                }
                AnimateElevator(targetFloor);
                await Task.Delay(Math.Abs(_vieWModel.CurrentFloor - targetFloor) * FloorTravelTimeMs);
                _vieWModel.CurrentFloor = targetFloor;
            }
            AnimateDoorsOpen();
            _doorsOpen = true;
            UpdateStatusPanel();
            await Task.Delay(3500);
            _requests.Remove(targetFloor);
            UpdateStatusPanel();
        }

        private void AnimateElevator(int floor)
        {
            double targetY = (5 - floor) * 60;
            var duration = TimeSpan.FromMilliseconds(Math.Abs(_vieWModel.CurrentFloor - floor) * FloorTravelTimeMs);
            var ani = new DoubleAnimation(targetY, duration);
            ElevatorBox.BeginAnimation(Canvas.TopProperty, ani);
            DoorLeft.BeginAnimation(Canvas.TopProperty, ani);
            DoorRight.BeginAnimation(Canvas.TopProperty, ani);
        }

        private void AnimateDoorsClose()
        {
            var la = new DoubleAnimation(DoorLeftClosedX, TimeSpan.FromMilliseconds(500));
            var ra = new DoubleAnimation(DoorRightClosedX, TimeSpan.FromMilliseconds(500));
            DoorLeft.BeginAnimation(Canvas.LeftProperty, la);
            DoorRight.BeginAnimation(Canvas.LeftProperty, ra);
        }

        private void AnimateDoorsOpen()
        {
            var la = new DoubleAnimation(DoorLeftClosedX - DoorOffset, TimeSpan.FromMilliseconds(500));
            var ra = new DoubleAnimation(DoorRightClosedX + DoorOffset, TimeSpan.FromMilliseconds(500));
            DoorLeft.BeginAnimation(Canvas.LeftProperty, la);
            DoorRight.BeginAnimation(Canvas.LeftProperty, ra);
        }

        private void UpdateStatusPanel()
        {
            for (int floor = 1; floor <= 5; floor++)
            {
                var ellipse = (Ellipse)FindName($"Status{floor}");
                if (floor == _vieWModel.CurrentFloor)
                    ellipse.Fill = _doorsOpen ? System.Windows.Media.Brushes.Green : System.Windows.Media.Brushes.Red;
                else
                    ellipse.Fill = System.Windows.Media.Brushes.Gray;
            }
        }
    }
}