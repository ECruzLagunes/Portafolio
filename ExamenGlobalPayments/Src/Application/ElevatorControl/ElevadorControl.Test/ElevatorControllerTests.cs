namespace ElevadorControl.Test
{
    using global::ElevatorControl.Application.Services;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Xunit;

    namespace ElevatorControl.Application.Tests
    {
        public class ElevatorControllerTests
        {
            private ElevatorController CreateController()
            {
                var ctrl = new ElevatorController
                {
                    FloorTravelTimeMs = 0,
                    DoorOperationTimeMs = 0
                };
                return ctrl;
            }

            [Fact(DisplayName = "Solo se llama a un piso")]
            public async Task RequestSingleFloor_OpensDoorsAtSameFloor()
            {
                var ctrl = CreateController();
                bool doorsOpened = false;
                ctrl.DoorsStateChanged += (_, open) => doorsOpened = open;

                ctrl.RequestFloor(1);
                await Task.Delay(10);

                Assert.True(doorsOpened);
                Assert.Equal(1, ctrl.CurrentFloor);
            }

            [Fact(DisplayName = "Multiple peticion de pisos o llamados")]
            public async Task RequestMultipleFloors_StopsInOrder_UpDirection()
            {
                var ctrl = CreateController();
                var stops = new List<int>();
                ctrl.FloorArrived += (_, floor) => stops.Add(floor);

                ctrl.RequestFloor(3);
                ctrl.RequestFloor(5);
                ctrl.RequestFloor(4);

                await Task.Delay(100);

                // Orden esperado: 2->3->4->5
                // Desde que empieza en 1, priemro arriba en 2 y va de camino a 3
                Assert.Contains(3, stops);
                Assert.Contains(4, stops);
                Assert.Contains(5, stops);
                Assert.True(stops.IndexOf(3) < stops.IndexOf(4));
                Assert.True(stops.IndexOf(4) < stops.IndexOf(5));
            }
        }
    }

}
