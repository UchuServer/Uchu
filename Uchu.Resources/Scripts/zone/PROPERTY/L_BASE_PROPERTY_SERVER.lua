----------------------------------------------------------------
--	Base Server script for property pushback. 
--	this script is required from a level specific script
--	this script will only work with the client script as well
-- updated mrb... 9/7/10 - added brickLinkMissionID updateMissionTask
-- updated brandi.. 9/10/10 - put variable in to keep from getting in infinate loop, and added function to keep prints from printing externally
-- updated abeechler.. 7/8/11 - Property Guard function created for initial spawns
----------------------------------------------------------------
local OwnerChecked = false
local GUIDMaelstrom = "{7881e0a1-ef6d-420c-8040-f59994aa3357}"  -- ambient sounds for when the Maelstrom is on
local GUIDPeaceful	= "{c5725665-58d0-465f-9e11-aeb1d21842ba}" -- happy ambient sounds when no maestrom is preset
----------------------------------------------------------------
-- Define empty tables that will be set from the level specific script
----------------------------------------------------------------
local Group = {}
local Spawners = {}
local Flags = {}


----------------------------------------------------------------
-- variables passed of the level specific script that are used throughout the base script
----------------------------------------------------------------
function setGameVariables(passedGroups,passedSpawners,passedFlags)
	if passedGroups then
		Group = passedGroups
	end
	if passedSpawners then
		Spawners = passedSpawners
	end
	if passedFlags then
		Flags = passedFlags
	end
end


----------------------------------------------------------------
-- 
----------------------------------------------------------------
function checkForOwner(self)

	-- get the property plaque by group set in happy flower, the property plaque is coded to share certain information with LUA
    local propertyPlaques = self:GetObjectsInGroup{ group = Group.PropertyPlaque, ignoreSpawners = true }.objects
    if not propertyPlaques or #propertyPlaques == 0 then 
        GAMEOBJ:GetTimer():AddTimerWithCancel( 0.5, "runPlayerLoadedAgain", self ) 
        return
    end
	
	-- table for property data to be filled out by the for loop below
    for i = 1, table.maxn(propertyPlaques) do
		-- check to see if there is more than on object in the property plaque group, if there is, it can seriously break stuff
        if i > 1 then
			WarningPrint(self,"WARNING --  YOU HAVE TO MANY PROPERTY PLAQUES IN THIS LEVEL. Please make sure you only have 1!")
			break
		end
		
		local propertyData = propertyPlaques[i]:PropertyGetState{}
        self:SetVar("PropertyOwner",propertyData.ownerID:GetID())
        OwnerChecked = true
    end
end

----------------------------------------------------------------
-- 
----------------------------------------------------------------
function onFireEvent(self, msg)
	-- Receive the sending object ID and the message to parse
	local eventType = msg.args
	local sendObj = msg.senderID
	
	-- Missing a valid event type?
	if not eventType then return end
	
	if eventType == "CheckForPropertyOwner" then
		-- An object has queried us for the ID of the property owner,
		-- return it to the sender
		local propertyOwner = self:GetVar("PropertyOwner") or 0
		sendObj:SetNetworkVar("PropertyOwnerID", propertyOwner)
	end
end

