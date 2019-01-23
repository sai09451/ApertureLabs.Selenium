var tinyMCEUtilties = (function() {
	var obj = {};

	/**
	 * Goes thru each editor in tinyMCE.editors and call matches each editors
	 * getElement() against the passed in element.
	 * @param {Element} element - The element used to match against
	 * Editor.getElement().
	 * @returns {Editor} The tinyMCE.Editor.
	 */
	obj.getEditor = function (element) {
		if (element === null || element === undefined) {
			throw new Error('Argument element cannot be null.');
		}

		var editor = null;

		for (var i = 0; i < tinyMCE.editors.length; i++) {
			var _editor = tinyMCE.editors[i];
			var bodyEl = _editor.getElement();
			if (bodyEl === el) {
				editor = _editor;
				break;
			}
		}

		return editor;
	};

    /**
     * Waits for the editor to finish intializing and calls the resolver. If
     * the timeout occurrs before the editor is initialized then rejector is
     * called.
     * @param {Editor} editor The tinyMCE.Editor.
     * property targets.
     * @param {Number} timeoutMS The timeout in milli-seconds.
     * @param {Number} pollMS How often to poll the editor.
     * @param {Function} resolver The resolve callback.
     * @param {Function} rejector The reject callback.
     */
	obj.waitForInitialization = function (editor,
			timeoutMS,
			pollMS,
			resolver,
			rejector) {
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
	};

	return obj;
}());
