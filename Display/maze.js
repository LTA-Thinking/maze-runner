const CELL_SIZE = 40;
const WALL_COLOR = '#00d4ff';
const WALL_WIDTH = 3;
const PADDING = 20;

async function fetchMaze() {
    const status = document.getElementById('status');
    status.className = '';
    status.textContent = 'Loading maze...';

    try {
        const response = await fetch('/api/maze');
        if (!response.ok) throw new Error(`HTTP ${response.status}`);
        const mazeString = await response.text();
        const mazeData = parseMaze(mazeString);
        drawMaze(mazeData);
        status.textContent = `Maze loaded (${mazeData.cellsX} x ${mazeData.cellsY})`;
    } catch (err) {
        status.className = 'error';
        status.textContent = `Error: ${err.message}`;
        console.error(err);
    }
}

function parseMaze(mazeString) {
    // Format: comma-separated rows, each row is a string of "1" (wall) / "0" (no wall)
    // Even rows (0,2,4...) = north walls for logical row y
    // Odd rows (1,3,5...) = west walls for logical row y
    // Columns are width+1 to include east walls of rightmost cells
    const rows = mazeString.split(',');
    const grid = rows.map(row => row.split('').map(c => c === '1'));
    const numDataRows = grid.length;
    const numCols = grid[0] ? grid[0].length : 0;

    // Logical cell dimensions
    const cellsX = numCols - 1;
    const cellsY = Math.floor(numDataRows / 2);

    return { grid, cellsX, cellsY };
}

function drawMaze({ grid, cellsX, cellsY }) {
    const canvas = document.getElementById('mazeCanvas');
    const width = cellsX * CELL_SIZE + PADDING * 2;
    const height = cellsY * CELL_SIZE + PADDING * 2;

    canvas.width = width;
    canvas.height = height;

    const ctx = canvas.getContext('2d');
    ctx.clearRect(0, 0, width, height);

    // Draw cell backgrounds with subtle grid
    ctx.fillStyle = 'rgba(255, 255, 255, 0.02)';
    for (let y = 0; y < cellsY; y++) {
        for (let x = 0; x < cellsX; x++) {
            if ((x + y) % 2 === 0) {
                ctx.fillRect(
                    PADDING + x * CELL_SIZE,
                    PADDING + y * CELL_SIZE,
                    CELL_SIZE,
                    CELL_SIZE
                );
            }
        }
    }

    // Draw walls
    ctx.strokeStyle = WALL_COLOR;
    ctx.lineWidth = WALL_WIDTH;
    ctx.lineCap = 'round';

    for (let y = 0; y < cellsY; y++) {
        for (let x = 0; x < cellsX; x++) {
            const px = PADDING + x * CELL_SIZE;
            const py = PADDING + y * CELL_SIZE;

            // North wall: grid[y*2][x]
            if (grid[y * 2] && grid[y * 2][x]) {
                ctx.beginPath();
                ctx.moveTo(px, py);
                ctx.lineTo(px + CELL_SIZE, py);
                ctx.stroke();
            }

            // West wall: grid[y*2+1][x]
            if (grid[y * 2 + 1] && grid[y * 2 + 1][x]) {
                ctx.beginPath();
                ctx.moveTo(px, py);
                ctx.lineTo(px, py + CELL_SIZE);
                ctx.stroke();
            }

            // East wall: grid[y*2+1][x+1]
            if (grid[y * 2 + 1] && grid[y * 2 + 1][x + 1]) {
                ctx.beginPath();
                ctx.moveTo(px + CELL_SIZE, py);
                ctx.lineTo(px + CELL_SIZE, py + CELL_SIZE);
                ctx.stroke();
            }

            // South wall: grid[(y+1)*2][x]
            if (grid[(y + 1) * 2] && grid[(y + 1) * 2][x]) {
                ctx.beginPath();
                ctx.moveTo(px, py + CELL_SIZE);
                ctx.lineTo(px + CELL_SIZE, py + CELL_SIZE);
                ctx.stroke();
            }
        }
    }

    // Draw corner dots for polish
    ctx.fillStyle = WALL_COLOR;
    for (let y = 0; y <= cellsY; y++) {
        for (let x = 0; x <= cellsX; x++) {
            ctx.beginPath();
            ctx.arc(
                PADDING + x * CELL_SIZE,
                PADDING + y * CELL_SIZE,
                WALL_WIDTH / 2, 0, Math.PI * 2
            );
            ctx.fill();
        }
    }
}

// Load on page open
fetchMaze();
