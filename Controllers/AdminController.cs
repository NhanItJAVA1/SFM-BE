using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SFM_BE.DTOs.Categories;
using SFM_BE.Services.Admin;

namespace SFM_BE.Controllers
{
    [Route("api/v1/admin")]
    [Authorize(Policy = "AdminOnly")]
    public class AdminController : BaseController
    {
        private readonly IAdminService _adminService;

        public AdminController(IAdminService adminService)
        {
            _adminService = adminService;
        }
        [HttpGet("categories")]
        public async Task<IActionResult> GetDefaultCategories()
            => Ok(await _adminService.GetDefaultCategoriesAsync());

        [HttpPost("categories")]
        public async Task<IActionResult> CreateDefaultCategory(CreateCategoryDto dto)
            => Ok(await _adminService.CreateDefaultCategoryAsync(dto));

        [HttpPut("categories/{id:long}")]
        public async Task<IActionResult> UpdateDefaultCategory(long id, UpdateCategoryDto dto)
            => Ok(await _adminService.UpdateDefaultCategoryAsync(id, dto));

        [HttpDelete("categories/{id:long}")]
        public async Task<IActionResult> DeleteDefaultCategory(long id)
        {
            await _adminService.DeleteDefaultCategoryAsync(id);
            return NoContent();
        }
    }
}
