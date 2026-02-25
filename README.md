# KillTheRatKing 2.0  
<img width="308" height="261" alt="Title" src="https://github.com/user-attachments/assets/faf740bf-cf9d-42d0-9285-ab2863e97c1e" />  

updated version of KillTheRatking -> https://github.com/bjornpetersson1/KillTheRatking_SchoolAssignment.git

## Videos
New game and High Score: [Video](https://github.com/user-attachments/assets/1f6645c1-d3ca-4d93-893e-075890a7d1ce)  
Load game and death: [Video](https://github.com/user-attachments/assets/5626fa21-e46f-4e17-92ef-16152cd857a1)  

  
<img width="437" height="207" alt="TheRatKingIsDead" src="https://github.com/user-attachments/assets/f3cf0b0d-2034-45ad-a730-b7cf9a9ffcf3" />  

## New features  
### Save- and load games  
<img width="398" height="287" alt="LoadSave" src="https://github.com/user-attachments/assets/909155f9-c979-43e0-8321-3e7aad539e15" />  

KillTheRatKing now uses MongoDB to save multiple different games.  
When loading a save you continue your game from where you left off.  
The game autosaves every 10 turn or when you exit/start a level.

### High Score  
<img width="281" height="224" alt="HighScore" src="https://github.com/user-attachments/assets/32552e18-6376-4f4b-802a-08c0a8e693e4" />  

Upon death or saving your current game. Your score is added to the high score (if you made it to the top 10)  

### Choose Class  
<img width="326" height="309" alt="ChooseClass" src="https://github.com/user-attachments/assets/71be7ff3-aa96-4df6-ad85-d7e899310ab2" />  

When starting a new game you can choose a class.

### Level System  
<img width="266" height="237" alt="LevelSystem" src="https://github.com/user-attachments/assets/fc29fe96-f367-400d-a652-8bb192a88b8f" />  

When starting a new game only "Level 1" and "Generate level" is unlocked.
You unlock the other levels by playing the game.  

### Hardcore mode  
<img width="407" height="273" alt="Death" src="https://github.com/user-attachments/assets/a4f5aa95-1159-46d4-89f7-2544f98e12bd" />  

Upon death your save is deleted and gone forever.

## Instructions  
- Clone the repo
- Build the app
- Run the game  

    *the game currently connects to localhost, if you would prefer something else: Change the connectionstring in the class MongoConnection*
