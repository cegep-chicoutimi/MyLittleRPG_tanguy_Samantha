async function getTileAsync(x, y) {
    try {
      const url = `https://localhost:7061/api/Tiles/?PositionX=10&PositionY=10`;
      const response = await fetch(url);
      
      if (!response.ok) {
        throw new Error(`Erreur HTTP: ${response.status}`);
      }
      
      return tile = await response.json();

      
    } catch (error) {
      console.error('Erreur lors du chargement:', error);
      showErrorMessage('Impossible de charger les tuiles');
    }
  }
  function AfficherGrilleInitial()
  {
    var positionInitialX = 10;
    var positionInitialY = 10; 
    
    let tiles = [];

    tiles.push(getTileAsync(positionInitialX, positionInitialY));
    tiles.push(getTileAsync(positionInitialX + 1, positionInitialY));
    tiles.push(getTileAsync(positionInitialX - 1, positionInitialY));
    tiles.push(getTileAsync(positionInitialX, positionInitialY + 1));
    tiles.push(getTileAsync(positionInitialX, positionInitialY - 1));
    tiles.push(getTileAsync(positionInitialX + 1, positionInitialY + 1));
    tiles.push(getTileAsync(positionInitialX + 1, positionInitialY - 1));
    tiles.push(getTileAsync(positionInitialX - 1, positionInitialY + 1));
    tiles.push(getTileAsync(positionInitialX - 1, positionInitialY - 1));


    displayTiles(tiles);
    //Afficher tuile
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
      tileDiv.textContent = tile.imageURL;
      grille.appendChild(tileDiv);
    });
  }
