async function getTileAsync(x, y) {
    try {
      
      const url = `https://localhost:7061/api/Tiles/${x}%2C${y}?PositionX=${x}&PositionY=${y}`;
      const response = await fetch(url);
      
      if (!response.ok) {
        throw new Error(`Erreur HTTP: ${response.status}`);
      }

      return await response.json();

      
    } catch (error) {
      console.error('Erreur lors du chargement:', error);
      handleAPIError(error, 'Impossible de charger les tuiles');
    }
  }
  async function AfficherGrilleInitial() {
  const centerX = 10;
  const centerY = 10;

  const fetchedPositions = [
    [0, 0], [1, 0], [-1, 0], [0, 1], [0, -1], [1, 1], [1, -1], [-1, 1], [-1, -1]
  ];

  // Fetch 9 tiles around the center
  let fetchedTilesPromises = fetchedPositions.map(([dx, dy]) => getTileAsync(centerX + dx, centerY + dy));
  const fetchedTiles = await Promise.all(fetchedTilesPromises);

  // Create a map of fetched tiles for quick lookup
  const fetchedMap = new Map();
  fetchedTiles.forEach(tile => {
    if (tile) {
      const key = `${tile.positionX},${tile.positionY}`;
      fetchedMap.set(key, tile);
    }
  });

  // Build the full 5x5 tiles grid, including placeholders where tiles are missing
  let fullGridTiles = [];

  for (let dx = -2; dx <= 2; dx++) {
    for (let dy = -2; dy <= 2; dy++) {
      const posX = centerX + dx;
      const posY = centerY + dy;
      const key = `${posX},${posY}`;
      if (fetchedMap.has(key)) {
        fullGridTiles.push(fetchedMap.get(key));
      } else {
        // Placeholder tile object with default image (change as needed)
        fullGridTiles.push({
          positionX: posX,
          positionY: posY,
          imageURL: "tuileCache.png",  // your default image filename
          type: "PLACEHOLDER"
        });
      }
    }
  }

  displayTiles(fullGridTiles);
}
  function handleAPIError(error, userMessage = 'Une erreur est survenue') {
    console.error('Erreur API:', error);
    
    // Afficher un message à l'utilisateur
    const errorDiv = document.getElementById('error-message');
    errorDiv.textContent = userMessage;
    errorDiv.style.display = 'block';
    
    // Cacher le message après 5 secondes
    setTimeout(() => {
      errorDiv.style.display = 'none';
    }, 5000);
  }
  function displayTiles(tiles) {
    const grille = document.querySelector('.grille');
    grille.innerHTML = ''; // Effacer le contenu précédent

    tiles.forEach(tile => {
      const tileDiv = document.createElement('div');
      tileDiv.className = 'tile';


      const img = document.createElement('img');
      img.src =  'img/' + tile.imageURL;
      img.alt = 'Tile';
      tileDiv.appendChild(img);
      grille.appendChild(tileDiv);
    });
  }
window.addEventListener('DOMContentLoaded', AfficherGrilleInitial);