using System.Threading.Tasks;
using Xunit;
using Sloader.Engine.Util;

namespace Sloader.Engine.Tests.UtilTests
{
    public class StringExtensionCleanerTests
    {

	    [Fact]
	    public void Can_Deal_With_null()
	    {
		    string test = null;

		    Assert.Equal(string.Empty, test.ToCleanString());
	    }

		[Fact]
        public void Returns_Normal_String_Without_ControlChar()
        {
	        string test = "foobar test";

			Assert.Equal("foobar test", test.ToCleanString());
        }

	    [Fact]
	    public void Preserves_rnt()
	    {
		    string test = "\tfoobar\r\ntest";

		    Assert.Equal("\tfoobar\r\ntest", test.ToCleanString());
	    }

		[Fact]
	    public void Returns_Cleaned_String_With_ControlChar()
	    {
			// control char \u7f 
			char c = '\u007f';

			string test = "foobar" + c + "test";

			Assert.Equal("foobartest", test.ToCleanString());
	    }

	    [Fact]
	    public void Returns_Cleaned_String_With_ControlChar_Pure()
	    {
		    // control char \u7f 
		    char c = '';

		    string test = "foobar" + c + "test";

		    Assert.Equal("foobartest", test.ToCleanString());
	    }

	}
}