using Xunit;
using Pages;

namespace Pages.Tests.Services
{
    public class Program_IsPrimeShould
    {
        [Fact]
        public void IsPrime_InputIs1_ReturnFalse()
        {
            bool result = Image.IsPrime(1);

            Assert.False(result, "1 should not be prime");
        }
    }
}