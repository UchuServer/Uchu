require('State')
require('o_StateCreate')
require('o_mis')
require('o_Main')


function onStartup(self)
	 self:FollowWaypoints()
     
     -- make this a constant somewhere
     self:SetProximityRadius { radius = 15 }
     
     self:SetVar("IsFollowing", 0)
     self:SetVar("LinePosit", 0)  
	 self:SetVar("ReturningHome", 0)
	 
	 local pos = getHomePoint(self)
	 self:SetTetherPoint { tetherPt = self:GetPosition().pos, radius = 100 }
	
end



 --###########################################################
 --########## PROXIMITY 
 --###########################################################

function onProximityUpdate(self, msg)

	if(self:GetVar("IsFollowing") == 1) then
		return
	end
	
	if(self:GetVar("ReturningHome") == 1) then
		return
	end
	
	-- Stop evading on leave
	if msg.status == "ENTER" then
		
		-- get the local character
        local player = msg.objId
        
        -- only follow players
        if(player:IsCharacter().isChar == true) then
			local MissionState = player:GetMissionState{missionID = 133}.missionState
			if(MissionState == 2) then 
			
				-- REQUEST FOLLOW, GET BACK POSITION, AND YES/NO
				local FollowMsg = GAMEOBJ:GetZoneControlID():RequestFollow{ targetID = msg.objId, requestorID = self }
				
				-- can we follow?
				if(FollowMsg.bCanFollow == true) then
					GAMEOBJ:GetTimer():CancelAllTimers( self )
					self:SetVar("LinePosit", FollowMsg.iPosit)
									
					-- follow the player
					local mySpeed = self:GetSpeed().speed
					self:FollowTarget
					{ 
						targetID = player, 
						radius = FollowMsg.iPosit * 5,
						speed = mySpeed,	-- speed = math.random(2, 5),
						keepFollowing = true 
					}
			
					self:SetVar("IsFollowing", 1)
			
					 self:SetTetherPoint { tetherPt = self:GetPosition().pos, radius = 0 }
					-- Start despawn timer 
					GAMEOBJ:GetTimer():AddTimerWithCancel(140, "despawnTimer", self) -- dont hardcode despawn time
				
				else
					print("CAN'T FOLLOW")
				end
			
			-- Evade them. It is a character without the mission.
			else
			
				GAMEOBJ:GetTimer():CancelAllTimers( self )
				
				-- Should we call this multiple times?
				-- Prolly no need to call this until we get a stopped evading message or tether return
				local mySpeed = self:GetSpeed().speed
				self:EvadeTarget
				{ 
					targetID = player, 
					radius = 20,
					speed = 7,	
					keepEvading = true 
				}
			
			end
			
        end
        -- store the player in felix object to face on arrival
		--  storeObjectByName(self, "playerTarget", player)
	end     
end





 --###########################################################
 --########## ON TIMER DONE
 --###########################################################
 
onTimerDone = function(self, msg)

	if msg.name == "despawnTimer" then
		-- Despawn self
		self:Die{killType = "SILENT"}
	end
	
	if msg.name == "ReturnHome" then
		self:SetVar("ReturningHome", 1)
		self:FollowWaypoints()
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
  self:FollowWaypoints()
  self:SetPathingSpeed{speed = 10}

end