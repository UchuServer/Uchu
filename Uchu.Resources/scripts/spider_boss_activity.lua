 --------------------------------------------------------------
-- Includes
--------------------------------------------------------------
require('o_mis')

Minions = {}
Eggs = {}
static = {}

-- Event Delays 
static['Button_to_EMP_Blast'] = 2 
static['Boss_Stunned_Time_Phase_1'] = 10
static['Boss_Stunned_Time_Phase_2'] = 10
static['Boss_Stunned_Time_Phase_3'] = 10




CONSTANTS = {}
CONSTANTS["NO_OBJECT"] = "0"


--------------------------------------------------
-- Spider Boss Start up --------------------------
--------------------------------------------------

function onStartup(self)

	con = {}
	
	-- Health By Phase
	
	con["Phase_1_health"] =  64 -- 80%  Starting Phase One skill
	con["Phase_2_health"] =  48	-- 60% The Spider begins using one or more new attack skills 
	con["Phase_3_health"] =  24	-- 30% Attacks begin to form out of the Maelstrom cloud, The Maelstrom  occasionally break the quickbuild pipes

	
	-- Level Objects
	con["Spider_Minion"] = 9463
	con["Spider_Egg"] = 9313
	con["Spider_Boss"] = 8445
	con["Exit_Bridge"] = 0000

	-- to be replaced with instance matching 
	con["numberOfplayers"] = 4
	con["numberOfrebuilds"] = 2
	con["currentRebuilds"] = 0
	
	-- Number of Buttons to activate EMP
	con["ButtonToBePressed"] = 2
	-- Current Buttons Pressed
	con["ButtonPressed"] = 0
	con["RebuildsComplete"] = false
	
	
	con["Eggs_Spawned"] = 0
	con["Eggs_Total"]   = 0
	
	con["Spiders_Spawned"] = 0 
	con["Spiders_Total"]   = 0
	
	con["Egg_Markers"]   = 0
	
	con["totalnumOfminios"] = 0 

	self:SetVar("con",con)

	self:SetVar("Phase", 1 )
			
		
	GAMEOBJ:GetTimer():AddTimerWithCancel( 5 , "SpawnBoss", self )

	self:SetActivityParams{  modifyMaxUsers = true, maxUsers = con["numberOfplayers"] , modifyActivityActive = true,  activityActive = true} 
end





function onChildLoaded(self,msg)
	
	-- Store Spider Boss Obj
	if (msg.templateID == self:GetVar("con.Spider_Boss")) then 

		storeObjectByName(self, "bossObj", msg.childID)
	    SendNetWorkVar( self , "StoreBossObj", msg.childID:GetID() , "", "", "", "", "" )
		
	-- Store Spider Eggs 	
	elseif (msg.templateID == self:GetVar("con.Spider_Egg") ) then
	
		if not Eggs[msg.childID:GetID()] then
		
			Eggs[msg.childID:GetID()] = msg.childID:GetID()
			
		end
	-- Store Spider Minions 	
	elseif (msg.templateID == self:GetVar("con.Spider_Minion")) then
		
		
		if not Minions[msg.childID:GetID()] then
		
			Minions[msg.childID:GetID()] = msg.childID:GetID()
			
		end
	
		
	end
end





function PhaseIntro(self,msg)

	if msg.name == "SpawnBoss" then
		-- Get Boss Spawn Point
		local spawn = self:GetObjectsInGroup{ group = "bossSpawn1" ,ignoreSpawners = true }.objects
		local Markpos = spawn[1]:GetPosition().pos 
		local Markrot = spawn[1]:GetRotation()
		-- Spawn Spider Boss Start Pos	
		local config = { {"attached_path", "bosspath0"}, {"attached_path_start", 0 }   }		
		RESMGR:LoadObject { objectTemplate = 8445 , x = Markpos.x ,
		y = Markpos.y , z = Markpos.z ,rw = Markrot.w , rx = Markrot.x, ry = Markrot.y  , 
		rz = Markrot.z, owner = self , configData = config};
		
		
	end



end



