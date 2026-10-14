namespace Domain.UnitTests.Abstractions;

public class ValueObjectTests
{
    private sealed class Hex(string value) : ValueObject
    {
        public string Value { get; } = value;

        protected override IEnumerable<object> GetEqualityComponents()
        {
            yield return Value;
        }
    }

    [Fact]
    public void ValueObjects_WithSameComponents_ShouldBeEqual()
    {
        (new Hex("#3584e4") == new Hex("#3584e4")).ShouldBeTrue();
        new Hex("#3584e4").GetHashCode().ShouldBe(new Hex("#3584e4").GetHashCode());
    }

    [Fact]
    public void ValueObjects_WithDifferentComponents_ShouldDiffer()
    {
        (new Hex("#3584e4") != new Hex("#e01b24")).ShouldBeTrue();
    }
}
