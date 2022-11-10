const width = 1200
const height = 800

function setup() {
  createCanvas(width, height);

  for (var i = 0; i < 10; ++i) {
    points.push([
      random(0, width),
      random(0, height)
    ])

    velocities.push([0, 0])
  }
}

var done = false;
// array of [x, y]
var points = []
var velocities = []
var mouseWasPressed = false

function draw() {
  if (mouseIsPressed) {
    if (!mouseWasPressed) {
      points.push(
        [points[points.length - 1][0], points[points.length - 1][1]]
      )
      velocities.push([0, 0])

      // regenerate
      for (var i = 0; i < points.length; ++i) {
        maxV = 7
        velocities[i] = [
          random(-maxV, maxV), random(-maxV, maxV)
        ]
      }
    } else {
      // migrate
      for (var i = 0; i < points.length; ++i) {
        points[i] = [
          points[i][0] + velocities[i][0],
          points[i][1] + velocities[i][1]
        ]

        min = 30

        if (points[i][0] < min) {
          points[i][0] = min;
          velocities[i][0] = -velocities[i][0]
        }
        if (points[i][1] < min) {
          points[i][1] = min;
          velocities[i][1] = -velocities[i][1]
        }
        if (points[i][0] > width - min) {
          points[i][0] = width - min;
          velocities[i][0] = -velocities[i][0]
        }
        if (points[i][1] > height - min) {
          points[i][1] = height - min;
          velocities[i][1] = -velocities[i][1]
        }
      }
    }
    mouseWasPressed = true
  } else mouseWasPressed = false;

  noFill()
  stroke(255, 0, 0)
  strokeWeight(5)

  background(255);

  for (var i = 0; i < points.length - 1; ++i) {
    var coord = points[i]
    var next = points[i + 1]

    joint(coord)
    line(coord[0], coord[1], next[0], next[1])
  }


  joint(points[points.length - 1])
}

function joint(coord) {
  ellipse(coord[0], coord[1], 30, 30)
}
