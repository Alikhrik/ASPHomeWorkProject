// Событие "DOMContentLoaded" подается когда готов DOM
document.addEventListener("DOMContentLoaded", () => {
    //const app = document.getElementById("app");
    //const app = document.querySelector("app");
    //if (!app) throw "Forum script: APP not found";
    //app.innerHTML = "APP will be here";
    //loadTopics(app);
    const table = document.querySelector("table");
    if (!table) throw "Forum script: Table not found";
    loadTopics(table);
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

//function showTopics(elem, j) {
//    //elem.innerHTML = "topics will be here";
//    for (let topic of j) {
//        elem.innerHTML += `<div class='topic' data-id'${topic.id}'>
//        <b>${topic.title}</b><i> ${topic.description}</i></div>`;
//    }
//}
function showTopics(elem, j) {
    //elem.innerHTML = "topics will be here";
    for (let topic of j) {
        elem.innerHTML += 
            `<tr data-id'${topic.id}>
                <td><b>${topic.title}</b></td>
                <td>${topic.description}</td>
            </tr>`;
    }
}