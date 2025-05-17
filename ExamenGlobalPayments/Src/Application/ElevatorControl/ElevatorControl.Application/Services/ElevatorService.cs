using ElevatorControl.Application.Interface;
using ElevatorControl.Domain.Entities;

namespace ElevatorControl.Application.Services
{
    public class ElevatorService : IElevatorService
    {
        private readonly Elevator _elevator;

        public ElevatorService()
        {
            _elevator = new Elevator();
        }

        public async Task MoveElevatorAsync(int targetFloor)
        {
            await Task.Delay(Math.Abs(_elevator.CurrentFloor - targetFloor) * 1000);
            _elevator.MoveTo(targetFloor);
        }

        public int GetCurrentFloor() => _elevator.CurrentFloor;
        public bool AreDoorsOpen() => _elevator.DoorsOpen;
    }
}
