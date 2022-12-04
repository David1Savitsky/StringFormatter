using Application.Exception;
using Application.Service;

namespace Test;

[TestFixture]
public class Tests
{
    private User _user;
    
    [SetUp]
    public void Setup()
    {
        _user = new User("Петя", "Иванов");
    }
    
    private class User
    {
        private string firstName;
        public string LastName { get; }
    
        public User(string firstName, string lastName)
        {
            this.firstName = firstName;
            LastName = lastName;
        }
    }

    [Test]
    public void PropertyAndFieldTest()
    {
        string expectedResult = "Привет, Петя Иванов!";
        
        string actualResult = StringFormatterService.Shared.Format("Привет, {FirstName} {LastName}!", _user);
        
        Assert.AreEqual(expectedResult, actualResult);
    }

    [Test]
    public void ScreeningTest()
    {
        string expectedResult = "Привет, {FirstName}";

        string actualResult = StringFormatterService.Shared.Format("Привет, {{FirstName}}", _user);
        
        Assert.AreEqual(expectedResult, actualResult);
    }

    [Test]
    public void UnopenBracketTest()
    {
        Assert.That(() => StringFormatterService.Shared.Format("Привет, {FirstName}}", _user),
            Throws.TypeOf<InvalidInputException>()
                .With.Message.EqualTo("Symbol on 9 position is unavailable"));
    }

    [Test]
    public void UnclosedBracketTest()
    {
        Assert.That(() => StringFormatterService.Shared.Format("Привет, {{FirstName}", _user),
            Throws.TypeOf<InvalidInputException>()
                .With.Message.EqualTo("Symbol on 19 position is unavailable"));
    }

    [Test]
    public void PropertyOrFieldNotExistTest()
    {
        Assert.That(() => StringFormatterService.Shared.Format("Привет, {1111}", _user),
            Throws.TypeOf<PropertyOrFieldNotExistException>()
                .With.Message.EqualTo("Field or property 1111 not found in object Test.Tests+User"));
    }
}
