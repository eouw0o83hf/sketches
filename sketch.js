// consts
const width = 1600
const height = 1000

function setup() {
  createCanvas(width, height);
  drawCore(0, false)
}

// state
var _t = 0
var mouseWasPressed = false

function draw() {
  background(0);

  drawCore(_t, {
    mouseWasPressed,
    mouseBecamePressed: mouseIsPressed && !mouseWasPressed
  });

  ++_t;
  mouseWasPressed = mouseIsPressed
}

// boilerplate above, helpers below

// #region sprites

// points which generate force in ways that varies over time.
// (x, y) vector of force based on time
var sprites = [
  (t) => ({ x: 0, y: 0 })
].slice(0, 0);

// most recent paths of the sprites, in lockstep indeces.
var paths = [[{ x: 0, y: 0 }]].slice(0, 0);
const maxLength = 150;

function addSprite() {
  sprites.unshift(function (t) {
    var a = random(4, 10)
    var b = random(10, 70)
    var c = random(400, 1000)

    return {
      x: Math.sin(t / a * 10) * b,
      y: 0,// Math.cos(t / c) * c / 1000000
    }
  })
  // start at a random point
  paths.unshift([{
    x: random(0, width),
    y: random(0, height)
  }])
}

function pruneSprites(num) {
  if (num === undefined) {
    sprites.pop()
    paths.pop()
  } else {
    sprites = sprites.slice(0, num)
    paths = paths.slice(0, num)
  }
}

// #endregion

// #region Lifecycle Management

const spriteNextAddFreq = 750
const spriteNextPruneFreq = 800
const maxSprites = 40

var tNextAdd = 0
var tNextPrune = spriteNextPruneFreq * 2

function doLifecycleManagement(t) {
  if (t >= tNextPrune) {
    pruneSprites()
    tNextPrune += random(spriteNextPruneFreq)
  }

  if (t >= tNextAdd) {
    addSprite()
    tNextAdd += random(spriteNextAddFreq)
  }

  pruneSprites(maxSprites)
}

// #endregion

const maxStroke = 200
function drawCore(t, state) {
  doLifecycleManagement(t)

  if (state.mouseBecamePressed) {
    if (random([false, true, true])) addSprite()
    else pruneSprites()
  }

  for (var i = 0; i < sprites.length; ++i) {
    var force = sprites[i](t)
    var next = {
      x: paths[i][0].x + force.x,
      y: paths[i][0].y + force.y
    }

    // windowing. allow it to run off a little but not wander too far
    var margin = 100;
    next.x = Math.min(Math.max(-margin, next.x), width + margin)
    next.y = Math.min(Math.max(-margin, next.y), height + margin)

    // insert at the beginning and trim
    var temp = paths[i]
    temp.unshift(next)
    paths[i] = temp.slice(0, maxLength)

    strokeWeight(maxStroke * (sprites.length - i) / sprites.length)
    stroke(hslForIndex(i * 100, 100))
    // noStroke()

    noFill()
    // fill(hslForIndex(i * 100, 100))

    // anchor
    curveTightness(-10)
    beginShape()
    curveVertex(paths[i][0].x, paths[i][0].y)
    var pathLength = paths[i].length

    for (var j = 0; j < pathLength; ++j) {
      var power = (pathLength - j) / pathLength

      curveVertex(paths[i][j].x, paths[i][j].y)
    }

    // anchor
    curveVertex(paths[i][pathLength - 1].x, paths[i][pathLength - 1].y)
    endShape()
  }
}

var baseline = 0;
function hslForIndex(i, a) {
  step = 5

  angle = baseline + step * i
  while (angle > 360) angle -= 360
  angle = Math.floor(angle)
  return 'hsla(' + angle + ', 100%, 50%, ' + a + ') '
}