----------------------------------------------------------------
-- 
----------------------------------------------------------------
function basePlayerLoaded(self, msg)	
	if  OwnerChecked == false then  -- only check for owner when the first player enters property
		checkForOwner(self)
    end
	local propertyOwner = self:GetVar("PropertyOwner")
	-- returns true or false if the property is rented or not
	local rented = false
	if propertyOwner ~= 0 then  
	    rented = true
	end
	local player = msg.playerID
	
	self:SetVar("IsInternal", player:GetVersioningInfo().bIsInternal)
	-- if the property is rented, delete all the objects that may be on the property
	if rented == true then
	
		-- kill the tornado effect object
		if not self:GetVar("FXObjectGone") then
		    GAMEOBJ:GetTimer():AddTimerWithCancel( 1, "killFXObject", self ) 
		end
		player:Play2DAmbientSound{m_NDAudioEventGUID = GUIDPeaceful}
		
		-- spawn the property safe object
		ActivateSpawner(self,Spawners.PropObjs)
		
		-- tell the client script that the property is rented and who the renter is
		self:SetNetworkVar("renter",propertyOwner)

		--  check to see if the player standing on the property is the person who owns the property
		if player:GetID() ~= propertyOwner then
			return
		end
			
	-- if the property hasnt been rented, then assume that this is the players property to rent (if this is broken, it is through code)
	else
		-- flag for if the player has defeated the maelstrom property yet
		local defeatedflag = player:GetFlag{ iFlagID = Flags.defeatedProperty }.bFlag
		-- tell the client script that the property is not rented
		self:SetNetworkVar("unclaimed",true)
		
		self:SetVar("playerID",player:GetID())
		
		-- check to see if the player has defeated the maelstrom before (they could defeat it and leave without renting it)
		if defeatedflag == false then
			-- custom function that starts all teh maelstrom 
			StartMaelstrom(self,player)
			SpawnSpots(self)
			self:SetVar("playerID",player:GetID())
			player:Play2DAmbientSound{m_NDAudioEventGUID = GUIDMaelstrom}
					
		else
			player:Play2DAmbientSound{m_NDAudioEventGUID = GUIDPeaceful}
			GAMEOBJ:GetTimer():AddTimerWithCancel( 1, "killFXObject", self ) 
		end
		
	end
	
	-- spawn the Property Guard?
    propGuardCheck(self, msg)
	
end

----------------------------------------------------------------
-- Utility function used to determine whether to spawn the 
-- Property Guard based on mission status
----------------------------------------------------------------
function propGuardCheck(self, msg)
    -- mission state
    local mState = msg.playerID:GetMissionState{missionID = Flags.guardMission}.missionState
    
    -- do we meet the requirements?
    if mState ~= 8 then
		ActivateSpawner(self,Spawners.PropMG) -- spawn guard
	end
end

----------------------------------------------------------------
-- returned from the server when the property is rented
----------------------------------------------------------------
function baseZonePropertyRented(self, msg)
	
	-- show cinematic of the property bounds being turned on
	GAMEOBJ:GetZoneControlID():NotifyClientObject{ name = "PlayCinematic", paramStr = "ShowProperty" }
	--msg.playerID:PlayCinematic { pathName = "ShowProperty" }
	-- delay the property border from being animatated until the camera is looking at it
	GAMEOBJ:GetTimer():AddTimerWithCancel( 2, "BoundsVisOn",self )
	-- delay deleting the guard until the camera isnt looking at him
	--GAMEOBJ:GetTimer():AddTimerWithCancel( 3, "GuardFlyAway",self )
	
	self:SetVar("PropertyOwner",msg.playerID:GetID())
	
	if Flags.brickLinkMissionID then
        -- get the property plaque by group set in happy flower, the property plaque is coded to share certain information with LUA
        local propertyPlaques = self:GetObjectsInGroup{ group = Group.PropertyPlaque, ignoreSpawners = true }.objects
        
        for k,v in ipairs(propertyPlaques) do 
            msg.playerID:UpdateMissionTask{taskType = "complete", value = Flags.brickLinkMissionID, value2 = 1, target = v}
            break
        end
	end	
	
	-- spawn the property safe object
	ActivateSpawner(self,Spawners.PropObjs)
end 

----------------------------------------------------------------
-- message sent when a model is placed on the property
----------------------------------------------------------------
function baseZonePropertyModelPlaced(self, msg)
	-- check the flag to see if the player has placed a model before, if they haven't deleted the maelstrom spots and set the flag to say they have placed a model
	if msg.playerID:GetFlag{ iFlagID = Flags.placedModel }.bFlag == false then	
		msg.playerID:SetFlag{iFlagID = Flags.placedModel, bFlag=true}	
	end	
end

----------------------------------------------------------------
-- deletes the maelstrom damaging clouds
----------------------------------------------------------------
function KillClouds(self)

	DeactivateSpawner(self,Spawners.DamageFX)
	DestroySpawner(self,Spawners.DamageFX)
end

