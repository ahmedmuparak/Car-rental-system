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
            try
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                var messages = await messageService.GetMessagesByConversationId(conversationId, userId);
                return Ok(messages);
            }
            catch (Exception ex)
            {
                return BadRequest(new
                {
                    message = ex.Message
                });
            }
        }

        [HttpPost("conversation/{conversationId}")]
        public async Task<IActionResult> SendMessage(int conversationId, [FromBody] string content)
        {
            try
            {
                var senderId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                var message = await messageService.SendMessage(conversationId, senderId, content);
                return Ok(message);
            }
            catch (Exception ex)
            {
                return BadRequest(new
                {
                    message = ex.Message
                });
            }
        }

        [HttpGet("unread")]
        public async Task<IActionResult> GetUnreadMessages(int conversationId)
        {
            try
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                var unreadMessages = await messageService.GetUnreadMessagesByConversationId(conversationId, userId);
                return Ok(unreadMessages);
            }
            catch (Exception ex)
            {
                return BadRequest(new
                {
                    message = ex.Message
                });
            }
        }

        [HttpPut("{messageId}")]
        public async Task<IActionResult> EditMessage(int messageId, [FromBody] string newContent)
        {
            try
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                var updatedMessage = await messageService.EditMessage(messageId, userId, newContent);
                return Ok(updatedMessage);
            }
            catch (Exception ex)
            {
                return BadRequest(new
                {
                    message = ex.Message
                });
            }
        }

        [HttpDelete("{messageId}")]
        public async Task<IActionResult> DeleteMessage(int messageId)
        {
            try
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                await messageService.Delete(messageId, userId);
                return Ok(new { message = "Message deleted successfully." });
            }
            catch (Exception ex)
            {
                return BadRequest(new
                {
                    message = ex.Message
                });
            }
        }
    }
}
