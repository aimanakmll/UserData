using Application.Commands;
using Domain;
using MediatR;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Threading;
using System;


namespace Application.CommandHandlers
    
{
    // Query Handler
    public class GetAllUsersQueryHandler : IRequestHandler<GetAllUsersQuery, List<UserAdd>>
    {
        private readonly Func<string, Database> _databaseResolver;
     
        public GetAllUsersQueryHandler(Func<string, Database> databaseResolver)
        {
            _databaseResolver = databaseResolver;
        }

        public async Task<List<UserAdd>> Handle(GetAllUsersQuery request, CancellationToken cancellationToken)
        {
            if (string.IsNullOrEmpty(request.Servertype))
            {
                throw new ArgumentException("Server information is required.");
            }

            var database = _databaseResolver(request.Servertype.ToLower());
            if (database == null)
            {
                throw new ArgumentException("Invalid database type.");
            }
            return database.Select<UserAdd>("dbo.Users");
        }
    }

    // Command Handlers
    public class AddUserCommandHandler : IRequestHandler<AddUserCommand, Unit>
    {
        private readonly Func<string, Database> _databaseResolver;

        public AddUserCommandHandler(Func<string, Database> databaseResolver)
        {
            _databaseResolver = databaseResolver;
        }

        public async Task<Unit> Handle(AddUserCommand request, CancellationToken cancellationToken)
        {
            if (string.IsNullOrEmpty(request.ServerType))
            {
                throw new ArgumentException("Server information is required.");
            }

            var database = _databaseResolver(request.ServerType.ToLower());

            if (database == null)
            {
                throw new ArgumentException("Invalid server type specified.");
            }

            database.Insert(request.User);

            return Unit.Value;
        }
    }

    public class UpdateUserCommandHandler : IRequestHandler<UpdateUserCommand, Unit>
    {
        private readonly Func<string, Database> _databaseResolver;

        public UpdateUserCommandHandler(Func<string, Database> databaseResolver)
        {
            _databaseResolver = databaseResolver;
        }

        public async Task<Unit> Handle(UpdateUserCommand request, CancellationToken cancellationToken)
        {
            if (string.IsNullOrEmpty(request.ServerType))
            {
                throw new ArgumentException("Server information is required.");
            }

            var database = _databaseResolver(request.ServerType.ToLower());

            if (database == null)
            {
                throw new ArgumentException("Invalid server type specified.");
            }

            database.Insert(request.User);

            return Unit.Value;
        }
    }



}

