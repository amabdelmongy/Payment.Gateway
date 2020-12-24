using Domain.Extension;
using NUnit.Framework;

namespace Domain.Test
{
    class StringExtensionsMaskedTests
    {
        [Test]
        public void WHEN_call_masked_THEN_return_masked_string()
        {
            var expected = "123456xxxxxx3456";
            var testString = "1234567890123456";
            var actual = testString.Masked();
 
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void WHEN_call_masked_with_parameters_THEN_return_masked_string()
        {
            var expected = "xxxxxxxxxx123456";
            var testString = "1234567890123456";
            var actual = testString.Masked(0,10);

            Assert.AreEqual(expected, actual);
        }
    }
}
