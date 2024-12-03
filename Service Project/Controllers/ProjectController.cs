using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Service_Project.DTOs;
using Service_Project.Models;
using Service_Project.Repositories;

namespace Service_Project.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProjectController : ControllerBase
    {
        
        private readonly IProjectRepository _projectRepository;
        private readonly IBlobRepository _blobRepository;
        
        private readonly IMapper _mapper;

        public ProjectController(IProjectRepository projectRepository, IBlobRepository blobRepository, IMapper mapper)
        {
            _projectRepository = projectRepository;
            _blobRepository = blobRepository;
            
            _mapper = mapper;
        }
        
        // GET: api/<ProjectController>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Project>>> GetAllProjects()
        {
            throw new NotImplementedException();
        }

        // GET api/<ProjectController>/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Project>> GetProject(int id)
        {
            throw new NotImplementedException();
        }

        // POST api/<ProjectController>
        [HttpPost]
        public async Task<IActionResult> CreateProject([FromBody] ProjectRequestDto projectDto)
        {
            // Map the request DTO to the Project model
            var project = _mapper.Map<Project>(projectDto); 
            
            // Add the project to the database (using your repository)
            var result = await _projectRepository.AddProjectAsync(project);

            if (result.IsFailed)
            {
                // Return a BadRequest response with the error message from FluentResults
                return BadRequest(result.Errors.First().Message);
            }
            
            
            // Create the container in Blob Storage and set up the folder structure
            var createContainerResult = await _blobRepository.CreateContainerAsync(project.id);

            if (createContainerResult.IsFailed)
                {
                    // Handle the error and return an appropriate response
                    var errorMessages = string.Join(", ", createContainerResult.Errors.Select(e => e.Message));
                    return BadRequest($"Error creating container: {errorMessages}");
                }
            
            
            // Return success response
            var responseDto = _mapper.Map<ProjectResponseDto>(project);
            return Ok(responseDto);
        }

        // PUT api/<ProjectController>/5
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateProject(int id, [FromBody] Project project)
        {
            throw new NotImplementedException();
        }

        // DELETE api/<ProjectController>/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteProject(int id)
        {
            throw new NotImplementedException();
        }
    }
}
