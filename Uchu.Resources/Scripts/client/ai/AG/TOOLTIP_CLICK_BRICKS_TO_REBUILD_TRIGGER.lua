require('o_mis')

local ROCKETGROUP="RocketBuild"
local ProxRadius = 20
local ToolDisplayTime = 4

function onStartup(self)
    -- set the proximity radius for the qb
    self:SetProximityRadius{radius = ProxRadius, name = "qbBouncer"} 
end

function onProximityUpdate(self, msg)
    -- If this isn't the proximity radius for the bouncer, then we're done here
    if (msg.name ~= "qbBouncer") or self:GetVar('bIsDisplayed') then return end
        
    local playerID = GAMEOBJ:GetLocalCharID()
        
    if (msg.objId:GetID() == playerID) and msg.status == "ENTER" then        
		local targetID = msg.objId	        
		local BobMissionStatus = targetID:GetMissionState{missionID = 173}.missionState  --Check the Bob mission to see the status
		local QBGroup = self:GetObjectsInGroup{ group = ROCKETGROUP, ignoreSpawners=true }.objects
		local bIsInActivity = false;
		
		-- checks to see if the player is in an activity
		for i = 1, table.maxn (QBGroup) do
			 --print("found group with i: "..i.."with id: "..QBGroup[i]:GetLOT().objtemplate )
			if( QBGroup[i]:ActivityUserExists{userID=GAMEOBJ:GetObjectByID(playerID)}.bExists ) then
				bIsInActivity=true;
			end
		end  
		
		--If the player is not doing the Rocket Build and they have completed the mission, print the hint text.
		if (not bIsInActivity and (BobMissionStatus == 8)) then 
		    local isClickOnceType = self:GetVar("isClickOnceType")
		    
		    -- decides which tool tip to use, it should always be else but you never know...
			if (isClickOnceType == true) then
				targetID:DisplayTooltip {bShow=true, strText = Localize("QB_TOOLTIP_CLICK_BRICKS_TO_REBUILD"), iTime = -1 }
			else
				targetID:DisplayTooltip {bShow=true, strText = Localize("QB_TOOLTIP_CLICK_BRICKS_AND_HOLD_TO_REBUILD"), iTime = -1 }
			end
			
			self:SetVar('bIsDisplayed', true)
            GAMEOBJ:GetTimer():AddTimerWithCancel( ToolDisplayTime, "Tool_Tip_Time", self )
		end
	end 
end

function HideTooltip(self)
    if (self:GetVar('bIsDisplayed')) then
		self:SetVar('bIsDisplayed', false)
		local playerID = GAMEOBJ:GetObjectByID(GAMEOBJ:GetLocalCharID())
		playerID:DisplayTooltip {bShow=false}
	end
end

function onRebuildNotifyState(self, msg)
    --print('onRebuildNotifyState - prevState = ' .. msg.iPrevState .. ' curState = ' .. msg.iState)

    if ( msg.iState == 4 ) then -- qb was reset cancel all timers qb is respawned so the proximity will take care of the tool tip
		GAMEOBJ:GetTimer():CancelAllTimers(self)
		HideTooltip(self)
		
	elseif msg.iState == 5 then -- qb was started - if local player started it, cancel timers and start new timer to display the tool tip until its finished building
		self:ClearProximityRadius()
		HideTooltip(self)
		if (msg.player:GetID() == GAMEOBJ:GetLocalCharID()) then
			GAMEOBJ:GetTimer():CancelAllTimers(self) 
			GAMEOBJ:GetTimer():AddTimerWithCancel( 0.1, "Start_Tool_Tip", self )
		end

    elseif msg.iState == 3 then -- qb was completed clear the proximity and turn off the tool tip
        GAMEOBJ:GetTimer():CancelAllTimers(self)
        HideTooltip(self)
    end	
end

function onTimerDone(self, msg)
    if msg.name == "Tool_Tip_Time" then -- tool tip time is up reset it
        local playerID = GAMEOBJ:GetObjectByID(GAMEOBJ:GetLocalCharID())
        
        self:SetVar('bIsDisplayed', false)
        playerID:DisplayTooltip {bShow=false}
    elseif msg.name == "Start_Tool_Tip" then  -- delay the display tool tip on reset, cinematic was canceling it out
        local playerID = GAMEOBJ:GetObjectByID(GAMEOBJ:GetLocalCharID())
        
        self:SetVar('bIsDisplayed', true)
        playerID:DisplayTooltip {bShow=true, strText = Localize("QB_TOOLTIP_CLICK_BRICKS_AND_HOLD_TO_REBUILD"), iTime = -1 }    
    end
end 