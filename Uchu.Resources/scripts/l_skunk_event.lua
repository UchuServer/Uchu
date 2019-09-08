--///////////////////////////////////////////////////////////////////////////////////////
--//            YouReeka Skunk Event Zone Script
--///////////////////////////////////////////////////////////////////////////////////////

--------------------------------------------------------------
-- Includes
--------------------------------------------------------------
require('o_mis')
require('c_Zorillo')


-- stores NPCs that are modified during event
SPAWNEDNPCS = {}

-- stores Skunks spawned during the event
INVASION_SKUNKS = {}

-- stores Hazmat NPCs spawned during the event
HAZMAT_NPCS = {}

-- stores all loaded spouts
SPOUTS = {}

-- stores all bubble blowers
BUBBLE_BLOWERS = {}

-- stores Stink Clouds spawned during the event
INVASION_STINK_CLOUDS = {}
INVASION_STINK_CLOUD_WAYPOINTS = {}

-- stores players involved in the invasion
INVASION_PLAYERS = {}

-- stores all flowers
FLOWERS = {}


--------------------------------------------------------------
-- check to see if a string starts with a substring
--------------------------------------------------------------
function string.starts(String,Start)
   return string.sub(String,1,string.len(Start))==Start
end


--------------------------------------------------------------
-- returns true if invasion is in active state
--------------------------------------------------------------
function IsInvasionActive(self)

    -- these are Non Invasion states
    local state = GetZoneState(self)
    if (state == CONSTANTS["ZONE_STATE_NO_INFO"] or 
        state == CONSTANTS["ZONE_STATE_NO_INVASION"] or 
        state == CONSTANTS["ZONE_STATE_DONE_TRANSITION"]) then
        return false
    else
        -- otherwise state means we are in the invasion
        return true
    end
end

--------------------------------------------------------------
-- Get the state of the zone
--------------------------------------------------------------
function GetZoneState(self)
    return self:GetVar("ZoneState")
end


--------------------------------------------------------------
-- Set the state of the zone
--------------------------------------------------------------
function SetZoneState(self, state)

    print("Setting Server Zone State to " .. state)
    
    -- get current state
    local prevState = GetZoneState(self)
    
    self:SetVar("ZoneState", state)

    -- notify all clients about the state change
    if (prevState and prevState ~= state) then

        -- do our own actions
        if (state == CONSTANTS["ZONE_STATE_NO_INVASION"]) then
            DoNoInvasionStateActions(self)
        elseif (state == CONSTANTS["ZONE_STATE_TRANSITION"]) then
            DoTransitionStateActions(self)
        elseif (state == CONSTANTS["ZONE_STATE_HIGH_ALERT"]) then
            DoHighAlertStateActions(self)
        elseif (state == CONSTANTS["ZONE_STATE_MEDIUM_ALERT"]) then
            DoMediumAlertStateActions(self)
        elseif (state == CONSTANTS["ZONE_STATE_LOW_ALERT"]) then
            DoLowAlertStateActions(self)
        elseif (state == CONSTANTS["ZONE_STATE_DONE_TRANSITION"]) then
            DoDoneTransitionActions(self)
        end

        -- inform clients about state
        GAMEOBJ:GetZoneControlID():NotifyClientZoneObject{ name="zone_state_change", param1 = state }
        
    end
    
end    


--------------------------------------------------------------
-- do actions associated with No Invasion state
--------------------------------------------------------------
function DoNoInvasionStateActions(self)
    
    ResetTotalCleanPoints(self)
    
    -- update spouts
    SendStateToSpouts(self)

    -- update bubble blowers
    SendStateToBubbleBlowers(self)
    
	GAMEOBJ:GetTimer():AddTimerWithCancel( CONSTANTS["PEACE_TIME_DURATION"], "startEventTimer", self )
    
end


--------------------------------------------------------------
-- do actions associated with Transition state
--------------------------------------------------------------
function DoTransitionStateActions(self)

    -- update spouts
    SendStateToSpouts(self)

    -- update bubble blowers
    SendStateToBubbleBlowers(self)

    ResetTotalCleanPoints(self)
    
	-- start a timer to panic the NPCs
	GAMEOBJ:GetTimer():AddTimerWithCancel(CONSTANTS["EARTHQUAKE_DURATION"], "DoPanicNPCs", self )
	
    -- start a timer to spawn skunks and change states
	GAMEOBJ:GetTimer():AddTimerWithCancel(CONSTANTS["SKUNK_SPAWN_TIMING"], "SkunksSpawning", self )

    -- start a timer to spawn skunks and change states
	GAMEOBJ:GetTimer():AddTimerWithCancel(CONSTANTS["SKUNK_SPAWN_TIMING"], "StinkCloudsSpawning", self )

    -- start a timer to trigger hazmat van
	GAMEOBJ:GetTimer():AddTimerWithCancel(CONSTANTS["HAZMAT_VAN_TIMING"], "HazmatVanTimer", self )
 
   -- start a timer to trigger window washer
	GAMEOBJ:GetTimer():AddTimerWithCancel(CONSTANTS["POLE_SLIDE_TIMING"], "PoleSlideTimer", self )

    -- start a timer to end the transition state
	GAMEOBJ:GetTimer():AddTimerWithCancel(CONSTANTS["INVASION_TRANSITION_DURATION"], "EndInvasionTransition", self )
    
    
end


--------------------------------------------------------------
-- do actions associated with High Alert state
--------------------------------------------------------------
function DoHighAlertStateActions(self)
	GAMEOBJ:GetTimer():AddTimerWithCancel(CONSTANTS["MAX_INVASION_DURATION"], "MaxInvasionTimer", self )
