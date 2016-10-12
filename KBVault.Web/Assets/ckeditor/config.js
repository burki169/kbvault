/**
 * @license Copyright (c) 2003-2013, CKSource - Frederico Knabben. All rights reserved.
 * For licensing, see LICENSE.html or http://ckeditor.com/license
 */

CKEDITOR.editorConfig = function (config) {
    // Define changes to default configuration here.
    // For complete reference see:
    // http://docs.ckeditor.com/#!/api/CKEDITOR.config
    config.htmlEncodeOutput = true;
    config.extraPlugins = 'iframe,lightbox,codesnippet';
    config.toolbar = 'Full';
    config.toolbar_Full =
    [
        /*{ name: 'document', items: ['Source', '-', 'Save', 'NewPage', 'Preview', '-' ,'Templates'] },*/
        { name: 'clipboard', items: ['Cut', 'Copy', 'Paste', 'PasteText', 'PasteFromWord', '-', 'Undo', 'Redo'] },
        { name: 'editing', items: ['Find', 'Replace', '-', 'SelectAll', '-', 'SpellChecker', 'Scayt'] },
        { name: 'forms', items: ['Form', 'Checkbox', 'Radio', 'TextField', 'Textarea', 'Select', 'Button', 'ImageButton', 'HiddenField'] },
        '/',
        { name: 'basicstyles', items: ['Bold', 'Italic', 'Underline', 'Strike', 'Subscript', 'Superscript', '-', 'RemoveFormat'/*, 'CodeSnippet'*/] },
        { name: 'paragraph', items: ['NumberedList', 'BulletedList', '-', 'Outdent', 'Indent', '-', 'Blockquote', 'CreateDiv', '-', 'JustifyLeft', 'JustifyCenter', 'JustifyRight', 'JustifyBlock', '-', 'BidiLtr', 'BidiRtl'] },
        { name: 'links', items: ['Link', 'Unlink', 'Anchor'] },
        { name: 'insert', items: ['Iframe','Image', 'Flash', 'Table', 'HorizontalRule', 'lightbox','Smiley', 'SpecialChar', 'PageBreak'] },
        '/',
        { name: 'styles', items: ['Styles', 'Format', 'Font', 'FontSize'] },
        { name: 'colors', items: ['TextColor', 'BGColor'] },
        { name: 'tools', items: ['Maximize', 'ShowBlocks', '-', 'About'] }
    ];
    // The toolbar groups arrangement, optimized for a single toolbar row.
    /*
    config.toolbarGroups = [
        { name: 'document', groups: ['mode', 'document', 'doctools'] },        
		{ name: 'clipboard', groups: ['basicstyles','clipboard', 'undo','styles'] },
        { name: 'paragraph', groups: ['list', 'indent', 'blocks', 'align', 'bidi'] },
		{ name: 'editing', groups: ['find', 'selection', 'spellchecker','links'] },        
		{ name: 'insert', groups: ['insert','colors', 'others', 'about'] },
        { name: 'forms', groups: ['forms', 'tools'] },        
        { name: 'basicstyles', groups: [ 'cleanup'] }
    ];*/
    
    // The default plugins included in the basic setup define some buttons that
    // are not needed in a basic editor. They are removed here.
    //config.removeButtons = 'Cut,Copy,Paste,Undo,Redo,Anchor,Underline,Strike,Subscript,Superscript';

    // Dialog windows are also simplified.
    //config.removeDialogTabs = 'link:advanced';
};
