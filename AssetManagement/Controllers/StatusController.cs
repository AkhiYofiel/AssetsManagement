using AssetManagementApi.Dtos.Status;
using AssetManagementApi.Services;
using Microsoft.AspNetCore.Mvc;

namespace AssetManagementApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class StatusController : ControllerBase
    {
        private readonly IStatusService _statusService;

        public StatusController(IStatusService statusService)
        {
            _statusService = statusService;
        }

        /// <summary>
        /// Gets all asset status.
        /// </summary>
        /// <returns>List of status</returns>
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> GetStatuses()
        {
            var statuses = await _statusService.GetStatusesAsync();
            return Ok(statuses);
        }

        /// <summary>
        /// Get a single status by id
        /// </summary>
        /// <param name="id">The status id</param>
        /// <returns>The requested status</returns>
        [HttpGet("{id:int}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetStatus(int id)
        {
            var status = await _statusService.GetStatusAsync(id);
            return status is null ? NotFound() : Ok(status);
        }

        /// <summary>
        /// Creates a new asset status
        /// </summary>
        /// <param name="createDto">The status data</param>
        /// <returns>The status</returns>
               [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        public async Task<IActionResult> CreateStatus(StatusCreateUpdateDto createDto)
        {
            var result = await _statusService.CreateStatusAsync(createDto);

            if (!result.Succeeded)
            {
                return result.IsNotFound ? NotFound() : BadRequest(result.Errors);
            }

            return CreatedAtAction(nameof(GetStatus), new { id = result.Data!.Id }, result.Data);
        }

        /// <summary>
        /// Updates an existing status.
        /// </summary>
        /// <param name="id">The status identifier.</param>
        /// <param name="updateDto">The status data to update.</param>
        /// <returns>No content.</returns>

        [HttpPut("{id:int}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> UpdateStatus(int id, StatusCreateUpdateDto updateDto)
        {
            var result = await _statusService.UpdateStatusAsync(id, updateDto);

            if (result.Succeeded)
            {
                return NoContent();
            }

            return result.IsNotFound ? NotFound() : BadRequest(result.Errors);
        }

        /// <summary>
        /// Deletes a status.
        /// </summary>
        /// <param name="id">The status identifier.</param>
        /// <returns>No content.</returns>
        [HttpDelete("{id:int}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> DeleteStatus(int id)
        {
            var result = await _statusService.DeleteStatusAsync(id);

            if (result.Succeeded)
            {
                return NoContent();
            }

            return result.IsNotFound ? NotFound() : BadRequest(result.Errors);
        }
    }
}