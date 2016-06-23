﻿namespace ContosoUniversity.Features.Department
{
    using System;
    using System.ComponentModel.DataAnnotations;
    using System.Linq;
    using System.Threading.Tasks;
    using AutoMapper;
    using DAL;
    using MediatR;

    public class Delete
    {
        public class Query : IAsyncRequest<Command>
        {
            public int Id { get; set; }
        }

        public class Command : IAsyncRequest
        {
            public string Name { get; set; }

            public decimal Budget { get; set; }

            public DateTime StartDate { get; set; }

            public int DepartmentID { get; set; }

            [Display(Name = "Administrator")]
            public string AdministratorFullName { get; set; }

            public byte[] RowVersion { get; set; }
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
                var department = await _db.Departments
                    .Where(d => d.DepartmentID == message.Id)
                    .ProjectToSingleOrDefaultAsync<Command>(_config);

                return department;
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
                var department = await _db.Departments.FindAsync(message.DepartmentID);

                _db.Departments.Remove(department);
            }
        }
    }
}