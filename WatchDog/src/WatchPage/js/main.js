
$(document).ready(function () {
    $(".tabs").click(function () {
        $(".tabs").removeClass("active");
        $(".tabs h6").removeClass("font-weight-bold");
        $(".tabs h6").addClass("text-muted");
        $(this).children("h6").removeClass("text-muted");
        $(this).children("h6").addClass("font-weight-bold");
        $(this).addClass("active");
        current_fs = $(".active");
        next_fs = $(this).attr('id');
        next_fs = "#" + next_fs + "1";
        $("fieldset").removeClass("show");
        $(next_fs).addClass("show");
        current_fs.animate({}, {
            step: function () {
                current_fs.css({
                    'display': 'none',
                    'position': 'relative'
                });
                next_fs.css({
                    'display': 'block'
                });
            }
        });
    });
});

var pageIndex = 1;
var connection = new signalR.HubConnectionBuilder().withUrl("/wtchdlogger").build();

connection.on("getLogs", function (data) {
    if (sessionStorage.getItem("loggedIn") !== null && sessionStorage.getItem("loggedIn") === sessionStorage.getItem("newloggedIn")) {
        const firstDigitStr = String(data.responseStatus)[0];
        var statusColor = firstDigitStr === '2' ? "text-success" : firstDigitStr === '1' || firstDigitStr === '3' ? "text-primary" : "text-danger";
        const tr = $("<tr data-toggle='modal' data-target='#myModal' aria-expanded='false' aria-controls='collapse' class='collapsed'>" +
            "<td><b>" + data.method + "</b></td><td>" + data.path + "</td><td><b class='" + statusColor + "'> " + data.responseStatus + "</b></td><td>" + data.timeSpent + "</td><td>" + moment(data.startTime).format('LLL') + "</td></tr>");
        tr.on('click', populateModal.bind(null, data));
        $('#tableBody').prepend(tr);
    }

});

connection
    .start()
    .then(function () {
        getLogs()
    }).catch(err => console.log('Error while starting connection: ' + err));

$("#myVerbDropDown").change(function (event) {
    $('#tableBody').empty();
    if ($("#myVerbDropDown")[0].selectedIndex === 0) {
        getLogs()
    } else {
        filterVerb($(this).val());
    }

});

$("#myStatusCodeDropDown").change(function (event) {
    $('#tableBody').empty();
    if ($("#myStatusCodeDropDown")[0].selectedIndex === 0) {
        getLogs()
    } else {
        filterStatusCode($(this).val());
    }

});

function getLogs(searchString = "", verb = "", statusCode = "") {
    $.ajax({
        type: "GET",
        url: "/WTCHDwatchpage?pageNumber=" + pageIndex + "&searchString=" + searchString + "&verbString=" + verb + "&statusCode=" + statusCode,
        context: document.body,
        success: function (data) {
            var totalPages = data.totalPages === 0 ? 1 : data.totalPages
            $('#pageMap').text("Page " + data.pageIndex + " of " + totalPages);
            if (data.hasNext === false) {
                $('#next').hide();
            }
            else {
                $('#next').show();

            }

            if (data.hasPrevious === false) {
                $('#prev').hide();
            }
            else {
                $('#prev').show();
            }

            if (sessionStorage.getItem("loggedIn") === null) {
                $('#myLoginModal').modal('show');

            } else {
                if (sessionStorage.getItem("loggedIn") !== sessionStorage.getItem("newloggedIn")) {
                    $('#myLoginModal').modal('show');
                } else {
                    for (var i = 0; i < data.logs.length; i++) {
                        const firstDigitStr = String(data.logs[i].responseStatus)[0];
                        var statusColor = firstDigitStr === '2' ? "text-success" : firstDigitStr === '1' || firstDigitStr === '3' ? "text-primary" : "text-danger";
                        const tr = $("<tr data-toggle='modal' data-target='#myModal' aria-expanded='false' aria-controls='collapse" + i + "' class='collapsed'>" +
                            "<td><b>" + data.logs[i].method + "</b></td><td>" + data.logs[i].path + "</td><td><b class='" + statusColor + "'> " + data.logs[i].responseStatus + "</b ></td ><td>" + data.logs[i].timeSpent + "</td><td>" + moment(data.logs[i].startTime).format('LLL') + "</td></tr > ");
                        tr.on('click', populateModal.bind(null, data.logs[i]));
                        $('#tableBody').append(tr);
                    }
                }
            }

        }
    });
}


