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
        private readonly AESEncryption _aesEncryption;
        private readonly RSAEncryption _rsaEncryption;
        public UserController(IMediator mediator, ILogger<UserController> logger,  RSAEncryption rsaEncryption, AESEncryption aESEncryption)
        {
            _mediator = mediator;
            _logger = logger;
            _rsaEncryption = rsaEncryption;
            _aesEncryption = aESEncryption;
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

                _logger.LogInformation("Data retrieved successfully from {ServerType}.", serverType);
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

                return Ok(new { message = "User added successfully." });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = $"An error occurred: {ex.Message}" });
            }
        }

        [HttpPost("encrypt")]
        public ActionResult EncryptPassword([FromQuery] string encryptionType, [FromBody] UserEncrypt encryptionRequest)
        {
            _logger.LogInformation("Encrypt request received with encryption type {EncryptionType}", encryptionType);

            if (string.IsNullOrEmpty(encryptionType))
            {
                _logger.LogWarning("Encryption type is missing in the request.");
                return BadRequest("Encryption type is required. Choose 'AES' or 'RSA'.");
            }

            if (encryptionRequest == null)
            {
                _logger.LogWarning("Request body is null.");
                return BadRequest("Request body cannot be null.");
            }

            try
            {
                string encryptedPassword;
                string decryptedPassword;

                if (encryptionType.Equals("AES", StringComparison.OrdinalIgnoreCase))
                {
                    _logger.LogInformation("Using AES encryption.");
                    encryptedPassword = _aesEncryption.EncryptPassword(encryptionRequest.Password);
                    decryptedPassword = _aesEncryption.DecryptPassword(encryptedPassword);
                }
                else if (encryptionType.Equals("RSA", StringComparison.OrdinalIgnoreCase))
                {
                    _logger.LogInformation("Using RSA encryption.");
                    encryptedPassword = _rsaEncryption.EncryptPassword(encryptionRequest.Password);
                    decryptedPassword = _rsaEncryption.DecryptPassword(encryptedPassword);
                }
                else
                {
                    _logger.LogWarning("Invalid encryption type provided: {EncryptionType}", encryptionType);
                    return BadRequest("Invalid encryption type. Choose 'AES' or 'RSA'.");
                }

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
                _logger.LogError(ex, "Error encrypting/decrypt");
                return StatusCode(500, $"Error encrypting/decrypting password: {ex.Message}");
            }
        }
    }
}
