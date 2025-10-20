// Initialisation au chargement de la page
window.addEventListener('DOMContentLoaded', AfficherGrilleInitial);

//Vérifier if logged in
document.addEventListener('DOMContentLoaded', VerifIfLoggedIn);

// Modal elements
const openModalBtn = document.getElementById('btn-simuler');
if(openModalBtn) openModalBtn.addEventListener('click', () => {
  myModal.style.display = 'block';
  modalBackdrop.style.display = 'block';
});
const closeModalBtn = document.getElementById('closeModalBtn');
if(closeModalBtn) closeModalBtn.addEventListener('click', () => {
  myModal.style.display = 'none';
  modalBackdrop.style.display = 'none';
});

const myModal = document.getElementById('myModal');
const modalBackdrop = document.getElementById('modal-backdrop');
if(modalBackdrop) modalBackdrop.addEventListener('click', () => {
  myModal.style.display = 'none';
  modalBackdrop.style.display = 'none';
});

// Variables globales
let personnage = JSON.parse(localStorage.getItem("personnage"));
let user = JSON.parse(localStorage.getItem("utilisateur"));
let selectedTileDiv = document.querySelector('.card-body.selected-tile');
let allTiles = [];
const tilesVisible = new Map();

function ChangementTheme() {
  const body = document.body;
  const toggleBtn = document.getElementById('themeChange');

  if (toggleBtn) {
    toggleBtn.addEventListener('click', () => {
      body.classList.toggle('light-theme');
      body.classList.toggle('dark-theme');
      localStorage.setItem('theme', body.classList.contains('light-theme') ? 'light' : 'dark');
    });
  }

  // Apply saved theme
  const savedTheme = localStorage.getItem('theme') || 'dark';
  body.classList.add(savedTheme + "-theme");
}

document.addEventListener("DOMContentLoaded", ChangementTheme);

// Simulation de combat
function SimulationCombat() {
  if (!personnage || !user) return;

  const monstre = selectedTileDiv.monstre;
  const modalContent = document.querySelector('#myModal .modal-content');

  if(monstre == null){
    modalContent.innerHTML = `<h3>Aucun monstre sur cette tuile !</h3>`;
    myModal.style.display = 'block';
    modalBackdrop.style.display = 'block';
    return;
  }


  const resultats = LogiqueCombat(personnage, monstre);

  modalContent.innerHTML = `
    <h2 class="modalTitle text-center mb-3">Simulation de Combat</h2>
    <h4 class="text-center mb-4">${personnage.nom} <span class="text-danger">vs</span> ${monstre.nom}</h4>

    <div class="combat-characters d-flex justify-content-around mb-4">
      <div class="character-card text-center p-2">
        <img src="img/sprite.png" alt="Sprite Joueur" class="mb-2" style="width:60px;height:60px;">
        <h5>Joueur</h5>
        <p>Force: <strong>${personnage.force}</strong></p>
        <p>Défense: <strong>${personnage.defense}</strong></p>
        <p>PV: <strong>${personnage.pv}</strong></p>
      </div>

      <div class="character-card text-center p-2">
        <img src="${monstre.spriteUrl}" alt="Sprite Monstre" class="mb-2" style="width:60px;height:60px;">
        <h5>Monstre</h5>
        <p>Force: <strong>${monstre.attaque}</strong></p>
        <p>Défense: <strong>${monstre.defense}</strong></p>
        <p>PV: <strong>${monstre.pointsVieActuels}</strong></p>
      </div>
    </div>

    <hr>

    <div class="combat-results text-center">
      <h5>Résultats (1000 simulations)</h5>
      <p>Victoires du joueur : <strong>${resultats.nbVictoires}</strong></p>
      <p>Défaites du joueur : <strong>${resultats.nbDefaites}</strong></p>
      <p>Égalités : <strong>${resultats.nbEgalites}</strong></p>
      <p>Dégâts moyens infligés par le joueur : <strong>${resultats.moyenneDegatsJoueur.toFixed(2)}</strong></p>
      <p>Dégâts moyens infligés par le monstre : <strong>${resultats.moyenneDegatsMonstre.toFixed(2)}</strong></p>
      <p>PV moyens restants du joueur : <strong>${resultats.moyennePVRestantsJoueur.toFixed(2)}</strong></p>
      <p>PV moyens restants du monstre : <strong>${resultats.moyennePVRestantsMonstre.toFixed(2)}</strong></p>
      <hr>
    </div>
  `;

  // Afficher la modal
  myModal.style.display = 'block';
  modalBackdrop.style.display = 'block';
}

