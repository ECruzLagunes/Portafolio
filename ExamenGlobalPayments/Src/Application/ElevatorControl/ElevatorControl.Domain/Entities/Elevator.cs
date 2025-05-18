namespace ElevatorControl.Domain.Entities
{
    public class Elevator
    {
        public int CurrentFloor { get; private set; } = 1;
        public bool DoorsOpen { get; private set; }
        public int FloorTravelTimeMs { get; set; } = 800;
        public int DoorOperationTimeMs { get; set; } = 1300;
    }
}
