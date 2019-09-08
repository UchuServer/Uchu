--///////////////////////////////////////////////////////////////////////////////////////
--//            YouReeka zone script - client side
--///////////////////////////////////////////////////////////////////////////////////////

--------------------------------------------------------------
-- Includes
--------------------------------------------------------------
require('o_mis')
require('c_Zorillo')


--------------------------------------------------------------
-- Locals
--------------------------------------------------------------
-- stores all loaded lamps
LAMPS = {}

-- stores all bubble blower statues
BUBBLE_BLOWERS = {}

-- stores all loaded spouts
SPOUTS = {}

-- stores all loaded mid-air stink effects
AIR_STINKS = {}

-- stores all ambient NPC's that give out hint chat bubbles during peace time
AMBIENT_NPCS = {}


--------------------------------------------------------------
-- Get the state of the zone
--------------------------------------------------------------
function GetZoneState(self)

    return self:GetVar("ZoneState")
    
end


--------------------------------------------------------------
-- Set the state of the zone
--------------------------------------------------------------
function SetZoneState(self, state, bInitialUpdate)

    -- get current state
    local prevState = GetZoneState(self)
    
    -- set to new state
    self:SetVar("ZoneState", state)
    
    --print("Setting Client Zone State to " .. state)

    -- do actions based on new state
    if (prevState and prevState ~= state) then
        if (state == CONSTANTS["ZONE_STATE_NO_INVASION"]) then
            DoNoInvasionStateActions(self, bInitialUpdate)
        elseif (state == CONSTANTS["ZONE_STATE_TRANSITION"]) then
            DoTransitionStateActions(self, bInitialUpdate)
        elseif (state == CONSTANTS["ZONE_STATE_HIGH_ALERT"]) then
            DoHighAlertStateActions(self, bInitialUpdate)
        elseif (state == CONSTANTS["ZONE_STATE_MEDIUM_ALERT"]) then
            DoMediumAlertStateActions(self, bInitialUpdate)
        elseif (state == CONSTANTS["ZONE_STATE_LOW_ALERT"]) then
            DoLowAlertStateActions(self, bInitialUpdate)
        elseif (state == CONSTANTS["ZONE_STATE_DONE_TRANSITION"]) then
            DoDoneTransitionActions(self, bInitialUpdate)            
        end
    end
    
end    


--------------------------------------------------------------
-- do actions associated with No Invasion state
--------------------------------------------------------------
function DoNoInvasionStateActions(self, bInitialUpdate)

    -- update lamps
    SendStateToLamps(self)
    
    -- update bubble blowers
    SendStateToBubbleBlowers(self)
    
    -- update spouts
    SendStateToSpouts(self)
    
    --update mid-air stink
    SendStateToAirStink(self)
    
    --update the hazmat truck
    SendStateToHazmatTruck(self)
    
    -- Play animation on fountain alert
    SendStateToFountain(self) 

    --update chat bubbles for ambient NPC's
    SendStateToAmbientNPCs(self)   
    
    -- set skybox to normal
    useNormalSky(self)
    
end


--------------------------------------------------------------
-- do actions associated with Transition state
--------------------------------------------------------------
function DoTransitionStateActions(self, bInitialUpdate)

    -- update spouts
    SendStateToSpouts(self)
    
    --update mid-air stink
    SendStateToAirStink(self)
    
    --update the hazmat truck
    SendStateToHazmatTruck(self)

	-- Start earthquake
	shakeCamera(self)
	
	-- animate the inventor building
	animateInventorBuilding(self)
	
	--update chat bubbles for ambient NPC's
    SendStateToAmbientNPCs(self)
	
	-- start a timer to trigger the siren/lamps (func playSirenSound)
	GAMEOBJ:GetTimer():AddTimerWithCancel(CONSTANTS["EARTHQUAKE_DURATION"], "EarthquakeEnd", self )
	
	-- start a timer to trigger fountain alert
	GAMEOBJ:GetTimer():AddTimerWithCancel(CONSTANTS["FOUNTAIN_ALERT_TIMING"], "StartFountainAlert", self )
	
	-- start a timer to trigger skunks/sky
	GAMEOBJ:GetTimer():AddTimerWithCancel(CONSTANTS["SKUNK_SPAWN_TIMING"], "SkunksSpawning", self )

end


--------------------------------------------------------------
-- Shake the camera
--------------------------------------------------------------
function shakeCamera(self)

	local quakeCenter = getObjectByName( self, "earthquakeCenter" )
	if(quakeCenter ~= nil) then
		quakeCenter:PlayEmbeddedEffectOnAllClientsNearObject{ radius = 50000.0, fromObjectID = quakeCenter, effectName = "camshake" }
	end
	
