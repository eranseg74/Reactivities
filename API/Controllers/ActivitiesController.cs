using Application.Activities.Commands;
using Application.Activities.DTOs;
using Application.Activities.Queries;
using Application.Core;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

public class ActivitiesController : BaseApiController
{
    [HttpGet]
    // The Task represents an asynchronous operation that will eventually return a value. In this case, a result of type ActionResult<List<Activity>>. ActionResult is a type that encapsulates the result of an action method, allowing for various HTTP responses.
    // In order to use the cancellation token, we need to add it as a parameter to the GetActivities method. This allows us to pass the cancellation token from the controller to the handler, where it can be used to cancel the database operation if needed. By including the cancellation token in the method signature, we can ensure that our application can handle cancellations gracefully and improve its responsiveness.
    public async Task<ActionResult<PagedList<ActivityDto, DateTime?>>> GetActivities([FromQuery] ActivityParams activityParams, CancellationToken token)
    {
        // Whenever we create database queries with Entity Framework, we have to use the await keyword to asynchronously wait for the operation to complete. This helps keep our application responsive and efficient. This will make sure that the thread is not blocked while waiting for the database operation to complete by executing the operation on a background thread.
        // Without Mediator
        //return await context.Activities.ToListAsync();
        // With Mediator
        return HandleResult(await Mediator.Send(new GetActivityList.Query { Params = activityParams }, token));
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<ActivityDto>> GetActivityDetail(string id, CancellationToken token)
    {
        return HandleResult(await Mediator.Send(new GetActivityDetails.Query { Id = id }, token));

    }

    [HttpPost]
    // Note that as a parameter we are getting the CreateActivityDto object
    public async Task<ActionResult<string>> CreateActivity(CreateActivityDto activityDto)
    {
        return HandleResult(await Mediator.Send(new CreateActivity.Command { ActivityDto = activityDto }));
    }

    [HttpPut("{id}")]
    [Authorize(Policy = "IsActivityHost")] // This is the policy we defined in the Program.cs file
    public async Task<ActionResult> EditActivity(string id, EditActivityDto activity)
    {
        activity.Id = id;
        return HandleResult(await Mediator.Send(new EditActivity.Command { ActivityDto = activity }));
    }

    [HttpDelete("{id}")]
    [Authorize(Policy = "IsActivityHost")]
    public async Task<ActionResult> DeleteActivity(string id)
    {
        await Mediator.Send(new DeleteActivity.Command { Id = id });
        return Ok();
    }

    [HttpPost("{id}/attend")]
    public async Task<ActionResult> Attend(string id)
    {
        return HandleResult(await Mediator.Send(new UpdateAttendance.Command { Id = id }));
    }
}
