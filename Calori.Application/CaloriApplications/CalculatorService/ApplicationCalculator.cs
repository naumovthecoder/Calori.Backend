using Calori.Application.CaloriApplications.Commands.CreateApplication;
using Calori.Application.CaloriApplications.Commands.CreateApplication.CalculatorService;

namespace Calori.Application.CaloriApplications.CalculatorService
{
    public class ApplicationCalculator
    {
        public ApplicationBodyParametersDto CalculateApplicationParameters(CreateCaloriApplicationCommand request)
        {
            var result = new ApplicationBodyParametersDto();

            if (request.Weight.HasValue && request.Height.HasValue)
            {
                decimal weightKg = request.Weight.Value;
                decimal heightM = request.Height.Value / 100.0m; 

                decimal bmi = weightKg / (heightM * heightM);

                result.MaxWeight = (int)(24.9m * heightM * heightM);
                result.MinWeight = (int)(18.5m * heightM * heightM);

                result.BMI = bmi;
                
                if (request.Gender == 0)
                {
                    result.BMR = 
                        (66.47m + (13.75m * weightKg) + (5.0m * (heightM * 100)) - (6.74m * request.Age))!;
                }
                else
                {
                    result.BMR = 
                        (655.1m + (9.6m * weightKg) + (1.85m * (heightM * 100)) - (4.68m * request.Age))!;
                }
            }
            else
            {
                result.ErrorMessage = "Incorrect weight and height data.";
            }

            return result;
        }
    }
}