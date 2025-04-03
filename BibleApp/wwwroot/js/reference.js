function loadBook(id, cn) {
    $("#book-selector").dropdown("hide")
    spinner.show()
    $.ajax("/Home/LoadBook?bookId=" + id, {
        type: "GET",
        complete: function(data) {
            $("#chapter-selector").replaceWith(data.responseText)
            $("#bookSelectorBtn").html(cn)
            $("#book-num").val(id)
            spinner.hide()
        }
    })
}
function loadChapter(bookId, chapId) {
    const startTime = Date.now()
    $("#chapter-selector").dropdown("hide")
    spinner.show()
    $.ajax(`/Home/LoadChapter?bookId=${bookId}&chapterId=${chapId}`, {
        type: "GET",
        complete: function(data) {
            $("#verses").replaceWith(data.responseText)
            $("#chapterSelectorButton").html("Chapter " + chapId)
            const time = (Date.now() - startTime)
            $("#chapter-header-loadtime-num").html(`${time/1000}`)
            $("#book-num").val(bookId)
            $("#chapter-num").val(chapId)
            spinner.hide()
        }
    })
}
function checkUrlRef() {
    const url = new URLSearchParams(window.location.search)
    const book = url.get("book")
    const chapter = url.get("chapter")
    if (book && !Number.isNaN(book) && chapter && !Number.isNaN(chapter)) {
        const book = Number.parseInt(url.get("book"))
        const bookName = $(`#book-name-${book}`).val()
        const chapter = Number.parseInt(url.get("chapter"))
        loadBook(book, bookName)
        loadChapter(book, chapter)
    }
}
function refShare() {
    let url = window.location.protocol + "//" + window.location.host + "/Home/Reference"
    const bookNum = $("#book-num").val()
    const chapterNum = $("#chapter-num").val()
    if (bookNum !== -1 && chapterNum !== -1) {
        url += "?book=" + bookNum + "&chapter=" + chapterNum
    }
    navigator.clipboard.writeText(url).then(r => {
        const ssa = $("#share-success-alert")
        ssa.show()
        ssa.addClass("show")

        setTimeout(() => {
            ssa.removeClass("show")
            ssa.hide()
        }, 2000)
    })
}