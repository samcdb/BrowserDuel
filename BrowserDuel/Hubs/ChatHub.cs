using Microsoft.AspNetCore.SignalR;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace BrowserDuel.Hubs
{
    public class ChatHub : Hub
    {
        int x => 2;
        
        public async override Task OnConnectedAsync()
        {
            await base.OnConnectedAsync();
            Console.WriteLine("Connection");
            var a = x;
        }
         
        public async Task SendMessage(string message)
        {
            Console.WriteLine(message);
            await Clients.All.SendAsync("Message", message);
        }
    }
}