function Reset(self)


	-- remove Eggs
	removeEggs(self)
	fxOffButtons(self)
	-- remove Minions
	removeMinions(self)
	
	-- tele players
	local objects = self:GetAllActivityUsers{}.objects
	local spawnPos = self:GetObjectsInGroup{ignoreSpawners=true, group = "spawn1" }.objects
	
	for i = 1, #objects do

		local player =  objects[i]
		
		
		local Markpos = spawnPos[1]:GetPosition().pos 
		local Markrot = spawnPos[1]:GetRotation()	
		player:Teleport{pos = Markpos, y = Markrot.y, w = Markrot.w, bSetRotation = true }

		-- Temp Start
		player:SetMaxImagination{imagination = 10}
		player:SetMaxArmor{armor = 10}
		player:SetMaxHealth{health = 10}			
		player:SetHealth{health = 10}
		player:SetArmor{armor = 10}
		player:SetImagination{imagination = 10}
		-- Temp End

		-- remove player from Activity
		self:RemoveActivityUser{ userID = player }

	end

	
	-- reset Vars

	self:SetVar("con.currentRebuilds" , 0 )
	self:SetVar("con.Eggs_Spawned" , 0 )
	self:SetVar("con.Eggs_Total" , 0 )
	self:SetVar("con.Spiders_Spawned" , 0 )
	self:SetVar("con.Spiders_Total" , 0 )
	self:SetVar("con.totalnumOfminios" , 0 )
	self:SetVar("con.ButtonPressed" , 0 )
	self:SetVar("con.RebuildsComplete" , false )
	
	
	
	self:SetVar("Stage", 1 )
	
	

	-- reset Intro Trigger
	local introTrig  = getObjectByName(self, "IntroTrigger")
	
	
	introTrig:NotifyObject{name = "reset"}
	-- Teleport and Reset Spider Boss
	local spider = getObjectByName(self, "bossObj")
	local spawn = self:GetObjectsInGroup{ group = "bossSpawn1" ,ignoreSpawners = true }.objects
	local Markpos = spawn[1]:GetPosition().pos 
	local Markrot = spawn[1]:GetRotation()
	-- Reset Health and Armor
	
	local h = spider:GetMaxHealth().health
	local a = spider:GetMaxArmor().armor
	
	spider:SetHealth{health = h}
	spider:SetArmor{armor = a }
	spider:NotifyObject{name = "SetShield" }
	
	spider:Teleport{pos = Markpos, y = Markrot.y, w = Markrot.w, bSetRotation = true }
	-- Pop Animation State

	spider:SetVar("Phase", 1 )
	spider:ChangeIdleFlags{off = 13}

	-- timer kill clouds
	local obj = self:GetObjectsInGroup{ignoreSpawners=true, group = "shooters" }.objects
	for i = 1, #obj do
		obj[i]:NotifyObject{name = "OFF"}
	end
	
	-- kill rebuilds
	local obj = self:GetObjectsInGroup{ignoreSpawners=true, group = "rebuilds" }.objects
	for i = 1, #obj do
		obj[i]:RebuildReset()
	end
	self:SetVar("con.currentRebuilds", 0 )
	-- Start Timer to respawn Eggs
	GAMEOBJ:GetTimer():AddTimerWithCancel( 2 , "respawnEggs", self )
	
	local gate = self:GetObjectsInGroup{ group = "gate" ,ignoreSpawners = true }.objects
	gate[1]:GoToWaypoint{iPathIndex = 0, bAllowPathingDirectionChange = true}
	
end

function hatchEggs(self)
	
	 for k,v in pairs(Eggs) do 

		local Obj = GAMEOBJ:GetObjectByID(v)
		if (Obj:Exists()) then
			 Obj:NotifyObject{name = "hatch"}
		end

	end

end

function removeMinions(self)

         for k,v in pairs(Minions) do 
         
            local Obj = GAMEOBJ:GetObjectByID(v)
            if (Obj:Exists()) then
                GAMEOBJ:DeleteObject(Obj) 
                Minions[v] = nil
                table.remove(Minions,v)
            end
           
        end
end

