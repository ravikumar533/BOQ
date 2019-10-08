using System;
using System.Threading.Tasks;
using ConferenceApi.Models;

namespace ConferenceApi.Services.Interfaces
{
    public interface IConferenceService
    {
        Task<SpeakerSessionCollection> GetCollectionAsync();
        Task<Session> GetSession(int sessionId);
    }
}
