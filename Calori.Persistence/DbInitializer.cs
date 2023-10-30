// using System.Linq;
// using Calori.Domain.Models.CaloriAccount;
//
// namespace Calori.Persistence
// {
//     public class DbInitializer
//     {
//         public static void Initialize(CaloriDbContext context)
//         {
//             if (!context.CaloriSlimmingPlan.Any())
//             {
//                 context.CaloriSlimmingPlan.AddRange(
//                     new CaloriSlimmingPlan { Calories = 1250 },
//                     new CaloriSlimmingPlan { Calories = 1500 },
//                     new CaloriSlimmingPlan { Calories = 1750 },
//                     new CaloriSlimmingPlan { Calories = 2000 },
//                     new CaloriSlimmingPlan { Calories = 2250 },
//                     new CaloriSlimmingPlan { Calories = 2500 }
//                 );
//                 context.SaveChanges();
//             }
//             context.Database.EnsureCreated();
//         }
//     }
// }
