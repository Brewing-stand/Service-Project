using AutoMapper;
using Service_Project.Models;
using Service_Project.DTOs;

public class ProjectMappingProfile : Profile
{
    public ProjectMappingProfile()
    {
        // Mapping from Project to ProjectResponseDto
        CreateMap<Project, ProjectResponseDto>();

        // Mapping from ProjectRequestDto to Project
        CreateMap<ProjectRequestDto, Project>();
    }
}