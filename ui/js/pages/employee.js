$(document).ready(function () {
  // Gan cac su kien cho cac element
  initEvent();
  // Lay du lieu
  loadData();
});

var act = null;
var employeeSelected = null;
var dir = "http://localhost:9074/";
var allDepartments = null;
var allPositions = null;

/**
 * Lay du lieu tu API
 * Author: Nguyen Van Thinh
 */
// Tải dữ liệu
function loadData() {
  // Goi API loc du lieu hien thi
  filterData();
  // Goi API lay tat ca Phong ban
  getAllDepartments();
  // Goi API lay tat ca Vi tri
  getAllPositions();
}

/**
 * Tao cac su kien
 * Author: Nguyen Van Thinh
 * Form luôn mặc định focus vào ô nhập liệu Mã nhân viên
 */

function initEvent() {
  /**
   * MENU
   */
  // click vao menu items
  $(".menu-content").click(function () {
    $(this).siblings().removeClass("menu-content--selected");
    $(this).addClass("menu-content--selected");
    title = $(this).attr("title") + ".html";
    // Change pages
    window.location.href = title;
  });
  /**
   * Events in Content
   */
  // Loc nhan vien
  $(".input-search").keyup(filterData);
  $("select#department.select").change(filterData);
  $("select#position.select").change(filterData);
  $("#pageSize").change(filterData);

  /**
   *  Events in POP-UP
   */
  // click nut them moi nhan vien
  $(".icon-add").click(function () {
    act = "create";
    // Hien thi pop-up
    $("#newEmployee").show();

    // Lam sach cac o inputs
    const inputs = $("#newEmployee select, #newEmployee input");
    for (const input of inputs) {
      input.value = "";
    }

    // Focus vào ô nhập liệu Mã nhân viên
    $("#newEmployee input")[0].focus();

    // Ngày sinh/ Ngày cấp/ Ngày gia nhập công ty nho hon hoang bang ngày hiện tại
    $("input[date]").attr("max", limitDate());

    // Thiết lập giá trị mặc định của ô Vị trí và Phòng ban
    const firstDepartment = $("select#department.input-select option")[0].value;
    $("select#department.input-select").val(firstDepartment);
    const firstPosition = $("select#position.input-select option")[0].value;
    $("select#position.input-select").val(firstPosition);

    // Mã nhân viên tự động có và tự tăng theo tiêu chí: “NV” + mã số nhân viên lớn nhất trong hệ thống + 1

    // thay doi mau khung khi o thong tin can nhap bi bo qua
    $("input[mandatory]").blur(function () {
      let value = this.value;
      if (!value) {
        $(this).addClass("input-err");
        $(this).attr("title", "Thông tin bắt buộc");
      } else {
        $(this).removeClass("input-err");
        $(this).removeAttr("title");
      }
    });

    $("input[number]").keydown(function () {
      // Thêm tiêu đề cho input
      $(this).attr("title", "Chỉ được nhập số");
      // Chỉ được nhập ký tự số
      const keyCode = event.keyCode;
      if (
        (keyCode >= 48 && keyCode <= 57) ||
        keyCode == 8 ||
        (keyCode >= 37 && keyCode <= 40)
      )
        return true;
      else return false;
    });

    // Email phải đúng định dạng (VD: example@domain.com)
    $("input[email]").blur(function () {
      let email = this.value;
      if (!checkEmailFormat(email)) {
        $(this).addClass("input-err");
        $(this).attr("title", "Sai định dạng");
      } else {
        $(this).removeClass("input-err");
        $(this).removeAttr("title");
      }
    });
  });

  // click vao nut lam moi
  $("#btnRefresh").click(function () {
    filterData();
  });

  // click vao nut nhan ban
  $(".icon-duplicate").click(duplicateData);

  // click vao nut xoa
  $(".icon-remove").click(function () {
    if (!employeeSelected) return;
    $("#warning").show();
    $("#warning .dialog_text").empty();
    let txt = `Bạn có chắc chắn muốn xóa nhân viên <b>${employeeSelected.employeeCode}</b> không?`;
    $("#warning .dialog_text").append(txt);
  });

  // click vao nut "Co"
  $("#btnOK").click(deleteData);

  // click vao 1 dong trong bang
  $(document).on("click", ".table-info .row", function () {
    $(this).siblings().removeClass("row-selected");
    $(this).addClass("row-selected");
    employeeSelected = $(this).data().entity;
  });

  // double click vao 1 dong trong bang
  $(document).on("dblclick", ".table-info .row", function () {
    act = "modify";
    $("#newEmployee").show();
    $("#employeeCode").focus();
    // Dữ liệu nhân viên cần sửa sẽ tự động được điền vào các ô dữ liệu tương ứng.
    const employee = $(this).data().entity;
    const inputs = $("#newEmployee select, #newEmployee input");
    for (const input of inputs) {
      const propValue = $(input).attr("propValue");
      if (
        propValue == "IdentityDate" ||
        propValue == "JoinDate" ||
        propValue == "DateOfBirth"
      )
        input.value = "2022-09-08";
      else input.value = employee[propValue];
    }
  });

  // click nut x
  $(".pop-up__background .icon-x").click(function () {
    $(this).parents(".pop-up__background").hide();
  });

  $(".toast .icon-x").click(function () {
    $(this).parents(".toast").hide();
  });

  // click nut luu
  $(".icon-save").click(saveData);

  // click nut cancel
  $(".button-cancel").click(function () {
    $(this).parents(".pop-up__background").hide();
  });

  // click vao so trang
  $(".page-number button").click(function () {
    $(this).siblings().removeClass("page-number--selected");
    $(this).addClass("page-number--selected");
    employeeSelected = $(this).data().entity;
  });
}

