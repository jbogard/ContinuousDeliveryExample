﻿namespace ContosoUniversity.Features.Instructor
{
    using System;
    using System.ComponentModel.DataAnnotations;
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
            public int? ID { get; set; }

            public string LastName { get; set; }
            [Display(Name = "First Name")]
            public string FirstMidName { get; set; }

            [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}")]
            public DateTime? HireDate { get; set; }

            [Display(Name = "Location")]
            public string OfficeAssignmentLocation { get; set; }
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
                return await _db.Instructors.Where(i => i.ID == message.Id).ProjectToSingleOrDefaultAsync<Model>(_config);
            }
        }
    }
}