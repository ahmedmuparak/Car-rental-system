using CarRental.Application.Interfaces;
using CarRental.Infrastructure.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace CarRental.API.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class MessageController : ControllerBase
    {
        private readonly IMessageService messageService;
        private readonly IConversationService conversationService;

        public MessageController(IMessageService messageService,IConversationService conversationService)
        {
            this.messageService = messageService;
            this.conversationService = conversationService;
        }

        [HttpGet("conversation/{conversationId}")]
        public async Task<IActionResult> GetMessagesByConversationId(int conversationId)
        {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                var messages = await messageService.GetMessagesByConversationId(conversationId, userId);
                return Ok(messages);
        }

        [HttpPost("conversation/{conversationId}")]
        public async Task<IActionResult> SendMessage(int conversationId, [FromBody] string content)
        {
                var senderId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                var message = await messageService.SendMessage(conversationId, senderId, content);
                return Ok(message);
        }

        [HttpGet("unread")]
        public async Task<IActionResult> GetUnreadMessages(int conversationId)
        {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                var unreadMessages = await messageService.GetUnreadMessagesByConversationId(conversationId, userId);
                return Ok(unreadMessages);
        }

        [HttpPut("{messageId}")]
        public async Task<IActionResult> EditMessage(int messageId, [FromBody] string newContent)
        {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                var updatedMessage = await messageService.EditMessage(messageId, userId, newContent);
                return Ok(updatedMessage);
        }

        [HttpDelete("{messageId}")]
        public async Task<IActionResult> DeleteMessage(int messageId)
        {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                await messageService.Delete(messageId, userId);
                return Ok(new { message = "Message deleted successfully." });
        }
    }
}
