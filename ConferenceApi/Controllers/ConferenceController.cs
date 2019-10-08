using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ConferenceApi.Models;
using ConferenceApi.Services;
using ConferenceApi.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace ConferenceApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class ConferenceController : ControllerBase
    {
        private IConferenceService conferenceService;
        private ILogger logger;

        
        public ConferenceController(IConferenceService confSevice, ILogger<ConferenceController> logger)
        {
            conferenceService = confSevice;
            this.logger = logger;
        }

        /// <summary>
        /// Get all speakers and sessions
        /// </summary>
        /// <returns></returns>
        [HttpGet("allSessionsAndSpeakers")]
        [ProducesResponseType(typeof(SpeakerSessionCollection), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetAllSpeakersAndSessions()
        {
            try
            {
                var result = await conferenceService.GetCollectionAsync();
                if(result == null) // count
                {
                    return NotFound("Collection not found");
                }
                return Ok(result);
            }
            catch (Exception ex)
            {
                logger.LogError("GetAllSpeakersAndSessions - Error:", ex);
                return BadRequest("Invalid Get Request");
            }
        }

        // GET api/Conference/105
        /// <summary>
        /// Get session details
        /// </summary>
        /// <param name="id">session id</param>
        /// <returns></returns>
        [HttpGet("session/{id}")]
        [ProducesResponseType(typeof(Session), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetSession(int id)
        {
            try
            {
                var result = await conferenceService.GetSession(id);
                if (result == null)
                {
                    logger.LogInformation("Session not found for id " + id.ToString());
                    return NotFound($"Session Not found for id {id}");
                }
                return Ok(result);
            }
            catch (Exception ex)
            {
                logger.LogError("GetSession - Error", ex);   
                return BadRequest("Invalid Get Request");
            }
        }
    }
}