function LogiqueCombat(monstre) {
  //Variables pour les statistiques
  const nbSimulations = 1000;
  var nbVictoires = 0;
  var nbDefaites = 0;
  var nbEgalites = 0;
  var moyenneDegatsJoueur = 0;
  var moyenneDegatsMonstre = 0;
  var moyennePVRestantsJoueur = 0;
  var moyennePVRestantsMonstre = 0;

  // Variables de combat
  var ForceJoueur = personnage.force;
  var DefenseJoueur = personnage.defense;
  var ForceMonstre = monstre.attaque;
  var DefenseMonstre = monstre.defense;

  // Simulation des combats
  for (let i = 0; i < nbSimulations; i++) {
    //Réinitialisation des variables de combat
    var PointsDeVieJoueur = personnage.pv;
    var PointsDeVieMonstre = monstre.pointsVieActuels;
    var facteurAleatoireMonstre = Math.random() * (1.25 - 0.8) + 0.8;
    var facteurAleatoireJoueur = Math.random() * (1.25 - 0.8) + 0.8;
    var degatsMonstre = (ForceJoueur - DefenseMonstre) * facteurAleatoireMonstre;
    var degatsJoueur = (ForceMonstre - DefenseJoueur) * facteurAleatoireJoueur;

    if (degatsMonstre < 0) degatsMonstre = 0;
    if (degatsJoueur < 0) degatsJoueur = 0;

    PointsDeVieMonstre -= degatsMonstre;
    PointsDeVieJoueur -= degatsJoueur;
    
    //Mise à jour des moyennes
    moyenneDegatsJoueur += degatsJoueur;
    moyenneDegatsMonstre += degatsMonstre;
    moyennePVRestantsJoueur += PointsDeVieJoueur;
    moyennePVRestantsMonstre += PointsDeVieMonstre;

  //Vérification du résultat du combat
      if(PointsDeVieMonstre <= 0 && PointsDeVieJoueur > 0) 
      { 
        nbVictoires++;
      }
      else if(PointsDeVieJoueur <= 0)
      {
        nbDefaites++; 
      }
      else{
        nbEgalites++;
      }
  }
  //Calcul des moyennes
  moyenneDegatsJoueur = moyenneDegatsJoueur / nbSimulations;
  moyenneDegatsMonstre = moyenneDegatsMonstre / nbSimulations;
  moyennePVRestantsJoueur = moyennePVRestantsJoueur / nbSimulations;
  moyennePVRestantsMonstre = moyennePVRestantsMonstre / nbSimulations;

  return { nbVictoires, nbDefaites, nbEgalites, moyenneDegatsJoueur, moyenneDegatsMonstre, moyennePVRestantsJoueur, moyennePVRestantsMonstre };
}

document.addEventListener('DOMContentLoaded', () => {
  const simulerButton = document.getElementById('btn-simuler');
  if(simulerButton) simulerButton.addEventListener('click', SimulationCombat);
});


