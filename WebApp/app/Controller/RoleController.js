app.controller("RoleController", function ($scope, $uibModal, $ngConfirm, showToast, hideLoading) {
    $scope.modelSearch = {};
    $scope.modelSearch.totalItems = 0;
    $scope.modelSearch.currentPage = 1;
    $scope.modelSearch.maxSize = 5;
    $scope.modelSearch.pageSize = 10;
    $scope.modelSearch.SortColumn = "Name DESC";
    $scope.ListRole = [];

    angular.element(document).ready(function () {
        showToast();
        GetBottomAction();
        $scope.LoadPage();
    });

    $scope.RoleBtnCreate = false;
    $scope.RoleBtnUpdate = false;
    $scope.RoleBtnSearch = false;
    $scope.RoleBtnDelete = false;

    function GetBottomAction() {
        $.ajax({
            type: 'post',
            url: '/Role/GetBottomAction',
            data: {},
            success: function (response) {
                if (response.Buttoms != null) {
                    angular.forEach(response.Buttoms, function (item) {
                        if (item == 'btnCreate') {
                            $scope.RoleBtnCreate = true;
                        }
                        if (item == 'btnUpdate') {
                            $scope.RoleBtnUpdate = true;
                        }
                        if (item == 'btnSearch') {
                            $scope.RoleBtnSearch = true;
                        }
                        if (item == 'btnDelete') {
                            $scope.RoleBtnDelete = true;
                        }
                    });
                }
                console.log('RoleBtnUpdate');
                console.log($scope.RoleBtnUpdate);
                $scope.$apply();
            }
        });
    }

    $scope.pageChanged = function () {
        $scope.LoadPage();
    };

    $scope.ViewDetail = function (ID, rowIndex) {
        $scope.selectedRow = rowIndex;
    };

    $scope.LoadPage = function () {
        $.ajax({
            type: 'post',
            url: '/Role/GetAllByPage',
            data: $scope.modelSearch,
            success: function (data) {
                $scope.modelSearch.totalItems = data.totalItems;
                $scope.ListRole = data.data;
                $scope.modelSearch.pageSize = data.pageSize;
                $scope.$apply();
                 hideLoading();
            }
        });
    };



    $scope.Refesh = function () {
        $scope.LoadPage();
    };

    $scope.Sort = function (event, sortRow) {
        if (event.currentTarget.classList.contains('arrow-up')) {
            $scope.modelSearch.SortColumn = sortRow + " DESC";
            $scope.LoadPage();
        }
        if (event.currentTarget.classList.contains('arrow-down')) {
            $scope.modelSearch.SortColumn = sortRow + " ASC";
            $scope.LoadPage();
        }
    };

    $scope.add = function () {
        var modalInstance = $uibModal.open({
            animation: $scope.animationsEnabled,
            templateUrl: '/Role/_Add',
            controller: 'add',
            size: 'xl',
            backdrop: 'static'
        });

        //kết quả trả về của modal
        modalInstance.result.then(function (response) {
            $scope.LoadPage();
        });
    };
    $scope.edit = function (itemId) {
        var modalInstance = $uibModal.open({
            animation: $scope.animationsEnabled,
            templateUrl: '/Role/_Edit',
            controller: 'edit',
            size: 'xl',
            backdrop: 'static',
            resolve: {
                itemId: function () {
                    return itemId;
                }
            }
        });

        //kết quả trả về của modal
        modalInstance.result.then(function (response) {
            $scope.LoadPage();
        });
    };
    $scope.delete = function (itemId, name) {
        $ngConfirm({
            title: 'Thông báo',
            content: 'Bạn có chắc chắn muốn xóa quyền ' + name + ' không?',
            scope: $scope,
            buttons: {
                delete: {
                    text: 'Xóa',
                    btnClass: 'btn-blue',
                    action: function (scope, button) {
                        $.ajax({
                            type: 'post',
                            url: '/Role/Delete',
                            data: { Id: itemId  },
                            success: function (data) {
                                if (data.Error) {
                                    toastr.error(data.Title);
                                } else {
                                    toastr.success(data.Title);
                                    //$scope.cancel();
                                    $scope.LoadPage();
                                }
                            }
                        });

                    }
                },
                close: {
                    text: 'Hủy',
                    action: function (scope, button) {

                    }
                }
            }
        });
    };
});

