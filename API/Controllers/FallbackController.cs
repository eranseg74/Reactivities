using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

[AllowAnonymous]
public class FallbackController : Controller
{
    public IActionResult Index()
    {
        // The PhysicalFile method Returns the file specified by physicalPath with the specified contentType as the Content-Type. The Combine method combines three strings into a path and return it.
        return PhysicalFile(Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "index.html"), "text/HTML");
    }
}
