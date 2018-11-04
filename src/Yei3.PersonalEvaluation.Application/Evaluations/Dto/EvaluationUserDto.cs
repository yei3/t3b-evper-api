namespace Yei3.PersonalEvaluation.Evaluations.Dto
{
    using Abp.Application.Services.Dto;
    using Abp.AutoMapper;
    using Yei3.PersonalEvaluation.Authorization.Users;

    [AutoMap(typeof(User))]
    public class EvaluationUserDto : EntityDto<long>
    {
        public string EmployeeNumber { get; set; }
        public string Name { get; set; }
        public string Surname { get; set; }
    }
}