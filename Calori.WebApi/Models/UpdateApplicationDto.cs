using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;
using AutoMapper;
using Calori.Application.CaloriApplications.Commands.UpdateApplication;
using Calori.Application.Common.Mappings;
using Calori.Domain.Models.Enums;

namespace Calori.WebApi.Models
{
    public class UpdateApplicationDto : IMapWith<UpdateApplicationCommand>
    {
        [Required(ErrorMessage = "Gender is required.")]
        [Range(0, 1, ErrorMessage = "Your gender must be 0 or 1.")]
        public int Gender { get; set; }

        [Required(ErrorMessage = "Weight is required.")]
        [Range(30, 150, ErrorMessage = "Your weight must be between 30kg and 150kg.")]
        public decimal Weight { get; set; }

        [Required(ErrorMessage = "Height is required.")]
        [Range(100, 250, ErrorMessage = "Your height must be between 100cm and 250cm.")]
        public int Height { get; set; }

        [Required(ErrorMessage = "Age is required.")]
        [Range(12, 80, ErrorMessage = "Your age must be between 12 and 80 years.")]
        public int Age { get; set; }

        [Required(ErrorMessage = "Activity is required.")]
        [Range(0, 2, ErrorMessage = "Your activity level must be a positive integer between 0 and 2.")]
        public int Activity { get; set; }

        [Required(ErrorMessage = "Goal is required.")]
        [Range(20, 150, ErrorMessage = "Your goal weight must be between 20kg and 150kg.")]
        public int Goal { get; set; }

        [Required(ErrorMessage = "Email is required.")]
        [EmailAddress(ErrorMessage = "Invalid email address.")]
        public string Email { get; set; } = string.Empty;

        public List<int>? Allergies { get; set; }
        public string AnotherAllergy { get; set; } = string.Empty;
        
        public void Mapping(Profile profile)
        {
            profile.CreateMap<UpdateApplicationDto, UpdateApplicationCommand>();
        }
    }
}