using Application.Base.Dtos;
using NUnit.Framework;

namespace UnitTests.Domain;

[TestFixture]
public class BaseDtoTests
{
    // Concrete implementation for testing abstract class
    private class TestDto : BaseDto { }

    [Test]
    public void Constructor_ShouldInitializeDefaultValues()
    {
        // Arrange & Act
        var dto = new TestDto();

        // Assert
        Assert.IsFalse(dto.Error);
        Assert.IsEmpty(dto.ErrorMessage);
    }

    [Test]
    public void Properties_ShouldBeSettable()
    {
        // Arrange
        var dto = new TestDto();
        var id = 1;
        var createdAt = DateTimeOffset.Now;
        var updatedAt = DateTimeOffset.Now.AddHours(1);
        var isActive = true;
        var errorMessage = "Error";
        var error = true;

        // Act
        dto.Id = id;
        dto.CreatedAt = createdAt;
        dto.UpdatedAt = updatedAt;
        dto.IsActive = isActive;
        dto.ErrorMessage = errorMessage;
        dto.Error = error;

        // Assert
        Assert.AreEqual(id, dto.Id);
        Assert.AreEqual(createdAt, dto.CreatedAt);
        Assert.AreEqual(updatedAt, dto.UpdatedAt);
        Assert.AreEqual(isActive, dto.IsActive);
        Assert.AreEqual(errorMessage, dto.ErrorMessage);
        Assert.AreEqual(error, dto.Error);
    }
}
