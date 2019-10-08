
using ConferenceApi.Controllers;
using ConferenceApi.Models;
using ConferenceApi.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using NSubstitute;
using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace ConferenceApi.Tests.Controllers
{
    public class ConferenceControllerTest
    {
        private readonly ConferenceController conferenceController;
        private readonly IConferenceService conferenceService;
        private readonly ILogger<ConferenceController> logger;

        public ConferenceControllerTest()
        {
            conferenceService = Substitute.For<IConferenceService>();
            logger = Substitute.For<ILogger<ConferenceController>>();
            conferenceController = new ConferenceController(conferenceService,logger);
        }

        [Fact]
        public async void GetAllSessionsAndSpeakers_ReturnsOkResult()
        {
            //Arrange
            SpeakerSessionCollection speakerSessionCollection = new SpeakerSessionCollection()
            {
                SpeakersList = new List<ResponseItem>()
                {
                    new ResponseItem()
                    {
                        Id = 1
                    },
                    new ResponseItem()
                    {
                        Id = 2
                    }
                },

                SessionsList = new List<ResponseItem>()
                {
                    new ResponseItem()
                    {
                        Id = 1
                    },
                    new ResponseItem()
                    {
                        Id = 2
                    }
                }
            };
            conferenceService.GetCollectionAsync().Returns(speakerSessionCollection);

            //act
            var result = await conferenceController.GetAllSpeakersAndSessions();

            //Assert
            var viewResult = Assert.IsType<OkObjectResult>(result);
            var model = Assert.IsAssignableFrom<SpeakerSessionCollection>(
                viewResult.Value);
            Assert.Equal(2, model.SessionsList.Count);
            Assert.Equal(2, model.SpeakersList.Count);
        }

        [Fact]
        public async void GetAllSessionsAndSpeakers_NotFound()
        {
            //Arrange
            conferenceService.GetCollectionAsync().Returns((SpeakerSessionCollection)null);

            //act
            var result = await conferenceController.GetAllSpeakersAndSessions();

            //Assert
            var notFoundObjectResult = Assert.IsType<NotFoundObjectResult>(result);
            Assert.Equal("Collection not found", notFoundObjectResult.Value);
        }

        [Fact]
        public async void GetAllSessionsAndSpeakers_ReturnsBadRequestResult()
        {
            //Arrange
            conferenceService.GetCollectionAsync().Returns<SpeakerSessionCollection>(x => { throw new Exception(); });

            //act
            var result = await conferenceController.GetAllSpeakersAndSessions();

            //Assert
            var badRequestObjectResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal("Invalid Get Request", badRequestObjectResult.Value);
        }

        [Fact]
        public async void GetSessoinById_OkRsults() {

            Session session = new Session {
                Id = 105,
                Description = "Testing"
            };
            conferenceService.GetSession(105).Returns(session);
            var result = await conferenceController.GetSession(105);
            //Assert
            var viewResult = Assert.IsType<OkObjectResult>(result);
            var model = Assert.IsAssignableFrom<Session>(
                viewResult.Value);
            Assert.Equal(105, model.Id);
            Assert.Equal("Testing", model.Description);
        }

        [Fact]
        public async void GetSessoinById_NotFound()
        {

            conferenceService.GetSession(5).Returns((Session)null);
            var result = await conferenceController.GetSession(5);
           
            //Assert
            var notFoundObjectResult = Assert.IsType<NotFoundObjectResult>(result);
            Assert.Equal("Session Not found for id 5", notFoundObjectResult.Value);
        }
        [Fact]
        public async void GetSessionById_ReturnsBadRequestResult()
        {
            //Arrange
            conferenceService.GetSession(58998998).Returns<Session>(x => { throw new Exception(); });

            //act
            var result = await conferenceController.GetSession(58998998);

            //Assert
            var badRequestObjectResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal("Invalid Get Request", badRequestObjectResult.Value);
        }
    }
}
