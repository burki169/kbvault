/**
 * @license Copyright (c) 2003-2013, CKSource - Frederico Knabben. All rights reserved.
 * For licensing, see LICENSE.html or http://ckeditor.com/license
 */

CKEDITOR.editorConfig = function (config) {
    // Define changes to default configuration here.
    // For complete reference see:
    // http://docs.ckeditor.com/#!/api/CKEDITOR.config
    config.htmlEncodeOutput = true;
    // The toolbar groups arrangement, optimized for a single toolbar row.
    config.toolbarGroups = [
        { name: 'document', groups: ['mode', 'document', 'doctools'] },        
		{ name: 'clipboard', groups: ['basicstyles','clipboard', 'undo','styles'] },
        { name: 'paragraph', groups: ['list', 'indent', 'blocks', 'align', 'bidi'] },
		{ name: 'editing', groups: ['find', 'selection', 'spellchecker','links'] },        
		{ name: 'insert', groups: ['insert', 'colors', 'others', 'about'] },
        { name: 'forms', groups: ['forms', 'tools'] },
        { name: 'basicstyles', groups: [ 'cleanup'] }
    ];

    // The default plugins included in the basic setup define some buttons that
    // are not needed in a basic editor. They are removed here.
    //config.removeButtons = 'Cut,Copy,Paste,Undo,Redo,Anchor,Underline,Strike,Subscript,Superscript';

    // Dialog windows are also simplified.
    //config.removeDialogTabs = 'link:advanced';
};
