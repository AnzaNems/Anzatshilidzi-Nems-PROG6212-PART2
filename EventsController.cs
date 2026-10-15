/// <summary>
/// Manages road race events. Public read access, Organiser-only write access.
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class EventsController : ControllerBase
{
    // ...

    /// <summary>
    /// Retrieves all upcoming road events. Public endpoint - no authentication required.
    /// </summary>
    /// <response code="200">Returns the list of events</response>
    [HttpGet]
    public async Task<IActionResult> GetAllEvents()
