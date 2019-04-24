--------------------------------------------------------------
-- Server script for the new Concert smash choicebuild QB's
-- that are spawned in when the box is smashed.
--
-- updated mrb... 11/13/10 -- uses manager object for choicebuild control
--------------------------------------------------------------
-- constants for the concert prop choicebuilds
--------------------------------------------------------------

local resetActivatorTime = 15.0	-- how long before the activator will reset if the player doesn't use it
local resetTime = 20.0			-- how long before the completed QB resets
local resetStageTime = 66.5		-- how long to wait if all 4 of the same QB's are built before all QB's reset
local blinkResetTime = 6		-- how long before the the QB resets to start blinking
local gVars = 					-- fx and object tables for when all 4 of the same QB's are built
{
	tFX = 	{ 
				{ name = 'discoball', group = 'effectsDiscoball' },
				{ name = 'speaker', group = 'effectsShell' },
				{ name = 'speakerHill', group = 'effectsHill' },
				{ name = 'spotlight', group = 'effectsHill' },
				{ name = 'discofloor', group = 'effectsShell' },
				{ name = 'flamethrower', group = 'effectsShell' },
				{ name = 'stagelights', group = 'effectsShell' },
			},
		
	tSets = {
				{ name = 'laser',  fx = {'discoball', 'discofloor', 'stagelights', 'spotlight',} },
				{ name = 'spotlight', fx = {'spotlight', 'stagelights',} },
				{ name = 'rocket', fx = {'flamethrower',} },
				{ name = 'speaker', fx = {'speaker', 'speakerHill', 'stagelights', 'spotlight',} },
			},     
}

--------------------------------------------------------------

function onStartup(self)
	-- add this object to the correct group
	self:AddObjectToGroup{ group = "QB_" .. self:GetLOT().objtemplate }
end

-- happens when the crate tells the QB activator to become visible
function onDisplayRebuildActivator(self, msg)
	-- get the group name and number
    local groupName = self:GetVar("groupID") or ""
    local myGroup = string.gsub(groupName,"%;","")
	local myGroupNum = tonumber(string.sub(myGroup, -1))
	
	-- get the choicbuild manager based on this objects group number
    local cbMgr = self:GetObjectsInGroup{group = "CB_" .. myGroupNum, ingnorSpawners = true}.objects[1]
    
    -- cant find choicbuild manager, so something really bad happened
    if cbMgr then    
		if cbMgr:Exists() then
			-- notify then cb manager that this qb is active and set a notification for the manager when we die
			cbMgr:NotifyObject{ObjIDSender = self, name = "QB_Shown"}
			cbMgr:SendLuaNotificationRequest{requestTarget = self, messageName = "Die"}
		end
	end
	
	-- clear timers and set the reset activator time and the blink time
	GAMEOBJ:GetTimer():CancelAllTimers( self )
	GAMEOBJ:GetTimer():AddTimerWithCancel( resetActivatorTime , "qbResetTimeActivator", self )
	GAMEOBJ:GetTimer():AddTimerWithCancel( getResetBlinkTime(self, resetActivatorTime) , "qbResetTimeBlink", self )
end

-- find out the index based on this objects lot
function getSet(self)
    local iLOT = self:GetLOT().objtemplate
    local index = -1
    
    if iLOT == 5846 then
        index = 1
    elseif iLOT == 5847 then
        index = 2
    elseif iLOT == 5848 then
        index = 3
    elseif iLOT == 5845 then
        index = 4
    end
    
    return index
end

function onRebuildComplete(self, msg)      
	-- get the group name of this lot type
    local myGroup = "QB_" .. self:GetLOT().objtemplate
    -- find the objects in this objects lot group
    local groupObjs = self:GetObjectsInGroup{ group = myGroup, ignoreSpawners = true }.objects	
	local indexCount = 0
    local playerNum = string.sub(string.gsub(self:GetVar("groupID"),"%;",""), -1)
    
    
    for k,v in ipairs(groupObjs) do
		if v:Exists() then
			-- find the number of the same built QBs
			if v:GetRebuildState().iState == 2 then
				indexCount = indexCount + 1
			end
			
			-- set the player who completed the QB on all the QB's in this group
			v:SetVar("Player_" .. playerNum, "|" .. msg.userID:GetID())
		end
    end
    
    -- update the mission for the player who completed this qb
    msg.userID:UpdateMissionTask{taskType = "complete", value = 283, value2 = 1, target = self}
	-- clear any blink effect that might be active
	self:SetNetworkVar("startEffect", -1)
	
	-- if we have all 4 qb's of the same lot build then we start up the fx event
    if indexCount >= 3 then
		-- get the platform objects 
        local pathObjs = self:GetObjectsInGroup{ group = 'ConcertPlatforms', ignoreSpawners = true }.objects
        
        -- start the pathing for each object in the platform group
        for k,v in ipairs(pathObjs) do
            v:StartPathing{}
        end
        
        -- for each QB cancel the timers and make them unsmashable
        for k,v in ipairs(groupObjs) do
			-- clear any blink fx
			v:SetNetworkVar("startEffect", -1)
			-- cancel all timers and set the stage reset and blink timers for each qb
            GAMEOBJ:GetTimer():CancelAllTimers( v )
			GAMEOBJ:GetTimer():AddTimerWithCancel( resetStageTime , "StageResetTime", v )
			GAMEOBJ:GetTimer():AddTimerWithCancel( getResetBlinkTime(self, resetStageTime) , "StageResetTimeBlink", v )
			
            v:SetFaction{faction = -1}
        end
        
        -- for each player who built one of the qb's complete the mission
        for i = 1, 4 do
            local player = GAMEOBJ:GetObjectByID(self:GetVar("Player_" .. i))
            
            if player:Exists() then
		        player:UpdateMissionTask{taskType = "complete", value = 598, value2 = 1, target = self}
		    end            
        end
        
        -- start up the fx for this event
		UpdateParticleEffects( self, getSet(self) )		
        
        return
    end
    
    -- cancel all timers and start up the reset and blink timers, since we didn't have all 4 of the same built
	GAMEOBJ:GetTimer():CancelAllTimers( self )
	GAMEOBJ:GetTimer():AddTimerWithCancel( resetTime , "qbResetTime", self )
	GAMEOBJ:GetTimer():AddTimerWithCancel( getResetBlinkTime(self, resetTime) , "qbResetTimeBlink", self )
