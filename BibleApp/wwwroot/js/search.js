// JS translation for C# testaments enum
const testamentsEnum = ["Both", "Old", "New", "Neither"]

/**
 * perform AJAX search according to page state
 */
function doSearch() {
    
    // check state of applicable inputs
    const searchTerm = $("#search-term").val()
    const searchInVerseName = $("#search-in-vn").is(":checked")
    const searchInVerseText = $("#search-in-vt").is(":checked")
    const searchInOldTestament = $("#search-in-ot").is(":checked")
    const searchInNewTestament = $("#search-in-nt").is(":checked")
    const matchBy = $("input[name='match-radio']:checked").val()
    
    // format testament so that the server can understand
    let testament = ''
    if (searchInOldTestament && searchInNewTestament) {
        testament = "Both"
    } else if (searchInOldTestament) testament = "Old"
    else if (searchInNewTestament) testament = "New"
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
            searchInVerseName: searchInVerseName,
            searchInVerseText: searchInVerseText,
            // translate to C# enum
            testament: testamentsEnum.indexOf(testament),
            searchTerm: searchTerm,
            matchBy: matchBy
        }),
        complete: function (data) {
            $("#search-results").replaceWith(data.responseText)
            // highlight text that was found in search
            addHighlights(searchTerm, searchInVerseName, searchInVerseText)
            // show load time
            const time = (Date.now() - startTime)
            $("#chapter-header-loadtime-num").html(`${time/1000}`)
            // show done loading
            spinner.hide()
        }
    })
    
}

/**
 * highlight the text that was found in a search
 * @param searchTerm the search term
 * @param searchInVerseName whether or not to highlight the verse name
 * @param searchInVerseText whether or not to highlight the verse text
 */
function addHighlights(searchTerm, searchInVerseName, searchInVerseText) {
    // take match by to decide highlight algorithm
    const matchBy = $("input[name='match-radio']:checked").val()
    if (searchInVerseName) {
        // loop through every verse name
        $(".highlightable-vn").each(function() {
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
    if (searchInVerseText) {
        // loop through every verse name
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
}

/**
 * checks if the page was accessed via an empty or parametrized URL
 */
function checkUrlSearch() {
    const url = new URLSearchParams(window.location.search)
    const searchInVerseName = url.get("searchInVerseName")
    const searchInVerseText = url.get("searchInVerseText")
    const searchTerm = url.get("searchTerm")
    // testaments will default to true if not included in the url
    const oldTestament = url.get("oldTestament") ? url.get("oldTestament") === "true" : true
    const newTestament = url.get("newTestament") ? url.get("newTestament") === "true" : true
    // if the matchBy param is not one of the 3 valid options, set it to undefined
    const matchBy = "any word phrase".includes(url.get("matchBy")) ? url.get("matchBy") : undefined
    // verse names, texts, term, and matchBy are required 
    if (searchInVerseName && searchInVerseText && searchTerm && matchBy) {
        // update inputs to URL values
        $("#search-term").val(searchTerm)
        $("#search-in-vn").prop("checked", searchInVerseName === "true")
        $("#search-in-vt").prop("checked", searchInVerseText === "true")
        $("#search-in-ot").prop("checked", oldTestament)
        $("#search-in-nt").prop("checked", newTestament)
        $(`#match-radio-${matchBy}`).prop("checked", true)
        // perform search normally
        doSearch()
    }
}
/**
 * converts state of page into a shareable URL
 */
function searchShare() {
    // base url
    let url = window.location.protocol + "//" + window.location.host + "/Home/Search"
    
    // current values of inputs
    const searchTerm = $("#search-term").val()
    const searchInVerseName = $("#search-in-vn").is(":checked")
    const searchInVerseText = $("#search-in-vt").is(":checked")
    const searchInOldTestament = $("#search-in-ot").is(":checked")
    const searchInNewTestament = $("#search-in-nt").is(":checked")
    const matchBy = $("input[name='match-radio']:checked").val()
    
    // searchTerm is the only one that doesn't have a default vbalue
    if (searchTerm !== "") {
                                                           // use regex to ensure url safety
        url += `?searchTerm=${searchTerm.replace(/[^a-z0-9 _-]/gi, '-').toLowerCase()}&searchInVerseName=${searchInVerseName}&searchInVerseText=${searchInVerseText}&newTestament=${searchInNewTestament}&oldTestament=${searchInOldTestament}&matchBy=${matchBy}`
    }
    // write to clipboard
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