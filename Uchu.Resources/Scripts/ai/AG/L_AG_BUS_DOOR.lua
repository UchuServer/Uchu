---- Moves the bus door while players are close to it.
---- Updated: 7/01/09 jnf...
---- Updated: 5/19/10 djames... added second proxy radius and outerCounter

-- the logic behind opening and closing the door is based on two proximity radii
-- it is possible for players to leave the proximity sphere unintentionally 
-- when the bus moves up because the sphere moves with the bus
-- by maintaining two counters, for both the inner and outer radii, players will
-- only be considered "in" proximity if they are inside both radii and they will
-- only be considered "out" of proximity if they are outside both radii

local ProxRadius = 75			-- the radius for "inner" proximity detection
local OuterProxRadius = 85		-- the radius for "outer" proximity detection
local soundName =  '{9a24f1fa-3177-4745-a2df-fbd996d6e1e3}'

function onStartup(self)
    self:SetVar("counter", 0)
    self:SetVar("outerCounter", 0)
    self:SetProximityRadius{radius = ProxRadius, name = "busDoor"} 
    self:SetProximityRadius{radius = OuterProxRadius, name = "busDoorOuter"} 
    self:StopPathing()
end

function moveDoor(self, bOpen)  
    --print('move door **************')
    
	if bOpen then
			--print('open door **************')
			self:GoToWaypoint{iPathIndex = 0}
	else  
			--print('close door **************')
			self:GoToWaypoint{iPathIndex = 1}               
	end
		    
	self:PlayNDAudioEmitter{m_NDAudioEventGUID = soundName}
end

function onProximityUpdate(self, msg)

    -- If this isn't the proximity radius for the bus door behavior, then we're done here
    if (msg.name ~= "busDoor" and msg.name ~= "busDoorOuter") then return end
    
    -- Make sure only humans are taken into account
    if not msg.objId:BelongsToFaction{factionID = 1}.bIsInFaction then return end 
            
    local counter = self:GetVar("counter")
    local outerCounter = self:GetVar("outerCounter")
    
	if (msg.status == "ENTER") then    
	    if (msg.name == "busDoor") then
			counter = counter + 1 
		elseif (msg.name == "busDoorOuter") then
			outerCounter = outerCounter + 1 
		end
		-- move up when a player is inside both radii
		if (counter == 1 and outerCounter >= 1) then
			moveDoor(self, true)
		end
	elseif (msg.status == "LEAVE") then
	    if (msg.name == "busDoor") then
			if counter > 0 then
				counter = counter - 1
			end
	    elseif (msg.name == "busDoorOuter") then
			if outerCounter > 0 then
				outerCounter = outerCounter - 1
			end
		end
		-- move down when no players are inside either radii
		if (counter == 0 and outerCounter == 0) then
			moveDoor(self, false)
		end
	end
	
    --print('proximity update: status='..msg.status..', name='..msg.name..', counter='..counter..', outerCounter='..outerCounter)
    
	self:SetVar("counter", counter)
	self:SetVar("outerCounter", outerCounter)
end

function onArrivedAtDesiredWaypoint(self, msg)	
    if msg.iPathIndex == 1 then        
        self:PlayFXEffect{ name  = "busDust", effectType = "create"} -- effectID = 642, 
        --print('play effect busDust')
    end
end