--------------------------------------------------------------

-- L_BOOTYDIG_CLIENT.lua

-- Client side Booty Dig Script
-- Communicates with the server side Booty Script
-- created dcross ... 6/3/11
-- fixed by mbermann ... 6/6/11
--------------------------------------------------------------

local PropertyMissionID = 1881  -- The mission given by Swabby Bilgewater in the AG picnic area
local BootyFlag = 1110          -- This flag is set in L_BOOTYDIG_SERVER when a player has succesfully dug once on a property

function onStartup(self)
	self:SetVisible{visible = false}
	checkState(self)
end

-- Looping check to see if the player is on property. 
-- After 10 tries, script gives up and object stays hidden until its duration expires and its deleted
function checkState(self)
	local propertyOwner = self:GetNetworkVar("PropertyOwnerID")

	if not propertyOwner then
		GAMEOBJ:GetTimer():AddTimerWithCancel(0.25, "check", self)
		return
	end
	
	-- This is a property, Start the digging sequence and notify the server script
	if propertyOwner ~= 0 then
		local player = self:GetParentObj().objIDParent
		if not player:Exists() then
			return 
		end
		local playerID = player:GetID()
		local missionstate = player:GetMissionState{missionID = PropertyMissionID}.missionState

		-- mission won't complete on players own property
		if playerID ~= propertyOwner then
			-- check to make sure the player hasn't already completed the mission
			if  missionstate == 2 or missionstate == 10 then
			    -- check to make sure the player hasn't dug up on this property before. NOTE: Server script sets the flag
			    if not player:GetFlag{iFlagID = BootyFlag}.bFlag then 
				    local anim_time = self:GetAnimationTime{ animationID = "shoveldigup" }.time
				    self:SetVisible{visible = true, fadeTime = 0.1}
				    self:PlayAnimation{ animationID = "shoveldigup", bPlayImmediate = true } 
				    GAMEOBJ:GetTimer():AddTimerWithCancel( anim_time, "ChestDugUp", self )
				    self:FireEventServerSide{ args = 'ChestReady' }
				end
			end
		end		
	end
end

function onTimerDone(self, msg)      
	if msg.name == "ChestDugUp" then
		-- Start chest opening animations
		local anim_time = self:GetAnimationTime{  animationID = "open" }.time
        self:PlayAnimation{animationID = "open", bPlayImmediate = true}
        GAMEOBJ:GetTimer():AddTimerWithCancel( anim_time , "ChestOpened", self )
    elseif msg.name == "ChestOpened" then
        -- Start chest death animations and notify server
		local anim_time = self:GetAnimationTime{  animationID = "death" }.time
		self:PlayAnimation{animationID = "death", bPlayImmediate = true}
		self:FireEventServerSide{ args = 'ChestOpened' }
		GAMEOBJ:GetTimer():AddTimerWithCancel( anim_time , "ChestDead", self )
	elseif msg.name == "ChestDead" then
		-- Notify server that it can now kill the chest
		self:FireEventServerSide{ args = 'ChestDead' }
    elseif msg.name == "check" then
        local count = self:GetVar("count") or 0
        
        -- polling count
        if count < 10 then
			count = count + 1
		
			self:SetVar("count", count)
			checkState(self)
		end
    end
end




