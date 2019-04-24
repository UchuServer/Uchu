require('State')
require('o_StateCreate')
require('o_mis')
require('o_Main')

function onStartup(self)
     self:SetProximityRadius { radius = 5 }     
     self:SetVar("IsFollowing", 0)
     self:SetVar("LinePosit", 0)  
	 self:SetVar("ReturningHome", 0)
	 local pos = getHomePoint(self)
	 self:SetTetherPoint { tetherPt = self:GetPosition().pos, radius = 500 }
end


function onProximityUpdate(self, msg)

	if(self:GetVar("IsFollowing") == 1) then
		return
	end
	
	if(self:GetVar("ReturningHome") == 1) then
		return
	end
	
	-- Stop evading on leave
	if msg.status == "ENTER" then
    
        local player = msg.objId
        if(player:IsCharacter().isChar == true) then
        
            GAMEOBJ:GetTimer():CancelAllTimers( self )
            local mySpeed = self:GetSpeed().speed
            self:EvadeTarget
            { 
                targetID = player, 
                radius = 8,
                speed = 7,	
                keepEvading = true 
            }
			
        end

	end     
    
end

 
onTimerDone = function(self, msg)

	if msg.name == "despawnTimer" then
		-- Despawn self
		self:Die{killType = "SILENT"}
	end
	
	if msg.name == "ReturnHome" then
		self:SetVar("ReturningHome", 1)
		--self:FollowWaypoints()
		self:SetPathingSpeed{speed = 10}
    end

end


function onArrived(self, msg)
	
	-- Ick hack
	self:SetPathingSpeed{speed = 1}
	self:SetVar("ReturningHome", 0)
	
	self:ContinueWaypoints()

end


function onStoppedEvading(self, msg)

	GAMEOBJ:GetTimer():AddTimerWithCancel( 10, "ReturnHome", self )
    
end


onLeftTetherRadius = function(self, msg)

  if(self:GetVar("IsFollowing") == 1) then
		return
  end
  self:SetVar("ReturningHome", 1)

end