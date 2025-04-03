const defaultCommentsModal = "<div class=\"modal\" tabindex=\"-1\" role=\"dialog\" id=\"comments-modal\">" +
    "    <div class=\"spinner-border\" role=\"status\">" +
    "        <span class=\"sr-only\">Loading...</span>" +
    "    </div>" +
    "</div>"
const addingCommentHtml = "<textarea class=\"form-control\" id=\"comment-txt\" placeholder=\"Enter your comment here!\"></textarea>\n" +
    "                    <div class=\"d-flex flex-row gap-1\">\n" +
    "                        <button class=\"btn btn-primary flex-grow\" onclick='submitComment(%CID%)'>Submit</button>\n" +
    "                        <button class=\"btn btn-secondary flex-grow\" onclick='toggleCommentInput(%CID%)'>Cancel</button>\n" +
    "                    </div>"
const notAddingComment = "<button class=\"btn btn-primary\" onclick='toggleCommentInput(%CID%)'>Add Comment</button>"
const noComments = "                        <li id=\"no-comments\" class=\"list-group-item text-center\">No comments!</li>\n"
let addingComment = false
let currentOpenedVerse = 0
function loadComments(id) {
    $.ajax("/Home/GetComments?verseId=" + id, {
        type: "GET",
        complete: function(data) {
            $("#comments-modal").replaceWith(data.responseText)
            $("#comments-modal").modal("show")
            bindModalClose()
            currentOpenedVerse = id
            $("#verse-li-"+id).addClass("active")
        }
    })
}
function hideCommentsModal() {
    $("#comments-modal").modal("hide")
}
function toggleCommentInput(verseId) {
    if (addingComment) {
        $("#comment-input").html(notAddingComment.replaceAll("%CID%", verseId))
    } else {
        $("#comment-input").html(addingCommentHtml.replaceAll("%CID%", verseId))
    }
    addingComment = !addingComment
}
function submitComment(verseId) {
    if (addingComment) {
        const commentTxtbox = $("#comment-txt")
        if (commentTxtbox) {
            const txt = commentTxtbox.val()
            const date = Date.now()
            $.ajax("/Home/AddComment", {
                type: "POST",
                dataType: "application/json",
                contentType: "application/json",
                data:JSON.stringify({
                    content: txt,
                    verseId: verseId,
                }),
                complete: function(data) {
                    commentTxtbox.val("");
                    appendComment(verseId, data.responseText)
                }
            })
        }
    }
}
function appendComment(id, txt) {
    const clg = $("#comments-listgroup")
    const nc = $("#no-comments")
    const cli = $(".comment-li")
    if (nc && cli.length === 0) {
        clg.html("")
    }
    clg.append(txt)
    const commentCounter = $(`#comment-count-num-${id}`)
    if (!Number.isNaN(commentCounter.html())) {
        let num = Number.parseInt(commentCounter.html())
        commentCounter.html(++num)
    }
}
function deleteComment(vid, cid) {
    $.ajax("/Home/DeleteComment", {
        type: "DELETE",
        dataType: "application/json",
        contentType: "application/json",
        data: JSON.stringify({
            commentId: cid,
        }),
        complete: function(data) {
            removeComment(vid, cid)
        }
    })
}
function removeComment(vid, cid) {
    $(`#comment-${cid}`).remove()
    const clg = $("#comments-listgroup")
    if (clg.children().length === 0) {
        clg.append(noComments)
    }
    const commentCounter = $(`#comment-count-num-${vid}`)
    if (!Number.isNaN(commentCounter.html())) {
        let num = Number.parseInt(commentCounter.html())
        commentCounter.html(--num)
    }
}
function preventDefault(e) {
    e.preventDefault()
}
function bindModalClose() {
    $("#comments-modal").on("hidden.bs.modal", function() {
        removeActiveOnVerse()
    })
}
function removeActiveOnVerse() {
    $("#verse-li-"+currentOpenedVerse).removeClass("active")
    currentOpenedVerse=0
}
function copyVerse(id, event) {
    event.stopPropagation()
    window.event.cancelBubble = true
    const title = $(`#verse-title-${id}`).html()
    const text = $(`#verse-text-${id}`).html()
    navigator.clipboard.writeText(`${title} - \"${text}\"`).then(r => {
        const ssa = $("#share-success-alert")
        ssa.show()
        ssa.addClass("show")

        setTimeout(() => {
            ssa.removeClass("show")
            ssa.hide()
        }, 2000)
    })
}