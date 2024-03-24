using AutoJobSearchShared.Helpers;
using FluentAssertions;

namespace AutoJobSearchShared.Tests.Helpers
{
    public class StringHelpers_Tests
    {
        [Theory]
        [InlineData("HelloWorld.ThisIsTestString1", "Hello\nWorld.\nThis\nIs\nTest\nString1")]
        [InlineData("AnotherTestString.ForYou", "Another\nTest\nString.\nFor\nYou")]
        [InlineData("OneMoreTest.ToGo", "One\nMore\nTest.\nTo\nGo")]
        public void AddNewLinesToMisformedString_Should_AddNewLinesCorrectly(string input, string expected)
        {
            // Act
            string result = StringHelpers.AddNewLinesToMisformedString(input);

            // Assert
            result.Should().Be(expected);
        }

        [Theory]
        [InlineData("Hello,World,This,Is,A,Test,\r\n", new string[] { "Hello", "World", "This", "Is", "Test" })]
        [InlineData("Another,Test,String,For,You,\r\n", new string[] { "Another", "Test", "String", "For", "You" })]
        [InlineData("One,More,Test,To,Go,\r\n", new string[] { "One", "More", "Test", "To", "Go" })]
        public void ConvertCommaSeperatedStringsToIEnumerable_Should_ConvertCorrectly(string input, IEnumerable<string> expected)
        {
            // Act
            var result = StringHelpers.ConvertCommaSeperatedStringsToIEnumerable(input);

            // Assert
            result.Should().BeEquivalentTo(expected);
        }

        [Fact]
        public void TestHighestFrequencyWordsInString()
        {
            // Arrange
            var textInput = "hello world, hello everyone. this is a test, a test for everyone.";
            var textToIgnore = new HashSet<string> { "this", "is", "a" };
            var maxNumOfWords = 3;

            // Act
            var result = StringHelpers.HighestFrequencyWordsInString(textInput, textToIgnore, maxNumOfWords);

            // Assert
            result.Count().Should().Be(maxNumOfWords);
            result.Should().NotContain("this");
            result.Should().NotContain("is");
            result.Should().NotContain("a");
            result.Should().Contain("hello");
            result.Should().Contain("everyone");
            result.Should().Contain("test");
            result.Should().NotBeNullOrEmpty();
        }
    }
}