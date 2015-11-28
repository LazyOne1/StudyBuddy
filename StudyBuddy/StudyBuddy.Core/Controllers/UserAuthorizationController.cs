using Nancy;
using StudyBuddy.DbModule;
using StudyBuddy.DbModule.DbHelpers;

namespace StudyBuddy.Core.Controllers
    {
    public class UserAuthorizationController : NancyModule
        {
        public UserAuthorizationController()
            {
            Get["/login/"] = _ =>
                {
                using (var dbWrapper = new DbWrapper("AuthenticationDbCore"))
                    {
                    if (!dbWrapper.DoesDbExist())
                        {
                        StudyBuddyDbAssistant.CreateDatabase("AuthenticationDbCore");
                        StudyBuddyDbAssistant.CreateTables("AuthenticationDbCore");
                        }
                    }
                return HttpStatusCode.BadRequest;
                };

            Post["/register/"] = _ =>
                {
                using (var dbWrapper = new DbWrapper("AuthenticationDbCore"))
                    {
                    if (!dbWrapper.DoesDbExist())
                        {
                        StudyBuddyDbAssistant.CreateDatabase("AuthenticationDbCore");
                        StudyBuddyDbAssistant.CreateTables("AuthenticationDbCore");
                        }
                    }
                return HttpStatusCode.BadRequest;
                };
            }
        }
    }