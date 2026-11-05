using CarRental.Application.DTOs.Conversation;
using CarRental.Application.Interfaces;
using CarRental.Domain.Entities;
using CarRental.Infrastructure.Identity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
namespace CarRental.API.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class ConversationController : ControllerBase
    {
        private readonly IConversationService conversationService;

        public ConversationController(IConversationService conversationService)
        {
            this.conversationService = conversationService;
        }


        [HttpPost]
        public async Task<IActionResult> CreateConversation(CreateConversationDTO model)
        {
                var user1Id = User.FindFirstValue(ClaimTypes.NameIdentifier); 
                var conversation = await conversationService.CreateConservation(model, user1Id);
                return Ok(conversation);
        }

        [HttpGet("my-conversations")]
        public async Task<IActionResult> GetMyConversations()
        {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                var conversations = await conversationService.GetMyConversations(userId);
                return Ok(conversations);
        }
    }
}

