namespace ElevatorControl.Application.Interface
{
    public interface IElevatorService
    {
        Task MoveElevatorAsync(int targetFloor);
        int GetCurrentFloor();
        bool AreDoorsOpen();
    }
}
