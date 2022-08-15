document.addEventListener("DOMContentLoaded", function () {
    const buttonPublish = document.getElementById("button-publish");
    if (!buttonPublish) throw "button-publish element not found"
    buttonPublish.onclick = buttonPublishClick;
});

function buttonPublishClick(e) {
    const articleText = document.getElementById("article-text");
    if (!articleText) throw "article-text element not found"
    const articleImage = document.getElementById("article-image");
    if (!articleText) throw "article-image element not found"
    console.log({
        txt: articleText.value,
        authorId: articleText.dataset.authorId,
        topicId: articleText.dataset.topicId,
        datatime: new Date(),
        Image: articleImage.files[0]
    });
}