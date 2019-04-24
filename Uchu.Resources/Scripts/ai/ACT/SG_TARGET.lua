-- ================================================
-- SG Target Script
-- Server Side
-- updated 9/30/10 mrb... changed mover despawn time
-- ================================================

--------------------------------------------------------------
-- Includes
--------------------------------------------------------------
--require('o_mis')
local CONSTANTS = {}
CONSTANTS["STREAK_MOD"] = 5
CONSTANTS["STREAK_BONUS"] = {1,5,10}

--------------------------------------------------------------
-- Startup of the object
--------------------------------------------------------------
function onStartup(self) 
	-- pick a random explode factor
	math.randomseed( os.time() )
	
	-- set explode factor
	self:SetSmashableParams{fExplodeFactor = 2.5}
end

--------------------------------------------------------------
-- Get the score for the target
--------------------------------------------------------------
function onGetActivityPoints(self, msg)
	local SpawnData = self:GetVar("SpawnData")
	
	if (SpawnData) then
		if (SpawnData.sdScore ) then
			msg.points = (SpawnData.sdScore )
		end
		
		msg.addTime = SpawnData.sdTimeScore		
		self:SetActivityUserData{userID = GAMEOBJ:GetZoneControlID():GetActivityUser{}.userID, typeIndex =5 , value = msg.points } 
	end
	
	return msg
end


function NotifyParentToRespawn(self)
    local parentID = GAMEOBJ:GetObjectByID(self:GetVar("My_Parent_ID")) 
    
    if parentID:Exists() then
        parentID:NotifyObject{ObjIDSender = self, name="FinishedPath"}	
    end
    
    Despawn(self)
end

--------------------------------------------------------------
-- continue doign waypoints
-- @TODO: modify speed/path/etc
--------------------------------------------------------------
function onArrived(self, msg)
	if (msg.isLastPoint == true) then
        NotifyParentToRespawn(self)
		
		return
	end
	
	-- do speed change
	ChangeSpeed(self)
	
	if (msg.actions) then 
		if msg.actions[1].name == "PlayAim" then
			local aimName =  msg.actions[1].value 
            local anim_time = self:GetAnimationTime{  animationID = aimName }.time
            
            if (tonumber(anim_time) > 0) then
                self:PlayAnimation{animationID = aimName}
            end
		end
	end
	                        
    self:ContinueWaypoints()    
end

--------------------------------------------------------------
-- Timers
--------------------------------------------------------------
function onTimerDone(self, msg)    
    -- Change the speed every 5 seconds
    if msg.name == "ChangeSpeed" then 
		-- pick a random speed
		local ran = math.random(1,4)
		
		self:SetPathingSpeed{ speed = ran }
		GAMEOBJ:GetTimer():AddTimerWithCancel( 5.0, "ChangeSpeed",self )
    elseif msg.name == "Despawn" then 
		GAMEOBJ:GetTimer():CancelAllTimers( self )
		self:Die{ killerID = self, killType = "SILENT" }
    end          
end

--------------------------------------------------------------
-- Try to change the targets speed
--------------------------------------------------------------
function ChangeSpeed(self)	
	-- get spawn data
	local SpawnData = self:GetVar("SpawnData")
	
	if (SpawnData) then		
		-- should we try to change speed?
	    if ((SpawnData.sdChangeSpeed == true) and 
	        (math.random() <= SpawnData.sdSpeedChance)) then
		
			-- get a speed
			local newSpeed = math.random(SpawnData.sdMinSpeed, SpawnData.sdMaxSpeed) 
			
			-- set the new speed
			self:SetPathingSpeed{ speed = newSpeed }	    
	    end	    
	end	
end

--------------------------------------------------------------
-- Try to despawn the target
--------------------------------------------------------------
function Despawn(self)
	-- get spawn data
	local SpawnData = self:GetVar("SpawnData")
	
	if (SpawnData) then		
		-- should we try to despawn?
	    if (SpawnData.sdDespawnTime == true) then
			local timeout = 1
			
			-- use 10 second timeout so platforms aren't killed on the client before they finish their path
			if ( SpawnData.sdMovingPlat == true ) then
				timeout = 2
			end			
			
			GAMEOBJ:GetTimer():AddTimerWithCancel( timeout, "Despawn",self )
	    end	
	else    
		GAMEOBJ:GetTimer():CancelAllTimers( self )
		self:Die{ killerID = self, killType = "SILENT" }
    end
end 