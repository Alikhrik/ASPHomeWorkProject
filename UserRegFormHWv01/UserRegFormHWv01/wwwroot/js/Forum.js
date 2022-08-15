// Событие "DOMContentLoaded" подается когда готов DOM
document.addEventListener("DOMContentLoaded", () => {
    //const app = document.getElementById("app");
    const app = document.querySelector("app");
    if (!app) throw "Forum script: APP not found";
    loadTopics(app);
});

function loadTopics(elem) {
    fetch("/api/topic", {
        method: "GET",
        headers: {
            "User-Id": "",
            "Culture": ""
        },
        body: null
    })
        .then(r => r.json())
        .then(j => {
            //console.log(j);
            if (j instanceof Array) {
                showTopics(elem, j);
            }
            else {
                throw "showTopics: Backend data invalid";
            }
        })
}

function showTopics(elem, j) {
    //var trTemplate = `...`;
    fetch("/templates/topic.html")
        .then(r => r.text())
        .then(trTemplate => {
            var appHtml = "";
            for (let topic of j) {
                let tpl = trTemplate;
                for (let prop in topic) {
                    tpl = tpl.replaceAll(`{{${prop}}}`, topic[prop]);
                }
                appHtml += tpl;
                //appHtml +=
                //    trTemplate
                //        .replace("{{title}}", topic.title)
                //        .replace("{{description}}", topic.description)
                //        .replace("{{id}}", topic.id);
            }
            elem.innerHTML = appHtml;
            topicLoaded();
        });
}

function showTopic(e) {
    fetch(`/api/topic/${e.target.dataset.id}`, {
        method: "GET",
        body: null
    })
        .then(r => r.json())
        .then(j => console.log(j));
}
function topicClick(e) {
    location = "/Forum/Topic/" +
        e.target.closest(".topic").getAttribute("data-id");
}

async function topicLoaded() {
    for (let topic of document.querySelectorAll(".topic")) {
        topic.onclick = topicClick;
    }
}