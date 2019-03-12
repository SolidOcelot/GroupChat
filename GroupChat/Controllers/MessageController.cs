using GroupChat.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using PusherServer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GroupChat.Controllers
{
    [Route("api/[controller]")]
    public class MessageController : Controller
    {
        private readonly GroupChatContext _context;
        private readonly UserManager<IdentityUser> _userManager;
        public MessageController(GroupChatContext context, UserManager<IdentityUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        [HttpGet("{group_id}")]
        public IEnumerable<Message> GetById(int group_id)
        {
            return _context.Message.Where(gb => gb.GroupId == group_id);
        }

        //[HttpPost]
        //public async Task<IActionResult> CreateAsync([FromBody] MessageViewModel message)
        //{
        //    Message new_message = new Message { AddedBy = _userManager.GetUserName(User), message = message.message, GroupId = message.GroupId };

        //    _context.Message.Add(new_message);
        //    _context.SaveChanges();


        //    //change pusher app details for custom credentials
        //    var options = new PusherOptions
        //    {
        //        Cluster = "eu",
        //        Encrypted = true
        //    };
        //    var pusher = new Pusher(
        //            "733746",
        //            "2197134b840b0f51ef40",
        //            "6d3361985f6d36d29def",
        //        options
        //    );

        //    //WTF SAME
        //    var result = await pusher.TriggerAsync(
        //        "private-" + message.GroupId,
        //        "new_message",
        //    new { new_message },
        //    new TriggerOptions() { SocketId = message.SocketId });


        //    return new ObjectResult(new { status = "success", data = new_message });
        //}

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] MessageViewModel message)
        {
            Message new_message = new Message { AddedBy = _userManager.GetUserName(User), message = message.message, GroupId = message.GroupId };

            _context.Message.Add(new_message);
            _context.SaveChanges();


            var options = new PusherOptions
            {
                Cluster = "eu",
                Encrypted = true
            };
            var pusher = new Pusher(
                        "733746",
                        "2197134b840b0f51ef40",
                        "6d3361985f6d36d29def",
                    options
                );

            var result = await pusher.TriggerAsync(
                "private-" + message.GroupId,
                "new_message",
            new { new_message },
            new TriggerOptions() { SocketId = message.SocketId });

            return new ObjectResult(new { status = "success", data = new_message });
        }
    }
}
