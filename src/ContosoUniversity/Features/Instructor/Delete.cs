﻿namespace ContosoUniversity.Features.Instructor
{
    using System;
    using System.ComponentModel.DataAnnotations;
    using System.Data.Entity;
    using System.Linq;
    using System.Threading.Tasks;
    using AutoMapper;
    using DAL;
    using FluentValidation;
    using MediatR;
    using Models;

    public class Delete
    {
        public class Query : IAsyncRequest<Command>
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

        public class Command : IAsyncRequest
        {
            public int? ID { get; set; }

            public string LastName { get; set; }
            [Display(Name = "First Name")]
            public string FirstMidName { get; set; }

            [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}")]
            public DateTime? HireDate { get; set; }

            [Display(Name = "Location")]
            public string OfficeAssignmentLocation { get; set; }
        }

        public class QueryHandler : IAsyncRequestHandler<Query, Command>
        {
            private readonly SchoolContext _db;
            private readonly MapperConfiguration _config;

            public QueryHandler(SchoolContext db, MapperConfiguration config)
            {
                _db = db;
                _config = config;
            }

            public async Task<Command> Handle(Query message)
            {
                return await _db.Instructors.Where(i => i.ID == message.Id).ProjectToSingleOrDefaultAsync<Command>(_config);
            }
        }

        public class CommandHandler : AsyncRequestHandler<Command>
        {
            private readonly SchoolContext _db;

            public CommandHandler(SchoolContext db)
            {
                _db = db;
            }

            protected override async Task HandleCore(Command message)
            {
                Instructor instructor = await _db.Instructors
                    .Include(i => i.OfficeAssignment)
                    .Where(i => i.ID == message.ID)
                    .SingleAsync();

                instructor.OfficeAssignment = null;
                _db.Instructors.Remove(instructor);

                var department = await _db.Departments
                    .Where(d => d.InstructorID == message.ID)
                    .SingleOrDefaultAsync();
                if (department != null)
                {
                    department.InstructorID = null;
                }

            }
        }
    }

}