/**go
 * Hàm thực thi
 * Comment: Thực hiện tính năng (Gọi API)
 */
// Lọc dữ liệu
async function filterData() {
  let queryParams = "";
  const keyword = $(".icon-search").val();
  const departmentID = $("select#department.select").val();
  const positionID = $("select#position.select").val();
  const pageSize = $("#pageSize").val();
  queryParams +=
    "keyword=" +
    keyword +
    "&departmentID=" +
    departmentID +
    "&positionID=" +
    positionID +
    "&pageSize=" +
    pageSize;
  try {
    await $.ajax({
      type: "GET",
      async: false,
      url: dir + "api/v1/Employees/filter?" + queryParams,
      success: function (employees) {
        console.log("Filtering Data ...");
        // Hiển thị số lượng bản ghi
        const totalCount = employees.totalCount;
        const value = pageSize < totalCount ? pageSize : totalCount;
        let selector = $(".footer-left b");
        selector.empty();
        selector.append("1-" + value + "/" + totalCount);
        // Làm sạch table
        $(".table-info").empty();
        // Xu ly du lieu
        let cols = $(".table-header div");
        for (const employee of employees.data) {
          let rowOfTable = $('<div class="row"></div>');
          for (const col of cols) {
            // Lay ra propValue tương ứng với các cột:
            const propValue = $(col).attr("propValue");
            // Lay ra format tương ứng với các cột
            const format = $(col).attr("format");
            // Lay thong tin
            let value = employee[propValue];
            // Dinh dang (Ngay sinh, Gioi tinh, Muc luong co ban)
            if (value) {
              switch (format) {
                // Chuyen doi tu so ve chuoi cho gender va workStatus
                case "gender": {
                  value = value.toString();
                  const options = $("#gender option");
                  for (const opt of options) {
                    const optValue = $(opt).attr("value");
                    if (optValue == value) {
                      value = $(opt).attr("label");
                      break;
                    }
                  }
                  break;
                }
                case "workStatus": {
                  value = value.toString();
                  const options = $("#workStatus option");
                  for (const opt of options) {
                    const optValue = $(opt).attr("value");
                    if (optValue == value) {
                      value = $(opt).attr("label");
                      break;
                    }
                  }
                  break;
                }
                case "date":
                  value = formatDate(value);
                  break;
                case "money":
                  value = Math.round(value);
                  value = formatMoney(value);
                  break;
                default:
                  break;
              }
            }
            // Tạo rowHTML:
            const thHTML = `<div class='${propValue}'>${value || ""}</div>`;
            rowOfTable.append(thHTML);
          }
          $(rowOfTable).data("entity", employee); // init data() of row
          $(".table-info").append(rowOfTable);
        }
      },
    });
  } catch (error) {
    console.log("filterData\n", error);
  }
}
// Lấy tất cả nhân viên
function getAllEmployees() {
  try {
    $.ajax({
      type: "GET",
      async: false,
      url: dir + "api/v1/Employees/",
      success: function (employees) {
        console.log("Loading...");
        // Xu ly du lieu
        let cols = $(".table-header div");
        for (const employee of employees) {
          let rowOfTable = $('<div class="row"></div>');
          for (const col of cols) {
            // Lay ra propValue tương ứng với các cột:
            const propValue = $(col).attr("propValue");
            // Lay ra format tương ứng với các cột
            const format = $(col).attr("format");
            // Lay thong tin
            let value = employee[propValue];
            // Dinh dang (Ngay sinh, Gioi tinh, Muc luong co ban)
            if (value) {
              switch (format) {
                // Chuyen doi tu so ve chuoi cho gender va workStatus
                case "gender": {
                  value = value.toString();
                  const options = $("#gender option");
                  for (const opt of options) {
                    const optValue = $(opt).attr("value");
                    if (optValue == value) {
                      value = $(opt).attr("label");
                      break;
                    }
                  }
                  break;
                }
                case "workStatus": {
                  value = value.toString();
                  const options = $("#workStatus option");
                  for (const opt of options) {
                    const optValue = $(opt).attr("value");
                    if (optValue == value) {
                      value = $(opt).attr("label");
                      break;
                    }
                  }
                  break;
                }
                case "date":
                  value = formatDate(value);
                  break;
                case "money":
                  value = Math.round(value);
                  value = formatMoney(value);
                  break;
                default:
                  break;
              }
            }
            // Tạo rowHTML:
            const thHTML = `<div class='${propValue}'>${value || ""}</div>`;
            rowOfTable.append(thHTML);
          }
          $(rowOfTable).data("entity", employee); // init data() of row
          $(".table-info").append(rowOfTable);
        }
      },
    });
  } catch (error) {
    errorMessage("Có lỗi xảy ra", error);
  }
}
// Lấy tất cả phòng ban
async function getAllPositions() {
  try {
    await $.ajax({
      type: "GET",
      // async: false,
      url: dir + "api/v1/Positions/",
      success: function (positions) {
        console.log("Loading positions data ...");
        // Lay thong tin vi tri
        let filterPostion = $("select#position.select");
        let option = null;
        // Ghép thông tin vào ô lọc vị trí
        for (const position of positions) {
          option = $("<option></option>");
          option.attr("value", position.positionID);
          option.append(position.positionName);
          filterPostion.append(option);
        }
        // Ghép thông tin vào ô chọn vị trí (form tạo mới, chỉnh sửa)
        let selectPosition = $("select#position.input-select");
        for (const position of positions) {
          option = $("<option></option>");
          option.attr("value", position.positionName);
          option.attr("guid", position.positionID);
          option.append(position.positionName);
          selectPosition.append(option);
        }
      },
    });
  } catch (error) {
    errorMessage("Có lỗi xảy ra", error);
  }
}
// Lây tất cả vị trí
async function getAllDepartments() {
  try {
    $.ajax({
      type: "GET",
      // async: false,
      url: dir + "api/v1/Departments/",
      success: function (departments) {
        console.log("Loading departments data ...");
        allDepartments = departments;
        // Lay thong tin vi tri
        let filterPostion = $("select#department.select");
        let option = null;
        // Ghép thông tin vào ô lọc vị trí
        for (const department of departments) {
          option = $("<option></option>");
          option.attr("value", department.departmentID);
          option.append(department.departmentName);
          filterPostion.append(option);
        }
        // Ghép thông tin vào ô chọn vị trí (form tạo mới, chỉnh sửa)
        let selectdepartment = $("select#department.input-select");
        for (const department of departments) {
          option = $("<option></option>");
          option.attr("value", department.departmentName);
          option.attr("guid", department.departmentID);
          option.append(department.departmentName);
          selectdepartment.append(option);
        }
      },
    });
  } catch (error) {
    errorMessage("Có lỗi xảy ra", error);
  }
}
// Luu nhan vien vao database
function saveData() {
  // Khoi tao du lieu ban dau
  let employee = null;
  if (act == "create")
    employee = {
      positionID: $("select#position.input-select option")[0].attributes["guid"]
        .value,
      departmentID: $("select#department.input-select option")[0].attributes[
        "guid"
      ].value,
    };
  else employee = employeeSelected;

  // Thu thap du lieu
  const inputs = $("#newEmployee select, #newEmployee input");
  for (const input of inputs) {
    let val = input.value;
    const propValue = $(input).attr("propValue");
    employee[propValue] = val ? val : "";
  }

  employee["createdBy"] = "";
  employee["modifiedBy"] = "";
  if (employee.dateOfBirth == "") delete employee.dateOfBirth;
  if (employee.identityIssuedDate == "") delete employee.identityIssuedDate;
  if (employee.gender == "") delete employee.gender;
  if (employee.workStatus == "") delete employee.workStatus;
  if (employee.salary == "") delete employee.salary;

  console.log(employee);
  if (!checkMandatoryInput(employee)) return;
  employee = JSON.stringify(employee);
  // Goi API
  if (act == "create") {
    try {
      $.ajax({
        type: "POST",
        url: dir + "api/v1/Employees/",
        data: employee,
        dataType: "json",
        contentType: "application/json",
        success: function (response) {
          successMessage(`Thêm mới thành công`);
          filterData();
        },
      });
    } catch (error) {
      errorMessage("Có lỗi xảy ra", error);
    }
  } else {
    try {
      $.ajax({
        type: "PUT",
        url: dir + "api/v1/Employees/" + employeeSelected.employeeID,
        data: employee,
        dataType: "json",
        contentType: "application/json",
        success: function (response) {
          successMessage(`Chỉnh sửa thành công`);
          filterData();
        },
      });
    } catch (error) {
      errorMessage("Có lỗi xảy ra", error);
    }
  }
  $(this).parents(".pop-up__background").hide();
}
// Tính năng Nhân bản
function duplicateData() {
  act = "create";
  if (!employeeSelected) return;
  // Hien thi pop-up
  $("#newEmployee").show();
  // Thu thap thong tin
  const inputs = $("#newEmployee select, #newEmployee input");
  for (const input of inputs) {
    const propValue = $(input).attr("propValue");
    input.value = employeeSelected[propValue];
  }
  deleteData();
}
// Xoa nhan vien khoi database
function deleteData() {
  // Goi API DELETE
  try {
    $.ajax({
      type: "DELETE",
      url: dir + "api/v1/Employees/" + employeeSelected.employeeID,
      success: function (response) {
        successMessage("Xóa thành công");
        filterData();
      },
    });
  } catch (error) {
    errorMessage("Có lỗi xảy ra", error);
  }

  // An pop-up
  $("#warning").hide();
}

