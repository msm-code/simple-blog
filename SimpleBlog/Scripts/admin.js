$(function () {

    var JustBlog = {}
    JustBlog.GridManager = {
        // create grid to manage posts
        postsGrid: function (gridName, pagerName) {
            var colNames = [
                'Id',
                'Title',
                'Short Description',
                'Description',
                'Category',
                'Category',
                'Tags',
                'Meta',
                'Url Slug',
                'Published',
                'Posted On',
                'Modified'
            ];

            var columns = [];

            columns.push({
                name: 'Id',
                hidden: true,
                key: true
            });

            columns.push({
                name: 'Title',
                index: 'Title',
                width: 250
            });

            columns.push({
                name: 'ShortDescription',
                width: 250,
                sortable: false,
                hidden: true
            });

            columns.push({
                name: 'Description',
                width: 250,
                sortable: false,
                hidden: true
            });

            columns.push({
                name: 'Category.Id',
                hidden: true
            });

            columns.push({
                name: 'Category.Name',
                index: 'Category',
                width: 150
            });

            columns.push({
                name: 'Tags',
                width: 150
            });

            columns.push({
                name: 'Meta',
                width: 250,
                sortable: false
            });

            columns.push({
                name: 'UrlSlug',
                width: 200,
                sortable: false
            });

            columns.push({
                name: 'Published',
                index: 'Published',
                width: 100,
                align: 'center'
            });

            columns.push({
                name: 'PostedOn',
                index: 'PostedOn',
                width: 150,
                align: 'center',
                sorttype: 'date',
                datefmt: 'Y-m-d H:i:sZ'
            });

            columns.push({
                name: 'Modified',
                index: 'Modified',
                width: 100,
                align: 'center',
                sorttype: 'date',
                datefmt: 'Y-m-d H:i:sZ'
            });

            // create the grid
            $(gridName).jqGrid({
                // server url and other ajax stuff 
                url: '/Admin/Posts',
                datatype: 'json',
                mtype: 'GET',

                height: 'auto',

                // columns
                colNames: colNames,
                colModel: columns,

                // pagination options
                toppager: true,
                pager: pagerName,
                rowNum: 10,
                rowList: [10, 20, 30],

                // row number column
                rownumbers: true,
                rownumWidth: 40,

                // default sorting
                sortname: 'PostedOn',
                sortorder: 'desc',

                // display the no. of records message
                viewrecords: true,

                jsonReader: { repeatitems: false }
            });
        },

        // create grid to manage categories
        categoriesGrid: function (gridName, pagerName) {

        },

        // create grid to manage tags
        tagsGrid: function (gridName, pagerName) {

        }
    }

    $("#tabs").tabs({
        show: function (event, ui) {
            if (!ui.tab.isLoaded) {
                var mgr = JustBlog.GridManager, fn, gridName, pagerName;
                switch (ui.index) {
                    case 0:
                        fn = mgr.postsGrid;
                        gridName = "#tablePosts";
                        pagerName = "#pagerPosts"
                        break;
                    case 1:
                        fn = mgr.categoriesGrid;
                        gridName = "#tableCats";
                        pagerName = "#pagerCats"
                        break;
                    case 2:
                        fn = mgr.tagsGrid;
                        gridName = "#tableTags";
                        pagerName = "#pagerTags";
                        break;
                }

                fn(gridName, pagerName);
                ui.tab.isLoaded = true;
            }
        }
    });

});