using k8s.Models;
using Xunit;

namespace k8s.Tests
{
    public class ModelExtensionTests
    {
        [Fact]
        public void TestV1Status()
        {
            var s = new V1Status() { Status = "Success" };
            Assert.Equal("Success", s.ToString());

            s = new V1Status() { Status = "Failure" };
            Assert.Equal("Failure", s.ToString());

            s = new V1Status() { Status = "Failure", Reason = "BombExploded" };
            Assert.Equal("BombExploded", s.ToString());

            s = new V1Status() { Status = "Failure", Message = "Something bad happened." };
            Assert.Equal("Something bad happened.", s.ToString());

            s = new V1Status() { Status = "Failure", Code = 400 };
            Assert.Equal("BadRequest", s.ToString());

            s = new V1Status() { Status = "Failure", Code = 911 };
            Assert.Equal("911", s.ToString());

            s = new V1Status() { Status = "Failure", Code = 400, Message = "It's all messed up." };
            Assert.Equal("BadRequest - It's all messed up.", s.ToString());

            s = new V1Status()
            {
                Status = "Failure",
                Code = 400,
                Reason = "IllegalValue",
                Message = "You're breaking the LAW!",
            };
            Assert.Equal("IllegalValue - You're breaking the LAW!", s.ToString());
        }
    }
}
