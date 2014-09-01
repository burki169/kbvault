/**
 * @license Copyright (c) 2003-2013, CKSource - Frederico Knabben. All rights reserved.
 * For licensing, see LICENSE.html or http://ckeditor.com/license
 */

CKEDITOR.editorConfig = function( config ) {
	// Define changes to default configuration here.
	// For complete reference see:
	// http://docs.ckeditor.com/#!/api/CKEDITOR.config
    config.htmlEncodeOutput = true;
	// The toolbar groups arrangement, optimized for a single toolbar row.
	config.toolbarGroups = [		
		{ name: 'clipboard', groups: ['clipboard', 'undo'] },
        { name: 'paragraph', groups: ['list', 'indent', 'blocks', 'align', 'bidi'] },
		{ name: 'editing',     groups: [ 'find', 'selection', 'spellchecker' ] },
		{ name: 'forms' },
        { name: 'tools' },
		{ name: 'basicstyles', groups: ['basicstyles', 'cleanup'] },
		{ name: 'links' },
		{ name: 'insert' },
		{ name: 'styles' },
		{ name: 'colors' },		
		{ name: 'others' },
		{ name: 'about' },
        { name: 'document', groups: ['mode', 'document', 'doctools'] },
	];

	// The default plugins included in the basic setup define some buttons that
	// are not needed in a basic editor. They are removed here.
	//config.removeButtons = 'Cut,Copy,Paste,Undo,Redo,Anchor,Underline,Strike,Subscript,Superscript';

	// Dialog windows are also simplified.
	//config.removeDialogTabs = 'link:advanced';
};
