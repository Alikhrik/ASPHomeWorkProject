// Событие "DOMContentLoaded" подается когда готов DOM
document.addEventListener("DOMContentLoaded", () => {
    //const app = document.getElementById("app");
    const app = document.querySelector("app");
    if (!app) throw "Forum script: APP not found";
    //app.innerHTML = "APP will be here";
    loadTopics(app);
    //const table = document.querySelector("table");
    //if (!table) throw "Forum script: Table not found";
    //loadTopics(table);
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
                appHtml +=
                    trTemplate
                        .replace("{{title}}", topic.title)
                        .replace("{{description}}", topic.description)
                        .replace("{{id}}", topic.id)
                        .replace("{{showTopic}}", showTopic.name + "(event)");
            }
            elem.innerHTML = appHtml;
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
//function showTopics(elem, j) {
//    //elem.innerHTML = "topics will be here";
//    for (let topic of j) {
//        elem.innerHTML += 
//            `<tr data-id'${topic.id}>
//                <td><b>${topic.title}</b></td>
//                <td>${topic.description}</td>
//            </tr>`;
//    }
//}