// JS translation for C# testaments enum
const testamentsEnum = ["Both", "Old", "New", "Neither"]

let lastViewport = ""

let currentSearchTerm = ""
let currentSearchInOt = true
let currentSearchInNt = true
let currentMatchBy = ""

/**
 * perform AJAX search according to page state
 */
function doSearch(pageNum) {
    const totalResults = $("#total-results").val()
    
    if (pageNum >= 0 && (!totalResults || pageNum <= Math.ceil(totalResults/25)-1)) {
        // check state of applicable inputs
        const searchTerm = $("#search-term").val()
        const searchInOldTestament = $("#search-in-ot").is(":checked")
        const searchInNewTestament = $("#search-in-nt").is(":checked")
        const matchBy = $("input[name='match-radio']:checked").val()

        if (searchTerm && currentSearchTerm !== searchTerm) currentSearchTerm = searchTerm
        if (searchInOldTestament !== currentSearchInOt) currentSearchInOt = searchInOldTestament
        if (searchInNewTestament !== currentSearchInNt) currentSearchInNt = searchInNewTestament
        if (matchBy && matchBy !== currentMatchBy) currentMatchBy = matchBy

        // format testament so that the server can understand
        let testament = ''
        if (currentSearchInOt && currentSearchInNt) {
            testament = "Both"
        } else if (currentSearchInOt) testament = "Old"
        else if (currentSearchInNt) testament = "New"
        else testament = "Neither"

        // take start time for load badge
        const startTime = Date.now()
        // show loading
        spinner.show()
        $.ajax({
            url: '/Home/PostSearch',
            method: 'POST',
            dataType: 'application/json',
            contentType: 'application/json',
            data: JSON.stringify({
                // translate to C# enum
                testament: testamentsEnum.indexOf(testament),
                searchTerm: currentSearchTerm,
                matchBy: currentMatchBy,
                pageNum: pageNum
            }),
            complete: function (data) {
                $("#search-results").replaceWith(data.responseText)
                // highlight text that was found in search
                addHighlights(currentSearchTerm)
                // show load time
                const time = (Date.now() - startTime)
                $("#chapter-header-loadtime-num").html(`${time/1000}`)
                // show done loading
                spinner.hide()
            }
        })
    } else {
        const ssa = $("#page-fail-alert")
        ssa.show()
        ssa.addClass("show")

        setTimeout(() => {
            ssa.removeClass("show")
            ssa.hide()
        }, 2000)
    }
}

/**
 * highlight the text that was found in a search
 * @param searchTerm the search term
 */
function addHighlights(searchTerm) {
    // take match by to decide highlight algorithm
    const matchBy = $("input[name='match-radio']:checked").val()
    $(".highlightable-vt").each(function() {
        let txt = $(this).html()
        // if matching phrase, then highlight only exact occurrences of the whole phrase
        if (matchBy === "phrase") {
            let regex = new RegExp(`\\b${searchTerm.replace(/[.*+?^${}()|[\]\\]/g, '\\$&')}\\b`, "gi");
            txt = txt.replace(regex, match => `<span class="highlight">${match}</span>`)
        } else {
            // otherwise, we will match word by word
            for (let s of searchTerm.split(' ')) {
                let regex
                // if any, then we can ignore the start and end of words
                if (matchBy === "any") {
                    regex = new RegExp(s, "ig")
                } else {
                    // otherwise, we need to only match full words
                    regex = new RegExp(`\\b${s.replace(/[.*+?^${}()|[\]\\]/g, '\\$&')}\\b`, "gi");
                }
                // use regex matching to preserve original casing on all occurrences 
                txt = txt.replace(regex, match => `<span class="highlight">${match}</span>`)
            }
        }
        $(this).html(txt)
    })
}

/**
 * checks if the page was accessed via an empty or parametrized URL
 */
function checkUrlSearch() {
    const url = new URLSearchParams(window.location.search)
    const searchTerm = url.get("searchTerm")
    // testaments will default to true if not included in the url
    const oldTestament = url.get("oldTestament") ? url.get("oldTestament") === "true" : true
    const newTestament = url.get("newTestament") ? url.get("newTestament") === "true" : true
    // if the matchBy param is not one of the 3 valid options, set it to undefined
    const matchBy = "any word phrase".includes(url.get("matchBy")) ? url.get("matchBy") : undefined
    const pageNum = url.get("page") ? parseInt(url.get("page")) : 0
    // verse names, texts, term, and matchBy are required 
    if (searchTerm && matchBy && !isNaN(pageNum)) {
        // update inputs to URL values
        currentSearchTerm = searchTerm
        currentSearchInOt = oldTestament
        currentSearchInNt = newTestament
        currentMatchBy = matchBy
        mapSearchInputs()
        // perform search normally
        doSearch(pageNum)
    }
}
function mapSearchInputs() {
    $("#search-term").val(currentSearchTerm)
    $("#search-in-ot").prop("checked", currentSearchInOt)
    $("#search-in-nt").prop("checked", currentSearchInNt)
    $(`#match-radio-${currentMatchBy}`).prop("checked", true)
}
/**
 * converts state of page into a shareable URL
 */
function getShareLink() {
    // base url
    let url = window.location.protocol + "//" + window.location.host + "/Home/Search"

    // current values of inputs
    const searchTerm = $("#search-term").val()
    const searchInOldTestament = $("#search-in-ot").is(":checked")
    const searchInNewTestament = $("#search-in-nt").is(":checked")
    const matchBy = $("input[name='match-radio']:checked").val()
    const page = parseInt($("#page-num").val())

    // searchTerm is the only one that doesn't have a default vbalue
    if (searchTerm !== "") {
        // use regex to ensure url safety
        url += `?searchTerm=${searchTerm.replace(/[^a-z0-9 _-]/gi, '-').toLowerCase()}&page=${page}&newTestament=${searchInNewTestament}&oldTestament=${searchInOldTestament}&matchBy=${matchBy}`
    }
    return url
}

/**
 * writes the url to the clipboard
 */
function searchShare() {

    // write to clipboard
    navigator.clipboard.writeText(getShareLink()).then(r => {
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
function getBSViewport() {
    const width = Math.max(
        document.documentElement.clientWidth,
        window.innerWidth || 0
    )
    if (width <= 576) return 'xs'
    if (width <= 768) return 'sm'
    if (width <= 992) return 'md'
    if (width <= 1200) return 'lg'
    if (width <= 1400) return 'xl'
    return 'xxl'
}
function getSearchInputs() {
    if ('lg xl xxl'.includes(getBSViewport())) {
        $.ajax("/Home/GetDesktopSearchInput",{
            method: "GET",
            complete: function (data) {
                $(".search-input-placeholder").replaceWith(data.responseText)
                mapSearchInputs()
            }
        })
        // get desktop input
    } else {
        $.ajax("/Home/GetMobileSearchInput",{
            method: "GET",
            complete: function (data) {
                $(".search-input-placeholder").replaceWith(data.responseText)
                mapSearchInputs()
            }
        })
        // get mobile input
    }
}
function updateViewport() {
    const vp = getBSViewport()
    if (vp !== lastViewport) {
        lastViewport = vp
        getSearchInputs()
    }
}
function toPage(pageNumber, opt) {
    let newPageNumber = pageNumber
    if (opt === "inc") {
        newPageNumber = pageNumber + 1
    } else {
        newPageNumber = pageNumber - 1
    }
    doSearch(newPageNumber)
}
function pageInputChange() {
    const page = parseInt($("#page-input").val())
    toPage(page, 'dec')
}
function searchInputChange() {
    currentSearchTerm = $("#search-input").val()
}