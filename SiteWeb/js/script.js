let personnage = JSON.parse(localStorage.getItem("personnage"));


async function AfficherGrilleInitial() {
  if (!personnage) return;

  const centerX = personnage.positionX; // utiliser la position du joueur
  const centerY = personnage.positionY;

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
function updateSelectedTile(tile) {
  const selectedTileDiv = document.querySelector('.card-body.selected-tile');

  console.log(tile)
  

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
        const revealedTile = await getTileAsync(tile.positionX, tile.positionY);
        if (revealedTile) {
          // Mettre à jour l'image
          img.src = 'img/' + revealedTile.imageURL;

          // Mettre à jour l'objet tile
          Object.assign(tile, revealedTile);

          // Mettre à jour la partie "Tuile Sélectionnée"
          updateSelectedTile(tile);
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
  });
}


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

async function deplacer(dx, dy) {
if (!personnage) return;

const nouvelleX = personnage.positionX + dx;
const nouvelleY = personnage.positionY + dy;

console.log("Tentative de déplacement vers", nouvelleX, ";", nouvelleY);

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
  localStorage.setItem("personnage", JSON.stringify(personnage));

  // Reconstruire la grille complète
  const fullGrid = buildFullGrid(nouvelleX, nouvelleY, nouvellesTiles);
  displayTiles(fullGrid);

} catch (err) {
  console.error("Erreur déplacement:", err);
}
}

document.addEventListener('DOMContentLoaded', () => {
document.getElementById("btn-nord").addEventListener("click", () => deplacer(-1, 0));
document.getElementById("btn-sud").addEventListener("click", () => deplacer(1, 0));
document.getElementById("btn-ouest").addEventListener("click", () => deplacer(0, -1));
document.getElementById("btn-est").addEventListener("click", () => deplacer(0, 1));
});

function buildFullGrid(centerX, centerY, tilesFetched) {
const mapTiles = new Map();
tilesFetched.forEach(t => mapTiles.set(`${t.positionX},${t.positionY}`, t));

let fullGrid = [];
for(let dx=-2; dx<=2; dx++){
    for(let dy=-2; dy<=2; dy++){
        const posX = centerX+dx;
        const posY = centerY+dy;
        const key = `${posX},${posY}`;
        if(mapTiles.has(key)){
            fullGrid.push(mapTiles.get(key));
        } else {
            fullGrid.push({ positionX: posX, positionY: posY, imageURL:"tuileCache.png", type:"PLACEHOLDER" });
        }
    }
}
return fullGrid;
}






window.addEventListener('DOMContentLoaded', AfficherGrilleInitial);