end


--------------------------------------------------------------
-- animate the inventor building, and objects involved with it
--------------------------------------------------------------
function animateInventorBuilding(self)

	local building = getObjectByName( self, "inventorBuildingObject" )
	if(building ~= nil) then
		building:PlayFXEffect{effectType = "shake"}
	end
	
	local thrower = getObjectByName( self, "switchThrowerObject" )
	if(thrower ~= nil) then
		thrower:PlayAnimation{animationID = "shake"}
	end
	
end


--------------------------------------------------------------
-- plays siren sound on fountain
--------------------------------------------------------------
function playSirenSound(self)
	
	local quakeCenter = getObjectByName( self, "earthquakeCenter" )
	if(quakeCenter ~= nil) then
		quakeCenter:PlayFXEffect{ effectType = "siren" }
	end

end


--------------------------------------------------------------
-- play animation on fountain alert object
--------------------------------------------------------------
function SendStateToFountain(self, anim)

	local myObject = getObjectByName( self, "fountainAlertObject" )
	if(myObject ~= nil) then
		myObject:NotifyObject{ name="zone_state_change", param1 = GetZoneState(self) }
	end

end


--------------------------------------------------------------
-- send lamp notifications based on state
--------------------------------------------------------------
function SendStateToLamps(self)

    -- notify lamps of state
	for lampID = 1, #LAMPS do
		    local lamp = GAMEOBJ:GetObjectByID(LAMPS[lampID])
		    lamp:NotifyObject{ name="zone_state_change", param1 = GetZoneState(self) }
	end

end


--------------------------------------------------------------
-- send bubble blower notifications based on state
--------------------------------------------------------------
function SendStateToBubbleBlowers(self)

    -- notify bubble blowers of state
	for bbID = 1, #BUBBLE_BLOWERS do
		    local bb = GAMEOBJ:GetObjectByID(BUBBLE_BLOWERS[bbID])
		    bb:NotifyObject{ name="zone_state_change", param1 = GetZoneState(self) }
	end

end


--------------------------------------------------------------
-- send spout notifications based on state
--------------------------------------------------------------
function SendStateToSpouts(self)

    -- notify spouts of state
	for spoutID = 1, #SPOUTS do
		    local spout = GAMEOBJ:GetObjectByID(SPOUTS[spoutID])
		    spout:NotifyObject{ name="zone_state_change", param1 = GetZoneState(self) }
	end

end



--------------------------------------------------------------
-- send air sitnk notifications based on state
--------------------------------------------------------------
function SendStateToAirStink(self)

    -- notify air stink of state
	for airStinkID = 1, #AIR_STINKS do
		    local airStink = GAMEOBJ:GetObjectByID(AIR_STINKS[airStinkID])
		    airStink:NotifyObject{ name="zone_state_change", param1 = GetZoneState(self) }
	end

end



--------------------------------------------------------------
-- send ambient NPC's notifications based on state
--------------------------------------------------------------
function SendStateToAmbientNPCs(self)

    -- notify ambient NPCs of state
	for ambientID = 1, #AMBIENT_NPCS do
		    local ambient = GAMEOBJ:GetObjectByID(AMBIENT_NPCS[ambientID])
		    ambient:NotifyObject{ name="zone_state_change", param1 = GetZoneState(self) }
	end

end



--------------------------------------------------------------
-- send the hazmat truck notifications based on state
--------------------------------------------------------------
function SendStateToHazmatTruck(self)
	
	local truck = getObjectByName( self, "hazmatTruckObject" )
	if(truck ~= nil) then
		truck:NotifyObject{ name="zone_state_change", param1 = GetZoneState(self) }
	end

end



--------------------------------------------------------------
-- do actions associated with High Alert state
--------------------------------------------------------------
function DoHighAlertStateActions(self, bInitialUpdate)

    -- update spouts
    SendStateToSpouts(self)

    -- update lamps
    SendStateToLamps(self)
    
    -- update bubble blowers
    SendStateToBubbleBlowers(self)
    
    --update mid-air stink
    SendStateToAirStink(self)
      
    --update the hazmat truck
    SendStateToHazmatTruck(self)
    
    -- Play animation on fountain alert
    SendStateToFountain(self) 

    --update chat bubbles for ambient NPC's
    SendStateToAmbientNPCs(self)   
    
    -- set skybox to stinky
    useStinkySky(self)
    
end


