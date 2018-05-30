using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LoyaltyProgram.Domains;
using Microsoft.AspNetCore.Mvc;

namespace LoyaltyProgram.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private static IDictionary<int, LoyaltyProgramUser> registeredUsers = new Dictionary<int, LoyaltyProgramUser>();
        
        // POST api/users
        [HttpPost]
        public ActionResult Post([FromBody] LoyaltyProgramUser newUser)
        {
            AddRegisteredUser(newUser);
            return CreatedAtRoute("GetUser", new {userId = newUser.Id}, newUser);
        }

        // PUT api/users/123
        [HttpPut("{userId:int}")]
        public ActionResult Put(int userId, [FromBody] LoyaltyProgramUser updatedUser)
        {
            // Store the updatedUser to a data store
            registeredUsers[userId] = updatedUser;
            return Ok(updatedUser);
        }
        
        // GET api/users/123
        [HttpGet("{userId:int}", Name = "GetUser")]
        public ActionResult Get(int userId)
        {
            if (registeredUsers.ContainsKey(userId))
                return Ok(registeredUsers[userId]);
            
            return NotFound();
        }
        
        private void AddRegisteredUser(LoyaltyProgramUser newUser)
        {
            // Store the newUser to a data store
            var userId = registeredUsers.Count;
            newUser.Id = userId;
            registeredUsers[userId] = newUser;
        }
    }
}
