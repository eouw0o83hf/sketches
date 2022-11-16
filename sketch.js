// consts
const width = 1200
const height = 800
const lineWidth = 40
const leftLineX = (width - lineWidth) / 2
const rightLineX = (width + lineWidth) / 2
const maxNodeAge = 1000

function setup() {
  createCanvas(width, height);
  sources = [
    {
      x: width / 2,
      y: height / 2,
      amp: 33,
      freq: 0.1,
      phase: 0,
      timestamp: 0
    }
  ]
}

// state
var sources = []
var _t = 0
var mouseWasPressed = false
var lineXs = [width / 2]
var colors = [
  hslForIndex(0, 100), hslForIndex(100, 100)
]

function draw() {
  if (mouseIsPressed) {
    if (!mouseWasPressed) {
      sources = sources.concat(getSource(_t))
    }
    mouseWasPressed = true
  } else mouseWasPressed = false;

  background(255);

  stroke(hslForIndex(300, 100))
  strokeWeight(5)


  var lineXs = [250, 500, 750]
  // must be one longer than lineXs
  var colors = [hslForIndex(0, 100), hslForIndex(100, 100), hslForIndex(200, 100), hslForIndex(300, 100)];

  fill(colors[0]);

  // up
  beginShape()
  // from bottom left to top left, draw a curve way outside of the frame
  // this is a hack because you can't intermix curves and lines
  curveVertex(-lineMargin, height + lineMargin)
  // first entry is control entry, this starts drawing
  curveVertex(-lineMargin, height + lineMargin)
  curveVertex(-lineMargin, -lineMargin)

  var prevLastNode = { x: 0, y: height };

  for (var i = 0; i < lineXs.length; ++i) {
    var nodes = getLineValues(lineXs[i]);

    // first, finish the left one
    for (var j = 0; j < nodes.length; ++j) {
      curveVertex(nodes[j].x, nodes[j].y)
    }

    var lastNode = nodes[nodes.length - 1]
    curveVertex(prevLastNode.x, prevLastNode.y)

    endShape()

    prevLastNode = lastNode

    // now, start the right one
    beginShape()
    fill(colors[i + 1])
    for (var j = nodes.length - 1; j >= 0; --j) {
      // don't worry about double-entering the first one, we have plenty
      // of margin below the frame
      curveVertex(nodes[j].x, nodes[j].y)
    }
  }

  curveVertex(width + lineMargin, -lineMargin)
  curveVertex(width + lineMargin, height + lineMargin)
  curveVertex(width + lineMargin, height + lineMargin)

  endShape()

  for (var i = 0; i < sources.length; ++i) {
    var source = sources[i]
    var age = _t - source.timestamp
    if (age > maxNodeAge) {
      continue
    }

    fill(255, 0, 0, 255 * (maxNodeAge - age) / maxNodeAge)
    stroke(255, 255, 255, 255 * (maxNodeAge - age) / maxNodeAge)
    strokeWeight(3)
    circle(sources[i].x, sources[i].y, 15)
  }

  ++_t
}

const lineMargin = 50;
function getLineValues(x) {
  var result = []

  for (var i = -lineMargin; i < height + lineMargin; i++) {
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

  for (var i = 0; i < sources.length; ++i) {
    var source = sources[i]
    var age = t - source.timestamp
    if (age > maxNodeAge) {
      continue
    }

    var d = Math.sqrt(
      Math.pow(source.y - y, 2) +
      Math.pow(source.x - x, 2))
    // use width as the max range of a node
    if (d > width) {
      continue
    }

    var amplitude = source.amp * Math.sin((d + t) * source.freq + source.phase)
    // attenuate with distance
    amplitude *= Math.abs(width - d) / width
    // attenuate with time
    amplitude *= (maxNodeAge - age) / maxNodeAge

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

function getSource(t) {
  return {
    x: mouseX,
    y: mouseY,
    amp: random(0, 100),
    freq: random(0, 0.1),
    phase: random(0, Math.PI),
    timestamp: t
  }
}
