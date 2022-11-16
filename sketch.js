const width = 1200
const height = 800

var sources = []

var lineWidth = 40
var leftLineX = (width - lineWidth) / 2
var rightLineX = (width + lineWidth) / 2
var _t = 0

function getSource() {
  return {
    x: random(-width, width * 2),
    y: random(-height, height * 2),
    amp: random(0, 100),
    freq: random(0, 0.8),
    phase: random(0, Math.PI)
  }
}

function setup() {
  createCanvas(width, height);
  sources = [
    {
      x: width / 2,
      y: height / 2,
      amp: 33,
      freq: 0.1,
      phase: 0
    }
  ]
}

var mouseWasPressed = false

// function draw_curevshapeexample() {
//   fill(hslForIndex(0, 100))
//   noStroke()

//   beginShape();
//   curveVertex(84, 91);
//   curveVertex(84, 91);
//   curveVertex(68, 19);
//   curveVertex(21, 17);
//   curveVertex(32, 91);
//   curveVertex(32, 91);
//   endShape();
// }

function draw() {
  if (mouseIsPressed) {
    if (!mouseWasPressed) {
      sources = sources.concat(getSource())
    }
    mouseWasPressed = true
  } else mouseWasPressed = false;

  background(255);
  // noStroke();

  var lineXs = [250, 500, 750]
  // must be one longer than lineXs
  var colors = [hslForIndex(0, 100), hslForIndex(100, 100), hslForIndex(200, 100), hslForIndex(300, 100)];

  fill(colors[0]);

  // up
  line(0, height, 0, 0);
  line(0, 0, lineXs[0], 0)

  var prevLastNode = { x: 0, y: height };

  for (var i = 0; i < lineXs.length; ++i) {
    var nodes = getLineValues(lineXs[i]);

    // first, finish the left one
    for (var j = 0; j < nodes.length; ++j) {
      var points = [
        nodes[Math.max(0, j - 1)],
        nodes[j],
        nodes[Math.min(nodes.length - 1, j + 1)],
        nodes[Math.min(nodes.length - 1, j + 2)]
      ];

      curve(
        points[0].x, points[0].y,
        points[1].x, points[1].y,
        points[2].x, points[2].y,
        points[3].x, points[3].y,
      );
    }

    var lastNode = nodes[nodes.length - 1]
    line(lastNode.x, lastNode.y, prevLastNode.x, prevLastNode.y)
    prevLastNode = lastNode

    // now, start the right one
    fill(colors[i + 1])
    for (var j = nodes.length - 1; j >= 0; --j) {
      var points = [
        nodes[Math.min(nodes.length, j)],
        nodes[j],
        nodes[Math.max(0, j - 1)],
        nodes[Math.max(0, j - 2)]
      ];

      curve(
        points[0].x, points[0].y,
        points[1].x, points[1].y,
        points[2].x, points[2].y,
        points[3].x, points[3].y,
      );
    }
  }

  line(width, 0, width, height)

  ++_t
}

function getLineValues(x) {
  var result = [
    { x: x, y: 0 }
  ]

  for (var i = 0; i < height; i++) {
    field = getFieldStrength(x, i, _t)
    result = result.concat(
      {
        x: x + field.x,
        y: i + field.y,
      })
  }

  return result
}

function getFieldStrength(x, y, t) {
  var xDiff = 0
  var yDiff = 0
  t /= 10

  for (var i = 0; i < sources.length; ++i) {
    var source = sources[i]
    var d = Math.sqrt(
      Math.pow(source.y - y, 2) +
      Math.pow(source.x - x, 2))
    // use width as the max range of a node
    if (d > width) {
      continue
    }

    var amplitude = source.amp * Math.sin((d + t) * source.freq + source.phase)
    amplitude *= Math.abs(width - d) / width

    xDiff += amplitude * Math.cos((source.x - x) / d)
    yDiff += amplitude * Math.sin((source.y - y) / d)
  }

  return {
    x: xDiff,
    y: yDiff
  }
}

var baseline = 0

function hslForIndex(i, a) {
  step = 5

  angle = baseline + step * i
  while (angle > 360) angle -= 360
  angle = Math.floor(angle)
  return 'hsla(' + angle + ', 100%, 50%, ' + a + ') '
}
