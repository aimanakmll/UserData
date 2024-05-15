using Application;
using Application.Commands;
using Domain;
using Infrastructure;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        
        private readonly IMediator _mediator;

        public UserController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPost("Select")]
        public async Task<ActionResult<List<UserAdd>>> Select([FromQuery] string serverType)
        {
            var users = await _mediator.Send(new GetAllUsersQuery { Servertype = serverType });
            return Ok(users);
        }

        [HttpPost("Insert")]
        public async Task<ActionResult> Insert([FromBody] UserAdd newUser, [FromQuery] string serverType)
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
    }
}
