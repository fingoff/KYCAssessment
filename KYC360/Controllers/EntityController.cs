using KYC360.Models;
using KYC360.Services;
using Microsoft.AspNetCore.Mvc;

namespace KYC360.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class EntityController : ControllerBase
    {
        private readonly IEntityService _service;

        public EntityController(IEntityService service)
        {
            _service = service;
        }

        [HttpGet]
        public async Task<ActionResult<List<Entity>>> GetAll()
        {
            var entities = await _service.GetAllAsync();
            return Ok(entities);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Entity?>> GetById(string id)
        {
            var entity = await _service.GetByIdAsync(id);
            if (entity == null)
            {
                return NotFound();
            }
            return Ok(entity);
        }

        [HttpPost]
        public async Task<ActionResult> Create(Entity entity)
        {
            await _service.CreateAsync(entity);
            return CreatedAtAction(nameof(GetById), new { id = entity.Id }, entity);
        }

        [HttpPut("{id}")]
        public async Task<ActionResult> Update(string id, Entity entity)
        {
            if (id != entity.Id)
            {
                return BadRequest("ID mismatch");
            }

            var existingEntity = await _service.GetByIdAsync(id);
            if (existingEntity == null)
            {
                return NotFound();
            }

            await _service.UpdateAsync(entity);
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete(string id)
        {
            var existingEntity = await _service.GetByIdAsync(id);
            if (existingEntity == null)
            {
                return NotFound();
            }

            await _service.DeleteAsync(id);
            return NoContent();
        }

        [HttpGet("search")]
        public async Task<ActionResult<List<Entity>>> GetEntities(
            [FromQuery] string search,
            [FromQuery] string? gender,
            [FromQuery] DateTime? startDate,
            [FromQuery] DateTime? endDate,
            [FromQuery] List<string> countries,
            [FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = 10,
            [FromQuery] string sortBy = "FirstName",
            [FromQuery] string sortDirection = "asc")
        {
            try
            {
                var entities = await _service.GetEntitiesAsync(
                    search, gender, startDate, endDate, countries, 
                    pageNumber, pageSize, sortBy, sortDirection
                );
                return Ok(entities);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
    }
}