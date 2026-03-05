const CELL_SIZE = 40;
const WALL_COLOR = '#00d4ff';
const WALL_WIDTH = 3;
const PADDING = 20;

// Distinct colors for bots
const BOT_COLORS = [
    '#ff6b6b', '#ffd93d', '#6bcb77', '#4d96ff',
    '#ff922b', '#cc5de8', '#20c997', '#f06595'
];

let cachedMazeData = null;
let cachedBots = [];

async function refreshAll() {
    await fetchMaze();
    await fetchBots();
}

async function fetchMaze() {
    const status = document.getElementById('status');
    status.className = '';
    status.textContent = 'Loading maze...';

    try {
        const response = await fetch('/api/maze');
        if (!response.ok) throw new Error(`HTTP ${response.status}`);
        const mazeString = await response.text();
        cachedMazeData = parseMaze(mazeString);
        drawAll();
        status.textContent = `Maze loaded (${cachedMazeData.cellsX} x ${cachedMazeData.cellsY})`;
    } catch (err) {
        status.className = 'error';
        status.textContent = `Error: ${err.message}`;
        console.error(err);
    }
}

async function fetchBots() {
    try {
        const response = await fetch('/api/maze/bots');
        if (!response.ok) throw new Error(`HTTP ${response.status}`);
        cachedBots = await response.json();
        drawAll();
        renderBotPanel();
    } catch (err) {
        console.error('Failed to fetch bots:', err);
    }
}

function getBotColor(index) {
    return BOT_COLORS[index % BOT_COLORS.length];
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

function drawBots(ctx, bots) {
    const botRadius = CELL_SIZE * 0.3;
    bots.forEach((bot, i) => {
        const color = getBotColor(i);
        const cx = PADDING + bot.x * CELL_SIZE + CELL_SIZE / 2;
        const cy = PADDING + bot.y * CELL_SIZE + CELL_SIZE / 2;

        // Glow
        ctx.shadowColor = color;
        ctx.shadowBlur = 10;

        // Body circle
        ctx.fillStyle = color;
        ctx.beginPath();
        ctx.arc(cx, cy, botRadius, 0, Math.PI * 2);
        ctx.fill();

        // Reset shadow
        ctx.shadowBlur = 0;

        // Inner highlight
        ctx.fillStyle = 'rgba(255,255,255,0.35)';
        ctx.beginPath();
        ctx.arc(cx - botRadius * 0.25, cy - botRadius * 0.25, botRadius * 0.4, 0, Math.PI * 2);
        ctx.fill();

        // Index label
        ctx.fillStyle = '#fff';
        ctx.font = `bold ${Math.round(CELL_SIZE * 0.3)}px sans-serif`;
        ctx.textAlign = 'center';
        ctx.textBaseline = 'middle';
        const label = bot.name ? bot.name.charAt(0).toUpperCase() : (i + 1);
        ctx.fillText(label, cx, cy + 1);
    });
}

function drawAll() {
    if (!cachedMazeData) return;
    drawMaze(cachedMazeData);
    const canvas = document.getElementById('mazeCanvas');
    const ctx = canvas.getContext('2d');
    if (cachedBots.length > 0) {
        drawBots(ctx, cachedBots);
    }
}

function renderBotPanel() {
    const list = document.getElementById('botList');
    if (cachedBots.length === 0) {
        list.innerHTML = '<em>No bots yet</em>';
        return;
    }
    list.innerHTML = cachedBots.map((bot, i) => {
        const color = getBotColor(i);
        const pct = Math.max(0, Math.min(100, bot.energy));
        const barColor = pct > 50 ? '#6bcb77' : pct > 20 ? '#ffd93d' : '#ff6b6b';
        return `
            <div class="bot-card">
                <span class="bot-icon" style="background:${color}"></span>
                <span class="bot-name">${bot.name || 'Bot ' + (i + 1)}</span>
                <div class="bot-detail">ID: ${bot.id.substring(0, 8)}...</div>
                <div class="bot-detail">Pos: (${bot.x}, ${bot.y})</div>
                <div class="bot-detail">Energy: ${bot.energy}</div>
                <div class="energy-bar">
                    <div class="energy-bar-fill" style="width:${pct}%;background:${barColor}"></div>
                </div>
            </div>`;
    }).join('');
}

// Load on page open
refreshAll();
