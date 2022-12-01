// reference implementation of a sketcher

function setup() {
}

const maxLength = 20;
var history = []

function draw(t, mouseWasPressed) {
    rad = t / 2;
    history = [rad, ...history].slice(0, maxLength)

    for (var i = 0; i < history.length; ++i) {
        ampl = (history.length - i) / history.length;
        x = 800 + 300 * Math.cos(history[i]) * ampl;
        y = 500 + 200 * Math.sin(history[i]) * ampl;

        square(x, y, ampl);
    }
}

export default {
    setup,
    draw
}