/**
 * Hàm định dạng dữ liệu
 */

// Dinh dang hien thi ngay/thang/nam
function formatDate(date) {
  try {
    if (date) {
      date = new Date(date);
      // Lay ngay
      const year = date.getFullYear();
      if (year == 1) return null;
      let day = date.getDate();
      day = day <= 9 ? `0${day}` : day;
      // Lay thang
      let month = date.getMonth() + 1;
      month = month <= 9 ? `0${month}` : month;
      // Lay nam
      return `${day}/${month}/${year}`;
    }
  } catch (error) {
    errorMessage("Có lỗi xảy ra");
    console.log("formatDate\n", error);
  }
}

// Thông tin mức lương khi nhập cần được định dạng hiển thị tiền tệ: VD 2.000.000
function formatMoney(salary) {
  try {
    money = new Intl.NumberFormat("de-DE").format(salary);
    return money;
  } catch (error) {
    errorMessage("Có lỗi xảy ra");
    console.log("formatMoney\n", error);
  }
}

/**
 * Hàm thông báo toast
 */

function successMessage(content) {
  // tien xu ly
  const element = $("#toastOK");
  element.show();
  element.find(".toast__content").empty();
  // Them noi dung
  element.find(".toast__content").append(content);
  // in ra console
  console.log(content + "!");
}

