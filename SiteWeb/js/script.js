async function getTilesAsync(x, y, radius = 10) {
    try {
      const url = `http://localhost:3000/api/world/tiles/area?x=${x}&y=${y}&radius=${radius}`;
      const response = await fetch(url);
      
      if (!response.ok) {
        throw new Error(`Erreur HTTP: ${response.status}`);
      }
      
      const tiles = await response.json();
      displayTiles(tiles);
      
    } catch (error) {
      console.error('Erreur lors du chargement:', error);
      showErrorMessage('Impossible de charger les tuiles');
    }
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
      tileDiv.textContent = `(${tile.x}, ${tile.y}): ${tile.type}`;
      grille.appendChild(tileDiv);
    });
  }
