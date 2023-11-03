using System.ComponentModel.DataAnnotations;
using AutoMapper;
using Calori.Application.Common.Mappings;
using Calori.Application.Feedback;

namespace Calori.WebApi.Models
{
    public class FeedbackDto : IMapWith<CreateFeedbackCommand>
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }
        
        [Required]
        [Phone]
        public string Phone { get; set; }
        
        [Required]
        [MaxLength(400)]
        public string Message { get; set; }
        
        public void Mapping(Profile profile)
        {
            profile.CreateMap<FeedbackDto, CreateFeedbackCommand>();
        }
    }
}