namespace ElevatorControl.Domain.Entities
{
    public class Elevator
    {
        public int CurrentFloor { get; private set; } = 1;
        public bool IsMoving { get; private set; }
        public bool DoorsOpen { get; private set; }

        public void MoveTo(int targetFloor)
        {
            if (targetFloor == CurrentFloor) return;
            IsMoving = true;
            DoorsOpen = false;
            CurrentFloor = targetFloor;
            IsMoving = false;
            DoorsOpen = true;
        }
    }
}
