document.addEventListener("DOMContentLoaded", function () {
    const buttonPublish = document.getElementById("button-publish");
    if (buttonPublish) buttonPublish.onclick = buttonPublishClick;
    loadArticles();
});

function buttonPublishClick(e) {
    const articleText = document.getElementById("article-text");
    if (!articleText) throw "article-text element not found"
    const articleImage = document.getElementById("article-image");
    if (!articleImage) throw "article-image element not found"

    const topicId = articleText.dataset.topicId;
    const authorId = articleText.dataset.authorId;
    const txt = articleText.value;
    const replyId = articleText.dataset.replyId;

    const formData = new FormData();
    formData.append('TopicId', topicId);
    formData.append('Text', txt);
    formData.append('ReplyId', replyId);
    if (articleImage.files.length > 0) {
        formData.append('PictureFile', articleImage.files[0]);
    }
    fetch("/api/article", {
        method: 'POST',
        body: formData,
        headers: { 'User-Id': authorId }
    }).then(r => r.json()).then(j => {
        if (j.status == "Ok") {
            articleText.value = "";
            articleText.dataset.replyId = "";
            loadArticles();
        }
        else alert(j.message);
    });
}

function loadArticles() {
    const articles = document.querySelector('articles');
    if (!articles) throw "articles element not found";

    const articleText = document.getElementById("article-text");

    let tplPromise = fetch("/templates/article.html");

    const TopicId = articles.getAttribute("topic-id");
    const authorId = articleText ? articleText.getAttribute("data-author-id") : null;

    fetch(`/api/article/${TopicId}`)
        .then(r => r.json())
        .then(async j => {
            console.log(j)
            var html = "";
            const tpl = await tplPromise.then(r => r.text());
            for (let article of j) {
                html +=
                    tpl.replaceAll("{{author}}", article.author.realName)
                        .replaceAll("{{text}}", article.text)
                        .replaceAll("{{avatar}}", (article.author.avatar == null || article.author.avatar == "" ?
                            "no-avatar.png" : article.author.avatar))
                        .replaceAll("{{moment}}", new Date(article.createdDate).toLocaleString())
                        .replaceAll("{{id}}", article.id)
                        .replaceAll("{{picture}}", article.pictureFile == null || article.pictureFile == "" ? "no-picture.png" : article.pictureFile)
                        .replaceAll("{{reply}}",( article.replyId == null
                        ? ""
                        : `<b>${article.reply.author.realName}</b>:`
                            + article.reply.text.substring(0, 13)
                            + (article.reply.text.length > 13 ? "..." : "")
                        ))
                        .replace("{{reply-text}}", article.reply == null ? "" : article.reply.text)
                        .replace("{{del-display}}", /* кнопка удаления (стиль display) */
                            (article.authorId == authorId
                                ? "inline-block"
                                : "none"))
                        .replace("{{ins-display}}", /* кнопка редактирования (стиль display) */
                            ((article.authorId == authorId && article.reply == null)
                                ? "inline-block"
                                : "none"));
            }
            articles.innerHTML = html;
            onArticlesLoaded();
        });
}

function onArticlesLoaded() {
    for (let span of document.querySelectorAll(".article span")) {
        span.onclick = replyClick;
    }
    for (let del of document.querySelectorAll(".article del")) {
        del.onclick = deleteClick;
    }
}

function replyClick(e) {
    const id = e.target.closest(".article").getAttribute("data-id");
    //console.log(id);
    const articleText = document.getElementById("article-text");
    if (articleText) articleText.focus();
    articleText.setAttribute("data-reply-id", id);
}

function deleteClick(e) {
    const id = e.target.closest(".article").getAttribute("data-id");
    console.log(id);
}