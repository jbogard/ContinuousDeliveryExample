﻿namespace ContosoUniversity.Features.Course
{
    using System.Linq;
    using System.Threading.Tasks;
    using AutoMapper;
    using DAL;
    using FluentValidation;
    using MediatR;

    public class Details
    {
        public class Query : IAsyncRequest<Model>
        {
            public int? Id { get; set; }
        }

        public class Validator : AbstractValidator<Query>
        {
            public Validator()
            {
                RuleFor(m => m.Id).NotNull();
            }
        }

        public class Model
        {
            public int CourseID { get; set; }
            public string Title { get; set; }
            public int Credits { get; set; }
            public string DepartmentName { get; set; }
        }

        public class Handler : IAsyncRequestHandler<Query, Model>
        {
            private readonly SchoolContext _db;
            private readonly MapperConfiguration _config;

            public Handler(SchoolContext db, MapperConfiguration config)
            {
                _db = db;
                _config = config;
            }

            public async Task<Model> Handle(Query message)
            {
                return await _db.Courses.Where(i => i.CourseID == message.Id).ProjectToSingleOrDefaultAsync<Model>(_config);
            }
        }
    }
}