----------------------------------------------------------------
-- activates the spawner for the maelstrom spots
----------------------------------------------------------------
function SpawnSpots(self)
	ActivateSpawner(self,Spawners.FXSpots)
end

----------------------------------------------------------------
-- deactive the spawner for the maelstrom spots and then delete them
----------------------------------------------------------------
function KillSpots(self)
	DeactivateSpawner(self,Spawners.FXSpots)
	local spots = self:GetObjectsInGroup{ group = Group.Spots, ignoreSpawners = true }.objects
	ObjectRequestDie(self,spots)
end

----------------------------------------------------------------
-- turns all the maelstrom on if the player hasn't defeated the maelstrom for this property yet
----------------------------------------------------------------
function StartMaelstrom(self,player)
	-- pause the behaviors on the property
	--local plaque = self:GetObjectsInGroup{ group = Group.PropertyPlaque, ignoreSpawners = true }.objects[1]
	--plaque:TogglePropertyBehaviors{ bPaused = true}

	-- activate all the enemy spawners
	for k,v in ipairs(Spawners.Enemy) do
	    ActivateSpawner(self,v)
	end
	
	if Spawners.BehaviorObjs then
		for k,v in ipairs(Spawners.BehaviorObjs) do
			ActivateSpawner(self,v)
		end
	end
	
	--activate the damaging clouds spawner
	ActivateSpawner(self,Spawners.DamageFX)   
          
	-- activeate the spawner for the generator
	ActivateSpawner(self,Spawners.Generator)
	ActivateSpawner(self,Spawners.GeneratorFX)
	ActivateSpawner(self,Spawners.FXManager)	
	ActivateSpawner(self,Spawners.ImagOrb)
	ActivateSpawner(self,Spawners.Smashables)
	
    
	-- make sure the spawner for the claimmarker is off
	DeactivateSpawner(self,Spawners.ClaimMarker)
    ResetSpawner(self,Spawners.ClaimMarker)
	
	if Spawners.AmbientFX then
        for k,v in ipairs(Spawners.AmbientFX) do
            DeactivateSpawner(self,v)
            DestroySpawner(self,v)
            ResetSpawner(self,v)
        end
    end
    
    startTornadoFX(self)
    
	-- notify the client to change the env settings 
	self:NotifyClientObject{ name = "maelstromSkyOn", rerouteID = player }
	-- have the generator tell the script when it dies
	sendNotification(self,Group.Generator,"Die","startGenerator")
	sendNotification(self,Group.ImagOrb,"CollisionPhantom","startOrb")
	
end

----------------------------------------------------------------
-- Runs the tornado fx, including polling initiation for existence verification
----------------------------------------------------------------
function startTornadoFX(self)
    -- find the fx manager and turn on the tornado and the clouds	
	local fxObjs = self:GetObjectsInGroup{group = Group.FXManager, ignoreSpawners = true}.objects
	local fx = false
	-- Iterate through the group and catch the first valid object
    for k,obj in ipairs(fxObjs) do
		if obj:Exists() then
			fx = obj
		    break
	    end
	end
	
	if fx then
		fx:PlayFXEffect{name = "TornadoDebris", effectType = "debrisOn"}
		fx:PlayFXEffect{name = "TornadoVortex", effectType = "VortexOn"}
		fx:PlayFXEffect{name = "silhouette", effectType = "onSilhouette"}
	else
		WarningPrint(self,"Warning: No object found in the group "..Group.FXManager)
		GAMEOBJ:GetTimer():AddTimerWithCancel(0.1, "pollTornadoFX", self)
	end
	
end

----------------------------------------------------------------
--  called when the claim marker and the generator die
----------------------------------------------------------------
function baseNotifyDie(self,other,msg)
	-- the object that sent the notification
	local deadObj = other:GetLOT().objtemplate
	-- the generator died
	if deadObj == Flags.generatorID then
		-- spawn the claimmarker quickbuild and tell it to notify when it is rebuilt
		ActivateSpawner(self,Spawners.ClaimMarker)
		sendNotification(self,Group.ClaimMarker,"RebuildComplete","startQuickbuild")
		-- stop the enemies from spawning anymore      
		for k,v in ipairs(Spawners.Enemy) do
			DeactivateSpawner(self,v)
        end
        -- stop the generator spawner from spawning again
		DeactivateSpawner(self,Spawners.Generator)          
	end