end


--------------------------------------------------------------
-- do actions associated with Medium Alert state
--------------------------------------------------------------
function DoMediumAlertStateActions(self)
end


--------------------------------------------------------------
-- do actions associated with Low Alert state
--------------------------------------------------------------
function DoLowAlertStateActions(self)
end


--------------------------------------------------------------
-- do actions associated with done transition
--------------------------------------------------------------
function DoDoneTransitionActions(self)

	GAMEOBJ:GetTimer():CancelTimer("MaxInvasionTimer", self)

    -- reward for total points
    RewardPlayers(self)
    ResetTotalCleanPoints(self)

    -- animate the van to end
    local anim_time = animateVan(self, "end")
    
    -- set a callback to trigger van rebuild spawn after animation is over
    if (anim_time and tonumber(anim_time) > 0 ) then
        GAMEOBJ:GetTimer():AddTimerWithCancel(anim_time, "HazmatVanEndDone", self )
    end    	
    
    -- set npcs back to idle
    IdleNPCs(self)
    
    -- remove skunks
    killSkunks(self)
    
    -- remove stink Clouds
    killStinkClouds(self)
    
    -- remove the Hazmat NPCs
    killHazmatNPCs(self)

    -- start a timer to end the transition state
	GAMEOBJ:GetTimer():AddTimerWithCancel(CONSTANTS["DONE_TRANSITION_DURATION"], "EndDoneTransition", self )

end


--------------------------------------------------------------
-- send spout notifications based on state
--------------------------------------------------------------
function SendStateToSpouts(self)

    -- notify lamps of state
	for spoutID = 1, #SPOUTS do
        local spout = GAMEOBJ:GetObjectByID(SPOUTS[spoutID])
        spout:NotifyObject{ name="zone_state_change", param1 = GetZoneState(self) }
	end

end


--------------------------------------------------------------
-- send bubble blower notifications based on state
--------------------------------------------------------------
function SendStateToBubbleBlowers(self)

    -- notify lamps of state
	for bbID = 1, #BUBBLE_BLOWERS do
        local bb = GAMEOBJ:GetObjectByID(BUBBLE_BLOWERS[bbID])
        bb:NotifyObject{ name="zone_state_change", param1 = GetZoneState(self) }
	end

end


--------------------------------------------------------------
-- Get total clean points
--------------------------------------------------------------
function GetTotalCleanPoints(self)
    return self:GetVar("TotalCleanPoints")
end


--------------------------------------------------------------
-- Add to the total clean points, check to see if a state
-- change is needed and make it
-- returns true if the event ends with the increment
--------------------------------------------------------------
function IncrementTotalCleanPoints(self, inc)

    if (not IsInvasionActive(self)) then
        print ("No points added: event not active")
        return
    end
    
    local points = GetTotalCleanPoints(self)
    points = points + inc
    self:SetVar("TotalCleanPoints", points)
    
    print ("CleanPoints: " .. points)
    
    -- check states and see if we need to move on
    if (GetZoneState(self) == CONSTANTS["ZONE_STATE_HIGH_ALERT"] and points >= CONSTANTS["CLEANING_POINTS_MEDIUM"]) then
        SetZoneState(self, CONSTANTS["ZONE_STATE_MEDIUM_ALERT"])
    elseif (GetZoneState(self) == CONSTANTS["ZONE_STATE_MEDIUM_ALERT"] and points >= CONSTANTS["CLEANING_POINTS_LOW"]) then
        SetZoneState(self, CONSTANTS["ZONE_STATE_LOW_ALERT"])
    elseif (GetZoneState(self) == CONSTANTS["ZONE_STATE_LOW_ALERT"] and points >= CONSTANTS["CLEANING_POINTS_TOTAL"]) then
        SetZoneState(self, CONSTANTS["ZONE_STATE_DONE_TRANSITION"])
        return true
    end
    
    return false
    
end


--------------------------------------------------------------
-- Reset the total clean points
--------------------------------------------------------------
function ResetTotalCleanPoints(self)
    self:SetVar("TotalCleanPoints", 0)
end


