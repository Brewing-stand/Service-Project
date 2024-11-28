using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Service_Project.Models;
using Service_Project.Repositories;

namespace Service_Project.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProjectController : ControllerBase
    {
        
        private readonly IProjectRepository _projectRepository;

        public ProjectController(IProjectRepository projectRepository)
        {
            _projectRepository = projectRepository;
        }
        
        // GET: api/<ProjectController>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Project>>> GetAllProjects()
        {
            try
            {
                var projects = await _projectRepository.GetAllProjectsAsync();
                return Ok(projects);
            }
            catch (NotImplementedException)
            {
                return StatusCode(501, "Method not implemented.");
            }
        }

        // GET api/<ProjectController>/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Project>> GetProjectById(int id)
        {
            try
            {
                var project = await _projectRepository.GetProjectByIdAsync(id);
                if (project == null) return NotFound();
                return Ok(project);
            }
            catch (NotImplementedException)
            {
                return StatusCode(501, "Method not implemented.");
            }
        }

        // POST api/<ProjectController>
        [HttpPost]
        public async Task<IActionResult> CreateProject([FromBody] Project project)
        {
            try
            {
                await _projectRepository.AddProjectAsync(project);
                return CreatedAtAction(nameof(GetProjectById), new { id = project.Id }, project);
            }
            catch (NotImplementedException)
            {
                return StatusCode(501, "Method not implemented.");
            }
        }

        // PUT api/<ProjectController>/5
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateProject(int id, [FromBody] Project project)
        {
            try
            {
                if (id != project.Id) return BadRequest("ID mismatch");
                await _projectRepository.UpdateProjectAsync(project);
                return NoContent();
            }
            catch (NotImplementedException)
            {
                return StatusCode(501, "Method not implemented.");
            }
        }

        // DELETE api/<ProjectController>/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteProject(int id)
        {
            try
            {
                await _projectRepository.DeleteProjectAsync(id);
                return NoContent();
            }
            catch (NotImplementedException)
            {
                return StatusCode(501, "Method not implemented.");
            }
        }
    }
}
