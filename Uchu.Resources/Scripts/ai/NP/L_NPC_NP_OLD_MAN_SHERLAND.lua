--------------------------------------------------------------
-- (SERVER SIDE) Script for old man sherland
-- scene 3
--
-- Interacts with players in proximity and animates.
-- [proxDistance] : set in config data for proximity distance
--------------------------------------------------------------


--------------------------------------------------------------
-- Object specific constants
--------------------------------------------------------------
local defaultProx = 10.0


--------------------------------------------------------------
-- handle proximity updates
--------------------------------------------------------------
function onProximityUpdate(self, msg)

	if msg.status == "ENTER" and msg.objId:GetFaction().faction == 1 then 

		self:StopPathing()
		GAMEOBJ:GetTimer():CancelAllTimers( self )
		self:FaceTarget{target = msg.objId, degreesOff = 25, keepFacingTarget = true, bInstant = true}
		self:PlayAnimation{animationID = "missionState1"}

	elseif msg.status == "LEAVE" and msg.objId:GetFaction().faction == 1 then
	
		GAMEOBJ:GetTimer():CancelAllTimers( self )
		GAMEOBJ:GetTimer():AddTimerWithCancel( 3, "ProxCheck",self )
		
	end

end


--------------------------------------------------------------
-- Called when object is loaded into the level
--------------------------------------------------------------
function onStartup(self)

	local myProx = self:GetVar("proxDistance")
	if not (myProx) then
		myProx = defaultProx
	end
    self:SetProximityRadius{radius = myProx}	
    
	self:FollowWaypoints()
	
end


--------------------------------------------------------------
-- Determines if any players are currently in proximity
--------------------------------------------------------------
function ArePlayersInProximity(self)

	local objs = self:GetProximityObjects().objects
	local index = 1

	while index <= table.getn(objs)  do

		local target = objs[index]
		local faction = target:GetFaction()
		--verify that we are only bouncing players
		if faction and faction.faction == 1 then
			return true;
		end
		index = index + 1

	end
	return false;

end


--------------------------------------------------------------
-- Timers
--------------------------------------------------------------
onTimerDone = function(self, msg)
	
	-- check for players in proximity
    if (msg.name == "ProxCheck") then
	    
        if (ArePlayersInProximity(self) == false) then
            self:FollowWaypoints()
		end

    end
	
end