function removeEggs(self)


		
         for k,v in pairs(Eggs) do 
         
            local Obj = GAMEOBJ:GetObjectByID(v)
            if (Obj:Exists()) then
                GAMEOBJ:DeleteObject(Obj) 
                Eggs[v] = nil
                table.remove(Eggs,v)
            end
           
        end

end
function spawnEggs(self)

		print("spawn Eggs "..self:GetVar("con.Egg_Markers"))
		for i = 1 , self:GetVar("con.Egg_Markers") do 
	
			local Marker = getObjectByName(self, "Marker_"..i)
				
			if (Marker) then
			
				local Markpos = Marker:GetPosition().pos 
				local Markrot = Marker:GetRotation()		

				RESMGR:LoadObject { objectTemplate = 9313 , x = Markpos.x ,
				y = Markpos.y , z = Markpos.z ,rw = Markrot.w , rx = Markrot.x, ry = Markrot.y  , 
				rz = Markrot.z, owner = self};	
			
			end
			
				
		end	

end

function removeObjects(self,msg)


	if msg.name == "removeEgg" then

         for k,v in pairs(Eggs) do 
         
            local Obj = GAMEOBJ:GetObjectByID(v)
            local EggSearch  = 	msg.ObjIDSender
            
            if (Obj:GetID() == EggSearch:GetID()) then
                GAMEOBJ:DeleteObject(Obj) 
                Eggs[v] = nil
                table.remove(Eggs,v)
            end
           
        end
   end