app.controller('add', function ($scope, $uibModalInstance, $ngConfirm, showToast, hideLoading) {
    $scope.ListPageMenu = [];
    angular.element(document).ready(function () {
        showToast();
        GetDanhMuc();
    });

    var setting = {
        check: {
            enable: true
        },
        data: {
            simpleData: {
                enable: true,
                idKey: "id",
                pIdKey: "pId",
                rootPId: ""
            }
        }
    };

   
    function GetDanhMuc() {
        $scope.ListPageMenu = [];
        $.ajax({
            type: 'post',
            url: '/Role/GetDanhMuc',
            data: {},
            success: function (data) {
                 hideLoading();

                $scope.ListPageMenu = data.TreeDatas;
                $.fn.zTree.init($("#treeRole"), setting, $scope.ListPageMenu);
                var zTree = $.fn.zTree.getZTreeObj("treeRole");
                var type = { "Y": "ps", "N": "ps" };
                zTree.setting.check.chkboxType = type;
                
            }
        });
    }

    $scope.model = {};
    $scope.submit = function () {
        $scope.model.Status = $scope.model.StatusTemp === '1' ? true : false;
        $("#formSubmit").validate({
            rules: {
                ID: {
                    required: true,
                    maxlength: 50
                },
                Name: {
                    required: true,
                    maxlength: 50
                }

            },
            messages: {
                ID: {
                    required: "Vui lòng nhập mã",
                    maxlength: "Mã không được vượt quá 50 ký tự"
                },
                Name: {
                    required: "Vui lòng nhập tên quyền",
                    maxlength: "Tên quyền không được vượt quá 50 ký tự"
                }
            }
        });
        if ($("#formSubmit").valid()) {

            var treeObj = $.fn.zTree.getZTreeObj("treeRole");
            var nodes = treeObj.getCheckedNodes();

            angular.forEach($scope.ListPageMenu, function (pageMenu) {

                var dataSearch = nodes.filter(function (item) {
                    return item.id === pageMenu.id;
                });

                if (dataSearch !== null && dataSearch.length > 0) {
                    pageMenu.checked = true;
                }

            })


            $.ajax({
                type: 'post',
                url: '/Role/Add',
                data: { role: $scope.model, pageMenus: $scope.ListPageMenu },
                success: function (data) {
                    if (data.Error) {
                        toastr.error(data.Title);
                    } else {
                        toastr.success(data.Title);
                        $scope.cancel();
                        $scope.LoadPage();
                    }
                }
            });
        }
    };


    $scope.cancel = function () {
        $uibModalInstance.close();
    };

    $scope.SelectFile = function (e) {
        $scope.model.Avartar = e.target.files[0];
        document.getElementById("pathPhoto").src = e.target.files[0];
    };
});

app.controller('edit', function ($scope, $uibModalInstance, itemId, $ngConfirm, showToast, hideLoading) {
    $scope.ListPageMenu = [];
   
    $scope.ListRole = [];
    var setting = {
        check: {
            enable: true
        },
        data: {
            simpleData: {
                enable: true,
                idKey: "id",
                pIdKey: "pId",
                rootPId: ""
            }
        }
    };

   
    $scope.model = {};
    angular.element(document).ready(function () {

        $.ajax({
            type: 'post',
            url: '/Role/GetItemByID',
            data: { Id: itemId },
            success: function (data) {
                if (data.Error) {
                    toastr.error(data.Title);
                } else {
                    $scope.model = data.data;

                    $scope.ListPageMenu = data.TreeDatas;
                    $.fn.zTree.init($("#treeRole"), setting, $scope.ListPageMenu);
                    var zTree = $.fn.zTree.getZTreeObj("treeRole");
                    var type = { "Y": "ps", "N": "ps" };
                    zTree.setting.check.chkboxType = type;
                    $scope.$apply();
                }
            }
        });
    });

    $scope.submit = function () {
        $scope.model.Status = $scope.model.StatusTemp === '1' ? true : false;
        $("#formSubmit").validate({
            rules: {
                ID: {
                    required: true,
                    maxlength: 50
                },
                Name: {
                    required: true,
                    maxlength: 50
                }

            },
            messages: {
                ID: {
                    required: "Vui lòng nhập mã",
                    maxlength: "Mã không được vượt quá 50 ký tự"
                },
                Name: {
                    required: "Vui lòng nhập tên quyền",
                    maxlength: "Tên quyền không được vượt quá 50 ký tự"
                }
            }
        });
        if ($("#formSubmit").valid()) {

            var treeObj = $.fn.zTree.getZTreeObj("treeRole");
            var nodes = treeObj.getCheckedNodes();

            angular.forEach($scope.ListPageMenu, function (pageMenu) {

                var dataSearch = nodes.filter(function (item) {
                    return item.id === pageMenu.id;
                });

                if (dataSearch !== null && dataSearch.length > 0) {
                    pageMenu.checked = true;
                }

            })

            $.ajax({
                type: 'post',
                url: '/Role/Edit',
                data: { role: $scope.model, pageMenus: $scope.ListPageMenu },
                success: function (data) {
                    if (data.Error) {
                        toastr.error(data.Title);
                    } else {
                        toastr.success(data.Title);
                        $scope.cancel();
                        $scope.LoadPage();
                    }
                }
            });
        }
    };

    $scope.cancel = function () {
        $uibModalInstance.close();
    };
});