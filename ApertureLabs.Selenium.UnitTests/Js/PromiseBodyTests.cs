using System;
using System.Text.RegularExpressions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ApertureLabs.Selenium.Js
{
    [TestClass]
    public class PromiseBodyTests
    {
        const string TestScriptA =
@"var tinyMCEUtilities = new function() {
	this.getEditor = function (element) {
		if (element == null) {
			throw new Error('Argument element cannot be null.');
		}

		var editor = null;

		for (var i = 0; i < tinyMCE.editors.length; i++) {
			var _editor = tinyMCE.editors[i];
			var bodyEl = _editor.getElement();
			if (bodyEl == el) {
				editor = _editor;
				break;
			}
		}

		return editor;
	}

	this.waitForInitialization(editor, timeoutMS, pollMS, resolver, rejector) {
		var expiration = DateTime.now() + timeoutMS;

		var intervalId = setInterval(function () {
			if (Date.now() > expiration) {
				clearInterval(intervalId);
				rejector();
				return;
			}

			if (editor.initialized) {
				clearInterval(intervalId);
				resolver();
			}
		}, pollMS);
	}
}
";

        [TestMethod]
        public void EscapeScriptTest()
        {
            var before = TestScriptA;
            var after = JavaScript.Clean(before);

            StringAssert.DoesNotMatch(after, new Regex("[ ]{2,}"));
            StringAssert.DoesNotMatch(after, new Regex("\n"));
            Assert.IsFalse(after.Contains(Environment.NewLine));
        }
    }
}
