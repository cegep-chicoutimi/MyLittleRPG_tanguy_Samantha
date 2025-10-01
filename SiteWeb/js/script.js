// Initialisation au chargement de la page
window.addEventListener('DOMContentLoaded', AfficherGrilleInitial);

let personnage = JSON.parse(localStorage.getItem("personnage"));
let allTiles = [];
let tilesVisible = [];
let monsters = [];
let nbMonstresVaincus = 0;

// Met à jour l'affichage du nombre de monstres vaincus
function updateMonstresVaincus() {
  const compteurDiv = document.querySelector('.compteur-monstres-vaincus');
  compteurDiv.textContent = `Monstres vaincus : ${nbMonstresVaincus}`;
}

// Fonction appelée lorsqu'un monstre est vaincu
function onMonsterDefeated() {
  nbMonstresVaincus++;

  if(nbMonstresVaincus % 10 === 0) {
    fetchMonsters().then(newMonsters => {
      monsters = monsters.concat(newMonsters);
      //console.log("Nouveaux monstres chargés :", newMonsters);
    });
  }
  updateMonstresVaincus();
}

// Charger les monstres au démarrage
fetchMonsters().then(loadedMonsters => {
  monsters = loadedMonsters;
  //console.log("Monstres chargés au démarrage :", monsters);
});

// Fonction pour récupérer les 10 monstres
async function fetchMonsters() {
  try {
    const response = await fetch('https://localhost:7061/api/Monstres/monstre/generate10');
    if (!response.ok) {
      throw new Error(`Erreur HTTP: ${response.status}`);
    }
    const monsters = await response.json();
    return monsters;
  } catch (error) {
    console.error('Erreur lors du chargement des monstres:', error);
    handleAPIError(error, 'Impossible de charger les monstres');
    return [];
  }
}

