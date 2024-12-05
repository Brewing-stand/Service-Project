using System.Diagnostics;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
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
        
        private readonly IAuthorizationService _authorizationService;
        
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
            var userId = User.Identity.Name;

            // Fetch projects based on ownership and collaboration
            var result = await _projectRepository.GetAllProjectsByUserIdAsync(userId);
    
            if (result.IsFailed)
            {
                return NotFound(result.Errors.Select(e => e.Message));
            }

            var projects = result.Value;
            var projectDtos = _mapper.Map<IEnumerable<ProjectResponseDto>>(projects);
            return Ok(projectDtos);
        }

        // GET api/<ProjectController>/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Project>> GetProject(Guid id)
        {
            var result = await _projectRepository.GetProjectByIdAsync(id);

            if (result.IsFailed)
            {
                // Return a 404 Not Found response if the project doesn't exist
                return NotFound(result.Errors.Select(e => e.Message));
            }
            
            var project = result.Value;
            
            // Transform the project entity to ProjectResponseDto
            var responseDto = _mapper.Map<ProjectResponseDto>(project);

            // Return the project as a 200 OK response
            return Ok(responseDto);
        }
        
        // GET api/<ProjectController>/5/content
        [HttpGet("{id}/content")]
        public async Task<ActionResult> GetProjectContent(Guid id)
        {
            var projectResult = await _projectRepository.GetProjectByIdAsync(id);

            if (projectResult.IsFailed)
            {
                // Return a 404 Not Found response if the project doesn't exist
                return NotFound(projectResult.Errors.Select(e => e.Message));
            }
    
            // If project is found, fetch the content from blob storage
            var blobResult = await _blobRepository.GetContainerContent(id);

            if (blobResult.IsFailed)
            {
                // Return BadRequest if the blob storage content fetch failed
                return BadRequest(blobResult.Errors.Select(e => e.Message));
            }
            
            if (projectResult.Value == null)
            {
                return BadRequest("Project data is not available.");
            }

            if (blobResult.Value == null)
            {
                return BadRequest("Failed to retrieve blob content.");
            }
    
            // Map the project and content into the response DTO
            var projectDto = _mapper.Map<ProjectContentResponseDto>(projectResult.Value);

            // Add the Dictionary (blob content) to the DTO
            projectDto.Dictionary = blobResult.Value;

            return Ok(projectDto); // Return the project with content (including dictionary)
        }

        // POST api/<ProjectController>
        [HttpPost]
        public async Task<IActionResult> CreateProject([FromBody] ProjectRequestDto projectDto)
        {
            // Map the request DTO to the Project model
            var project = _mapper.Map<Project>(projectDto);
            
            // Add the project to the database
            var result = await _projectRepository.AddProjectAsync(project);

            if (result.IsFailed)
            {
                // Return a BadRequest response with the error message from FluentResults
                return BadRequest(result.Errors.Select(e => e.Message));
            }
            
            // Create the container in Blob Storage and set up the folder structure
            var createContainerResult = await _blobRepository.CreateContainerAsync(project.id);

            if (createContainerResult.IsFailed)
            {
                // Handle the error and return an appropriate response
                var errorMessages = string.Join(", ", createContainerResult.Errors.Select(e => e.Message));
                return BadRequest($"Error creating container: {errorMessages}");
            }
            
            // Return success response with status code 201 Created
            var responseDto = _mapper.Map<ProjectResponseDto>(project);
            return CreatedAtAction(nameof(GetProject), new { id = project.id }, responseDto);
        }

        // PUT api/<ProjectController>/5
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateProject(Guid id, [FromBody] ProjectRequestDto projectDto)
        {
            // Retrieve the project from the database
            var projectResult = await _projectRepository.GetProjectByIdAsync(id);

            if (projectResult.IsFailed)
            {
                // Return a 404 Not Found response if the project doesn't exist
                return NotFound(projectResult.Errors.Select(e => e.Message));
            }

            var project = projectResult.Value; // The existing project

            // Map the updated project data from the request DTO to the project entity
            _mapper.Map(projectDto, project);

            // Save the updated project back to the database
            var updateResult = await _projectRepository.UpdateProjectAsync(project);

            if (updateResult.IsFailed)
            {
                // Return a BadRequest response if the update failed
                return BadRequest(updateResult.Errors.Select(e => e.Message));
            }

            // Return the updated project as a response
            var updatedProjectDto = _mapper.Map<ProjectResponseDto>(project);
            
            return Ok(updatedProjectDto);
        }

        // DELETE api/<ProjectController>/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteProject(Guid id)
        {
            // Retrieve the project from the database
            var projectResult = await _projectRepository.GetProjectByIdAsync(id);

            if (projectResult.IsFailed)
            {
                // Return a 404 Not Found response if the project doesn't exist
                return NotFound(projectResult.Errors.Select(e => e.Message));
            }
            
            // Attempt to delete the project from the database
            var result = await _projectRepository.DeleteProjectAsync(id);

            if (result.IsFailed)
            {
                // Return a BadRequest response if the project couldn't be deleted from the database
                return BadRequest(result.Errors.Select(e => e.Message));
            }

            // Next, attempt to delete the container from Azure Blob Storage
            var deleteContainerResult = await _blobRepository.DeleteContainerAsync(id);

            if (deleteContainerResult.IsFailed)
            {
                // Return a BadRequest response if the container couldn't be deleted
                return BadRequest(deleteContainerResult.Errors.Select(e => e.Message));
            }
            
            return NoContent();
        }
    }
}
