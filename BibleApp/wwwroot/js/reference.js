/**
 * Sends an AJAX request to load the chapters for the selected book
 * @param id the id of the chapter
 * @param cn the name of the chapter
 */
function loadBook(id, cn) {
    // close dropdown
    $("#book-selector").dropdown("hide")
    // show loading
    spinner.show()
    // get chapter selector
    $.ajax("/Home/LoadBook?bookId=" + id, {
        type: "GET",
        complete: function(data) {
            $("#chapter-selector").replaceWith(data.responseText)
            // change dropdown text
            $("#bookSelectorBtn").html(cn)
            // change metadata hidden field
            $("#book-num").val(id)
            // show done loading
            spinner.hide()
        }
    })
}

/**
 * sends an AJAX request to load all the verses in a book and chapter
 * @param bookId id of the current book
 * @param chapId id of the selected chapter
 */
function loadChapter(bookId, chapId) {
    // start time for load badge
    const startTime = Date.now()
    // close dropdown
    $("#chapter-selector").dropdown("hide")
    // show loading
    spinner.show()
    $.ajax(`/Home/LoadChapter?bookId=${bookId}&chapterId=${chapId}`, {
        type: "GET",
        complete: function(data) {
            $("#verses").replaceWith(data.responseText)
            // change dropdown text
            $("#chapterSelectorButton").html("Chapter " + chapId)
            // take done loading time
            const time = (Date.now() - startTime)
            $("#chapter-header-loadtime-num").html(`${time/1000}`)
            // change hidden field metadata
            $("#book-num").val(bookId)
            $("#chapter-num").val(chapId)
            // show done loading
            spinner.hide()
        }
    })
}

/**
 * checks if the page was accessed via an empty or parametrized URL
 */
function checkUrlRef() {
    // example: /Home/Reference?book=1&chapter=3
    const url = new URLSearchParams(window.location.search)
    const book = url.get("book")
    const chapter = url.get("chapter")
    // book & chapter are required, and must be a number
    if (book && !Number.isNaN(book) && chapter && !Number.isNaN(chapter)) {
        const book = Number.parseInt(url.get("book"))
        // get book name from hidden field metadata
        const bookName = $(`#book-name-${book}`).val()
        const chapter = Number.parseInt(url.get("chapter"))
        // perform loading as if dropdown were clicked
        loadBook(book, bookName)
        loadChapter(book, chapter)
    }
}

/**
 * converts state of page into a shareable URL
 */
function refShare() {
    // base url
    let url = window.location.protocol + "//" + window.location.host + "/Home/Reference"
    // current book and chapter nums
    const bookNum = $("#book-num").val()
    const chapterNum = $("#chapter-num").val()
    // if both are valid numbers then append to page
    if (bookNum !== -1 && chapterNum !== -1) {
        url += "?book=" + bookNum + "&chapter=" + chapterNum
    }
    // write url to the clipboard
    navigator.clipboard.writeText(url).then(r => {
        // show success alert for 2000ms
        const ssa = $("#share-success-alert")
        ssa.show()
        ssa.addClass("show")

        setTimeout(() => {
            ssa.removeClass("show")
            ssa.hide()
        }, 2000)
    })
}