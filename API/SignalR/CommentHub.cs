using Application.Activities.Commands;
using Application.Activities.Queries;
using MediatR;
using Microsoft.AspNetCore.SignalR;

namespace API.SignalR;

// This will be the WebSocket controller which means that this will be the end point that clients will connect to
public class CommentHub(IMediator mediator) : Hub
{
    public async Task SendComment(AddComment.Command command)
    {
        var comment = await mediator.Send(command);
        // After creating the comment - send it to the group
        await Clients.Group(command.ActivityId).SendAsync("ReceiveComment", comment.Value);
    }

    // This method defines what we want to do when a client connects to this hub. The logic is that when a client connects we want to add him to the relevant SignalR group (according to thr activity id), so when a new comment is added it will be sent to all the users in the group
    public override async Task OnConnectedAsync()
    {
        // When a user makes a connection to SignalR, that is passed using HTTP but the rest of the communication is handled by WebSocket.
        // Using the Context object provided by SignalR to get the HttpContext assocciated with the connection
        var httpContext = Context.GetHttpContext();
        // Extracting the activity id from the http context
        var activityId = httpContext?.Request.Query["activityId"];
        // If the activity id is null or empty this means that there is no http request associated witht his connection
        if (string.IsNullOrEmpty(activityId))
        {
            throw new HubException("No activity with this id");
        }
        // Add the new connection to the relevant group by the activity id
        // We already checked that the activity id is not null or empty so we cann add the ! sign to avoid warnings
        await Groups.AddToGroupAsync(Context.ConnectionId, activityId!);

        var result = await mediator.Send(new GetComments.Query { ActivityId = activityId! });

        // Sending the caller (the connection to the hub)
        await Clients.Caller.SendAsync("LoadComments", result.Value); // result.Value is the list of comments
    }
}
