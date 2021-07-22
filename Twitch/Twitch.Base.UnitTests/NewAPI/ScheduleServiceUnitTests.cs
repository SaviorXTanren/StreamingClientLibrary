using Microsoft.VisualStudio.TestTools.UnitTesting;
using Twitch.Base.Models.NewAPI.Schedule;
using Twitch.Base.Models.NewAPI.Users;

namespace Twitch.Base.UnitTests.NewAPI
{
    [TestClass]
    public class ScheduleServiceUnitTests : UnitTestBase
    {
        [TestMethod]
        public void GetSchedule()
        {
            TestWrapper(async (TwitchConnection connection) =>
            {
                UserModel broadcaster = await UsersServiceUnitTests.GetCurrentUser(connection);

                ScheduleModel schedule = await connection.NewAPI.Schedule.GetSchedule(broadcaster, 5);

                Assert.IsNotNull(schedule);
                Assert.IsNotNull(schedule.segments);
                Assert.IsTrue(schedule.segments.Count > 0);
            });
        }
    }
}
