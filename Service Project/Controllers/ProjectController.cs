using System.Diagnostics;
using System.Security.Claims;
using AutoMapper;
using FluentResults;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Service_Project.DTOs;
using Service_Project.Models;
using Service_Project.Repositories;

namespace Service_Project.Controllers
{
    [Authorize]
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
            var userIdResult = GetUserId();

            if (userIdResult.IsFailed)
            {
                // Return a BadRequest with the error messages if the Result failed
                return BadRequest(userIdResult.Errors.Select(e => e.Message));
            }

            var userId = userIdResult.Value; // Get the valid Guid from the Result
            
            // Removed userId and authentication check
            var result = await _projectRepository.GetAllProjectsByUserIdAsync(userId); // Adjusted method to fetch all projects

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
            var userIdResult = GetUserId();

            if (userIdResult.IsFailed)
            {
                // Return a BadRequest with the error messages if the Result failed
                return BadRequest(userIdResult.Errors.Select(e => e.Message));
            }
            
            var userId = userIdResult.Value; // Get the valid Guid from the Result
            
            var result = await _projectRepository.GetProjectByIdAsync(id, userId);

            if (result.IsFailed)
            {
                return NotFound(result.Errors.Select(e => e.Message));
            }
            
            var project = result.Value;
            var responseDto = _mapper.Map<ProjectResponseDto>(project);

            return Ok(responseDto);
        }
        
        // GET api/<ProjectController>/5/content
        [HttpGet("{id}/content")]
        public async Task<ActionResult<ProjectResponseDto>> GetProjectContent(Guid id)
        {
            var userIdResult = GetUserId();

            if (userIdResult.IsFailed)
            {
                // Return a BadRequest with the error messages if the Result failed
                return BadRequest(userIdResult.Errors.Select(e => e.Message));
            }
            
            var userId = userIdResult.Value; // Get the valid Guid from the Result
            
            var projectResult = await _projectRepository.GetProjectByIdAsync(id, userId);

            if (projectResult.IsFailed)
            {
                return NotFound(projectResult.Errors.Select(e => e.Message));
            }

            var blobResult = await _blobRepository.GetContainerContent(id);

            if (blobResult.IsFailed)
            {
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

            var projectDto = _mapper.Map<ProjectContentResponseDto>(projectResult.Value);
            projectDto.Dictionary = blobResult.Value;

            return Ok(projectDto);
        }

        // POST api/<ProjectController>
        [HttpPost]
        public async Task<IActionResult> CreateProject([FromBody] ProjectRequestDto projectDto)
        {
            var userIdResult = GetUserId();

            if (userIdResult.IsFailed)
            {
                // Return a BadRequest with the error messages if the Result failed
                return BadRequest(userIdResult.Errors.Select(e => e.Message));
            }
            
            var userId = userIdResult.Value; // Get the valid Guid from the Result
            
            var project = _mapper.Map<Project>(projectDto);
            var result = await _projectRepository.AddProjectAsync(project, userId);

            if (result.IsFailed)
            {
                return BadRequest(result.Errors.Select(e => e.Message));
            }

            var createContainerResult = await _blobRepository.CreateContainerAsync(project.id);

            if (createContainerResult.IsFailed)
            {
                var errorMessages = string.Join(", ", createContainerResult.Errors.Select(e => e.Message));
                return BadRequest($"Error creating container: {errorMessages}");
            }

            var responseDto = _mapper.Map<ProjectResponseDto>(project);
            return CreatedAtAction(nameof(GetProject), new { id = project.id }, responseDto);
        }

        // PUT api/<ProjectController>/5
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateProject(Guid id, [FromBody] ProjectRequestDto projectDto)
        {
            var userIdResult = GetUserId();

            if (userIdResult.IsFailed)
            {
                // Return a BadRequest with the error messages if the Result failed
                return BadRequest(userIdResult.Errors.Select(e => e.Message));
            }
            
            var userId = userIdResult.Value; // Get the valid Guid from the Result
            
            var projectResult = await _projectRepository.GetProjectByIdAsync(id, userId);

            if (projectResult.IsFailed)
            {
                return NotFound(projectResult.Errors.Select(e => e.Message));
            }

            var project = projectResult.Value;
            _mapper.Map(projectDto, project);

            var updateResult = await _projectRepository.UpdateProjectAsync(project, userId);

            if (updateResult.IsFailed)
            {
                return BadRequest(updateResult.Errors.Select(e => e.Message));
            }

            var updatedProjectDto = _mapper.Map<ProjectResponseDto>(project);
            return Ok(updatedProjectDto);
        }

        // DELETE api/<ProjectController>/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteProject(Guid id)
        {
            var userIdResult = GetUserId();

            if (userIdResult.IsFailed)
            {
                // Return a BadRequest with the error messages if the Result failed
                return BadRequest(userIdResult.Errors.Select(e => e.Message));
            }
            
            var userId = userIdResult.Value; // Get the valid Guid from the Result
            
            
            var projectResult = await _projectRepository.GetProjectByIdAsync(id, userId);

            if (projectResult.IsFailed)
            {
                return NotFound(projectResult.Errors.Select(e => e.Message));
            }

            var result = await _projectRepository.DeleteProjectAsync(id, userId);

            if (result.IsFailed)
            {
                return BadRequest(result.Errors.Select(e => e.Message));
            }

            var deleteContainerResult = await _blobRepository.DeleteContainerAsync(id);

            if (deleteContainerResult.IsFailed)
            {
                return BadRequest(deleteContainerResult.Errors.Select(e => e.Message));
            }

            return NoContent();
        }
        
        private Result<Guid> GetUserId()
        {
            var idString = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (string.IsNullOrEmpty(idString))
            {
                return Result.Fail<Guid>("User ID is missing or invalid.");
            }

            if (!Guid.TryParse(idString, out Guid userId))
            {
                return Result.Fail<Guid>("Invalid User ID format.");
            }

            return Result.Ok(userId); // Return the successfully parsed Guid
        }
    }
}
