using Application.Commands;
using Domain;
using MediatR;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Threading;
using System;
using Application.Encryptions;


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

    public class EncryptPasswordCommandHandler : IRequestHandler<EncryptPasswordCommand, string>
    {
        private readonly AESEncryption _aesEncryption;
        private readonly RSAEncryption _rsaEncryption;

        public EncryptPasswordCommandHandler(AESEncryption aesEncryption, RSAEncryption rsaEncryption)
        {
            _aesEncryption = aesEncryption;
            _rsaEncryption = rsaEncryption;
        }

        public Task<string> Handle(EncryptPasswordCommand request, CancellationToken cancellationToken)
        {
            if (request.EncryptionType.Equals("AES", StringComparison.OrdinalIgnoreCase))
            {
                return Task.FromResult(_aesEncryption.EncryptPassword(request.Password));
            }
            else if (request.EncryptionType.Equals("RSA", StringComparison.OrdinalIgnoreCase))
            {
                return Task.FromResult(_rsaEncryption.EncryptPassword(request.Password));
            }
            else
            {
                throw new ArgumentException("Invalid encryption type. Choose 'AES' or 'RSA'.");
            }
        }
    }
    public class DecryptPasswordCommandHandler : IRequestHandler<DecryptPasswordCommand, string>
    {
        private readonly AESEncryption _aesEncryption;
        private readonly RSAEncryption _rsaEncryption;

        public DecryptPasswordCommandHandler(AESEncryption aesEncryption, RSAEncryption rsaEncryption)
        {
            _aesEncryption = aesEncryption;
            _rsaEncryption = rsaEncryption;
        }

        public Task<string> Handle(DecryptPasswordCommand request, CancellationToken cancellationToken)
        {
            if (request.EncryptionType.Equals("AES", StringComparison.OrdinalIgnoreCase))
            {
                return Task.FromResult(_aesEncryption.DecryptPassword(request.EncryptedPassword));
            }
            else if (request.EncryptionType.Equals("RSA", StringComparison.OrdinalIgnoreCase))
            {
                return Task.FromResult(_rsaEncryption.DecryptPassword(request.EncryptedPassword));
            }
            else
            {
                throw new ArgumentException("Invalid encryption type. Choose 'AES' or 'RSA'.");
            }
        }
    }
}

