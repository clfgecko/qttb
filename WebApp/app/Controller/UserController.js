app.controller("UserController", function ($scope, $uibModal, $ngConfirm, showToast, hideLoading) {
    $scope.modelSearch = {};
    $scope.modelSearch.totalItems = 0;
    $scope.modelSearch.currentPage = 1;
    $scope.modelSearch.maxSize = 5;
    $scope.modelSearch.pageSize = 10;
    $scope.modelSearch.SortColumn = "UserName DESC";
    $scope.ListUserGroup = [];
    $scope.ListUser = [];

    angular.element(document).ready(function () {
        GetBottomAction();
        $scope.LoadPage();
    });

    $scope.RoleBtnCreate = false;
    $scope.RoleBtnUpdate = false;
    $scope.RoleBtnSearch = false;
    $scope.RoleBtnDelete = false;
    $scope.RoleBtnView = false;

    function GetBottomAction() {
        $.ajax({
            type: 'post',
            url: '/User/GetBottomAction',
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
                        if (item == 'btnView') {
                            $scope.RoleBtnView = true;
                        }
                    });
                }
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
            url: '/User/GetListUser',
            data: $scope.modelSearch,
            success: function (data) {
                $scope.modelSearch.totalItems = data.totalItems;
                $scope.ListUser = data.data;
                $scope.$apply();
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
            templateUrl: '/User/_Add',
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
            templateUrl: '/User/_Edit',
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

    $scope.cancel = function () {
        $uibModalInstance.close();
    };

    $scope.delete = function (itemId, Username) {
        $ngConfirm({
            title: 'Thông báo',
            content: 'Bạn có chắc chắn muốn xóa tài khoản ' + Username + ' không?',
            scope: $scope,
            buttons: {
                delete: {
                    text: 'Xóa',
                    btnClass: 'btn-blue',
                    action: function (scope, button) {
                        showToast();
                        $.ajax({
                            type: 'post',
                            url: '/User/Delete',
                            data: { Id: itemId },
                            success: function (data) {
                                if (data.Error) {
                                    toastr.error(data.Title);
                                } else {
                                    toastr.success(data.Title);
                                    $scope.LoadPage();
                                }
                                hideLoading();
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
    $scope.ph_numbr = /(09|01[2|6|8|9])+([0-9]{8})\b/;
    $scope.ListUserGroup = [];
    $scope.ListEmployee = [];
    $scope.FileName = "";
    angular.element(document).ready(function () {
        showToast();
        GetDanhMuc();
    });

    function GetDanhMuc() {
        $.ajax({
            type: 'post',
            url: '/User/GetDanhMuc',
            data: {},
            success: function (data) {
                $scope.ListUserGroup = data.data;
                $scope.ListStatus = [{ ID: true, Name: 'Sử dụng' }, { ID: false, Name: 'Không sử dụng' }];
                $scope.model.GroupID = "";
                $scope.model.Status = true;
                $scope.$apply();
                hideLoading();
            }
        });
    }

    $scope.model = {};
    $scope.submit = function () {
        showToast();
        $("#formSubmit").validate({
            rules: {
                UserName: {
                    required: true,
                    maxlength: 50
                },
                Status: {
                    required: true
                }

            },
            messages: {
                UserName: {
                    required: "Vui lòng nhập họ và tên",
                    maxlength: "Họ và tên không được vượt quá 50 ký tự"
                },
                Status: {
                    required: "Vui lòng chọn trạng thái"
                },
                Email: {
                    email: "Vui lòng nhập đúng định dạng email"
                }
            }
        });
        if ($("#formSubmit").valid()) {
            $.ajax({
                type: 'post',
                url: '/User/Add',
                data: { user: $scope.model, fileName: $scope.FileName },
                success: function (data) {
                    if (data.Error) {
                        toastr.error(data.Title);
                    } else {
                        toastr.success(data.Title);
                        $scope.cancel();
                    }
                    hideLoading();
                }
            });
        }
    };


    $scope.cancel = function () {
        $uibModalInstance.close();
    };

    $scope.SelectFile = function (e) {
        $scope.model.Avartar = "";
        $scope.FileName = "";
        if (e.target.files[0]) {
            if (e.target.files[0].size > 5242880) {
                toastr.error("Bạn không được tải file lên lớn quá 5M.");
            } else {
                $scope.FileName = e.target.files[0].name;
                $("#lableFile").text($scope.FileName);
                var reader = new FileReader();
                reader.onload = function (e1) {
                    var base64 = "";
                    var checkPNG = e1.target.result.split(',');
                    if (checkPNG != null && checkPNG.length > 0) {
                        base64 = checkPNG[checkPNG.length - 1]
                    }
                    $scope.model.Avartar = base64;
                    $('#pathPhoto').attr('src', e1.target.result);
                };
                reader.readAsDataURL(e.target.files[0]);
            }
        }
    };
});

app.controller('edit', function ($scope, $uibModalInstance, itemId, $ngConfirm, showToast, hideLoading) {
    $scope.ListUserGroup = [];
    $scope.FileName = "";
    $scope.model = {};
    angular.element(document).ready(function () {
        showToast();
        GetDanhMuc();
        $.ajax({
            type: 'post',
            url: '/User/GetItemByID',
            data: { Id: itemId },
            success: function (data) {
                if (data.Error) {
                    toastr.error(data.Title);
                } else {
                    $scope.ListStatus = [{ ID: true, Name: 'Sử dụng' }, { ID: false, Name: 'Không sử dụng' }];
                    $scope.model = data.data; 
                    $scope.model.GroupID = $scope.model.UserGroupID;
                    $scope.$apply();
                    hideLoading();
                }
            }
        });
    });


    function GetDanhMuc() {
        $.ajax({
            type: 'post',
            url: '/User/GetDanhMuc',
            data: {},
            success: function (data) {
                $scope.ListUserGroup = data.data;
                $scope.$apply();
            }
        });
    }
    $scope.submit = function () {
        $("#formSubmit").validate({
            rules: {
                UserName: {
                    required: true,
                    maxlength: 50
                },
                Status: {
                    required: true
                }
            },
            messages: {
                UserName: {
                    required: "Vui lòng nhập họ và tên",
                    maxlength: "Họ và tên không được vượt quá 50 ký tự"
                },
                Status: {
                    required: "Vui lòng chọn trạng thái"
                }
            }
        });
        if ($("#formSubmit").valid()) {
            $.ajax({
                type: 'post',
                url: '/User/Edit',
                data: { user: $scope.model, fileName: $scope.FileName },
                success: function (data) {
                    if (data.Error) {
                        toastr.error(data.Title);
                    } else {
                        toastr.success(data.Title);
                        $scope.cancel();
                    }
                }
            });
        }
    };

    $scope.cancel = function () {
        $uibModalInstance.close();
    };

    $scope.SelectFile = function (e) {
        $scope.model.Avartar = "";
        $scope.FileName = "";
        if (e.target.files[0]) {
            if (e.target.files[0].size > 5242880) {
                toastr.error("Bạn không được tải file lên lớn quá 5M.");
            } else {
                $scope.FileName = e.target.files[0].name;
                $("#lableFile").text($scope.FileName);
                var reader = new FileReader();
                reader.onload = function (e1) {
                    var base64 = "";
                    var checkPNG = e1.target.result.split(',');
                    if (checkPNG != null && checkPNG.length > 0) {
                        base64 = checkPNG[checkPNG.length - 1]
                    }
                    $scope.model.Avartar = base64;
                    $('#pathPhoto').attr('src', e1.target.result);
                };
                reader.readAsDataURL(e.target.files[0]);
            }
        }
    };

    //$scope.ChangeEmployee = function () {

    //    var documentCheck = $scope.ListEmployee.filter(function (x) {
    //        return (x.Id == $scope.model.EmployeeId);
    //    });
    //    if (documentCheck != null && documentCheck.length > 0) {
    //        $scope.model.Name = documentCheck[0].FullName;
    //        $scope.model.Phone = documentCheck[0].Phone;
    //        $scope.model.Email = documentCheck[0].Email;
    //        if (documentCheck[0].AvatarPath != null && documentCheck[0].AvatarPath != "") {
    //            $.ajax({
    //                type: 'POST',
    //                dataType: 'json',
    //                cache: false,
    //                async: true,
    //                url: '/User/ConvertPathImageToBase64',
    //                data: {
    //                    path: documentCheck[0].AvatarPath
    //                },
    //                success: function (response) {
    //                    var base64File = "";
    //                    if (response.data != null && response.data != "") {
    //                        $scope.model.Avartar = response.data;
    //                        base64File = 'data:image/jpeg;base64,' + response.data;
    //                    }
    //                    else {
    //                        base64File = '/Content/Images/noimg.png';
    //                    }
    //                    $('#pathPhoto').attr('src', base64File);
    //                }
    //                , error: function (xhr) {
    //                }
    //            });
    //        }
    //    }
    //};

    ////type: 1 là create, 2 là update, 3 là detail
    //function ConvertImageToBase64(path) {
    //    $.ajax({
    //        type: 'POST',
    //        dataType: 'json',
    //        cache: false,
    //        async: true,
    //        url: '/User/ConvertPathImageToBase64',
    //        data: {
    //            path: path
    //        },
    //        success: function (response) {
    //            var base64File = "";
    //            if (response.data != null && response.data != "") {
    //                base64File = 'data:image/jpeg;base64,' + response.data;
    //            }
    //            else {
    //                base64File = '/Content/Images/noimg.png';
    //            }
    //            $('#pathPhoto').attr('src', base64File);
    //        }
    //        , error: function (xhr) {
    //        }
    //    });
    //}
});