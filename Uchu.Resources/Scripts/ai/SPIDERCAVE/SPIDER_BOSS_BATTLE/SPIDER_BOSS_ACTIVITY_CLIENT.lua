require('o_mis')

function onStartup(self)

	

end

function onScriptNetworkVarUpdate(self,msg)
    --local tableOfVars.con = msg
	if msg.tableOfVars.name == "StartIntro" then
		PhaseIntro(self, msg.tableOfVars.name)
	elseif msg.tableOfVars.name == "bossKilled" then
		local player = GAMEOBJ:GetObjectByID(GAMEOBJ:GetLocalCharID())
		player:PlayCinematic { pathName = "bossKill" } 
		player:DeactivateNDAudioMusicCue{m_NDAudioMusicCueName = "AG_Monument_3"}
	elseif msg.tableOfVars.name == "AudioBossLand" then
		local player = GAMEOBJ:GetObjectByID(GAMEOBJ:GetLocalCharID())
		player:PlayNDAudioEmitter{m_NDAudioEventGUID = "{00cdd107-e09b-4276-93ab-b13c3c37d1fa}"}
	elseif msg.tableOfVars.name == "StoreBossObj" then
	    local obj = GAMEOBJ:GetObjectByID(msg.tableOfVars.object1) 
	    storeObjectByName(self, "bossObj", obj)
	-- Debug GUI
	elseif msg.tableOfVars.name == "UIPhase" then
		if msg.tableOfVars.String1 then
			UI:SendMessage("SetBossVars", {{"phase", msg.tableOfVars.String1}} )
		end
	elseif msg.tableOfVars.name == "UITimer" then
	
		if msg.tableOfVars.String1 then
			UI:SendMessage("SetBossVars", {{"timer", msg.tableOfVars.String1}} )	
		end
		
	elseif msg.tableOfVars.name == "Exit" then
	
		PhaseExit(self,msg.tableOfVars.name)
	end
	

end

------------------------------------------------------------------------------------------------
--- Phase Intro Activity Start -----------------------------------------------------------------
------------------------------------------------------------------------------------------------
function PhaseIntro(self,name)

	local player = GAMEOBJ:GetObjectByID(GAMEOBJ:GetLocalCharID())
    local bossObj =  getObjectByName(self, "bossObj")
    
	if name == "StartIntro" then
		
		-- Play Cinematic
	 	player:PlayCinematic { pathName = "intro" } 
	 	-- Push game State to boss
		UI:SendMessage( "pushGameState", {{"state", "boss" }} )
		-- Start Timer Cue
		GAMEOBJ:GetTimer():AddTimerWithCancel( 1 , "bossAngry1", self )
		
	elseif name == "bossAngry1" then
	
		bossObj:PlayNDAudioEmitter{m_NDAudioEventGUID = "{18c88db5-d20f-47e4-a026-b83132e35bf2}"}
		GAMEOBJ:GetTimer():AddTimerWithCancel( 1, "bossAngry2", self )
	
	elseif name == "bossAngry2" then
	
		bossObj:PlayNDAudioEmitter{m_NDAudioEventGUID = "{6fd720fc-e8ed-43a8-8f88-e4d0f4b6df93}"}
		GAMEOBJ:GetTimer():AddTimerWithCancel( 2, "bossIntroMusic", self )
		
	elseif name == "bossAngry3" then
	
		bossObj:PlayNDAudioEmitter{m_NDAudioEventGUID = "{00cdd107-e09b-4276-93ab-b13c3c37d1fa}"}
		
	elseif name == "bossIntroMusic" then
	
		player:ActivateNDAudioMusicCue{m_NDAudioMusicCueName = "AG_Monument_3"}
		GAMEOBJ:GetTimer():AddTimerWithCancel( 7, "bossAngry3", self )	

	
	end
	
end
------------------------------------------------------------------------------------------------
------------------------------------------------------------------------------------------------
------------------------------------------------------------------------------------------------
--- Phase Exit Activity end  -----------------------------------------------------------------
------------------------------------------------------------------------------------------------
function PhaseExit(self,name)

	local player = GAMEOBJ:GetObjectByID(GAMEOBJ:GetLocalCharID())

	if name == "Exit" then
		
		-- Play Cinematic
	 	player:PlayCinematic { pathName = "exit" } 
	 	-- Push game State to boss
		UI:SendMessage( "popGameState", {{"state", "boss" }} )
		-- Start Timer Cue
	end
	
end
function PhaseReset(self,msg)

	


end

function onTimerDone(self,msg)

	if msg.name == "bossIntroMusic" then
		PhaseIntro(self,msg)
	elseif msg.name == "bossAngry1" then
		PhaseIntro(self,msg)
	elseif msg.name == "bossAngry2" then
		PhaseIntro(self,msg)
	elseif msg.name == "bossAngry3" then
		PhaseIntro(self,msg)		
	end

end






  