end
function onNotifyObject(self,msg)



	if msg.name == "addplayer" then
	
		local player = msg.ObjIDSender

		self:AddActivityUser{userID = player}
		
		local objects = self:GetAllActivityUsers{}.objects
		if self:GetVar("con.numberOfplayers") == #objects then
		
	
			StartBossIntro(self,objects)

		end
		
	elseif msg.name == "BossReset" then
	    Reset(self)
		
	elseif msg.name == "BossDead" then
	
		self:SetVar("BossDead", true)
		local obj = self:GetObjectsInGroup{ignoreSpawners=true, group = "rebuilds" }.objects
		for i = 1, #obj do
			obj[i]:RebuildReset()
		end
	elseif msg.name == "intbossGUI" then
	
		local spider = getObjectByName(self, "bossObj")

		SendNetWorkVar( spider , "intGUI"  , "", "", "", "", "", "" )
		
	elseif msg.name == "EndRebuildReady" then
	
		self:SetVar("BossDead" , true )
		self:SetVar("BossRebuld", true )
		
		
	elseif msg.name == "butttonDown" then
	
		
	
	    local pressed = self:GetVar("con.ButtonPressed") + 1
	    self:SetVar("con.ButtonPressed" , pressed )

		if pressed >= self:GetVar("con.ButtonToBePressed") and self:GetVar("con.RebuildsComplete" )then
	
			if not self:GetVar("BossDead") then
				-- Turn on Pre EMP FX's
				fxOffButtons(self)			
				local empSpawns = self:GetObjectsInGroup{ group = "preEMP" ,ignoreSpawners = true }.objects
				for i = 1, #empSpawns do
					DoObjectAction(empSpawns[i], "effect", "beam")

				end


				-- Reset Button Vars
				self:SetVar("con.ButtonPressed" , 0 )
				self:SetVar("con.RebuildsComplete" , false )
				-- Start Phase 1 Timer
				GAMEOBJ:GetTimer():AddTimerWithCancel( static['Button_to_EMP_Blast'] , "Button_to_EMP_Blast", self )
		  elseif self:GetVar("BossRebuld") then
				fxOffButtons(self)

				SendNetWorkVar( self , "Exit"  , "", "", "", "", "", "" )
				-- timer kill clouds
				local obj = self:GetObjectsInGroup{ignoreSpawners=true, group = "shooters" }.objects
				for i = 1, #obj do
					obj[i]:NotifyObject{name = "OFF"}
				end

				-- timer spawn bridge
				GAMEOBJ:GetTimer():AddTimerWithCancel( 0.1, "RemoveClouds", self )	
				GAMEOBJ:GetTimer():AddTimerWithCancel( 4, "TrunonBeams", self )		  
		  		
		  		self:SetVar("con.ButtonPressed", - 100)
		  		self:SetVar("BossRebuld", false)
		  
		  end
			
		end
	elseif msg.name == "removeMinion" then
	
		local sender = msg.ObjIDSender
		
	         for k,v in pairs(Minions) do 
	         
	            local Obj = GAMEOBJ:GetObjectByID(v)
	            if ( Obj:GetID() == sender:GetID() ) then
	            	Minions[v] = nil
	                table.remove(Minions,v)
	                break
	            end
	           
        end
	
	elseif msg.name == "butttonUp" then
		
		self:SetVar("con.ButtonPressed" , self:GetVar("con.ButtonPressed")   - 1 )
		if self:GetVar("con.ButtonPressed") < 0 then
		    self:SetVar("con.ButtonPressed" , 0 )
		end
	elseif msg.name == "removeEgg" then
		
		removeObjects(self,msg)
	
	elseif msg.name == "storeIntroTrigger" then
	
		storeObjectByName(self, "IntroTrigger", msg.ObjIDSender)
	elseif msg.name == "PhaseOneComplete" then
	
			CompletedPhase(self, 1)
			print "PhaseOneComplete"
			
	elseif msg.name == "PhaseTwoComplete" then
	
			CompletedPhase(self, 2)
			print "PhaseTwoComplete"
	elseif msg.name == "PhaseThreeComplete" then	
	
			CompletedPhase(self, 3)
			print "PhaseThreeComplete"
			
	elseif msg.name == "PhaseOneRepeat" then
	
			RepeatPhase(self, 1)
			print "PhaseOneRepeat"
	elseif msg.name == "PhaseTwoRepeat" then
	
			RepeatPhase(self, 2)
			print "PhaseTwoRepeat"
	elseif msg.name == "PhaseThreeRepeat" then	
	
			RepeatPhase(self, 3)
			print "PhaseThreeRepeat"		
	elseif msg.name == "RebuildCompleted" then
	
		self:SetVar("con.RebuildsComplete" , true )	
	
		-- Set all FX on Rebuilds
			local obj = self:GetObjectsInGroup{ignoreSpawners=true, group = "bossButtons" }.objects
			for i = 1, #obj do
				DoObjectAction(obj[i], "effect", "cast")
				print "play FX"
			end
		
		
	-------------------------------------------------------
	-- Spawn Egg on Load ----------------------------------
	-------------------------------------------------------
	elseif msg.name == "Egg_Spawner" then
	
			
	 		self:SetVar("con.Egg_Markers",  self:GetVar("con.Egg_Markers") + 1 )

			local num = self:GetVar("con.Egg_Markers")
			if not self:GetVar("Marker_"..num) then
				storeObjectByName(self, "Marker_"..num, msg.ObjIDSender)
			end
			
			local Markpos = msg.ObjIDSender:GetPosition().pos 
			local Markrot = msg.ObjIDSender:GetRotation()		

			RESMGR:LoadObject { objectTemplate = 9313 , x = Markpos.x ,
			y = Markpos.y , z = Markpos.z ,rw = Markrot.w , rx = Markrot.x, ry = Markrot.y  , 
			rz = Markrot.z, owner = self};	
			
			
	    
	elseif msg.name == "reset" then
	
		
		Reset(self)
	  
	
	elseif msg.name == "removeplayer" then
	

	end
end


function StartBossIntro(self,objects)

	SendNetWorkVar( self , "StartIntro"  , "", "", "", "", "", "" )
	-- Spider Path
	local spider = getObjectByName(self, "bossObj")
		
	spider:SetVar("attached_path", "bosspath1")
	spider:SetVar("attached_path_start", 0 )
	spider:FollowWaypoints{ bUseNewPath = true, newPathName = "bosspath1", newStartingPoint = 0 }
	

	--SendNetWorkVar( spider , "intGUI"  , "", "", "", "", "", "" )
	local gate = self:GetObjectsInGroup{ group = "gate" ,ignoreSpawners = true }.objects
	gate[1]:GoToWaypoint{iPathIndex = 1, bAllowPathingDirectionChange = true}
	
	
end



