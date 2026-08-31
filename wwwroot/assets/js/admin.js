(function ($) {
  "use strict";

  $(function () {
    var $body = $("body");
    var $sidebar = $(".admin-sidebar");
    var $backdrop = $(".sidebar-backdrop");

    // Mobile sidebar toggle
    $(".js-sidebar-toggle").on("click", function (e) {
      e.preventDefault();
      $sidebar.toggleClass("show");
      $backdrop.toggleClass("show");
    });
    $backdrop.on("click", function () {
      $sidebar.removeClass("show");
      $backdrop.removeClass("show");
    });

    // Collapsible sidebar sub-menus
    $(".admin-nav .nav-link.has-sub").on("click", function (e) {
      e.preventDefault();
      var $parent = $(this).closest(".nav-item");
      $parent.toggleClass("open");
      $parent.find("> .sub-nav").slideToggle(180);
    });

    // Dropdown close on outside click is handled by Bootstrap already.

    // Simple client-side "select all" for tables
    $(".js-select-all").on("change", function () {
      var checked = $(this).is(":checked");
      $(this).closest("table").find("tbody .js-row-select").prop("checked", checked);
    });

    // Delete confirm (demo only — front-end only template)
    $(".js-confirm-delete").on("click", function (e) {
      if (!confirm("Are you sure you want to delete this item?")) {
        e.preventDefault();
      }
    });
  });
})(jQuery);