// Affiche la grille initiale centrée sur le personnage
async function AfficherGrilleInitial() {
  if (!personnage) return;

  const centerX = personnage.positionX; // utiliser la position du joueur
  const centerY = personnage.positionY;

  let fetchedTilesDto = await getTilesAroundAsync(centerX, centerY);
  let fetchedTiles = fetchedTilesDto.tuiles;

  // Create a map of fetched tiles for quick lookup
  const fetchedMap = new Map();
  fetchedTiles.forEach(tile => {
    if (tile) {
      const key = `${tile.positionX},${tile.positionY}`;
      fetchedMap.set(key, tile);
      tilesVisible.push(key, tile);
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

// Récupère les tuiles autour d'une position donnée
async function getTilesAroundAsync(x, y) {
  try {
    
    const url = `https://localhost:7061/api/Tiles?PositionX=${x}&PositionY=${y}`;

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

// Récupère une tuile spécifique
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


// Met à jour la section "Tuile Sélectionnée"
function updateSelectedTile(tile) {
  const selectedTileDiv = document.querySelector('.card-body.selected-tile');
  
  console.log(tile)
  
  const key = `${tile.positionX},${tile.positionY}`;
  tilesVisible.push(key, tile);
  
  var typeStr = "default";
  
  switch(tile.type){
    case 0 : typeStr="plaine";break;
    case 1 :typeStr="eau";break;
    case 2 :typeStr="montagne";break;
    case 3 :typeStr="forest";break;
    case 4 :typeStr="ville";break;
    case 5 :typeStr="route";break;
    case "PLACEHOLDER": typeStr = "Inconnue"; break;
  }
  
  
  console.log(tile.type + " ; "+ typeStr)
  
  selectedTileDiv.innerHTML = `
  <div class="divCard"><p>Position : (${tile.positionX},${tile.positionY})</p></div>
  <div class="divCard"><p>Type : ${typeStr}</p></div>
  <div class="divCard"><p>Traversable : ${tile.estTraversable ? 'oui' : 'non'}</p></div>
  <div class="divCard"><p>Description : ${tile.description || 'Aucune'}</p></div>
  `;
}

// Affiche les tuiles dans la grille
function displayTiles(tiles) {
  const grille = document.querySelector('.grille');
  grille.innerHTML = ''; // Effacer le contenu précédent
  
  tiles.forEach(tile => {
    const tileDiv = document.createElement('div');
    tileDiv.className = 'tile';
    const img = document.createElement('img');
    img.src = 'img/' + tile.imageURL;
    img.alt = 'Tile';
    
    tileDiv.addEventListener('click', async() => {
      if(tile.type == "PLACEHOLDER"){
        console.log(tile.positionX);
        const revealedTile = await getTileAsync(tile.positionX, tile.positionY);
        if (revealedTile) {
          // Mettre à jour l'image
          img.src = 'img/' + revealedTile.imageURL;
          
          // Mettre à jour l'objet tile
          Object.assign(tile, revealedTile);

          // Mettre à jour la partie "Tuile Sélectionnée"
          updateSelectedTile(revealedTile);
        }
      } else {
        // Si déjà révélée → juste mise à jour infos
        updateSelectedTile(tile);
      }
      
      document.querySelectorAll('.tile').forEach(t => t.classList.remove('selected'));
      tileDiv.classList.add('selected');
      
      updateSelectedTile(tile);
    })
    
    tileDiv.appendChild(img);
    grille.appendChild(tileDiv);
    
    // après avoir généré toutes les tuiles
    const player = document.createElement("div");
    player.id = "player";
    grille.appendChild(player);
    
  });
}

// Reconstruit la grille complète en utilisant les tuiles visibles et des placeholders
function buildFullGrid(centerX, centerY, tilesFetched) {
  const mapTiles = new Map();
  
  tilesFetched.forEach(t => {
    const key = `${t.positionX},${t.positionY}`;
    mapTiles.set(key, t);      
    tilesVisible.push(key, t);           
  });
  
  let fullGrid = [];
  for(let dx=-2; dx<=2; dx++){
    for(let dy=-2; dy<=2; dy++){
      const posX = centerX+dx;
      const posY = centerY+dy;
      const key = `${posX},${posY}`;
      if(mapTiles.has(key)){
        fullGrid.push(mapTiles.get(key));             
      } else if(tilesVisible.includes(key))
        {
          fullGrid.push(tilesVisible.get(key));
        }
        else {
          fullGrid.push({ positionX: posX, positionY: posY, imageURL:"tuileCache.png", type:"PLACEHOLDER" });
        }
    }
  }
      return fullGrid;
}

// Vérifie la présence d'un monstre sur une tuile donnée
function VerifierIfMonstreOnTile(x,y){
  return monsters.find(m => m.positionX === x && m.positionY === y);
}

// Démarre un combat contre un monstre
async function startCombat(monstre){
  const url = `https://localhost:7061/api/Mosntres/monstre/fight/${monstre.positionX}/${monstre.positionY}`;
  const response = await fetch(url);

  if (!response.ok) {
    throw new Error(`Erreur HTTP: ${response.status}`);
    }

    return await response.json();
  }

// Gestion des événements de victoire/défaite
function onMonsterDefeated() {
  nbMonstresVaincus++;
  updateMonstresVaincus();
}
function onPlayerDefeated() {
 // Joueur retourne au dernier tuile ville

}

// Déplacement du personnage
async function deplacer(dx, dy) {
  if (!personnage) return;
  
  const nouvelleX = personnage.positionX + dx;
  const nouvelleY = personnage.positionY + dy;
  
  // Vérifier la présence d'un monstre sur la nouvelle tuile
  const monstre = VerifierIfMonstreOnTile(nouvelleX, nouvelleY);
  if(monstre){
    let resultat = await startCombat(monstre);
    if(resultat.code === "Win"){
      alert("Vous avez vaincu le monstre !");
      onMonsterDefeated();
      monsters = monsters.filter(m => m !== monstre);
    }
    else if(resultat.code === "Lose"){
      alert("Vous avez été vaincu par le monstre...");
      onPlayerDefeated();
    }
    else{
      alert("Le combat n'a pas généré de vainqueur.");
      return; // Ne pas se déplacer si le combat n'a pas de vainqueur
    }
  }
  
  // Appel API pour valider le déplacement
  try {
    const response = await fetch(`https://localhost:7061/api/Personnages/Deplacement?posX=${nouvelleX}&posY=${nouvelleY}&idPerso=${personnage.id}`);
    
    if (!response.ok) {
      const msg = await response.json();
      alert(msg.message || "Déplacement impossible");
      return;
    }
    
    const nouvellesTiles = await response.json();
    
    // Mettre à jour la position locale correctement (même casse que le JSON)
    personnage.positionX = nouvelleX;
    personnage.positionY = nouvelleY;
    updatePlayerPosition(nouvelleX,nouvelleY)
    
    localStorage.setItem("personnage", JSON.stringify(personnage));
    
    nouvellesTiles.forEach(tile => {
      const key = `${tile.positionX},${tile.positionY}`;
      tilesVisible.push(key, tile);
    });
    
    // Reconstruire la grille complète
    const fullGrid = buildFullGrid(nouvelleX, nouvelleY, tilesVisible);
    displayTiles(fullGrid);
    
  } catch (err) {
    console.error("Erreur déplacement:", err);
  }
}

// Met à jour la position du joueur dans la grille
function updatePlayerPosition(x, y) {
  const playerDiv = document.getElementById('player');
  if (!playerDiv) return;
  
  // Taille d'une tuile (ajuste si ce n’est pas 32px)
  const tileSize = 32;
  
  playerDiv.style.left = (x * tileSize) + "px";
  playerDiv.style.top = (y * tileSize) + "px";
}



// Gestion des boutons de déplacement
document.addEventListener('DOMContentLoaded', () => {
      document.getElementById("btn-nord").addEventListener("click", () => deplacer(-1, 0));
      document.getElementById("btn-sud").addEventListener("click", () => deplacer(1, 0));
      document.getElementById("btn-ouest").addEventListener("click", () => deplacer(0, -1));
      document.getElementById("btn-est").addEventListener("click", () => deplacer(0, 1));
    });
    
// Fonctions de connexion et d'inscription
async function login(email, motDePasse) {
      try {
        const response = await fetch("https://localhost:7061/api/Utilisateurs/auth/login", {
          method: "POST",
          headers: { "Content-Type": "application/json" },
          body: JSON.stringify({ email, motDePasse })
        });
    
        if (!response.ok) {
          const message = await response.text();
          throw new Error(message || "Erreur de connexion");
        }
        
        const utilisateur = await response.json();
        console.log("Connecté :", utilisateur);
        localStorage.setItem("utilisateur", JSON.stringify(utilisateur));
        window.location.href = "index.html";
      } catch (err) {
        console.error("Erreur lors de la connexion:", err);
        const errorDiv = document.getElementById('error-message');
        if(errorDiv){
          errorDiv.textContent = err.message;
          errorDiv.style.display = "block";
        }
      }
    }
    
    // Inscription avec création du personnage
    async function register(pseudo, email, motDePasse) {
      try {
        const response = await fetch("https://localhost:7061/api/Utilisateurs/auth/register", {
          method: "POST",
          headers: { "Content-Type": "application/json" },
          body: JSON.stringify({ pseudo, email, motDePasse })
        });
        
        if (!response.ok) {
          const message = await response.text();
          throw new Error(message || "Erreur d'inscription");
        }
        
        const utilisateur = await response.json();
        console.log("Utilisateur créé :", utilisateur);
        localStorage.setItem("utilisateur", JSON.stringify(utilisateur));
        
        // Création du personnage associé
        getPersonnageById(utilisateur.id, pseudo);
          
          // Redirection vers la carte
          window.location.href = "index.html";
          
        } catch (err) {
          console.error("Erreur lors de l'inscription:", err);
          const errorDiv = document.getElementById('error-message');
          if(errorDiv){
          errorDiv.textContent = err.message;
          errorDiv.style.display = "block";
        }
      }
    }
  // Récupère le personnage par ID ou le crée s'il n'existe pas
  async function CreatePersonnage(id, pseudo) {
    const responsePerso = await fetch(`https://localhost:7061/api/Personnages/${utilisateur.id},${encodeURIComponent(pseudo)}`, {
            method: "POST"
          });
          
          if(!responsePerso.ok){
            const message = await responsePerso.text();
            throw new Error(message || "Erreur lors de la création du personnage");
          }
          
          const personnage = await responsePerso.json();
          console.log("Personnage créé :", personnage);
          localStorage.setItem("personnage", JSON.stringify(personnage));
          return personnage;
  }
    
// Pour tes formulaires
document.addEventListener('DOMContentLoaded', () => {
      const loginForm = document.getElementById("loginForm");
      if (loginForm) {
        loginForm.addEventListener("submit", (e) => {
          e.preventDefault();
          const email = document.getElementById("email").value;
          const password = document.getElementById("password").value;
          login(email, password);
        });
      }
      // Formulaire d'inscription
      const registerForm = document.getElementById("registerForm");
      if (registerForm) {
        registerForm.addEventListener("submit", (e) => {
          e.preventDefault();
          const pseudo = document.getElementById("pseudo").value;
          const email = document.getElementById("email").value;
          const password = document.getElementById("password").value;
          register(pseudo, email, password);
        });
      }
    });

// Gestion des erreurs API
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