end

----------------------------------------------------------------
-- the orb notifies when it have been collided with
----------------------------------------------------------------
function baseNotifyCollisionPhantom(self,other,msg)
	if other:GetLOT().objtemplate == Flags.orbID then
	    if msg.objectID:BelongsToFaction{factionID = 1}.bIsInFaction then
            KillClouds(self)
            --deactivate the generator and claimmarker spawners
            DeactivateSpawner(self,Spawners.Generator)
            --DeactivateSpawner(self,Spawners.GeneratorFX)
                    
            --DeactivateSpawner(self,Spawners.ClaimMarker)
            local objNotify = self:GetObjectsInGroup{ group = Group.ImagOrb, ignoreSpawners = true }.objects[1]
            if objNotify then
                self:SendLuaNotificationCancel{requestTarget= objNotify, messageName="CollisionPhantom"}
            end
            for k,v in ipairs(Spawners.Enemy) do
                DeactivateSpawner(self,v)
            end
            DestroySpawner(self,Spawners.GeneratorFX)
            local player = GAMEOBJ:GetObjectByID(self:GetVar("playerID"))
            -- start the camera that shows the maelstrom dying
			GAMEOBJ:GetZoneControlID():NotifyClientObject{ name = "PlayCinematic", paramStr = "DestroyMaelstrom" }
            --player:PlayCinematic { pathName = "DestroyMaelstrom" }
            -- set the flage that player defeated the maelstrom on this property
            player:SetFlag{iFlagID = Flags.defeatedProperty, bFlag=true}
            -- start the timer for the next part	
            GAMEOBJ:GetTimer():AddTimerWithCancel( 0.5, "tornadoOff",self )
            GAMEOBJ:GetTimer():AddTimerWithCancel( 0.7, "KillMarker", self ) 
        end
	end
end
----------------------------------------------------------------
-- the quickbuild spawned from the generator tells this when it is done building
----------------------------------------------------------------
function baseNotifyRebuildComplete(self,other,msg)
	if msg.rebuildID:GetLOT().objtemplate == Flags.behavQBID then
		-- the quickbuild has behaviors on it that are looking for this password before they will start
		self:ChatMessageForZone{strMessage=Flags.password, senderID = 0 }
	end
end

----------------------------------------------------------------
-- when the player leaves the zone, destroy everything to reset the map
----------------------------------------------------------------
function basePlayerExit(self,other,msg)
	if self:GetNetworkVar("unclaimed") == true then
		if msg.playerID:GetID() == self:GetVar("playerID") then
			for k,v in pairs(Spawners) do
				if type(v) == "table" then
					for y,z in ipairs(v) do
						DeactivateSpawner(self,z)
						DestroySpawner(self,z)
						ResetSpawner(self,z)
					end
				else
					DeactivateSpawner(self,v)
					DestroySpawner(self,v)
					ResetSpawner(self,v)
				end
			end
		end
	end
end
----------------------------------------------------------------
-- tell the generator to notify the script when it dies
----------------------------------------------------------------
function sendNotification(self,obj,msgName,timer)
	-- grab the generator by its group set in happy flower, assume there is only one generator in the group
	local objNotify = self:GetObjectsInGroup{ group = obj, ignoreSpawners = true }.objects[1]
	-- make sure the script got an object
	if objNotify then
		-- send the message to the generator object to tell the script when it dies
		self:SendLuaNotificationRequest{requestTarget= objNotify, messageName=msgName}
	else 
		-- if the generator doesn't exist yet, try again
		GAMEOBJ:GetTimer():AddTimerWithCancel( 0.5, timer, self ) 
		
	end
	
end


----------------------------------------------------------------
-- deletes the guard
----------------------------------------------------------------
function KillGuard(self)
	local Guard = self:GetObjectsInGroup{ group = Group.Guard, ignoreSpawners = true }.objects
	ObjectRequestDie(self,Guard)
	DeactivateSpawner(self,Spawners.PropMG)
