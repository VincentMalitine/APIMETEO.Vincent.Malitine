================================================================================
                        APIMETEO - Application Météo WPF
                              Vincent Malitine
================================================================================

DESCRIPTION
-----------
APIMETEO est une application de bureau Windows développée en C# avec WPF (.NET 9)
qui affiche les prévisions météorologiques en temps réel. L'application utilise
l'API gratuite de prevision-meteo.ch pour récupérer les données météo.

FONCTIONNALITÉS
---------------
• Affichage de la météo actuelle (température, humidité, conditions)
• Températures min/max/moyenne du jour
• Prévisions sur 5 jours (J+0 à J+4)
• Mise à jour automatique toutes les secondes
• Personnalisation de la ville par défaut
• Système de thèmes personnalisables :
    - GitHub Dark (par défaut)
    - Ocean Blue
    - Forest Green
    - Sunset Orange
    - Purple Night
• Sauvegarde des préférences utilisateur

STRUCTURE DU PROJET
-------------------
APIMETEO.Vincent.Malitine/
├── MainWindow.xaml(.cs)      # Fenêtre principale
├── model/
│   └── meteo.cs              # Modèles de données API + Gestionnaire de thèmes
├── views/
│   ├── meteopage.xaml(.cs)   # Page d'affichage météo
│   └── settingspage.xaml(.cs)# Page des paramètres
└── Properties/
    └── Settings.settings     # Paramètres utilisateur persistants

TECHNOLOGIES UTILISÉES
----------------------
• Framework      : .NET 9
• Interface      : WPF (Windows Presentation Foundation)
• Langage        : C#
• Sérialisation  : Newtonsoft.Json
• API Météo      : https://www.prevision-meteo.ch/

PRÉREQUIS
---------
• Windows 10/11
• .NET 9 SDK ou Runtime
• Connexion Internet (pour l'API météo)

INSTALLATION
------------
1. Cloner le dépôt :
   git clone https://github.com/VincentMalitine/APIMETEO.Vincent.Malitine.git

2. Ouvrir la solution dans Visual Studio 2022/2026

3. Restaurer les packages NuGet

4. Compiler et exécuter (F5)

UTILISATION
-----------
• Au lancement, l'application affiche la météo de la ville par défaut (Annecy)
• Accédez aux paramètres pour :
    - Changer la ville par défaut
    - Modifier le thème de l'application
• Les données météo se mettent à jour automatiquement

API MÉTÉO
---------
L'application consomme l'API REST de prevision-meteo.ch :
- Endpoint : https://www.prevision-meteo.ch/services/json/{ville}
- Format   : JSON
- Données  : Conditions actuelles + prévisions J+0 à J+4

AUTEUR
------
Vincent Malitine

LICENCE
-------
Projet personnel / Usage éducatif

================================================================================