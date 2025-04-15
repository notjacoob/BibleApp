// comment input boxes
const addingCommentHtml = "<textarea class=\"form-control\" id=\"comment-txt\" placeholder=\"Enter your comment here!\"></textarea>\n" +
    "                    <div class=\"d-flex flex-row gap-1\">\n" +
    "                        <button class=\"btn btn-primary flex-grow\" onclick='submitComment(%CID%)'>Submit</button>\n" +
    "                        <button class=\"btn btn-secondary flex-grow\" onclick='toggleCommentInput(%CID%)'>Cancel</button>\n" +
    "                    </div>"
// comment input toggle button
const notAddingComment = "<button class=\"btn btn-primary\" onclick='toggleCommentInput(%CID%)'>Add Comment</button>"
// no comments list item
const noComments = "                        <li id=\"no-comments\" class=\"list-group-item text-center\">No comments!</li>\n"
// track if the comment input is open
let addingComment = false
// track the current opened verse (clicked on)
let currentOpenedVerse = 0

/**
 * ajax request to load the comments modal for a verse
 * @param id verse id
 */
function loadComments(id) {
    $.ajax("/Home/GetComments?verseId=" + id, {
        type: "GET",
        complete: function(data) {
            // show modal
            $("#comments-modal").replaceWith(data.responseText)
            $("#comments-modal").modal("show")
            // bind the close of the modal
            bindModalClose()
            currentOpenedVerse = id
            // highlight the verse list item
            $("#verse-li-"+id).addClass("active")
        }
    })
}

/**
 * hide the comments modal
 */
function hideCommentsModal() {
    $("#comments-modal").modal("hide")
}

/**
 * toggle the comment input box
 * @param verseId verse id
 */
function toggleCommentInput(verseId) {
    // if the comment box is open, close it
    if (addingComment) {
        // replace verse id where needed
        $("#comment-input").html(notAddingComment.replaceAll("%CID%", verseId))
    // if the comment box is closed, open it
    } else {
        // replace verse id where needed
        $("#comment-input").html(addingCommentHtml.replaceAll("%CID%", verseId))
    }
    // toggle JS tracker
    addingComment = !addingComment
}

/**
 * add a new comment
 * @param verseId verse to add the comment to
 */
function submitComment(verseId) {
    // only possible to add if the box is open
    if (addingComment) {
        // take the instance of the textbox
        const commentTxtbox = $("#comment-txt")
        // if the textbox exists on the dom
        if (commentTxtbox) {
            // get the content of the textbox
            const txt = commentTxtbox.val()
            $.ajax("/Home/AddComment", {
                type: "POST",
                dataType: "application/json",
                contentType: "application/json",
                data:JSON.stringify({
                    content: txt,
                    verseId: verseId,
                }),
                complete: function(data) {
                    // empty the textbox
                    commentTxtbox.val("");
                    // add the comment
                    appendComment(verseId, data.responseText)
                }
            })
        }
    }
}

/**
 * add a comment to the opened list box
 * @param id the id of the verse
 * @param txt the content of the comment
 */
function appendComment(id, txt) {
    // get the listgroup that contains the comments
    const clg = $("#comments-listgroup")
    // get instances of "no comments" list items if applicable
    const nc = $("#no-comments")
    // get all populated comments
    const cli = $(".comment-li")
    // if there is a no comments box and no populated comments
    if (nc && cli.length === 0) {
        // clear the list box
        clg.html("")
    }
    // append the comment to the list box
    clg.append(txt)
    // update the comment counter badge outside the modal
    const commentCounter = $(`#comment-count-num-${id}`)
    if (!Number.isNaN(commentCounter.html())) {
        let num = Number.parseInt(commentCounter.html())
        commentCounter.html(++num)
    }
}

/**
 * delete a comment from the list box
 * @param vid verse id 
 * @param cid comment id to delete
 */
function deleteComment(vid, cid) {
    $.ajax("/Home/DeleteComment", {
        type: "DELETE",
        dataType: "application/json",
        contentType: "application/json",
        data: JSON.stringify({
            commentId: cid,
        }),
        complete: function(data) {
            // remove the comment from the DOM
            removeComment(vid, cid)
        }
    })
}

/**
 * remove the comment from the listbox
 * @param vid verse id
 * @param cid comment id to delete
 */
function removeComment(vid, cid) {
    // remove the comment from the listbox
    $(`#comment-${cid}`).remove()
    // check if there are no more comments
    const clg = $("#comments-listgroup")
    if (clg.children().length === 0) {
        // append the no comments marker
        clg.append(noComments)
    }
    // change the comment count badge
    const commentCounter = $(`#comment-count-num-${vid}`)
    if (!Number.isNaN(commentCounter.html())) {
        let num = Number.parseInt(commentCounter.html())
        commentCounter.html(--num)
    }
}

/**
 * if the modal is closed, remove the active class from the clicked verse
 */
function bindModalClose() {
    $("#comments-modal").on("hidden.bs.modal", function() {
        removeActiveOnVerse()
    })
}

/**
 * remove the active class from the current opened verse and reset the indicator
 */
function removeActiveOnVerse() {
    $("#verse-li-"+currentOpenedVerse).removeClass("active")
    currentOpenedVerse=0
}

/**
 * copy a verse list item to readable text
 * @param id the id of the verse
 * @param event the html event
 */
function copyVerse(id, event) {
    // don't open the comments box when the copy button is clicked
    event.stopPropagation()
    // ms variant
    window.event.cancelBubble = true
    // get the verse title and text from their invisible metadata fields
    const title = $(`#verse-title-${id}`).html()
    const text = $(`#verse-text-${id}`).html().replaceAll("<span class=\"highlight\">", "").replaceAll("</span>", "")
    // write the text to the clipboard
    navigator.clipboard.writeText(
        `${title} - \"${text}\"`).then(r => {
        // show the success alert for 2000ms
        const ssa = $("#share-success-alert")
        ssa.show()
        ssa.addClass("show")

        setTimeout(() => {
            ssa.removeClass("show")
            ssa.hide()
        }, 2000)
    })
}