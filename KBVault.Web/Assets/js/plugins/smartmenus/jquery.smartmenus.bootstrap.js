/*!
 * SmartMenus jQuery Plugin Bootstrap Addon - v0.1.1 - August 25, 2014
 * http://www.smartmenus.org/
 *
 * Copyright 2014 Vasil Dinkov, Vadikom Web Ltd.
 * http://vadikom.com
 *
 * Licensed MIT
 */
(function ($) { $(function () { $("ul.navbar-nav").each(function () { var $this = $(this); $this.addClass("sm").smartmenus({ subMenusSubOffsetX: 2, subMenusSubOffsetY: -6, subIndicatorsPos: "append", subIndicatorsText: "...", collapsibleShowFunction: null, collapsibleHideFunction: null, rightToLeftSubMenus: $this.hasClass("navbar-right"), bottomToTopSubMenus: $this.closest(".navbar").hasClass("navbar-fixed-bottom") }).find("a.current").parent().addClass("active") }).bind({ "show.smapi": function (e, menu) { var $menu = $(menu), $scrollArrows = $menu.dataSM("scroll-arrows"), obj = $(this).data("smartmenus"); if ($scrollArrows) { $scrollArrows.css("background-color", $(document.body).css("background-color")) } $menu.parent().addClass("open" + (obj.isCollapsible() ? " collapsible" : "")) }, "hide.smapi": function (e, menu) { $(menu).parent().removeClass("open collapsible") }, "click.smapi": function (e, item) { var obj = $(this).data("smartmenus"); if (obj.isCollapsible()) { var $item = $(item), $sub = $item.parent().dataSM("sub"); if ($sub && $sub.dataSM("shown-before") && $sub.is(":visible")) { obj.itemActivate($item); obj.menuHide($sub); return false } } } }) }); $.SmartMenus.prototype.isCollapsible = function () { return this.$firstLink.parent().css("float") != "left" } })(jQuery);