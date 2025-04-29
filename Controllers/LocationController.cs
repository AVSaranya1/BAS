using DataAccessLayer.Model;
using DataAccessLayer.Uow.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace WebApi.Controllers;

[Authorize]
[Route("api/[controller]")]
[ApiController]
public class LocationController : ApiBaseController
{    
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly ILogger<LocationController> _logger;
    private readonly IUnitOfWork _repo;

    public LocationController(
        IHttpContextAccessor httpContextAccessor,
        IConfiguration configuration,
        ILogger<LocationController> logger,
        IUnitOfWork repository
    ) : base(configuration)
    {
        _logger = logger;
        _httpContextAccessor = httpContextAccessor;
        _repo = repository ?? throw new ArgumentNullException(nameof(repository));
        _repo.SwitchDatabase(DatabaseType.Master);
    }


    //[HttpGet("GetLocation")]
    //public async Task<IActionResult> GetLocations()
    //{
        
    //    var result = await _repo.LocationDALRepo.GetLocationsAsync();

    //    if (result == null)
    //    {
    //        return NotFound();
    //    }
    //    return Ok(result);
    //}

    //[HttpGet("GetLocationById/{locationGUID}")]
    //public async Task<IActionResult> GetLocationById([FromRoute] Guid locationGUID)
    //{
    //    var result = await _repo.LocationDALRepo.GetLocationByIdAsync(locationGUID);

    //    if (result == null)
    //    {
    //        return NotFound();
    //    }
    //    return Ok(result);
    //}

    //[HttpPost("AddLocation")]
    //public async Task<IActionResult> AddLocation([FromBody] LocationModel locationModel)
    //{
    //    if (locationModel == null)
    //    {
    //        return BadRequest("Location model is null");
    //    }

    //    _repo.BeginTransaction();
    //    var result = await _repo.LocationDALRepo.AddLocationAsync(locationModel);
    //    await _repo.CompleteAsync();

    //    if (result > 0)
    //    {
    //        return CreatedAtAction(nameof(GetLocationById), new { locationGUID = locationModel.GUID }, locationModel);
    //    }
    //    return BadRequest("Failed to add location");
    //}

    //[HttpPut("UpdateLocation")]
    //public async Task<IActionResult> UpdateLocation([FromBody] LocationModel locationModel)
    //{
    //    if (locationModel == null)
    //    {
    //        return BadRequest("Location model is null");
    //    }

    //    _repo.BeginTransaction();
    //    var result = await _repo.LocationDALRepo.UpdateLocationAsync(locationModel);
    //    await _repo.CompleteAsync();

    //    if (result > 0)
    //    {
    //        return NoContent();
    //    }
    //    return BadRequest("Failed to update location");
    //}

    //[HttpDelete("DeleteLocation")]
    //public async Task<IActionResult> DeleteLocation([FromQuery] Guid locationGUID)
    //{
    //    if (locationGUID == Guid.Empty)
    //    {
    //        return BadRequest("Location GUID is empty");
    //    }

    //    _repo.BeginTransaction();
    //    var result = await _repo.LocationDALRepo.DeleteLocationAsync(locationGUID);
    //    await _repo.CompleteAsync();

    //    if (result > 0)
    //    {
    //        return NoContent();
    //    }
    //    return NotFound("Location not found");
    //}
}
