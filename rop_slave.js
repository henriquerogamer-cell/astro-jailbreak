// Minimal worker protocol copied from vinasxexplosao/relapsenadarepublicano.
let my_worker = this;

self.onmessage = function (event) {
    self.postMessage(1);
}