--------------------------------------------------------------
-- do actions associated with Medium Alert state
--------------------------------------------------------------
function DoMediumAlertStateActions(self, bInitialUpdate)

    -- update spouts
    SendStateToSpouts(self)

    -- update lamps
    SendStateToLamps(self)
        
    -- update bubble blowers
    SendStateToBubbleBlowers(self)
    
    --update mid-air stink
    SendStateToAirStink(self)
    
    --update the hazmat truck
    SendStateToHazmatTruck(self)
    
    -- Play animation on fountain alert
    SendStateToFountain(self) 

    --update chat bubbles for ambient NPC's
    SendStateToAmbientNPCs(self)   

    -- set skybox to stinky
    useStinkySky(self)

end


--------------------------------------------------------------
-- do actions associated with Low Alert state
--------------------------------------------------------------
function DoLowAlertStateActions(self, bInitialUpdate)

    -- update spouts
    SendStateToSpouts(self)

    -- update lamps
    SendStateToLamps(self)
    
    -- update bubble blowers
    SendStateToBubbleBlowers(self)
    
    --update mid-air stink
    SendStateToAirStink(self)
    
    --update the hazmat truck
    SendStateToHazmatTruck(self)

    -- Play animation on fountain alert
    SendStateToFountain(self)

    --update chat bubbles for ambient NPC's
    SendStateToAmbientNPCs(self)    

    -- set skybox to stinky
    useStinkySky(self)

end


--------------------------------------------------------------
-- do actions associated with done transition
--------------------------------------------------------------
function DoDoneTransitionActions(self, bInitialUpdate)

    -- update spouts
    SendStateToSpouts(self)

    -- update lamps
    SendStateToLamps(self)
    
    -- update bubble blowers
    SendStateToBubbleBlowers(self)
    
    --update mid-air stink
    SendStateToAirStink(self)
    
    --update chat bubbles for ambient NPC's
    SendStateToAmbientNPCs(self)
    
    --update the hazmat truck
    SendStateToHazmatTruck(self)
    
    -- Play animation on fountain alert
    SendStateToFountain(self)

    -- set skybox to normal
    useNormalSky(self)
    
end


--------------------------------------------------------------
-- Init zone variables
--------------------------------------------------------------
function InitZoneVars(self)

    SetZoneState(self, CONSTANTS["ZONE_STATE_NO_INFO"], true)

    self:SetVar("StinkySkySet", false)

end


--------------------------------------------------------------
-- Object on added to world
--------------------------------------------------------------
function onStartup(self)

    -- init the zone vars
    InitZoneVars(self)

end
    

--------------------------------------------------------------
-- Generic message from a specific object
--------------------------------------------------------------
function onFireEvent(self, msg)

	-- object is telling us it is ready and to set its ready
	if (msg.args == "ZoneStateClientObjectReady") then
		
		-- respond with the state of the zone
		msg.senderID:NotifyObject{name = "zone_state_change", param1 = GetZoneState(self) }

	end
	
end


--------------------------------------------------------------
-- called when timers expire
--------------------------------------------------------------
function onTimerDone(self, msg)

    --print ("Client Timer name: "..msg.name)
	
	-- timer for earthquake ending
    if (msg.name == "EarthquakeEnd") then

        -- start siren sound
        playSirenSound(self)
        
        -- update lamps
        SendStateToLamps(self)
        
        -- update bubble blowers
        SendStateToBubbleBlowers(self)
        
    -- timer to start fountain alerts
    elseif (msg.name == "StartFountainAlert") then
    
        -- Play animation on fountain alert
        SendStateToFountain(self)
        
    -- timer to start skunk spawns and sky change
    elseif (msg.name == "SkunksSpawning") then
    
        -- set skybox to stinky
        useStinkySky(self)
        
    end    

end


--------------------------------------------------------------
-- set the sky to normal
--------------------------------------------------------------
function useNormalSky(self)

    -- do not set sky if already normal
    if (self:GetVar("StinkySkySet") == false) then
        return
    end

    self:SetVar("StinkySkySet", false)

	LEVEL:SetSkyDome (
		CONSTANTS["NORMAL_SKYBOX"]
	)
    
    LEVEL:SetLights(
		true, 0x5487B7,					--ambient color
		false, 0xf4efff,					--directional color
		false, 0xFFFFFF,					--specular color
		false, 0xFFFFFF,					--upper Hemi  color
		false, { 2084.81, -3500.98, 1557.21 },	--directional direction
		true, 0xd6efff,					--fog color

		true,                           --modifying draw distances (all of them)
        100.0, 100.0,					--fog near min/max
		700.0, 700.0,					--fog far min/max
		160.0, 160.0,					--post fog solid min/max
		500.0, 500.0,					--post fog fade min/max
		1360.0, 1360.0,	    			--static object cutoff min/max
		860.0, 860.0,	     			--dynamic object cutoff min/max

		true, "mesh/env/challenge_sky_light_2awesome.nif"
	)
			
