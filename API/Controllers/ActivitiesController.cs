using Domain;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Persistence;

namespace API.Controllers
{
    public class ActivitiesController(AppDbContext context) : BaseApiController
    {
        [HttpGet]
        // The Task represents an asynchronous operation that will eventually return a value. In this case, a result of type ActionResult<List<Activity>>. ActionResult is a type that encapsulates the result of an action method, allowing for various HTTP responses.
        public async Task<ActionResult<List<Activity>>> GetActivities()
        {
            // Whenever we create database queries with Entity Framework, we have to use the await keyword to asynchronously wait for the operation to complete. This helps keep our application responsive and efficient. This will make sure that the thread is not blocked while waiting for the database operation to complete by executing the operation on a background thread.
            return await context.Activities.ToListAsync();
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Activity>> GetActivityDetail(string id)
        {
            var activity = await context.Activities.FindAsync(id);
            if (activity == null)
            {
                return NotFound();
            }
            return activity;
        }
    }
}
