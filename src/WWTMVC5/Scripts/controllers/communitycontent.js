wwtng.controller('CommunityContent', ['$scope', 'dataproxy', '$timeout', 'UIHelper', function ($scope, dataproxy, $timeout, uiHelper) {
    //Data, WWT2, and Web.config, App_Code go with WWTWeb
    $scope.getEntities = function () {
        if (!$scope.types) {
            return;
        }
        $scope.loadingContent = true;
        contentPromise.then(function() {
            contentPromise = dataproxy.getEntities({
                highlightType: $scope.options.highlightType,
                entityType: $scope.options.entityType,
                currentPage: $scope.pageInfo.CurrentPage,
                pageSize: $scope.pageInfo.ItemsPerPage,
                categoryType: $scope.options.categoryType,
                contentType: $scope.options.contentType
            });
            contentPromise.then(entitiesReturned);
        });
        
    }

    var contentPromise = dataproxy.getAllTypes();
        contentPromise.then(function (data) {
        $scope.types = data;
        $scope.options.contentType = 'All';
        $scope.tabChange($scope.options.activeTab);
        $timeout(function () {
            resolutionChange();
            $('#ddContentType').val(0);//wth?
            initializeMultiSelect('searchContentTypes');
            initializeMultiSelect('searchCategories');
        }, 100);
    });

    var initializeMultiSelect = function (id) {
        $('#' + id + ' option').first().remove();
        var opts = {
            includeSelectAllOption: true,
            buttonContainer: '<div class="btn-group btn-group-sm"></div>',
            onChange: function(option) {
                var selected = [];
                if ($('#' + id).next().find('input[type=checkbox]').first().prop('checked')) {
                    selected = [0];
                } else {
                    $('#' + id).next().find('input[type=checkbox]').each(function() {
                        if ($(this).prop('checked')) {
                            selected.push(parseInt($(this).val()));
                        }
                    });
                }
                if (!selected.length) {
                    selected = [0];
                }
                console.log(selected);
                $scope.options[id] = selected;
                $timeout($scope.runSearch);
            }
        };
        if (id === 'searchCategories') {
            opts.maxHeight = 300;
            opts.enableCaseInsensitiveFiltering = true;
            opts.filterPlaceholder = 'Filter folders...';
            opts.templates= {
                filter: '<li class="multiselect-item filter"><div class="form-group form-group-sm"><input class="form-control multiselect-search" type="text"></div></li>',
                filterClearBtn: ''
            }
        }
        $('#'+id).multiselect(opts);
    }

    var searchTimer, searchPromise;
    var searchThen = function(opts) {
        return function (d) {
            // closure to prevent multiple results
            if (opts.query === $scope.options.query &&
                    opts.categories === $scope.options.searchCategories.join(',') &&
                    opts.contentTypes === $scope.options.searchContentTypes.join(',') &&
                    opts.currentPage === $scope.pageInfo.CurrentPage &&
                    opts.pageSize === $scope.pageInfo.ItemsPerPage
            )entitiesReturned(d);
        }
    }
    $scope.runSearch = function () {
        clearTimeout(searchTimer);
        searchTimer = setTimeout(function () {
            if ($scope.options.query.length >= 1) {
                $scope.loadingContent = true;
                contentPromise.then(function () {
                    var searchOpts = {
                            query:  $scope.options.query,
                            categories: $scope.options.searchCategories.join(','),
                            contentTypes: $scope.options.searchContentTypes.join(','),
                            currentPage: $scope.pageInfo.CurrentPage,
                            pageSize: $scope.pageInfo.ItemsPerPage
                    }
                    var thenAble = searchThen(searchOpts);
                    searchPromise = dataproxy.search(searchOpts);
                    searchPromise.then(thenAble);
                });
                
            }
        }, 300);
        
    }

    
    var entitiesReturned = function (data) {
        $scope.error = data.error;
        if (!data.error){
            $scope.loadingContent = false;
            $scope.noPageChange = true;
            if (data.entities) {
                $scope.entityList = data.entities;
            } else {
                $scope.searchList = data.searchResults;
            }

            data.pageInfo.TotalCount = data.pageInfo.ItemsPerPage * data.pageInfo.TotalPages;

            setTimeout(function () { $scope.noPageChange = false }, 111);
            $scope.pageInfo = data.pageInfo;
        }
        wwt.triggerResize();
    }

    $scope.options = {
        categoryType: 'All',
        entityType: 'Content',
        highlightType: 'MostDownloaded',
        contentType: 'All',
        activeTab: 'browse',
        searchCategories: [0],
        searchContentTypes:[0],
        query:''
    }
    $scope.tabChange = function(tab) {
        $scope.options.activeTab = tab;
        if (tab === 'search') {
            $scope.entityList = null;
        } else {
            $scope.getEntities();
        }
    }
    $scope.pageInfo = {
        CurrentPage: 1,
        ItemsPerPage: 30,
        TotalCount: 0,
        TotalPages: 1
    };
    $scope.pageSizeOptions = {
        lg: [12, 30, 60, 90],
        md: [12, 40, 60, 100],
        sm: [12, 40, 60, 100],
        xs: [12, 40, 60, 100]
    }
    $scope.pageCountChange = function(count) {
        $scope.pageInfo.ItemsPerPage = count || $scope.pageSizeOptions[$scope.currentResolution][1];
        $scope.getEntities();
    }
    $scope.$watch('pageInfo.CurrentPage', function() {
        if (!$scope.noPageChange) {
            if ($scope.options.activeTab === 'browse') {
                $scope.getEntities();
            } else {
                $scope.runSearch();
            }
        }
    });
    $scope.currentResolution = 'lg';
    var resolutionChange = function () {
        var changed = $scope.currentResolution !== wwt.currentResolution;
        if (changed && (wwt.currentResolution === 'lg' || $scope.currentResolution === 'lg')) {
            var index = $scope.pageSizeOptions[$scope.currentResolution].indexOf($scope.pageInfo.ItemsPerPage);
            $scope.pageInfo.ItemsPerPage = $scope.pageSizeOptions[wwt.currentResolution || $scope.currentResolution][index];
            $scope.getEntities();
        }
        $scope.currentResolution = wwt.currentResolution || $scope.currentResolution;
    }
    $(window).on('resolutionchange', resolutionChange);
    $scope.searchCategoryChange = function() {
        console.log($scope.options.searchCategories);
    }
    
    uiHelper.fixLinks('communityLink');
}]);