end


----------------------------------------------------------------
-- requests dies for objects
----------------------------------------------------------------
function ObjectRequestDie(self,objTable)
	if objTable then
		for k,v in ipairs(objTable) do
			v:RequestDie{ killerID = self, killtype = "VIOLENT" }
		end
	end
end

----------------------------------------------------------------
-- destroys spawners
----------------------------------------------------------------
function DestroySpawner(self,spawnerName)
	local spawnerID = LEVEL:GetSpawnerByName(spawnerName)
	if spawnerID then
		spawnerID:SpawnerDestroyObjects()
	else
		WarningPrint(self,'WARNING: No Spawner found for spawner named ' .. spawnerName)
	end

end
----------------------------------------------------------------
-- activates spawners
----------------------------------------------------------------
function ActivateSpawner(self,spawnerName)
	local spawnerID = LEVEL:GetSpawnerByName(spawnerName)
	if spawnerID then
		spawnerID:SpawnerActivate()
	else
		WarningPrint(self,'WARNING: No Spawner found for spawner named ' .. spawnerName)
	end
end

----------------------------------------------------------------
-- deactivates spawners
----------------------------------------------------------------
function DeactivateSpawner(self,spawnerName)
	local spawnerID = LEVEL:GetSpawnerByName(spawnerName)
	if spawnerID then
		spawnerID:SpawnerDeactivate()
	else
		WarningPrint(self,'WARNING: No Spawner found for spawner named ' .. spawnerName)
	end
end

----------------------------------------------------------------
-- resets spawners
----------------------------------------------------------------
function ResetSpawner(self,spawnerName)
	local spawnerID = LEVEL:GetSpawnerByName(spawnerName)
	if spawnerID then
		spawnerID:SpawnerReset()
	else
		WarningPrint(self,'WARNING: No Spawner found for spawner named ' .. spawnerName)
	end
end

----------------------------------------------------------------
-- print custom function to keep prints from showing up externally
----------------------------------------------------------------
function WarningPrint(self,msg)
    if self:GetVar("IsInternal") then       
        print(self,msg)
    end
end
    

