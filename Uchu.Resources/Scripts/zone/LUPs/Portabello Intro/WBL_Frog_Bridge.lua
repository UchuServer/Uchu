local bridgePieces = {} -- There is only one instance of this script per map.  Therefore a global is ok.

function onStartup(self, msg)
	self:SetProximityRadius{radius = 25}
	self:SetVar("TongueOut", false)
end

-- in the trigger, you define "Target".  Target  is "self" in this function.

function onProximityUpdate ( self, msg ) 
    if(#bridgePieces == 0) then
        bridgePieces[1] = self:GetObjectsInGroup{ group = "FrogBridge01", ignoreSelf = true, ignoreSpawners = true }.objects[1]
	    bridgePieces[2] = self:GetObjectsInGroup{ group = "FrogBridge02", ignoreSelf = true, ignoreSpawners = true }.objects[1]
	    bridgePieces[3] = self:GetObjectsInGroup{ group = "FrogBridge03", ignoreSelf = true, ignoreSpawners = true }.objects[1]
	    bridgePieces[4] = self:GetObjectsInGroup{ group = "FrogBridge04", ignoreSelf = true, ignoreSpawners = true }.objects[1]
    end

	if(msg.status == "ENTER") then
		StickOutTongue(self)
	end -- if Start Frog Bridge

end -- onProximityUpdate

function StickOutTongue(self)

	if(self:GetVar("TongueOut") == true) then
        return
	end
	
	local objs = self:GetProximityObjects{}.objects
	local numObjs = #objs
	for i = 1, numObjs do
			local player = objs[i]
			
			if( player:GetMissionState{missionID = 946}.missionState >= 8 ) then
                -- Get all the tongue bridges and tell them to start pathing
                local numBridgePieces = #bridgePieces
                for i = 1, numBridgePieces do
                    bridgePieces[i]:GoToWaypoint{ iPathIndex = 1; bStopAtWaypoint = true }
                end -- for loop 
                self:SetVar("TongueOut", true)
                GAMEOBJ:GetTimer():AddTimerWithCancel(10, "tongueWait", self)
                return
	        end -- if you have the mission    
	end
end

function onTimerDone(self, msg)   
-- we can't control the order in which HF gives us the pieces.  However, it always
-- gives them in the same order.  This happens to be the correct order.
-- Wait a second between each piece of the tongue so that lag will not make it break apart  
    if(msg.name == "tongueWait") then
        GAMEOBJ:GetTimer():AddTimerWithCancel(1, "back4", self)
        bridgePieces[4]:GoToWaypoint{ iPathIndex = 0; bStopAtWaypoint = true }
    elseif(msg.name == "back4") then
        GAMEOBJ:GetTimer():AddTimerWithCancel(1, "back3", self)
        bridgePieces[3]:GoToWaypoint{ iPathIndex = 0; bStopAtWaypoint = true }
    elseif(msg.name == "back3") then
        GAMEOBJ:GetTimer():AddTimerWithCancel(1, "back2", self)
        bridgePieces[2]:GoToWaypoint{ iPathIndex = 0; bStopAtWaypoint = true }
    elseif(msg.name == "back2") then
        GAMEOBJ:GetTimer():AddTimerWithCancel(7, "tongueIn", self)
        bridgePieces[1]:GoToWaypoint{ iPathIndex = 0; bStopAtWaypoint = true }
    elseif(msg.name == "tongueIn") then
        self:SetVar("TongueOut", false)
	    StickOutTongue(self)
    end
    
end