function errorMessage(content, error) {
  // tien xu ly
  const element = $("#toastERR");
  element.show();
  element.find(".toast__content").empty();
  // Them noi dung
  element.find(".toast__content").append(content);
  // in ra console
  if (error) console.log(error);
}

// Kiểm tra đầu vào
function checkMandatoryInput(employee) {
  if (
    !employee.employeeCode ||
    !employee.fullName ||
    !employee.identityNumber ||
    !employee.email ||
    !employee.phoneNumber
  ) {
    console.log(employee);
    debugger;
    errorMessage("Điền thiếu thông tin bắt buộc");
    const inputs = $("input[mandatory]");
    for (const input of inputs) {
      const value = input.value;
      if (!value) {
        $(input).addClass("input-err");
        $(input).attr("title", "Thông tin bắt buộc");
      } else {
        $(input).removeClass("input-err");
        $(input).removeAttr("title");
      }
    }
    return false;
  }
  return true;
}

function checkFullName(fullName) {
  if (fullName.length > 100) {
    errorMessage("Độ dài tên chứa tối đa 100 ký tự");
    return false;
  }
  return true;
}

// Gioi han ngay nhap
function limitDate() {
  let today = new Date();
  let dd = today.getDate();
  let mm = today.getMonth() + 1; //January is 0!
  let yyyy = today.getFullYear();

  if (dd < 10) {
    dd = "0" + dd;
  }

  if (mm < 10) {
    mm = "0" + mm;
  }

  today = yyyy + "-" + mm + "-" + dd;
  return today;
}

// Validate
function checkEmailFormat(email) {
  const re =
    /^(([^<>()[\]\.,;:\s@\"]+(\.[^<>()[\]\.,;:\s@\"]+)*)|(\".+\"))@(([^<>()[\]\.,;:\s@\"]+\.)+[^<>()[\]\.,;:\s@\"]{2,})$/i;
  return email.match(re);
}