function onTimerDone(self,msg)

	if msg.name == "SpawnBoss" then

		PhaseIntro(self,msg)
	
		-- Start MaelStormClouds
		local clouds = self:GetObjectsInGroup{ group = "mcloud" ,ignoreSpawners = true }.objects
		for i = 1, #clouds do
			DoObjectAction(clouds[i], "effect", "cloud")
		end
		
		-- Debug Update Phase UI --
		SendNetWorkVar( self , "UIPhase" , "", "", "1", "", "", "" )

		
	
	elseif msg.name == "bosskilled" then
	
        local spider = getObjectByName(self, "bossObj")
        spider:Die{ killerID = self, killType = "VIOLENT" }
	
	elseif msg.name == "AudioBossLand" then
	
		SendNetWorkVar( self , "AudioBossLand"  , "", "", "", "", "", "" )
		
	elseif msg.name == "respawnEggs" then
	
		spawnEggs(self)
		
	elseif msg.name == "RemoveClouds" then
	
			local clouds = self:GetObjectsInGroup{ group = "mcloud" ,ignoreSpawners = true }.objects
			for i = 1, #clouds do
				DoObjectAction(clouds[i], "stopeffects", "cloud")
			end
	elseif msg.name == "TrunonBeams" then
	
			local empSpawns = self:GetObjectsInGroup{ group = "endEMP" ,ignoreSpawners = true }.objects
			for i = 1, #empSpawns do
				
				DoObjectAction(empSpawns[i], "effect", "beam")
				
			end	
			local bridge = self:GetObjectsInGroup{ group = "bridge" ,ignoreSpawners = true }.objects
			for i = 1, #bridge do
				
				local Markpos = bridge[i]:GetPosition().pos 
				local Markrot = bridge[i]:GetRotation()		

				RESMGR:LoadObject { objectTemplate = 3286  , x = Markpos.x ,
				y = Markpos.y , z = Markpos.z ,rw = Markrot.w , rx = Markrot.x, ry = Markrot.y  , 
				rz = Markrot.z, owner = bridge[i] }  
				
			end			
	elseif msg.name == "Button_to_EMP_Blast" then
	
	
		-- Play EMP FX
		local spawn = self:GetObjectsInGroup{ group = "emp" ,ignoreSpawners = true }.objects

		spawn[1]:PlayFXEffect{effectType = "boom"}

		hatchEggs(self)
		local spider = getObjectByName(self, "bossObj")
		spider:NotifyObject{name = "Stunned"}


		-- Stop pre EMP FX
		local empSpawns = self:GetObjectsInGroup{ group = "preEMP" ,ignoreSpawners = true }.objects
		for i = 1, #empSpawns do

		DoObjectAction(empSpawns[i], "stopeffects", "beam")
		end
		
		if spider:GetVar("Phase") == 1 then
			ActivityTimers(self, "StartPhase_1" )
		elseif spider:GetVar("Phase") == 2 then
		   ActivityTimers(self, "StartPhase_2" )
		elseif spider:GetVar("Phase") == 3 then
		   ActivityTimers(self, "StartPhase_3" )

		   
		end
		
	end
	
		
end



function ActivityTimers(self, name )

 	if name == "StartPhase_1" then
 		-- Start Timers 
 		self:ActivityTimerSet{name = "Phase_1",duration = static['Boss_Stunned_Time_Phase_1'],  updateInterval = 1 }
 		
	elseif name == "StartPhase_2"  then
		self:ActivityTimerSet{name = "Phase_2",duration = static['Boss_Stunned_Time_Phase_2'],  updateInterval = 1 }
		
	elseif name == "StartPhase_3"  then
		self:ActivityTimerSet{name = "Phase_3",duration = static['Boss_Stunned_Time_Phase_3'],  updateInterval = 1 }
	end
	
end