// Affiche la grille initiale centrée sur le personnage
async function AfficherGrilleInitial() {
  if (!personnage && !user) return;
  if (!personnage && user) 
    personnage = await CreatePersonnage(user.id, user.pseudo);
  
  // Centrer la grille sur le personnage

  const centerX = personnage.positionX; // utiliser la position du joueur
  const centerY = personnage.positionY;

  let fetchedGrilleJeu = await getTilesAroundAsync(centerX, centerY);
  let fetchedTiles = fetchedGrilleJeu.tuiles;

  // Create a map of fetched tiles for quick lookup
  const fetchedMap = new Map();
  fetchedTiles.forEach(tile => {
    if (tile) {
      const key = `${tile.X},${tile.Y}`;
    console.log("Adding tile to map:", key, tile);
      fetchedMap.set(key, tile);
      tilesVisible.set(key, tile);
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
          X: posX,
          Y: posY,
          imageUrl: "tuileCache.png",
          type: "PLACEHOLDER",
          monstre: null,
        });
      }
    }
  }

  displayTiles(fullGridTiles);
}

// Affiche les tuiles dans la grille
function displayTiles(tiles) {
  const grille = document.querySelector('.grille');
  grille.innerHTML = ''; // Effacer le contenu précédent
  
  tiles.forEach(tile => {
    const tileDiv = document.createElement('div');
    tileDiv.className = 'tile';
    const img = document.createElement('img');
    img.src = 'img/' + tile.imageUrl;
    img.classList.add("imgTuile");
    img.alt = 'Tile';
    if(tile.monstre != null)
    {
      const imgSprite = document.createElement('img');
      imgSprite.src = tile.monstre.spriteUrl;
      imgSprite.classList.add("imgSprite");
      imgSprite.alt = 'TileMonstre';
      tileDiv.appendChild(imgSprite);
    }
    
    tileDiv.addEventListener('click', async() => {
      if(tile.type == "PLACEHOLDER"){
        const revealedTile = await getTileAsync(tile.X, tile.Y);
        if (revealedTile) {
          // Mettre à jour l'image
          img.src = 'img/' + revealedTile.imageUrl;
          
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
    
  });

  // après avoir généré toutes les tuiles
  const player = document.createElement("div");
  player.id = "player";
  grille.appendChild(player);
}

// Récupère les tuiles autour d'une position donnée
async function getTilesAroundAsync(X, Y) {
  try {
    const url = `https://localhost:7061/api/Tiles?X=${X}&Y=${Y}`;

    const response = await fetch(url);
    
    if (!response.ok) {
      throw new Error(`Erreur HTTP: ${response.status}`);
    }

    return await response.json();

  } catch (error) {
    handleAPIError(error, 'Impossible de charger les tuiles');
  }
}

// Récupère une tuile spécifique
async function getTileAsync(X, Y) {
  try {
    const url = `https://localhost:7061/api/Tiles/${X}%2C${Y}?X=${X}&Y=${Y}`;
    const response = await fetch(url);
    
    if (!response.ok) {
      throw new Error(`Erreur HTTP: ${response.status}`);
    }

    return await response.json();

    
  } catch (error) {
    handleAPIError(error, 'Impossible de charger les tuiles');
  }
}


// Met à jour la section "Tuile Sélectionnée"
function updateSelectedTile(tile) {
  selectedTileDiv.monstre = tile.monstre; // Stocker le monstre (ou null) dans la div

  UpdateInfosMonstre(tile.monstre);

  const key = `${tile.X},${tile.Y}`;
  tilesVisible.set(key, tile);
  
  var typeStr = "default";
  
  switch(tile.typeTuile){
    case "PLAINE" : typeStr="plaine";break;
    case "EAU" :typeStr="eau";break;
    case "MONTAGNE" :typeStr="montagne";break;
    case "FORET" :typeStr="forest";break;
    case "VILLE+" :typeStr="ville";break;
    case "ROUTE" :typeStr="route";break;
    case "PLACEHOLDER": typeStr = "Inconnue"; break;
  }
  
  selectedTileDiv.innerHTML = `
  <div class="divCard"><p>Position : ${tile.X},${tile.Y}</p></div>
  <div class="divCard"><p>Type : ${typeStr}</p></div>
  <div class="divCard"><p>Traversable : ${tile.estAccessible ? 'oui' : 'non'}</p></div>
  `;
}

function UpdateInfosMonstre(selectedTileMonster)
{
  if(selectedTileMonster != null){
    document.getElementById("monstreNom").textContent = selectedTileMonster.nom;
    document.getElementById("monstrePV").textContent = selectedTileMonster.pointsVieActuels;
    document.getElementById("monstreForce").textContent = selectedTileMonster.attaque;
    document.getElementById("monstreDefense").textContent = selectedTileMonster.defense;
    document.getElementById("monstrePosition").textContent = `${selectedTileMonster.x}, ${selectedTileMonster.y}`;
  }
  if(selectedTileMonster == null){
    document.getElementById("monstreNom").textContent = "Aucun monstre";
    document.getElementById("monstrePV").textContent = "-";
    document.getElementById("monstreForce").textContent = "-";
    document.getElementById("monstreDefense").textContent = "-";
    document.getElementById("monstrePosition").textContent = "-";
  }
}


// Reconstruit la grille complète en utilisant les tuiles visibles et des placeholders
function buildFullGrid(centerX, centerY, tilesFetched) {
  const mapTiles = new Map();
  
  tilesFetched.forEach(t => {
    const key = `${t.X},${t.Y}`;
    mapTiles.set(key, t);      
    tilesVisible.set(key, t);           
  });
  
  const fullGrid = [];

  for(let dx=-2; dx<=2; dx++){
    for(let dy=-2; dy<=2; dy++){
      const posX = centerX+dx;
      const posY = centerY+dy;
      const key = `${posX},${posY}`;
      if(mapTiles.has(key)){
        fullGrid.push(mapTiles.get(key));             
      } else if(tilesVisible.has(key)) {
          fullGrid.push(tilesVisible.get(key));
        }
        else {
          fullGrid.push({ X: posX, Y: posY, imageUrl:"tuileCache.png", type:"PLACEHOLDER" });
        }
    }
  }
      return fullGrid;
}

// Déplacement du personnage
async function deplacer(dx, dy) {
  if (!personnage && !user) return;
  
  const nouvelleX = personnage.X + dx;
  const nouvelleY = personnage.Y + dy;
  
  // Appel API pour valider le déplacement
  try {
    const response = await fetch(`https://localhost:7061/api/Personnages/Deplacement?X=${nouvelleX}&Y=${nouvelleY}&idPerso=${personnage.id}`);
    
    if (!response.ok) {
      const msg = await response.json();
      alert(msg.message || "Déplacement impossible");
      return;
    }
    
    const grilleRetour = await response.json();
    const resultFight = grilleRetour.resultFight;
    const nouvellesTiles = grilleRetour.tuiles;

    if(resultFight)
    {
      if(resultFight.code == "Draw")
      {
        alert("Égalité ! Vous êtes toujours en vie mais le monstre aussi !");
      }
      else if(resultFight.code == "Lose")
      {
        alert("Vous avez perdu le combat et êtes mort ! Vous revenez à la ville la plus proche.");
      }
      else if(resultFight.code == "Win")
      {
        alert("Vous avez vaincu le monstre !");
      }
      personnage = resultFight.personnage; 
    }
    else if(!resultFight)
    {
      personnage.X = nouvelleX;
      personnage.Y = nouvelleY;
    }
    

    updatePlayerPosition(personnage.X, personnage.Y);

    localStorage.setItem("personnage", JSON.stringify(personnage));
    
    nouvellesTiles.forEach(tile => {
      const key = `${tile.X},${tile.Y}`;
      tilesVisible.set(key, tile);
    });
    
    // Reconstruire la grille complète
    const fullGrid = buildFullGrid(personnage.X, personnage.Y, tilesVisible);
    displayTiles(fullGrid);
    
  } catch (err) {
    handleAPIError(err, "Incapable de faire le déplacement");
  }
}

// Met à jour la position du joueur dans la grille
function updatePlayerPosition(X, Y) {
  const playerDiv = document.getElementById('player');
  if (!playerDiv) return;
  
  // Taille d'une tuile (ajuste si ce n’est pas 32px)
  const tileSize = 32;
  
  playerDiv.style.left = (X * tileSize) + "px";
  playerDiv.style.top = (Y * tileSize) + "px";
}

// Gestion des boutons de déplacement
document.addEventListener('DOMContentLoaded', () => {
  const btnNord  = document.getElementById("btn-nord");
  const btnSud   = document.getElementById("btn-sud");
  const btnOuest = document.getElementById("btn-ouest");
  const btnEst   = document.getElementById("btn-est");

  if (btnNord)  btnNord.addEventListener("click", () => deplacer(-1, 0));
  if (btnSud)   btnSud.addEventListener("click", () => deplacer(1, 0));
  if (btnOuest) btnOuest.addEventListener("click", () => deplacer(0, -1));
  if (btnEst)   btnEst.addEventListener("click", () => deplacer(0, 1));
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
        getPersonnageByUserId(utilisateur.id, utilisateur.pseudo);

        localStorage.setItem("utilisateur", JSON.stringify(utilisateur));
        localStorage.setItem('isLoggedIn', 'true');

        window.location.href = "index.html";
      } catch (err) {
        handleAPIError(err, "Problème lors du login")
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

        login(email,motDePasse)
          
          // Redirection vers la carte
          window.location.href = "index.html";
          
        } catch (err) {
          handleAPIError(err, "Erreur lors de l'inscription");
        }
      }
    
    function deconnection() {
        localStorage.setItem("utilisateur", null);
        localStorage.setItem("isLoggedIn", false);
        localStorage.setItem("personnage", null);
        VerifIfLoggedIn();
    }
    function VerifIfLoggedIn()
    {
      const currentPage = window.location.pathname;

      // Don't redirect if already on login.html
      if (currentPage.endsWith('login.html') || currentPage.endsWith('register.html')) {
          return;
      }

      var LoggedIn = localStorage.getItem("isLoggedIn");

      // Since localStorage stores everything as strings:
      if (LoggedIn === "false") {
          window.location.href = 'login.html'; 
      }
    }

    //Bouton déconnection
    document.addEventListener('DOMContentLoaded', () => {
      const deconnectBtn = document.getElementById('btnDeconnecter');
      if(deconnectBtn) deconnectBtn.addEventListener('click', deconnection);
    });


    async function getPersonnageByUserId(userId, pseudo) {
      try {
        const response = await fetch(`https://localhost:7061/api/Personnages/User/${userId}`);
        if (!response.ok) {
          // Si le personnage n'existe pas, le créer
          personnage = await CreatePersonnage(userId, pseudo);
          return personnage;
        }

        personnage = await response.json();

        localStorage.setItem("personnage", JSON.stringify(personnage));
        return personnage;
      } catch (err) {
        handleAPIError(err, "Erreur lors de la récupération du personnage");
      }
    }
  // Récupère le personnage par ID ou le crée s'il n'existe pas
  async function CreatePersonnage(id, pseudo) {
    try{
      const responsePerso = await fetch("https://localhost:7061/api/Personnages", {
        method: "POST",
        headers: { "Content-Type": "application/json" },
        body: JSON.stringify({ idUser: id, nom: pseudo })
      });

      if (!responsePerso.ok) {
        const message = await responsePerso.text();
        throw new Error(message || "Erreur lors de la création du personnage");
      }
          
      const personnage = await responsePerso.json();
      localStorage.setItem("personnage", JSON.stringify(personnage));
      return personnage;
    }
    catch(err)
    {
      handleAPIError(err, "Erreur lors de la création du personnage");
    }
  }
    
// Pour les formulaires
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