end

function onRebuildNotifyState( self, msg)  
	if msg.iState == 4 or msg.iState == 5 then -- the choice build is smashed or resetQBs
		-- clear any blink effect and cancel all timers
		self:SetNetworkVar("startEffect", -1)
		GAMEOBJ:GetTimer():CancelAllTimers( self )
	end    
end

--------------------------------------------------------------
-- reset the choicebuilds
--------------------------------------------------------------
function resetQBs(self)	
    -- cancel any bonus particle effects received from having the choicebuilds match
    UpdateParticleEffects( self, -1 )
    
    local myGroup = "QB_" .. self:GetLOT().objtemplate
    local qbObjs = self:GetObjectsInGroup{ group = myGroup, ignoreSpawners = true }.objects	
	
	-- clear the blink effects and kill all QB's
    for k,v in ipairs(qbObjs) do
		self:SetNetworkVar("startEffect", -1)
        v:RequestDie()
    end
end

--------------------------------------------------------------
-- update the particle effects due to the current state of the 4 choicebuilds
-- if one of the choicebuild option indices is passed in, play the corresponding effect and cancel any existing ones
-- if -1 is passed in, then cancel any existing effects 
--------------------------------------------------------------
function UpdateParticleEffects( self, index )
	if ( index == -1 ) then
	    --print('CancelAllEffects')
		CancelAllEffects( self )
	else		
	    --print('UpdateEffects ' .. index)
		UpdateEffects(self, index)	
	end		
end

--------------------------------------------------------------
-- update the particle effects because all 4 props have the given LOT
--------------------------------------------------------------
function UpdateEffects( self, index )    
    CancelAllEffects( self )
    -- get the correct FX load out
    local tSet = gVars.tSets[index]
    
    -- play all the FX for this event
    for k,v in ipairs(tSet.fx) do
        local iFX = 1
        
        -- find the correct index for the fx
        for i = 1, #gVars.tFX do
            if gVars.tFX[i].name == v then
                iFX = i
                break
            end 
        end
        
        local desiredEffectName = gVars.tFX[iFX].name
        local desiredEffectSave = gVars.tFX[iFX].name .. 'Effect'
        local fxObj = self:GetObjectsInGroup{ group = gVars.tFX[iFX].group, ignoreSpawners = true }.objects[1]
                
        --print('play effect ' .. desiredEffectSave .. ' - ' .. desiredEffectName .. ' - ' .. gVars.tFX[iFX].group)
        fxObj:PlayFXEffect{ name = desiredEffectSave, effectType = desiredEffectName }
    end

end

--------------------------------------------------------------
-- cancel any effects on the concert hill
--------------------------------------------------------------
function CancelAllEffects( self )
    local tFX = gVars.tFX
    
    for k,v in ipairs(tFX) do        
        local cancelEffectName = v.name .. 'Effect'
        local fxObj = self:GetObjectsInGroup{ group = v.group, ignoreSpawners = true }.objects[1]
        
        --print('stop effect ' .. cancelEffectName .. ' - ' .. v.group)
        fxObj:StopFXEffect{ name = cancelEffectName }
    end	
end

-- adjust the time for the blink effect and return it
function getResetBlinkTime(self, time)
	local blinkTime = time - blinkResetTime
	
	if blinkTime < 1 then 
		blinkTime = 1
	end
	
	return blinkTime
end

--------------------------------------------------------------
-- timers
--------------------------------------------------------------
function onTimerDone(self, msg)
    if msg.name == "qbResetTimeActivator" then		-- clear the blink fx and kill this object silently	
		self:SetNetworkVar("startEffect", -1)
		self:RequestDie{killType = "SILENT"}
    elseif msg.name == "qbResetTime" then			-- clear the blink fx and kill this object violently
		self:SetNetworkVar("startEffect", -1)
		self:RequestDie()
    elseif msg.name == "StageResetTime" then		-- stage is done reset all the QB's
		resetQBs(self)
	elseif msg.name == "qbResetTimeBlink" then		-- start blinking the QB
		self:SetNetworkVar("startEffect", getResetBlinkTime(self, resetActivatorTime))
    elseif msg.name == "StageResetTimeBlink" then	-- start blinking the QB
		self:SetNetworkVar("startEffect", getResetBlinkTime(self, resetTime))
    end
end 
