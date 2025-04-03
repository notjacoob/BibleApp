const testamentsEnum = ["Both", "Old", "New", "Neither"]

function doSearch() {
    
    const searchTerm = $("#search-term").val()
    const searchInVerseName = $("#search-in-vn").is(":checked")
    const searchInVerseText = $("#search-in-vt").is(":checked")
    const searchInOldTestament = $("#search-in-ot").is(":checked")
    const searchInNewTestament = $("#search-in-nt").is(":checked")
    const matchBy = $("input[name='match-radio']:checked").val()
    
    let testament = ''
    if (searchInOldTestament && searchInNewTestament) {
        testament = "Both"
    } else if (searchInOldTestament) testament = "Old"
    else if (searchInNewTestament) testament = "New"
    else testament = "Neither"

    const startTime = Date.now()
    spinner.show()
    $.ajax({
        url: '/Home/PostSearch',
        method: 'POST',
        dataType: 'application/json',
        contentType: 'application/json',
        data: JSON.stringify({
            searchInVerseName: searchInVerseName,
            searchInVerseText: searchInVerseText,
            testament: testamentsEnum.indexOf(testament),
            searchTerm: searchTerm,
            matchBy: matchBy
        }),
        complete: function (data) {
            $("#search-results").replaceWith(data.responseText)
            addHighlights(searchTerm, searchInVerseName, searchInVerseText)
            const time = (Date.now() - startTime)
            $("#chapter-header-loadtime-num").html(`${time/1000}`)
            spinner.hide()
        }
    })
    
}
function addHighlights(searchTerm, searchInVerseName, searchInVerseText) {
    const matchBy = $("input[name='match-radio']:checked").val()
    if (searchInVerseName) {
        $(".highlightable-vn").each(function() {
            let txt = $(this).html()
            if (matchBy === "phrase") {
                let regex = new RegExp(`\\b${searchTerm.replace(/[.*+?^${}()|[\]\\]/g, '\\$&')}\\b`, "gi");
                txt = txt.replace(regex, match => `<span class="highlight">${match}</span>`)
            } else {
                for (let s of searchTerm.split(' ')) {
                    let regex
                    if (matchBy === "any") {
                        regex = new RegExp(s, "ig")
                    } else {
                        regex = new RegExp(`\\b${s.replace(/[.*+?^${}()|[\]\\]/g, '\\$&')}\\b`, "gi");
                    }
                    txt = txt.replace(regex, match => `<span class="highlight">${match}</span>`)
                }
            }
            $(this).html(txt)
        })
    }
    if (searchInVerseText) {
        $(".highlightable-vt").each(function() {
            let txt = $(this).html()
            if (matchBy === "phrase") {
                let regex = new RegExp(`\\b${searchTerm.replace(/[.*+?^${}()|[\]\\]/g, '\\$&')}\\b`, "gi");
                txt = txt.replace(regex, match => `<span class="highlight">${match}</span>`)
            } else {
                for (let s of searchTerm.split(' ')) {
                    let regex
                    if (matchBy === "any") {
                        regex = new RegExp(s, "ig")
                    } else {
                        regex = new RegExp(`\\b${s.replace(/[.*+?^${}()|[\]\\]/g, '\\$&')}\\b`, "gi");
                    }
                    txt = txt.replace(regex, match => `<span class="highlight">${match}</span>`)
                }
            }
            $(this).html(txt)
        })
    }
}
function checkUrlSearch() {
    const url = new URLSearchParams(window.location.search)
    const searchInVerseName = url.get("searchInVerseName")
    const searchInVerseText = url.get("searchInVerseText")
    const searchTerm = url.get("searchTerm")
    const oldTestament = url.get("oldTestament") ? url.get("oldTestament") === "true" : true
    const newTestament = url.get("newTestament") ? url.get("newTestament") === "true" : true
    const matchBy = "any word phrase".includes(url.get("matchBy")) ? url.get("matchBy") : undefined
    if (searchInVerseName && searchInVerseText && searchTerm && matchBy) {
        $("#search-term").val(searchTerm)
        $("#search-in-vn").prop("checked", searchInVerseName === "true")
        $("#search-in-vt").prop("checked", searchInVerseText === "true")
        $("#search-in-ot").prop("checked", oldTestament)
        $("#search-in-nt").prop("checked", newTestament)
        $(`#match-radio-${matchBy}`).prop("checked", true)
        doSearch()
    }
}
function searchShare() {
    let url = window.location.protocol + "//" + window.location.host + "/Home/Search"
    
    const searchTerm = $("#search-term").val()
    const searchInVerseName = $("#search-in-vn").is(":checked")
    const searchInVerseText = $("#search-in-vt").is(":checked")
    const searchInOldTestament = $("#search-in-ot").is(":checked")
    const searchInNewTestament = $("#search-in-nt").is(":checked")
    const matchBy = $("input[name='match-radio']:checked").val()
    
    if (searchTerm !== "") {
        url += `?searchTerm=${searchTerm.replace(/[^a-z0-9 _-]/gi, '-').toLowerCase()}&searchInVerseName=${searchInVerseName}&searchInVerseText=${searchInVerseText}&newTestament=${searchInNewTestament}&oldTestament=${searchInOldTestament}&matchBy=${matchBy}`
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