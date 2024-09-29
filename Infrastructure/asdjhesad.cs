//using Microsoft.FeatureManagement;
//using Microsoft.OpenApi.Models;
//using Swashbuckle.AspNetCore.SwaggerGen;
//using UtahPublicEd.StudentEdfiService.Domain.Interfaces;
//using UtahPublicEd.StudentEdfiService.Infrastructure.Repositories;
//using UtahPublicEd.StudentEdfiService.Infrastructure.Utils;

//namespace UtahPublicEd.StudentEdfiService.Application.EndpointsExtensions
//{
//    /// <summary>
//    /// Provides the enrollment and grade information                           
//    /// </summary>
//    public static class AssociationEndpointExtension
//    {
//        /// <summary>
//        /// Getting the student Ids and enrollment data for which the student enrolled in LEA and have assessments
//        /// </summary>
//        public static void AddAsscociationEndpoints(this IEndpointRouteBuilder endpointRouteBuilder)
//        {
//            Associationfactory.GenerateAssociationEndPointExtension(endpointRouteBuilder);
//            var featureManager = endpointRouteBuilder.se
//        }

//    }
//    /// <summary>
//    /// 
//    /// </summary>
//    public static class Associationfactory
//    {
//        /// <summary>
//        /// Getting the student Ids and enrollment data for which the student enrolled in LEA and have assessments
//        /// </summary>
//        /// <param name="endpointRouteBuilder"></param>
//        public async void GenerateAssociationEndPointExtension(IEndpointRouteBuilder endpointRouteBuilder, IFeatureManager )
//        {
//            var endpoints = endpointRouteBuilder.MapGroup("api/resources").WithGroupName("resources");

//            if (!await _featureManager.IsEnabledAsync(""))
//            {
//                return;
//            }

//            endpoints.MapGet("/{leaId}", async (IEnrollmentUtilityService _enrollmentUtilityService, IStudentAssessmentsRepository _studentAssessmentsRepository, ILogger<Associationfactory> logger, string leaId) =>
//            {

//                var studentEnrollmentIds = await _enrollmentUtilityService.GetStudentIdsByLeaId(leaId);

//                var dictionary = new Dictionary<string, object>();

//                if (studentEnrollmentIds.Item1 is null || !studentEnrollmentIds.Item1.Any())
//                {
//                    return Results.NotFound();
//                }
//                var studentAssessmentIds = await _studentAssessmentsRepository.GetStudentAssessments(studentEnrollmentIds.Item1);

//                if (studentAssessmentIds is null || !studentAssessmentIds.Any())
//                {
//                    return Results.NotFound();
//                }
//                dictionary.Add("Grades", studentEnrollmentIds.Item2!);
//                dictionary.Add("StudentEnrollmentIds", studentEnrollmentIds.Item1);
//                dictionary.Add("StudentAssessmentIds", studentAssessmentIds);
//                dictionary.Add("TotalCount", studentAssessmentIds.Count());

//                return Results.Ok(dictionary);
//            });

//        }
//    }
//}
