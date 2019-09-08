--------------------------------------------------------------
-- ***********************************************************
--
-- PVP Arena for Team Challenge.
--
-- Author: Erik Urdang
-- 
-- Team: Directrons
--
-- ***********************************************************
--------------------------------------------------------------

require('o_ShootingGallery')


--------------------------------------------------------------
-- ***********************************************************
-- ********************* Functions ***************************
-- ***********************************************************
--------------------------------------------------------------

--------------------------------------------------------------
-- Show socres to player -- currently not used
--------------------------------------------------------------

function DisplayPlayerScores(self) 

	local player1 = getObjectByName(self, "activityPlayer1")
	local player2 = getObjectByName(self, "activityPlayer2")
	local score1 = self:GetVar("Score1")
	local score2 = self:GetVar("Score2")

end
      

--------------------------------------------------------------
-- reset the scores to zero
--------------------------------------------------------------

function resetScores(self)

	self:SetVar("Score1",0)
	self:SetVar("Score2",0)

end


--------------------------------------------------------------
-- show the summary dialog
--------------------------------------------------------------
function showSummaryDialog(self, winner)

	-- get players
	
	local player1 = getObjectByName(self, "activityPlayer1")
	local player2 = getObjectByName(self, "activityPlayer2")
	

	-- get the player's scores
	
	local score1 = self:GetVar("Score1")
	local score2 = self:GetVar("Score2")
	
	local strText = ""
	local name1 = "Player 1 Unknown"
	local name2 = "Player 2 Unknown"
	
	if (player1) then
		name1 = player1:GetName().name
	end
	
	if (player2) then
		name2 = player2:GetName().name
	end
	
	
	strText = name1 .. ": " .. score1 .. " --  " ..
			  name2 .. ": " .. score2 .. " -- "  
			  
	if (winner) then
		strText = strText .. "The WINNER: " .. winner:GetName().name .. "!"
	end
	
	-- show the summary message box to each player
	
	showScore(self, player1, strText)
	showScore(self, player2, strText)
	

end

--------------------------------------------------------------
-- Send the score to any player
--------------------------------------------------------------

function showScore(self, player, strText)

	if (player) then
	
		player:DisplayMessageBox{bShow = true, 
								imageID = 2, 
							 	callbackClient = GAMEOBJ:GetZoneControlID(), 
								text = strText, 
								identifier = "Arena_Summary"}
								
	end
end

--------------------------------------------------------------
-- ***********************************************************
-- ***************** Message Handlers ************************
-- ***********************************************************
--------------------------------------------------------------

--------------------------------------------------------------
-- Startup
--------------------------------------------------------------
function onStartup(self) 

	resetScores(self)

end


--------------------------------------------------------------
-- Called when anyone dies
--------------------------------------------------------------
function onPlayerDied (self, msg)
	
	print ("Someone died!!!")
	
	-- if player 1 died, add 1 to player 2's score, and vice versa
	
	local player1 = getObjectByName(self, "activityPlayer1")
	local player2 = getObjectByName(self, "activityPlayer2")
	local winner = nil
	
	--print ("Player1: " .. player1:GetID() .. " Player 2: " .. player2:GetID() .. " ID = " .. msg.playerID)

	local id = msg.playerID
	if (id:GetID() == player1:GetID()) then
		self:SetVar("Score2", tonumber(self:GetVar("Score2")) + 1)
	else
		self:SetVar("Score1", tonumber(self:GetVar("Score1")) + 1)
     end
	
	local score1 = tonumber(self:GetVar("Score1"))
	local score2 = tonumber(self:GetVar("Score2"))


	-- DisplayPlayerScores(self)
	

	local winningScore = 5
	
	if (score1 >= winningScore) then
		print ("Player 1 wins!")
		showVictory (self, player1)
		winner = player1
	elseif (score2 >= winningScore) then
		print ("Player 2 wins!")
		showVictory (self, player2)
		winner = player2
	end
	
	showSummaryDialog(self, winner)
	
	if (winner) then
		
		-- Someone already won, so reset the scores to 0 for
		-- a new game.
		
		resetScores(self)
		
	end
	
end



--------------------------------------------------------------
-- Play fireworks when player wins
--------------------------------------------------------------
function showVictory(self, player)

	player:ShowEmbeddedEffect{type = "fireworks"}
	player:ShowEmbeddedEffect{type = "rebuild-celebrate"}
	
end

							

--------------------------------------------------------------
-- Called when Player Loads into Zone
--------------------------------------------------------------
function onPlayerLoaded(self, msg)


	-- print ("Player Entered: " .. msg.playerID:GetName().name)
	
	local player = msg.playerID
	
	local player1 = getObjectByName(self, "activityPlayer1")
	
	-- Assume this is either the first or second player. No others
	-- are allowed:
	
	if (player1 == nil or player == player1) then
	
		-- this is player 1
 		storeObjectByName(self, "activityPlayer1", msg.playerID)
 	else
 	
 		-- this is player 2
 		storeObjectByName(self, "activityPlayer2", msg.playerID)
 		
 		-- Pick a new faction to be safe (no collisions with
 		-- other NPC factions etc.). 
 		--
		-- Note: 101 is being used to make sure that this script:
 		--
 		-- ...\scripts\ai\CRATER\L_KILL_PLAYER_CONTEST_CREATURE.lua
 		--
 		-- works.
 		
 		player:SetFaction {faction = 101}
	end
	 	
 	showSummaryDialog(self, nil)

	
	
end

