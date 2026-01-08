using Domain.Base.Extensions;
using NUnit.Framework;

namespace UnitTests.Domain;

[TestFixture]
public class EmailValidateExtensionsTests
{
    [TestCase("test@example.com", ExpectedResult = true)]
    [TestCase("user.name@domain.co.uk", ExpectedResult = true)]
    [TestCase("user+tag@example.com", ExpectedResult = true)]
    [TestCase("plainaddress", ExpectedResult = false)]
    [TestCase("@example.com", ExpectedResult = false)]
    [TestCase("email.example.com", ExpectedResult = false)]
    [TestCase("email@example@example.com", ExpectedResult = false)]
    [TestCase(".email@example.com", ExpectedResult = false)]
    [TestCase("", ExpectedResult = false)]
    [TestCase(" ", ExpectedResult = false)]
    public bool IsValidEmail_ShouldValidateEmail(string email)
    {
        return email.IsValidEmail();
    }
}