--------------------------------------------------------------
-- Init zone variables
--------------------------------------------------------------
function InitZoneVars(self)

    -- store the number of waypoints in the Stink Cloud Path
	local pathMsg = LEVEL:GetPathWaypoints(CONSTANTS["STINK_CLOUD_PATH"])
	
	if (tostring(type(pathMsg)) == "table") then
	    self:SetVar("NumStinkCloudSpawnPoints", tonumber(#pathMsg))
	end	
	
    SetZoneState(self, CONSTANTS["ZONE_STATE_NO_INVASION"])
    ResetTotalCleanPoints(self)

end


--------------------------------------------------------------
-- Object starting up
--------------------------------------------------------------
function onStartup(self)

    -- init the zone vars
    InitZoneVars(self)

    -- spawn in the garage van
    spawnGarageVan(self)

    -- start a timer to spawn in trip build animals
    GAMEOBJ:GetTimer():AddTimerWithCancel( 10, "sporeTimer", self )

    -- start the first timer for the event    
    GAMEOBJ:GetTimer():AddTimerWithCancel( CONSTANTS["PEACE_TIME_DURATION"], "startEventTimer", self )

--[[    
    
	

    self:SetVar("NumberOfChildren", 0 )     -- Dont not change this value <<< 
    self:SetVar("WP_Num", 1)                -- Dont not change this value <<<  
    self:SetVar("Count", 1)                 -- On load Count    
    self:SetVar("Count2", 1)                -- On load Count 

    self:SetVar("PetNames", "Skunk")        -- Name as set in the NPC script
    self:SetVar("WP_Alpha", "skunkWP")      -- Starting string of the Way Point name (waypoints in the map need to be named in the format skunkWP_1, skunkWP_2, etc.)
    self:SetVar("TotalPets", 10)            -- Total number of pets to spawn
    self:SetVar("PetID", 3279)              -- LOT of pet
    self:SetVar("CurrentDestink", 0)        -- Number of skunks currently destunk
    
    -- self:SetVar("PetNames2", "Hazmat")       -- Name as set in the NPC script
    self:SetVar("HazmatWPPrefix", "hazmatWP") -- Starting string of the Way Point name (waypoints in the map need to be named in the format skunkWP_1, skunkWP_2, etc.)
    self:SetVar("HazmatNumber", 1)            -- Total number of hazmat guys to spawn
    self:SetVar("PetID2", 2628)               -- LOT of pet


	self:SetVar("EventLifetime", 180.0)      -- Amount of time in seconds that the skunks live for before despawning
    self:SetVar("PeaceTime", 120.0)		-- Amount of time in seconds after skunks die that the next wave comes in (note: add lifetime and respawntime together to get the full time between events)
     
    --self:SetVar("HazmatLifetime", 60.0)      -- Amount of time in seconds that the hazmat guys live for before despawning
   -- self:SetVar("HazmatRespawnTime", 220.0)  -- Amount of time in seconds after hazmat guys die that the next wave comes in. this needs to be synched wtih the skunks spawning. 
                                             -- Right now the hazmat guys come in 20 seconds after the skunks disappear and last for 60 seconds
                                            
    self:SetVar("earthquakeWarningTime", 5.0)		-- Amount of time in seconds between the earthquake and the start of the skunk invasion
    self:SetVar("SirenTime", 2.5)					-- Time from earthquake to siren
    self:SetVar("SirenToFountain", 1.5)			    -- Time from siren to fountain transformation
    self:SetVar("WindowShutterTime", 1.5)			-- Time from fountain transformation to window shutter close.
    self:SetVar("ShuttersToSkunks", 5.0)            -- Time from shutters closing to skunks
    self:SetVar("SkunksToTruck", 12.0)				-- Time from skunks to trunk start
    self:SetVar("TruckToHazmatNPC", 5.7)			-- Time from truck start to hazmat NPCs spawn
    
    -- self:SetVar("MaxSkunkFollow", 2)				-- Number of skunks yu need to follow you for the mother skunk mission
    
                                                                             
	
    
    self:SetVar("HazmatInitialSpawnTime", 200.0)    -- Amount of time to wait the very first time the event triggers before spawning hazmat guys
    
 
--]]
	 
end


--------------------------------------------------------------
-- When children are loaded by the object
--------------------------------------------------------------
function onChildLoaded(self,msg)
    
    -- was a skunk loaded
    if (IsValidSkunk(msg.templateID) == true) then

        -- store the parent    
        storeParent(self,  msg.childID)

        -- store the skunk so we can access it later   
        -- (so we can respawn it later)
        local num = msg.childID:GetVar("SkunkNum")
        INVASION_SKUNKS[num] = msg.childID:GetID()
        
        print("storing skunk " .. num)

    -- was a hazmat NPC loaded
    elseif (msg.templateID == CONSTANTS["SPAWNED_HAZMAT_NPC"]) then

        -- store the parent    
        storeParent(self,  msg.childID)

        -- store the hazmat npc so we can access it later   
        local num = msg.childID:GetVar("HazmatNum")
        HAZMAT_NPCS[num] = msg.childID:GetID()
        
        print("storing hazmat NPC " .. num)
        
    -- was a stink cloud loaded
    elseif (msg.templateID == CONSTANTS["INVASION_STINK_CLOUD_LOT"]) then
    
        -- store the parent    
        storeParent(self,  msg.childID)

        -- store the stink cloud so we can access it later   
        -- (so we can respawn it later)
        local num = msg.childID:GetVar("StinkCloudNum")
        INVASION_STINK_CLOUDS[num] = msg.childID:GetID()
        
        print("storing stink cloud " .. num .. " at waypoint " .. INVASION_STINK_CLOUD_WAYPOINTS[num])
    
    -- hazmat rebuild van loaded
    elseif (msg.templateID == CONSTANTS["HAZMAT_REBUILD_VAN_LOT"]) then

        -- store the parent    
        storeParent(self,  msg.childID)

        -- get current van and remove
        local van = getObjectByName( self, "HazmatVanID" )
        if (van) then
            GAMEOBJ:DeleteObject(van)
        end

        -- store the new van
        storeObjectByName( self, "HazmatVanID", msg.childID )
        
        -- break the rebuild
        msg.childID:RebuildReset()
        
        -- start timer to spawn in hazmat NPCs
        GAMEOBJ:GetTimer():AddTimerWithCancel(CONSTANTS["HAZMAT_NPC_SPAWN_TIMER"], "SpawnHazmatNPCTimer", self )

    -- garage hazmat van
    elseif ( msg.templateID == CONSTANTS["HAZMAT_VAN_LOT"] ) then

        -- store the parent    
        storeParent(self,  msg.childID)

        -- get current van and remove
        local van = getObjectByName( self, "HazmatVanID" )
        if (van) then
            GAMEOBJ:DeleteObject(van)
        end

        -- store the new van
        storeObjectByName( self, "HazmatVanID", msg.childID )

    end        



    --[[
    if msg.childID:GetLOT().objtemplate == 3553 then -- hazmat guy
		--print("Loading Hazmat Truck NPC")
		msg.childID:SetVar("attached_path", "hazmatWP_1")
		msg.childID:SetVar("attached_path_start", 0)
		-- start child on path
		msg.childID:FollowWaypoints()
		-- msg.childID:PlayAnimation{ animationID = "spawn" }
		
		storeObjectByName( self, "HazmatTruckNPCID", msg.childID )
		
	elseif msg.childID:GetLOT().objtemplate == 3279 then -- skunk
    
		print ("Loading Skunk")
        local FinalName = (self:GetVar("PetNames").."_"..self:GetVar("WP_Alpha")..self:GetVar("ChildLoadNUM"))
        local someName = (self:GetVar("PetNames").."_"..self:GetVar("Count"))
            
        msg.childID:SetVar("SpawnedVar", FinalName)
        msg.childID:SetVar("attached_path",  self:GetVar("WP_Alpha").."_"..self:GetVar("ChildLoadNUM"))
        msg.childID:SetVar("I_Have_A_Parent", true )
        storeParent(self,  msg.childID)
            
        storeObjectByName(self, someName, msg.childID)
        self:SetVar("Count",  self:GetVar("Count") + 1) 
    end
--]]
end 


--------------------------------------------------------------
-- When objects are loaded via zone notification
--------------------------------------------------------------
function onObjectLoaded(self, msg)

    if(IsValidNPC(msg.templateID) == true) then

		-- store the npc
        local nextActor = #SPAWNEDNPCS + 1
        SPAWNEDNPCS[nextActor] = msg.objectID:GetID()
    
    -- spout loaded        
	elseif ( msg.templateID == CONSTANTS["SPOUT_LOT"] ) then
	
		-- store object in the spout array
		local nextSpout = #SPOUTS + 1
        SPOUTS[nextSpout] = msg.objectID:GetID()
    
    -- bubble statue loaded        
	elseif ( msg.templateID == CONSTANTS["BUBBLE_BLOWER_LOT"] ) then
	
		-- store object in the bubble blower array
		local nextBB = #BUBBLE_BLOWERS + 1
        BUBBLE_BLOWERS[nextBB] = msg.objectID:GetID()
 
   elseif (msg.templateID == CONSTANTS["POLE_SLIDE_NPC"]) then

        -- store the parent    
        storeParent(self,  msg.objectID)
        storeObjectByName( self, "PoleSlideNPC", msg.objectID )
	
   elseif (msg.templateID == CONSTANTS["BALLOON_LOT"]) then

        -- store the parent    
        storeParent(self,  msg.objectID)
        storeObjectByName( self, "Balloon", msg.objectID )
        
	elseif ( msg.templateID == CONSTANTS["FLOWER_LOT"] ) then
	
		-- store object in the flowers array
		local nextFlower = #FLOWERS + 1
        FLOWERS[nextFlower] = msg.objectID:GetID()

	end

end


--------------------------------------------------------------
-- called when a player is loaded and ready
--------------------------------------------------------------
function onPlayerLoaded(self, msg)

    --print("-------- Player Loaded on Server, sending State Info")
    -- send a message down to ourself on the client to set zone state
    self:NotifyClientRebuildSectionState{ rerouteID = msg.playerID, iState = GetZoneState(self) }
    
	-- send the player's ID to the balloon so it can tell the newly-loaded client which anim to use
	local balloon = getObjectByName( self, "Balloon" )
	if (balloon ~= nil) then
		balloon:NotifyObject{ ObjIDSender = msg.playerID, name = "playerLoaded" }
	end
	
	-- send the player's ID to the flowers so each of them can tell the newly-loaded client which anim to use
	for flowerID = 1, #FLOWERS do
        local flower = GAMEOBJ:GetObjectByID(FLOWERS[flowerID])
        flower:NotifyObject{ ObjIDSender = msg.playerID, name = "playerLoaded" }
	end

end


--------------------------------------------------------------
-- called when timers expire
--------------------------------------------------------------
function onTimerDone(self, msg)

    print ("Timer name: "..msg.name)

	if (msg.name == "startEventTimer") then
	
	    -- the event is starting so put us into the transition state
		SetZoneState(self, CONSTANTS["ZONE_STATE_TRANSITION"])
	
	elseif (msg.name == "MaxInvasionTimer") then
		SetZoneState(self, CONSTANTS["ZONE_STATE_DONE_TRANSITION"])
	
	elseif (msg.name == "DoPanicNPCs") then
		
		-- tell NPCs to panic
		PanicNPCs(self)

    elseif (msg.name == "SkunksSpawning") then
    
        -- spawn the skunks
        spawnSkunks(self)
        
    elseif (msg.name == "StinkCloudsSpawning") then
    
        -- spawn the stink clouds
        spawnStinkClouds(self)

    elseif (msg.name == "EndInvasionTransition") then
        
        -- move to the next state
        SetZoneState(self, CONSTANTS["ZONE_STATE_HIGH_ALERT"])
    
    elseif (msg.name == "EndDoneTransition") then
        
        -- move to the next state
        SetZoneState(self, CONSTANTS["ZONE_STATE_NO_INVASION"])
    
    elseif (msg.name == "HazmatVanTimer") then
	
        -- animate the van to start
    	local anim_time = animateVan(self, "start")
    	
    	-- set a callback to trigger van rebuild spawn after animation is over
    	if (anim_time and tonumber(anim_time) > 0 ) then
            GAMEOBJ:GetTimer():AddTimerWithCancel(anim_time, "HazmatVanStartDone", self )
        end    	
	elseif (msg.name == "PoleSlideTimer") then
	
        -- animate the van to start
    	local slider = getObjectByName( self, "PoleSlideNPC" )

		if (slider ~= nil) then
			slider:PlayAnimation{ animationID = "slide" }
		end
    	
    elseif (msg.name == "HazmatVanStartDone") then

        -- spawn in rebuild (replaces current van stored object name)
        spawnRebuildVan(self)
            
    elseif (msg.name == "HazmatVanEndDone") then

        -- spawn in real van (replaces current van stored object name)
        spawnGarageVan(self)
       
    elseif (msg.name == "SpawnHazmatNPCTimer") then

        -- spawn in the hazmat NPCs
		SpawnHazmatNPCs(self)

    elseif (string.starts(msg.name,"SpawnSingleHazmatNPCTimer") == true) then

        -- get spawn number
        local i = string.sub(msg.name,26)
        
        -- spawn a single hazmat NPC
        spawnSingleHazmatNPC(self, i)

    elseif (string.starts(msg.name,"RespawnSkunk") == true) then

        -- if the invasion is still active then respawn the skunk    
        if (IsInvasionActive(self)) then

            -- get spawn number
            local i = string.sub(msg.name,13)

            spawnSingleSkunk(self,i, true)
            -- spawn a new skunk in this skunk's place

        end
                
	elseif (msg.name == "sporeTimer") then
	
		LoadSporeAnimals(self)

	end


end


--------------------------------------------------------------
--   Spawns the garage hazmat van from first point on path
--------------------------------------------------------------
function spawnGarageVan(self)

    local pathMsg = LEVEL:GetPathWaypoints(CONSTANTS["HAZMAT_REBUILD_VAN_SPAWN_PATH"])
    local trans
    if (tostring(type(pathMsg)) == "table") then
        trans = pathMsg[1]
    else
        return
    end

    -- load the object in the world
    RESMGR:LoadObject { objectTemplate = CONSTANTS["HAZMAT_VAN_LOT"],
                        x = trans.pos.x,
                        y = trans.pos.y,
                        z = trans.pos.z,
                        rw = trans.rot.w,
                        rx = trans.rot.x,
                        ry = trans.rot.y,
                        rz = trans.rot.z,
                        owner = self }
end


--------------------------------------------------------------
--   Spawns the rebuild hazmat van from second point on path
--------------------------------------------------------------
function spawnRebuildVan(self)

    local pathMsg = LEVEL:GetPathWaypoints(CONSTANTS["HAZMAT_REBUILD_VAN_SPAWN_PATH"])
    local trans
    if (tostring(type(pathMsg)) == "table") then
        trans = pathMsg[2]
    else
        return false
    end

    -- load the object in the world
    RESMGR:LoadObject { objectTemplate = CONSTANTS["HAZMAT_REBUILD_VAN_LOT"],
                        x = trans.pos.x,
                        y = trans.pos.y,
                        z = trans.pos.z,
                        rw = trans.rot.w,
                        rx = trans.rot.x,
                        ry = trans.rot.y,
                        rz = trans.rot.z,
                        owner = self }
                        
    return true
    
end


--------------------------------------------------------------
--   Spawn Skunks 
--------------------------------------------------------------
function spawnSkunks(self)

    -- spawn skunks
    for i = 1, CONSTANTS["NUM_SKUNKS"] do
        spawnSingleSkunk(self, i, false)
    end
   
end


--------------------------------------------------------------
--   Spawns a single skunk by number, param to say if its 
--   a respawn or not
--------------------------------------------------------------
function spawnSingleSkunk(self, num, bRespawn)

    -- construct path string, start at first waypoint
    local pathStr = CONSTANTS["SKUNK_PATH_PREFIX"] .. num
    local pathStart = 1
    
    -- if this is a respawn, append the suffix to the string
    -- Note: This means WP path names in the map must be exact
    if (bRespawn == true) then
    
        -- construct the path
        pathStr = pathStr .. CONSTANTS["SKUNK_ROAM_PATH_SUFFIX"]

        -- pick a random waypoint on the path
        pathStart = GetRandomWaypoint(pathStr)

    end        
    
    -- create the config data for the new object
    -- if first spawn, then they are immune
    local config = {{"SkunkNum", num},  
                    {"attached_path", pathStr},  
                    {"attached_path_start", pathStart - 1},
                    {"IsImmune", not bRespawn}}    

    -- get first waypoint data for spawn position
    local firstWP = GAMEOBJ:GetWaypointPos( pathStr, pathStart)
    
    -- pick a random skunk template
    local skunkTemplate = CONSTANTS["INVASION_SKUNK_LOT"][math.random(1,#CONSTANTS["INVASION_SKUNK_LOT"])]
    
    -- add the object
    RESMGR:LoadObject { objectTemplate = skunkTemplate, 
                        x = firstWP.x, 
                        y = firstWP.y, 
                        z = firstWP.z, 
                        owner = self,  
                        configData = config }

end


--------------------------------------------------------------
-- returns a random waypoint on a path, or 1 if there is a 
-- problem
--------------------------------------------------------------
function GetRandomWaypoint(path)

    -- find waypoint info and count for number of waypoints
	local pathMsg = LEVEL:GetPathWaypoints(path)
	
	if (tostring(type(pathMsg)) == "table") then
	    return (math.random(1,tonumber(#pathMsg)))
	end	
    return 1	
    
end



--------------------------------------------------------------
-- remove invasion skunks from the level
--------------------------------------------------------------
function killSkunks(self)

    for i,skunkID in pairs(INVASION_SKUNKS) do 
        local skunkObj = GAMEOBJ:GetObjectByID(skunkID)
        if (skunkObj and skunkObj:Exists()) then
            print("removing skunk " .. i)
            skunkObj:Die{killerID = skunkObj, killType = "SILENT"}
        end
    end
    INVASION_SKUNKS = {}
    
end


--------------------------------------------------------------
-- Checks to see if a waypoint is being used by another 
-- stink cloud 
--------------------------------------------------------------
function IsWaypointValid(self, waypoint)

    -- 0 or less is not a valid waypoint number
    if (tonumber(waypoint) <= 0) then
        return false
    end
    
    -- loop through waypoints
    for i,wpNum in pairs(INVASION_STINK_CLOUD_WAYPOINTS) do 
        if ( tonumber(waypoint) == tonumber(wpNum) ) then
            return false
        end
    end
    
    return true
    
end

--------------------------------------------------------------
-- Spawn Stink Clouds 
--------------------------------------------------------------
function spawnStinkClouds(self)

    -- spawn skunks
    for i = 1, CONSTANTS["NUM_STINK_CLOUDS"] do
        spawnSingleStinkCloud(self, i)
    end
   
end


--------------------------------------------------------------
-- Spawns a single stink cloud by number in a random place
--------------------------------------------------------------
function spawnSingleStinkCloud(self, num)

    -- construct path string
    local pathStr = CONSTANTS["STINK_CLOUD_PATH"]
    local maxPoints = self:GetVar("NumStinkCloudSpawnPoints")
    
    -- do nothing if the path has no points (doesn't exist)
    if (maxPoints == nil) then
        return
    end
    
    local maxTries = 20
    
    -- get a random waypoint
    local waypoint = math.random(1, maxPoints)
    
    -- check for validity, reroll a waypoint
    local numTries = 0
    while (IsWaypointValid(self,waypoint) == false) do

        -- roll a new waypoint
        waypoint = math.random(1, maxPoints)
        
        -- too many tries
        numTries = numTries + 1
        if (numTries >= maxTries) then
            print("ERROR: too many tries to create a stink cloud, add more points...")
            return
        end

    end

    -- store waypoint used    
    INVASION_STINK_CLOUD_WAYPOINTS[num] = waypoint

    -- create the config data for the new object
    local config = {{"StinkCloudNum", num}}

    -- get first waypoint data for spawn position
    local spawnLoc = GAMEOBJ:GetWaypointPos( pathStr, waypoint)
    
    -- add the object
    RESMGR:LoadObject { objectTemplate = CONSTANTS["INVASION_STINK_CLOUD_LOT"], 
                        x = spawnLoc.x, 
                        y = spawnLoc.y, 
                        z = spawnLoc.z, 
                        owner = self,  
                        configData = config }
end


--------------------------------------------------------------
-- remove invasion stinks from the level
--------------------------------------------------------------
function killStinkClouds(self)

    for i,stinkID in pairs(INVASION_STINK_CLOUDS) do 
        local stinkObj = GAMEOBJ:GetObjectByID(stinkID)
        if (stinkObj and stinkObj:Exists()) then
            print("removing stink cloud " .. i)
            stinkObj:Die{killerID = stinkObj, killType = "SILENT"}
        end
    end
    INVASION_STINK_CLOUDS = {}
    INVASION_STINK_CLOUD_WAYPOINTS = {}
    
end


--------------------------------------------------------------
-- add points for a player during the invasion
--------------------------------------------------------------
function AddPlayerPoints(self, player, points)

    if (not IsInvasionActive(self)) then
        print ("No points added: event not active")
        return
    end
    
    -- non existent or dead players don't get points
    if (points <= 0 or not player:Exists() or player:IsDead().bDead) then
        return
    end
    
    -- get the player ID for checking
    local playerID = player:GetID()
    
    for i,playerData in pairs(INVASION_PLAYERS) do 
    
        -- we found a player
        if (playerID == playerData.id) then
            -- add points for this player
            playerData.score = playerData.score + points
            print("adding " .. points .. " points for player " .. playerID)            
            return
        end
    end
    
    -- if we get here the player was not found, add a new one
    local nextPlayer = #INVASION_PLAYERS + 1

    -- add the data    
    local PLAYER_DATA = {}
    PLAYER_DATA.id = playerID
    PLAYER_DATA.score = points
    INVASION_PLAYERS[nextPlayer] = PLAYER_DATA
    print("new player: adding " .. points .. " points for player " .. playerID)       
    
end


--------------------------------------------------------------
-- reward players who participated in the invasion
--------------------------------------------------------------
function RewardPlayers(self)

    for i,playerData in pairs(INVASION_PLAYERS) do 
        local player = GAMEOBJ:GetObjectByID(playerData.id)
        if (player and player:Exists() and not player:IsDead().bDead) then
            local coins = playerData.score * CONSTANTS["REWARD_MULTIPLIER"]
            player:DropLoot{iCurrency = coins, owner = player, rerouteID = player, sourceObj = self}
            print("rewarding player " .. playerData.id .. " with a score of " .. playerData.score)
            -- player do some player reward
        end
    end
    INVASION_PLAYERS = {}
    
end


--------------------------------------------------------------
-- Zone script notified
--------------------------------------------------------------
function onNotifyObject(self, msg)

    -- a skunk was cleaned by a player
    if (msg.name == "skunk_cleaned") then

        if (not IsInvasionActive(self)) then
            return
        end
    
        -- add the points for the player
        AddPlayerPoints(self, msg.ObjIDSender, CONSTANTS["POINT_VALUE_SKUNK"])
        
        -- add clean points, spawn new skunk if event not over
        if (IncrementTotalCleanPoints(self, CONSTANTS["POINT_VALUE_SKUNK"]) == false) then
            
            if (msg.param1 and msg.param1 > 0) then
            
                -- find a random spawn time
                local spawnTime = math.random(CONSTANTS["SKUNK_RESPAWN_TIMER_MIN"], CONSTANTS["SKUNK_RESPAWN_TIMER_MAX"])
                
                -- start a timer to respawn the skunk with his spawn number
                GAMEOBJ:GetTimer():AddTimerWithCancel(spawnTime, "RespawnSkunk" .. msg.param1, self )        

            end
            
        end
        
    -- a stink cloud was cleaned by a player
    elseif (msg.name == "stink_cloud_cleaned_by_player" or 
            msg.name == "stink_cloud_cleaned_by_broombot") then
    
        if (not IsInvasionActive(self)) then
            return
        end
    
        -- add the points for the player
        if (msg.name == "stink_cloud_cleaned_by_player") then
            AddPlayerPoints(self, msg.ObjIDSender, CONSTANTS["POINT_VALUE_STINK_CLOUD"])
        end

        
        -- add clean points, spawn new cloud if event not over
        if (IncrementTotalCleanPoints(self, CONSTANTS["POINT_VALUE_STINK_CLOUD"]) == false) then
            -- spawn a new stink in this stink's place (at a new spot)
            spawnSingleStinkCloud(self,msg.param1)
        end

    elseif (msg.name == "hazmat_cleaned") then

        if (not IsInvasionActive(self)) then
            return
        end
    
        -- add the points for the player
        AddPlayerPoints(self, msg.ObjIDSender, CONSTANTS["POINT_VALUE_HAZMAT"])
        
        -- add clean points
        IncrementTotalCleanPoints(self, CONSTANTS["POINT_VALUE_HAZMAT"])

    elseif (msg.name == "broombot_fixed") then

        if (not IsInvasionActive(self)) then
            return
        end
    
        -- add the points for the player
        AddPlayerPoints(self, msg.ObjIDSender, CONSTANTS["POINT_VALUE_BROOMBOT"])
        
        -- add clean points
        IncrementTotalCleanPoints(self, CONSTANTS["POINT_VALUE_BROOMBOT"])

    end
        
--[[

	--elseif(msg.name == "can_complete_follow") then
	--	SKUNKFOLLOW[msg.targetID:GetID()] = msg.iPosit
	--	return msg
	end
	--]]
end

--[[
function EndSkunkEvent(self)

	self:SetVar( "invasionInProgress", false )

	killSkunks(self)
	openShutters(self)
	stopSkunkDetectors(self)
	IdleNPCs(self)
	self:SetVar("CurrentDestink", 0)
	EndTruck(self)
	KillHazmatNPCs(self)
	TransformFountainToNormal( self )
	
	GAMEOBJ:GetZoneControlID():NotifyClientZoneObject{ name="normal_sky" }
	GAMEOBJ:GetZoneControlID():NotifyClientZoneObject{ name="stop_lamp_detectors" }
end
--]]

--------------------------------------------------------------
-- set NPCs into panic state
--------------------------------------------------------------
function PanicNPCs(self)
	
	for actorNum = 1, #SPAWNEDNPCS do
		local actor = GAMEOBJ:GetObjectByID(SPAWNEDNPCS[actorNum])
		actor:NotifyObject{ name="npc_panic" }
	end

end


--------------------------------------------------------------
-- set NPCs as idle
--------------------------------------------------------------
function IdleNPCs(self)
	
	for actorNum = 1, #SPAWNEDNPCS do
		local actor = GAMEOBJ:GetObjectByID(SPAWNEDNPCS[actorNum])
		actor:NotifyObject{ name="npc_idle" }
	end

end


--------------------------------------------------------------
-- return if template is a valid panic npc
--------------------------------------------------------------
function IsValidNPC(templateID)

    -- list of npcs does not exist
    if (CONSTANTS["INVASION_PANIC_ACTORS"] == nil) then
        return false
    end
	
    -- look for a valid actor
	for actors = 1, #CONSTANTS["INVASION_PANIC_ACTORS"] do
		if (templateID == CONSTANTS["INVASION_PANIC_ACTORS"][actors]) then
			return true
		end
	end

	return false

end


--------------------------------------------------------------
-- return if template is a valid skunk npc
--------------------------------------------------------------
function IsValidSkunk(templateID)

    -- list of npcs does not exist
    if (CONSTANTS["INVASION_SKUNK_LOT"] == nil) then
        return false
    end
	
    -- look for a valid actor
	for actors = 1, #CONSTANTS["INVASION_SKUNK_LOT"] do
		if (templateID == CONSTANTS["INVASION_SKUNK_LOT"][actors]) then
			return true
		end
	end

	return false

end

--------------------------------------------------------------
-- Triggers the van animations
-- returns animation time
--------------------------------------------------------------
function animateVan(self, name)
	
	local van = getObjectByName( self, "HazmatVanID" )
	local anim_time = 0
	if (van ~= nil) then
	    -- clear timers on it because we are animating
	    GAMEOBJ:GetTimer():CancelAllTimers( van )
		anim_time = van:GetAnimationTime{  animationID = name }.time
		van:PlayAnimation{ animationID = name }
	end
	
	return anim_time
	
end


--------------------------------------------------------------
--   Spawn Hazmat NPCs
--------------------------------------------------------------
function SpawnHazmatNPCs(self)

    -- spawn npcs
    for i = 1, CONSTANTS["NUM_HAZMAT_NPCS"] do
        GAMEOBJ:GetTimer():AddTimerWithCancel(CONSTANTS["TIME_BETWEEN_HAZMAT_SPAWNS"] * i, "SpawnSingleHazmatNPCTimer" .. i, self )        
    end
   
end


--------------------------------------------------------------
--   Spawns a hazmat NPC for the event by number
--------------------------------------------------------------
function spawnSingleHazmatNPC(self, num)

    -- construct path string
    local pathStr = CONSTANTS["HAZMAT_NPC_PATH_PREFIX"] .. num
    
    -- create the config data for the new object
    local config = {{"HazmatNum", num},
                    {"attached_path", pathStr}, 
                    {"attached_path_start", 0}}

    -- get first waypoint data for spawn position
    local firstWP = GAMEOBJ:GetWaypointPos( pathStr, 1)
    
    local spawnX = firstWP.x
    local spawnY = firstWP.y
    local spawnZ = firstWP.z
    
    local rotX = 0.0
    local rotY = 0.0
    local rotZ = 0.0
    local rotW = 1.0
    
    -- get the truck's rotation
	local van = getObjectByName( self, "HazmatVanID" )
	if (van) then
		
		-- we want them to face the back of the truck, so 180 degrees from the way the truck faces
		rotX = 0.0 - van:GetRotation().z
		rotY = 0.0 - van:GetRotation().w
		rotZ = van:GetRotation().x
		rotW = van:GetRotation().y

	end
    
    -- add the object
    RESMGR:LoadObject { objectTemplate = CONSTANTS["SPAWNED_HAZMAT_NPC"], 
                        x = spawnX, 
                        y = spawnY, 
                        z = spawnZ,
                        rx = rotX,
                        ry = rotY,
                        rz = rotZ,
                        rw = rotW,
                        owner = self,  
                        configData = config }

		
end


--------------------------------------------------------------
-- remove spawned hazmat NPCs from the level
--------------------------------------------------------------
function killHazmatNPCs(self)

    for i,hazmatID in pairs(HAZMAT_NPCS) do 
        local hazmatObj = GAMEOBJ:GetObjectByID(hazmatID)
        if (hazmatObj and hazmatObj:Exists()) then
            print("removing hazmat NPC " .. i)
            hazmatObj:Die{killerID = hazmatObj} --, killType = "SILENT"}
        end
    end
    HAZMAT_NPCS = {}
    
end


-- Request follow message from baby skunks
function onRequestFollow(self, msg)
    
    print("IN ON REQUESTFOLLOW EVENT SCRIPT")
    
    -- Forward message on to the mother
    local mother = getObjectByName( self, "MotherSkunkID" )
    
    if(mother ~= nil) then
		print("mother not null")
		local FollowMsg = mother:RequestFollow{ targetID = msg.targetID, requestorID = msg.requestorID }
		return FollowMsg
	end
	
	return msg
 	
end




--------------------------------------------------------------
-- Event from client, right now we do slash commands
--------------------------------------------------------------
function onFireEventServerSide( self, msg )
	
	if ( msg.args == "toggleEvent" ) then
	
	    -- cancel all timers we will do our own based on state
	    GAMEOBJ:GetTimer():CancelAllTimers( self )
	    
	    -- get our state
	    local state = GetZoneState(self)
	    
	    -- modify state
	    if (state == CONSTANTS["ZONE_STATE_DONE_TRANSITION"]) then
	        state = CONSTANTS["ZONE_STATE_NO_INVASION"]
	    else
	        state = state + 1
        end
	    
	    -- set the state
	    SetZoneState(self, state)

	end
	
end


function CancelAllTimers( self )
	GAMEOBJ:GetTimer():CancelTimer("startEventTimer", self)
	GAMEOBJ:GetTimer():CancelTimer("skunkSpawnTimer", self)
	GAMEOBJ:GetTimer():CancelTimer("fountainTimer", self)
	GAMEOBJ:GetTimer():CancelTimer("shutterTimer", self)
	GAMEOBJ:GetTimer():CancelTimer("stopEventTimer", self)
	GAMEOBJ:GetTimer():CancelTimer("HazmatTruckTimer", self)
	GAMEOBJ:GetTimer():CancelTimer("HazmatNPCTimer", self)
	GAMEOBJ:GetTimer():CancelTimer("sirenTimer", self)
end


-- 3712, 3713, 3714
function LoadSporeAnimals(self)

	-- Chickens
	for i = 1, 3 do		-- TODO move some constants out to c_zoorilla
		--print("Loading chicken: " .. i)
		local wp = GAMEOBJ:GetWaypointPos("spore1", i)
		--print("x y x: " .. wp.x .. " ".. wp.y .. " " .. wp.z)
		RESMGR:LoadObject {objectTemplate = 3712, x=wp.x, y=wp.y, z=wp.z, owner=self, rw=0.0, rx= 0.0, ry=0.0, rz=0.0}
	end
	
	-- Skunks
	for i = 1, 3 do
		--print("Loading skunk: " .. i)
		local wp = GAMEOBJ:GetWaypointPos("spore2", i)
		--print("x y x: " .. wp.x .. " ".. wp.y .. " " .. wp.z)
		RESMGR:LoadObject {objectTemplate = 3713, x=wp.x, y=wp.y, z=wp.z, owner=self, rw=0.0, rx= 0.0, ry=0.0, rz=0.0}
	end
	
	-- Sheep
	for i = 1, 3 do
		--print("Loading sheep: " .. i)
		local wp = GAMEOBJ:GetWaypointPos("spore3", i)
		--print("x y x: " .. wp.x .. " ".. wp.y .. " " .. wp.z)
		RESMGR:LoadObject {objectTemplate = 3714, x=wp.x, y=wp.y, z=wp.z, owner=self, rw=0.0, rx= 0.0, ry=0.0, rz=0.0}
	end

end
