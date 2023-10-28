using System.Collections.Generic;
using Calori.Domain.Models.Enums;
using MediatR;

namespace Calori.Application.CaloriApplications.Commands.UpdateApplication
{
    public class UpdateApplicationCommand : IRequest
    {
        public int Id { get; set; }
        public CaloriGender? Gender { get; set; }
        public decimal? Weight { get; set; }
        public int? Height { get; set; }
        public int? Age { get; set; }
        public CaloriActivityLevel? Activity { get; set; }
        public int? Goal { get; set; }
        public string Email { get; set; } = string.Empty;
        public List<int>? Allergies { get; set; }
        public string AnotherAllergy { get; set; } = string.Empty;
    }
}