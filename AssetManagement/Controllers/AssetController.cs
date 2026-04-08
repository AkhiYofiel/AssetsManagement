using AssetManagementApi.Dtos.Assets;
using AssetManagementApi.Services;
using Microsoft.AspNetCore.Mvc;

namespace AssetManagementApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AssetsController : ControllerBase
    {
        private readonly IAssetService _assetService;

        public AssetsController(IAssetService assetService)
        {
            _assetService = assetService;
        }

        /// <summary>
        /// Gets all assets
        /// </summary>
        /// <returns>List of assets</returns>
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> GetAssets()
        {
            var assets = await _assetService.GetAssetsAsync();
            return Ok(assets);
        }

        /// <summary>
        /// Get an asset
        /// </summary>
        /// <param name="id">The asset id</param>
        /// <returns>The asset</returns>
        [HttpGet("{id:int}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetAsset(int id)
        {
            var asset = await _assetService.GetAssetAsync(id);
            return asset is null ? NotFound() : Ok(asset);
        }

        /// <summary>
        /// Creates a new asset
        /// </summary>
        /// <param name="createDto">The asset data to create</param>
        /// <returns>The created asset</returns>
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> CreateAsset(AssetCreateDto createDto)
        {
            var result = await _assetService.CreateAssetAsync(createDto);

            if (!result.Succeeded)
            {
                return result.IsNotFound ? NotFound() : BadRequest(result.Errors);
            }

            return CreatedAtAction(nameof(GetAsset), new { id = result.Data!.Id }, result.Data);
        }

        /// <summary>
        /// Updates an existing asset
        /// </summary>
        /// <param name="id">The asset id</param>
        /// <param name="updateDto">The asset data to update</param>
        /// <returns></returns>
        [HttpPut("{id:int}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> UpdateAsset(int id, AssetUpdateDto updateDto)
        {
            var result = await _assetService.UpdateAssetAsync(id, updateDto);

            if (result.Succeeded)
            {
                return NoContent();
            }

            return result.IsNotFound ? NotFound() : BadRequest(result.Errors);
        }

        /// <summary>
        /// Deletes an asset
        /// </summary>
        /// <param name="id">The asset id</param>
        /// <returns></returns>
        [HttpDelete("{id:int}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> DeleteAsset(int id)
        {
            var result = await _assetService.DeleteAssetAsync(id);

            if (result.Succeeded)
            {
                return NoContent();
            }

            return result.IsNotFound ? NotFound() : BadRequest(result.Errors);
        }

        /// <summary>
        /// Assigns a software license to an asset.
        /// </summary>
        /// <param name="id">The asset identifier.</param>
        /// <param name="licenseId">The software license identifier.</param>
        /// <returns>No content.</returns>
        [HttpPost("{id:int}/licenses/{licenseId:int}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> AssignLicense(int id, int licenseId)
        {
            var result = await _assetService.AssignLicenseAsync(id, licenseId);

            if (result.Succeeded)
            {
                return NoContent();
            }

            return result.IsNotFound ? NotFound() : BadRequest(result.Errors);
        }
    }
}