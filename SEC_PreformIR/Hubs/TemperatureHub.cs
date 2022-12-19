using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.SignalR;
using SEC_PreformIR.Models;

namespace SEC.PreformTempMonitor.Hubs
{
    [Route("/temperatureHub")]
    public class TemperatureHub : Hub
    {
        public Task NotifyConnection()
        {
            return Clients.All.SendAsync("TestBroadcasting", $"Testing a Basic HUB at {DateTime.Now.ToLocalTime()}");
        }

        public override async Task OnConnectedAsync()
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, "SignalR Users");
            await base.OnConnectedAsync();
        }

        public override async Task OnDisconnectedAsync(Exception exception)
        {
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, "SignalR Users");
            await base.OnDisconnectedAsync(exception);
        }

        public async Task NotifyTempChange(TemperatureModel model)
        {
            await Clients.All.SendAsync("NotifyTempChange", model);
        }

        public async Task NotifyTempArray(List<string> model)
        {
            await Clients.All.SendAsync("NotifyTempArray", model);
        }



    }
}
