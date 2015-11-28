using System.Collections.Generic;
using Nancy.Security;

namespace StudyBuddy.Core.Authentication
    {
    public class UserIdentity : IUserIdentity
        {
        public string UserName { get; set; }
        public IEnumerable<string> Claims { get; set; }
        }
    }