var tinyMCEUtilities = (function() {
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
			if (bodyEl === element) {
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

    /**
     * Gets the caret position (cols as x and rows as y).
     * @param {Editor} editor The tinyMCE editor.
     * @returns {object} Returns an object with x (cols) and y (rows)
     * properties.
     */
    obj.getCaretPosition = function (editor) {
        if (editor == null) {
            throw new Error("editor was null.");
        }

        var bodyEl = editor.getBody();

        var result = { x: 0, y: 0 };

        var range = editor.selection.getRng();
        result.x = range.startOffset;

        var rowEl = range.startContainer;

        if (rowEl === bodyEl) {

            // If the bodyEl is the same as the rowEl then the editor is empty.
            return result;
        } else if (rowEl.nodeType !== Node.ELEMENT_NODE) {

            // If rowEl isn't an element, select the the parent element.
            rowEl = rowEl.parentElement;
        }

        var parent = rowEl.parentElement;

        for (var y = 0; y < parent.children.length; y++) {
            var _rowEl = parent.children[y];

            if (_rowEl == rowEl) {
                result.y = y;
                break;
            }
        }

        return result;
    };

    /**
     * Sets the caret position of the editor.
     * @param {any} editor The tinyMCE.Editor.
     * @param {any} x The x-pos of the caret.
     * @param {any} y The y-pos of the caret.
     */
    obj.setCaretPosition = function (editor, x, y) {
        if (editor == null) {
            throw new Error("editor was null.");
        }

        var bodyEl = editor.getBody();

        if (bodyEl.children.length < y) {
            throw new Error("No such row at " + y + ".");
        }

        var rowEl = bodyEl.children[y];
        var node = null;
        var accumalatedPosition = 0;

        for (var i = 0; i < rowEl.childNodes.length; i++) {
            var _node = rowEl.childNodes[i];
            accumalatedPosition += (_node.length || 0);

            if (accumalatedPosition >= x) {
                node = _node;
                break;
            }
        }

        var rng = editor.selection.getRng();
        rng.setStart(node, x);
        rng.setEnd(node, x);
    };

    /**
     * Highlights the text/nodes between the two points.
     * @param {Editor} editor The tinyMCE.Editor.
     * @param {Number} x1 The x-coord of the first point.
     * @param {Number} y1 The y-coord of the first point.
     * @param {Number} x2 The x-coord of the second point.
     * @param {Number} y2 The y-coord of the second point.
     */
    obj.highlight = function (editor, x1, y1, x2, y2) {
        if (editor == null) {
            throw new Error("editor was null.");
        }

        var bodyEl = editor.getBody();

        if (bodyEl.children.length < y1 || bodyEl.children.length < y2) {
            throw new Error("No such row at " + y1 + " or " + y2 + ".");
        }

        var startRowEl = bodyEl.children[y1];
        var endRowEl = bodyEl.children[y2];
        var startNode = null;
        var endNode = null;
        var accumalatedPosition = 0;

        for (var i = 0; i < startRowEl.childNodes.length; i++) {
            var _node = startRowEl.childNodes[i];
            accumalatedPosition += (_node.length || 0);

            if (accumalatedPosition >= x1) {
                startNode = _node;
                break;
            }
        }

        // Reset accumulatedPosition.
        accumalatedPosition = 0;

        for (var i = 0; i < endRowEl.childNodes.length; i++) {
            var _node = endRowEl.childNodes[i];
            accumalatedPosition += (_node.length || 0);

            if (accumalatedPosition >= x2) {
                endNode = _node;
                break;
            }
        }

        var rng = editor.selection.getRng();
        rng.setStart(startNode, x1);
        rng.setEnd(endNode, x2);
    };

	return obj;
}());