----------------------------------------------------------------
-- called when a timer is done
----------------------------------------------------------------
function baseTimerDone(self,msg)

	if msg.name == "startGenerator" then
		-- tell the generator to notify the script when it dies
		sendNotification(self,Group.Generator,"Die","startGenerator")
		
	elseif msg.name == "startOrb" then
		-- tell the generator to notify the script when it dies
	    sendNotification(self,Group.ImagOrb,"CollisionPhantom","startOrb")
	    
	elseif msg.name == "startQuickbuild" then
		-- tell the generator to notify the script when it dies
	    sendNotification(self,Group.ClaimMarker,"RebuildComplete","startQuickbuild")
		
	elseif msg.name == "GuardFlyAway" then
		-- the guard is "flying" away, delete him and turn the boundry animation on
		if LEVEL:GetCurrentZoneID() ~= 1150 then
			local Guard = self:GetObjectsInGroup{ group = Group.Guard, ignoreSpawners = true }.objects[1]
			GAMEOBJ:GetZoneControlID():NotifyClientObject{ name = "GuardChat", paramObj = Guard }
			GAMEOBJ:GetTimer():AddTimerWithCancel( 5, "KillGuard", self ) 
		end
		-- tell the client script to play the animation on the boundry asset
		
	elseif msg.name == "KillGuard" then
		KillGuard(self)
    elseif msg.name == "tornadoOff" then
		-- finds the fx manager
		local fx = self:GetObjectsInGroup{ group = Group.FXManager, ignoreSpawners = true }.objects[1]
		-- turn the tornado and vortex fx off
		if fx then
			fx:StopFXEffect{name = "TornadoDebris"}
			fx:StopFXEffect{name = "TornadoVortex"} 
			fx:StopFXEffect{name = "silhouette"}
		else
			WarningPrint(self,"Warning: No object found in the group "..Group.FXManager)
		end
		GAMEOBJ:GetTimer():AddTimerWithCancel( 2, "ShowClearEffects", self ) 
	elseif msg.name == "ShowClearEffects" then
		-- finds the fx manager
		local fx = self:GetObjectsInGroup{ group = Group.FXManager, ignoreSpawners = true }.objects[1]
		-- play the light in the center of the property to show the maelstrom being defeated
		if fx then
			fx:PlayFXEffect{name = "beam", effectType = "beamOn"}
		else
			WarningPrint(self,"Warning: No object found in the group "..Group.FXManager)
		end
		GAMEOBJ:GetTimer():AddTimerWithCancel( 2, "killStrombies", self ) 
		GAMEOBJ:GetTimer():AddTimerWithCancel( 1.5, "turnSkyOff", self ) 
		GAMEOBJ:GetTimer():AddTimerWithCancel( 8, "killFXObject", self ) 

	elseif msg.name == "turnSkyOff" then
		GAMEOBJ:GetZoneControlID():NotifyClientObject{ name = "SkyOff" }
		
	elseif msg.name == "killStrombies" then
		-- kill all the enemies
		local enemies = self:GetObjectsInGroup{ group = Group.Enemies }.objects
		ObjectRequestDie(self,enemies)
		-- finds the fx manager
		DeactivateSpawner(self,Spawners.Smashables)
		DestroySpawner(self,Spawners.Smashables)
		KillSpots(self)
		local player = GAMEOBJ:GetObjectByID(self:GetVar("playerID"))
		player:Stop2DAmbientSound{m_NDAudioEventGUID = GUIDMaelstrom } -- kill the maelstrom windy sound
		player:Play2DAmbientSound{m_NDAudioEventGUID = GUIDPeaceful}-- start the happy bird sounds
		-- tell the client script to turn on the normal property env settings
		GAMEOBJ:GetTimer():AddTimerWithCancel( 5, "ShowVendor", self ) 
		
	elseif msg.name == "KillMarker" then
		-- kill the claimmarker
		DeactivateSpawner(self,Spawners.ClaimMarker)
		DestroySpawner(self,Spawners.ClaimMarker)
		if Spawners.BehaviorObjs then
			for k,v in ipairs(Spawners.BehaviorObjs) do
				DeactivateSpawner(self,v)
				DestroySpawner(self,v)
			end
		end
		--DestroySpawner(self,Spawners.ClaimMarker)
		local orb = self:GetObjectsInGroup{ group = Group.ImagOrb }.objects
		ObjectRequestDie(self,orb)
		GAMEOBJ:GetTimer():AddTimerWithCancel( 1, "ShowVendor", self )
		
	elseif msg.name == "ShowVendor" then
		-- tell the client script to show the vendor
		GAMEOBJ:GetZoneControlID():NotifyClientObject{ name = "vendorOn" }
		if Spawners.AmbientFX then
            for k,v in ipairs(Spawners.AmbientFX) do
                ActivateSpawner(self,v)
            end
        end
		
	elseif msg.name == "BoundsVisOn" then
		-- tell the client script to turn the boundry on
		--GAMEOBJ:GetZoneControlID():NotifyClientObject{name = "boundsOn" }
		GAMEOBJ:GetZoneControlID():NotifyClientObject{name = "boundsAnim" }
	elseif msg.name == "runPlayerLoadedAgain" then
		checkForOwner(self)
		
    elseif msg.name == "pollTornadoFX" then
        startTornadoFX(self)
	
	elseif msg.name == "killFXObject" then

		local fxT = self:GetObjectsInGroup{ group = Group.FXManager, ignoreSpawners = true }.objects
		local fx = fxT[1]
		--stop the light fx
		if fx then
			fx:StopFXEffect{name = "beam"}
			-- kill the fx object
			DestroySpawner(self,Spawners.FXManager)
			self:SetVar("FXObjectGone", true)
		else
		    WarningPrint(self,"Warning: No object found in the group "..Group.FXManager)
			GAMEOBJ:GetTimer():AddTimerWithCancel( 1, "killFXObject", self )
		end
	end	
end