function clearLogs() {
    $.ajax({
        type: "POST",
        url: "/WTCHDwatchpage/ClearLogs",
        success: function (data) {
            window.location.reload()
        }
    });
}

function search() {
    pageIndex = 1;
    $('#tableBody').empty();
    var ss = $('#searchString').val();
    getLogs(ss);
}

function filterVerb(verbString) {
    pageIndex = 1;
    $('#tableBody').empty();
    getLogs("", verbString, "");
}

function filterStatusCode(statusCodeString) {
    pageIndex = 1;
    $('#tableBody').empty();
    getLogs("", "", statusCodeString);
}

function nextPage() {
    ++pageIndex;
    $('#tableBody').empty();
    getLogs();
}

function prevPage() {
    if (pageIndex > 1)
        --pageIndex;
    $('#tableBody').empty();
    getLogs();
}

function backToList() {
    pageIndex = 1;
    $('#tableBody').empty();
    getLogs();
}

function login() {
    var form = document.querySelector('form')
    form.reportValidity()
    if (form.checkValidity()) {
        $.ajax({
            type: "POST",
            url: "/WTCHDwatchpage/Auth",
            data: {
                username: $("#uname").val(),
                password: $("#psw").val()
            },
            context: document.body,
            success: function (data) {
                if (data === true) {
                    sessionStorage.setItem("newloggedIn", generateUUID())
                    sessionStorage.setItem("loggedIn", sessionStorage.getItem("newloggedIn"))
                } else {
                    sessionStorage.setItem("loggedIn", null)
                }

                window.location.reload()
            }
        });
    }

}

function populateModal(data) {
    $('#host').text(data.host);
    $('#path').text(data.path);
    $('#query').text(data.queryString);
    $('#method').text(data.method);
    $('#ip').text(data.ipAddress);
    $('#statusCode').text(data.responseStatus);
    $('#time').text(data.timeSpent);
    $('#startTime').text(moment(data.startTime).format('LLL'));

    $('#reqHd').text(data.requestHeaders);
    $('#resHd').text(data.responseHeaders);
    $('#reqBody').text(data.requestBody);

    $('#resBody').text(data.responseBody);

    if (data.responseBody != null) {
        var element = document.getElementById("resBody");
        var obj = JSON.parse(element.innerText);
        element.innerHTML = JSON.stringify(obj, undefined, 2);
    }
    if (data.requestBody != null) {
        var element2 = document.getElementById("reqBody");
        var obj2 = JSON.parse(element2.innerText);
        element2.innerHTML = JSON.stringify(obj, undefined, 2);
    }
}

function generateUUID() { // Public Domain/MIT
    var d = new Date().getTime();//Timestamp
    var d2 = ((typeof performance !== 'undefined') && performance.now && (performance.now() * 1000)) || 0;//Time in microseconds since page-load or 0 if unsupported
    return 'xxxxxxxx-xxxx-4xxx-yxxx-xxxxxxxxxxxx'.replace(/[xy]/g, function (c) {
        var r = Math.random() * 16;//random number between 0 and 16
        if (d > 0) {//Use timestamp until depleted
            r = (d + r) % 16 | 0;
            d = Math.floor(d / 16);
        } else {//Use microseconds since page-load if supported
            r = (d2 + r) % 16 | 0;
            d2 = Math.floor(d2 / 16);
        }
        return (c === 'x' ? r : (r & 0x3 | 0x8)).toString(16);
    });
}