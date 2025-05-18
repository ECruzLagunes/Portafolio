using ElevatorControl.Application.Interface;

namespace ElevatorControl.Application.Services
{
    /// <summary>
    /// La clase ElevatorController simula la logica central de un elevador.
    /// Administra solicitudes de pisos, el movimiento del elevador entre pisos, la apertura/cierre de puertas, y controla la direccion del movimiento.
    /// Esta diseñada para funcionar desacoplada de la interfaz grafica, lo que facilita pruebas unitarias y reutilizacion.
    /// </summary>
    public class ElevatorController : IElevatorController
    {
        private readonly SortedSet<int> _upRequests = new SortedSet<int>();
        private readonly SortedSet<int> _downRequests = new SortedSet<int>();
        private int _direction;
        private bool _processing;

        public int CurrentFloor { get; private set; } = 1;
        public bool DoorsOpen { get; private set; }
        public int FloorTravelTimeMs { get; set; } = 800;
        public int DoorOperationTimeMs { get; set; } = 1300;

        public event EventHandler<int>? FloorArrived;
        public event EventHandler<bool>? DoorsStateChanged;

        /// <summary>
        /// Este metodo recive una solicitud para detener el elevador en un piso especifico. 
        /// Administra la logica de enrutamiento de solicitudes, 
        /// determinando si se debe abrir la puerta de inmediato (ya esta en el piso?)
        /// o si debe agregarse a la cola de pisos pendientes en la dirección apropiada (subida o bajada)
        /// </summary>
        /// <param name="floor">
        /// El número del piso solicitado por el usuario. 
        /// Este valor se compara contra el piso actual para decidir si se abre la puerta o se encola la solicitud.
        /// </param>
        public void RequestFloor(int floor)
        {
            if (!_processing && floor == CurrentFloor)
            {
                _ = OperateDoorsAsync(true);
                return;
            }
            if (floor > CurrentFloor) _upRequests.Add(floor);
            else if (floor < CurrentFloor) _downRequests.Add(floor);
            else if (_direction >= 0) _upRequests.Add(floor);
            else _downRequests.Add(floor);

            if (!_processing)
                _ = ProcessRequestsAsync();
        }

        /// <summary>
        /// Este metodo procesa las solicitudes de piso en cola, una a la vez, respetando la direccion actual del elevador (subiad o bajada).
        /// Controla la logica de movimiento y cambia de direccion cuando ya no hay más solicitudes en la direccion actual.
        /// </summary>
        private async Task ProcessRequestsAsync()
        {
            _processing = true;
            if (_direction == 0)
                _direction = _upRequests.Any() ? 1 : (_downRequests.Any() ? -1 : 0);

            while (_upRequests.Any() || _downRequests.Any())
            {
                if (_direction == 1 && _upRequests.Any())
                    await MoveStepwiseAsync(1);
                else if (_direction == -1 && _downRequests.Any())
                    await MoveStepwiseAsync(-1);
                else
                    _direction = -_direction;
            }

            _direction = 0;
            _processing = false;
        }

        /// <summary>
        /// Este metodo controla el movimeinto paso a paso del elevador en una direccion especifica (dir). Se encarga de:
        ///     Avanzar piso por piso.
        ///     Verificar si debe detenerse.
        ///     Abrir o cerrar las puertas cuando corresponde.
        ///     Continuar hasta que no haya más solicitudes en la dirección actual.
        /// </summary>
        /// <param name="dir">
        /// Dirección del movimiento:
        ///      1 -> Subida
        ///     -1 -> Bajada
        /// </param>
        private async Task MoveStepwiseAsync(int dir)
        {
            while (true)
            {
                int next = CurrentFloor + dir;
                bool has = dir == 1
                    ? _upRequests.Any(r => r >= next)
                    : _downRequests.Any(r => r <= next);
                if (!has) break;

                if (DoorsOpen)
                    await OperateDoorsAsync(false);

                FloorArrived?.Invoke(this, next);
                await Task.Delay(FloorTravelTimeMs);
                CurrentFloor = next;

                bool stopHere = (dir == 1 && _upRequests.Contains(CurrentFloor))
                             || (dir == -1 && _downRequests.Contains(CurrentFloor));
                if (stopHere)
                {
                    if (dir == 1) _upRequests.Remove(CurrentFloor);
                    else _downRequests.Remove(CurrentFloor);

                    await OperateDoorsAsync(true);
                }
            }
        }

        /// <summary>
        /// Este metodo controla la apertura o cierre de las puertas del elevador de manera asincronica.
        /// Notifica el estado de las puertas a traves de un evento,
        /// actualiza el estado interno y simula el tiempo de operacion de apertura/cierre.
        /// </summary>
        /// <param name="open">
        /// true: indica que las puertas deben abrirse.
        /// false: indica que las puertas deben cerrarse.
        /// </param>
        private async Task OperateDoorsAsync(bool open)
        {
            DoorsOpen = open;
            DoorsStateChanged?.Invoke(this, open);
            await Task.Delay(DoorOperationTimeMs);
        }
    }
}
