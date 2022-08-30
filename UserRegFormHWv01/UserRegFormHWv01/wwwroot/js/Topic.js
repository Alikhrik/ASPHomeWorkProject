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
    const formData = new FormData();
    formData.append('TopicId', topicId);
    formData.append('Text', txt);
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
            loadArticles();
        }
        else alert(j.message);
    });
}

function loadArticles() {
    const articles = document.querySelector('articles');
    if (!articles) throw "articles element not found";
    let tplPromise = fetch("/templates/article.html");

    const TopicId = articles.getAttribute("topic-id");
    fetch(`/api/article/${TopicId}`)
        .then(r => r.json())
        .then(async j => {
            console.log(j)
            var html = "";
            const tpl = await tplPromise.then(r => r.text());
            for (let articles of j) {
                html +=
                    tpl.replaceAll("{{author}}", articles.author.realName)
                        .replaceAll("{{text}}", articles.text)
                        .replaceAll("{{avatar}}", (articles.author.avatar == null || articles.author.avatar == "" ?
                            "no-avatar.png" : articles.author.avatar))
                        .replaceAll("{{moment}}", new Date(articles.createdDate).toLocaleString())
                        .replaceAll("{{id}}", articles.id)
                    .replaceAll("{{picture}}", articles.pictureFile == null || articles.pictureFile == "" ? "no-picture.png" : articles.pictureFile)
                        .replaceAll("{{reply}}", articles.replyId == null ? "" : articles.replyId);
            }
            articles.innerHTML = html;
            onArticlesLoaded();
        });
}

function onArticlesLoaded() {
    for (let span of document.querySelectorAll(".article span")) {
        span.onclick = replyClick;
    }
}

function replyClick(e) {
    console.log(e.target.closest(".article").getAttribute("data-id"));
    const articleText = document.getElementById("article-text");
    if (articleText) articleText.focus();
}