function onActivityTimerUpdate(self, msg)

	if msg.name == "Phase_1" then
	
		local time = round(static['Boss_Stunned_Time_Phase_1'] - msg.timeElapsed, 0)
		SendNetWorkVar( self , "UITimer", "", "", tostring(time), "", "", ""  )
		
	elseif msg.name == "Phase_2" then
	
		local time = round(static['Boss_Stunned_Time_Phase_2'] - msg.timeElapsed, 0)
		SendNetWorkVar( self , "UITimer", "", "", tostring(time), "", "", ""  )
		
	elseif msg.name == "Phase_3" then
	
		local time = round(static['Boss_Stunned_Time_Phase_3'] - msg.timeElapsed, 0)
		SendNetWorkVar( self , "UITimer", "", "", tostring(time), "", "", ""  )
		
	end
	
	
end

function onActivityTimerDone(self,msg) 

	if msg.name == "Phase_1" then
	
		self:NotifyObject{name = "PhaseOneRepeat" }
	
	elseif msg.name == "Phase_2" then
	
		self:NotifyObject{name = "PhaseTwoRepeat" }
	
	elseif msg.name == "Phase_3" then
	
		self:NotifyObject{name = "PhaseThreeRepeat" }
		
	end


end


function CompletedPhase(self, phase)

		
		SendNetWorkVar( self , "UITimer", "", "", "done", "", "", ""  )
		local spider = getObjectByName(self, "bossObj")
		
		spider:ChangeIdleFlags{off = 13}
		spider:SetArmor{armor = 40 }
		SendNetWorkVar( spider , "UITimer", "", "", "", "", "", ""  )
		
		-- kill rebuilds
		local obj = self:GetObjectsInGroup{ignoreSpawners=true, group = "rebuilds" }.objects
		for i = 1, #obj do
			obj[i]:RebuildReset()
		end
		self:SetVar("con.currentRebuilds", 0 )
		self:SetVar("con.RebuildsComplete", false )
		-- Respawn Eggs 
		spawnEggs(self)
		
		spider:NotifyObject{name = "SetShield" }
		
		if phase == 1 then
		
			spider:SetVar("Phase", 2 )
			SendNetWorkVar( self , "UIPhase", "", "", "2", "", "", ""  )
			
		elseif phase == 2 then
		
			spider:SetVar("Phase", 3 )
			SendNetWorkVar( self , "UIPhase", "", "", "3", "", "", ""  )
			
		local obj = self:GetObjectsInGroup{ignoreSpawners=true, group = "shooters" }.objects
		for i = 1, #obj do
			obj[i]:NotifyObject{name = "ON"}
		end
			
		elseif phase == 3 then
		
			spider:SetVar("Phase", 4 )
			SendNetWorkVar( self , "UIPhase", "", "", "4", "", "", ""  )
		end



end
function RepeatPhase(self, phase)

		
		SendNetWorkVar( self , "UITimer", "", "", "done", "", "", ""  )
		
		local spider = getObjectByName(self, "bossObj")
		
		spider:ChangeIdleFlags{off = 13}
		spider:SetArmor{armor = 40 }
		SendNetWorkVar( spider , "resetArmore", "", "", "", "", "", ""  )
		-- kill rebuilds
		local obj = self:GetObjectsInGroup{ignoreSpawners=true, group = "rebuilds" }.objects
		for i = 1, #obj do
			obj[i]:RebuildReset()
		end
		self:SetVar("con.currentRebuilds", 0 )
		self:SetVar("con.RebuildsComplete", false )
		-- Respawn Eggs 
		spawnEggs(self)
		
		spider:NotifyObject{name = "SetShield" }
		
		if phase == 1 then
		
			spider:SetVar("Phase", 1 )	
			SendNetWorkVar( self , "UIPhase", "", "", "Repeat_1", "", "", ""  )
			
		elseif phase == 2 then
		
			spider:SetVar("Phase", 2 )
			SendNetWorkVar( self , "UIPhase", "", "", "Repeat_2", "", "", ""  )
			
		elseif phase == 3 then
		
			spider:SetVar("Phase", 3 )
			SendNetWorkVar( self , "UIPhase", "", "", "Repeat_3", "", "", ""  )
			
		end



end


function fxOffButtons(self)

			local obj = self:GetObjectsInGroup{ignoreSpawners=true, group = "bossButtons" }.objects
			for i = 1, #obj do
				DoObjectAction(obj[i], "stopeffects", "cast")
			
			end


end
