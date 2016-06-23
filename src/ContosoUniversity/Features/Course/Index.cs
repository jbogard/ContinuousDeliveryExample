﻿namespace ContosoUniversity.Features.Course
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using AutoMapper;
    using DAL;
    using MediatR;
    using Models;

    public class Index
    {
        public class Query : IAsyncRequest<Result>
        {
            public Department SelectedDepartment { get; set; }
        }

        public class Result
        {
            public Department SelectedDepartment { get; set; }
            public List<Course> Courses { get; set; }

            public class Course
            {
                public int CourseID { get; set; }
                public string Title { get; set; }
                public int Credits { get; set; }
                public string DepartmentName { get; set; }
            }
        }

        public class Handler : IAsyncRequestHandler<Query, Result>
        {
            private readonly SchoolContext _db;
            private readonly MapperConfiguration _config;

            public Handler(SchoolContext db, MapperConfiguration config)
            {
                _db = db;
                _config = config;
            }

            public async Task<Result> Handle(Query message)
            {
                int? departmentID = message.SelectedDepartment == null 
                    ? (int?)null
                    : message.SelectedDepartment.DepartmentID;

                var courses = await _db.Courses
                    .Where(c => !departmentID.HasValue || c.DepartmentID == departmentID)
                    .OrderBy(d => d.CourseID)
                    .ProjectToListAsync<Result.Course>(_config);

                return new Result
                {
                    Courses = courses,
                    SelectedDepartment = message.SelectedDepartment
                };
            }
        }
    }
}