end


--------------------------------------------------------------
-- set the sky to stinky
--------------------------------------------------------------
function useStinkySky(self)

    -- do not set sky if already stinky
    if (self:GetVar("StinkySkySet") == true) then
        return
    end
    
    self:SetVar("StinkySkySet", true)

	LEVEL:SetSkyDome (
		CONSTANTS["STINKY_SKYBOX"]
	)
    
    LEVEL:SetLights(
		true, 0x333333,					--ambient color
		false, 0xf4f4f4,					--directional color
		false, 0xFFFFFF,					--specular color
		false, 0xFFFFFF,					--upper Hemi  color
		false, { 2084.81, -3500.98, 1557.21 },	--directional direction
        true, 0x333333,					--fog color

        true,                           --modifying draw distances (all of them)
        50.0, 50.0,		       			--fog near min/max
		200.0, 200.0,					--fog far min/max
		160.0, 160.0,					--post fog solid min/max
		500.0, 500.0,					--post fog fade min/max
		860.0, 860.0,	    			--static object cutoff min/max
		360.0, 360.0,	     			--dynamic object cutoff min/max

		true, "mesh/env/env_sky_won_yore_skunk-stink.nif"
	)
			
end


--------------------------------------------------------------
-- Called on networked notification from server
--------------------------------------------------------------
function onNotifyClientZoneObject(self, msg)
    
    -- the zone state has changed
    if (msg.name == "zone_state_change") then
        GAMEOBJ:GetTimer():CancelAllTimers( self )
        SetZoneState(self, msg.param1, false)
    end        

end



--------------------------------------------------------------
-- Called when an object is loaded into the zone
--------------------------------------------------------------
function onObjectLoaded(self, msg)
	
	-- earthquake lot loaded
	if ( msg.templateID == CONSTANTS["EARTHQUAKE_CENTER_LOT"] ) then
		storeObjectByName( self, "earthquakeCenter", msg.objectID )

    -- fountain alert loaded
	elseif ( msg.templateID == CONSTANTS["FOUNTAIN_ALERT_LOT"] ) then
		storeObjectByName( self, "fountainAlertObject", msg.objectID )
	
    -- inventor building loaded
	elseif ( msg.templateID == CONSTANTS["INVENTOR_BUILDING_LOT"] ) then
		storeObjectByName( self, "inventorBuildingObject", msg.objectID )

    -- switch thrower loaded
	elseif ( msg.templateID == CONSTANTS["SWITCH_THROWER_LOT"] ) then
		storeObjectByName( self, "switchThrowerObject", msg.objectID )

    -- lamp loaded        
	elseif ( msg.templateID == CONSTANTS["SINGLE_LAMP_LOT"] ) then
	
		-- store object in the single lamps array
		local nextLamp = #LAMPS + 1
        LAMPS[nextLamp] = msg.objectID:GetID()
    
    -- bubble blower loaded        
	elseif ( msg.templateID == CONSTANTS["BUBBLE_BLOWER_LOT"] ) then
	
		-- store object in the bubble blowers array
		local nextBB = #BUBBLE_BLOWERS + 1
        BUBBLE_BLOWERS[nextBB] = msg.objectID:GetID()
        
    -- spout loaded        
	elseif ( msg.templateID == CONSTANTS["SPOUT_LOT"] ) then
	
		-- store object in the single lamps array
		local nextSpout = #SPOUTS + 1
        SPOUTS[nextSpout] = msg.objectID:GetID()
        
	elseif ( msg.templateID == CONSTANTS["AIR_STINK_LOT"] ) then
	
		-- store object in the single lamps array
		local nextAirStink = #AIR_STINKS + 1
        AIR_STINKS[nextAirStink] = msg.objectID:GetID()
        
    -- hazmat truck loaded
	elseif ( msg.templateID == CONSTANTS["HAZMAT_VAN_LOT"] ) then
		storeObjectByName( self, "hazmatTruckObject", msg.objectID )
		
	elseif ( IsPanicNPC( msg.templateID ) ) then
	
		-- store object in the single ambient NPC's array
		local nextAmbient = #AMBIENT_NPCS + 1
		AMBIENT_NPCS[nextAmbient] = msg.objectID:GetID()
    
	end
	
end


--------------------------------------------------------------
-- Sent by the server to notify client of zone state on login
--------------------------------------------------------------
function onNotifyClientRebuildSectionState(self, msg)

    --print("-------- Player Getting info on Client, setting state")
    
    SetZoneState(self, msg.iState, true)

end



--------------------------------------------------------------
-- return if template is a valid panic npc
--------------------------------------------------------------
function IsPanicNPC(templateID)

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
