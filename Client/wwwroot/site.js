let refreshing = false;
const cdnBase = "https://fcdn.satroki.tech";

function showUpdate() {
    var div = document.getElementById("div-update");
    div.removeAttribute("hidden");
    update();
};

function hideUpdate() {
    var div = document.getElementById("div-update");
    div.setAttribute("hidden", "");
};

function update() {
    navigator.serviceWorker.getRegistration().then(reg => {
        if (reg.waiting)
            reg.waiting.postMessage('skipWaiting');
        else
            hideUpdate();
    })
}

let originalFetch = window.fetch;
window.fetch = function (url, options) {
    if (url === '_framework/blazor.boot.json' && location.hostname !== 'localhost') {
        return originalFetch(`${cdnBase}/_framework/blazor.boot.json`, options);
    } else {
        return originalFetch.apply(this, arguments);
    }
};

const start = async () => {
    try {
        await Blazor.start({
            loadBootResource: function (type, name, defaultUri, integrity) {
                if (location.hostname === 'localhost')
                    return defaultUri;
                return `${cdnBase}/_framework/${name}`;
            }
        });
    } catch (err) {
        console.error(err);
    }

    let reg = await navigator.serviceWorker.register('service-worker.js');
    if (reg.waiting) {
        showUpdate();
        return;
    }
    reg.onupdatefound = () => {
        console.info("Service worker: Update Found");
        let newsw = reg.installing;
        newsw.onstatechange = () => {
            if (newsw.state === 'installed') {
                showUpdate();
            }
        }
    }
};

navigator.serviceWorker.addEventListener('controllerchange', () => {
    if (refreshing)
        return;
    refreshing = true;
    window.location.reload();
});

window.navBack = () => {
    history.back();
}

window.saveFileLink = (filename, link) => {
    var a = document.createElement('a');
    a.target = "_blank";
    a.download = filename;
    a.href = link;
    document.body.appendChild(a);
    a.click();
    document.body.removeChild(a);
}

window.saveAsFile = (filename, base64) => {
    var a = document.createElement('a');
    a.target = "_blank";
    a.download = filename;
    a.href = "data:application/octet-stream;base64," + base64;
    document.body.appendChild(a);
    a.click();
    document.body.removeChild(a);
}

window.saveString = (filename, content, type) => {
    let blob = new Blob([content], { type: type });
    let a = document.createElement('a');
    let url = URL.createObjectURL(blob);
    a.target = "_blank";
    a.download = filename;
    a.href = url;
    document.body.appendChild(a);
    a.click();
    document.body.removeChild(a);
}

window.getSolveResult = (model) => {
    if (!window.solver)
        return { error: -1 };
    try {
        let results = window.solver.Solve(model);
        if (results) {
            return results;
        }
        return { error: -1 };
    } catch (err) {
        return { error: -2 };
    }
}

document.onkeyup = (ev) => {
    if (ev.keyCode === 27)
        DotNet.invokeMethodAsync("PcrBlazor.Client", "CloseDialog");
}

document.onclick = (ev) => {
    let node = ev.target || ev.srcElement;
    if (node.classList.contains('rz-dialog-mask'))
        DotNet.invokeMethodAsync("PcrBlazor.Client", "CloseDialog");
}

start();