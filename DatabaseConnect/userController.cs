using Application;
using Application.Commands;
using Application.Encryptions;
using Domain;
using Infrastructure;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Serilog;
using ILogger = Microsoft.Extensions.Logging.ILogger;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        
        private readonly IMediator _mediator;
        private readonly ILogger _logger;
 
        public UserController(IMediator mediator, ILogger<UserController> logger)
        {
            _mediator = mediator;
            _logger = logger;
        }

        [HttpPost("Select")]
        public async Task<ActionResult<List<UserAdd>>> Select([FromQuery] string serverType)
        {
            var users = await _mediator.Send(new GetAllUsersQuery { Servertype = serverType });
            _logger.LogInformation("Data retreived successfully from {ServerType}.", serverType);
            return Ok(users);
        }

        [HttpPost("Insert")]    
        public async Task<ActionResult> Insert([FromBody] UserAdd newUser, [FromQuery] string serverType)
        {
            if (string.IsNullOrEmpty(serverType))
            {
                _logger.LogWarning("Server infromation is required.");
                return BadRequest(new { message = "Server information is required." });
            }

            try
            {
                var command = new AddUserCommand
                {
                    User = newUser,
                    ServerType = serverType
                };

                //Sending the information 
                await _mediator.Send(command);

                _logger.LogInformation("Data inserted successfully into {ServerType}.", serverType);
                 Log.CloseAndFlushAsync();
                return Ok(new { message = "User added successfully." });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while adding the user.");
                return StatusCode(500, new { message = $"An error occurred: {ex.Message}" });
            }
        }


        [HttpPut("Update")]
        public async Task<ActionResult> Update([FromBody] UserAdd newUser, [FromQuery] string serverType)
        {
            if (string.IsNullOrEmpty(serverType))
            {
                return BadRequest(new { message = "Server information is required." });
            }

            try
            {
                var command = new AddUserCommand
                {
                    User = newUser,
                    ServerType = serverType
                };

                //Sending the information 
                await _mediator.Send(command);
                
                _logger.LogInformation($"Updated user: {newUser}");
                Log.CloseAndFlushAsync();
                return Ok(new { message = "User added successfully." });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while updating the user.");
                return StatusCode(500, new { message = $"An error occurred: {ex.Message}" });
            }
        }

        [HttpPost("encrypt-decrypt")]
        public async Task<ActionResult> EncryptPassword([FromQuery] string encryptionType, [FromBody] UserEncrypt encryptionRequest)
        {
            if (string.IsNullOrEmpty(encryptionType))
            {
                return BadRequest("Encryption type is required. Choose 'AES' or 'RSA'.");
            }

            if (encryptionRequest == null)
            {
                return BadRequest("Request body cannot be null.");
            }

            try
            {
                var encryptCommand = new EncryptPasswordCommand
                {
                    EncryptionType = encryptionType,
                    Password = encryptionRequest.Password
                };

                var encryptedPassword = await _mediator.Send(encryptCommand);

                var decryptCommand = new DecryptPasswordCommand
                {
                    EncryptionType = encryptionType,
                    EncryptedPassword = encryptedPassword
                };

                var decryptedPassword = await _mediator.Send(decryptCommand);
                _logger.LogInformation($"Password Encrypted and Decrypted  {decryptedPassword} ");
                return Ok(new
                {
                    Name = encryptionRequest.username,
                    OriginalPassword = encryptionRequest.Password,
                    EncryptedPassword = encryptedPassword,
                    DecryptedPassword = decryptedPassword
                });
                
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error encrypting/decrypting password.");
                return StatusCode(500, $"Error encrypting/decrypting password: {ex.Message}");
            }
        }
    }
}

