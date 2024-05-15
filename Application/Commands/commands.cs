using Domain;
using MediatR;
using System.Collections.Generic;

namespace Application.Commands
{
    // Commands
    public class AddUserCommand : IRequest<Unit>
    {
        public UserAdd User { get; set; }
        public string ServerType { get; set; }
    }

    public class UpdateUserCommand : IRequest<Unit>
    {
        public UserAdd User { get; set; }
        public string ServerType { get; set; }

    }

    // Queries
    public class GetAllUsersQuery : IRequest<List<UserAdd>>
    {
        public string Servertype { get; set; }
    }

}


