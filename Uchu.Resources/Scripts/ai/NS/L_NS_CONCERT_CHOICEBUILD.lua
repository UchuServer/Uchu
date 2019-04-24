--------------------------------------------------------------
-- Server script for the new Concert choicebuild smashable crate,
-- makes the crates unsmashable when they first spawn in and spawn out.
--
-- updated mrb... 11/12/10 -- Moved most choicebuild logic to 
-- /02_server/Map/NS/CONCERT_CHOICEBUILD_MANAGER.lua, this script
-- only handles making the crate unsmashable during switching.
--------------------------------------------------------------
-- constants
--------------------------------------------------------------

local leadInUnsmashableTime = 0.2
local leadOutUnsmashableTime = 0.2

--------------------------------------------------------------

function onStartup(self)
	-- save out the current fractions on the object for later use
	self:SetVar("save_factions", self:GetFaction().factionList)
	
	-- set the faction to -1, so the player cant hit the crate
	self:SetFaction{faction = -1}
	
	-- start up the timer to make the object smashable again
	GAMEOBJ:GetTimer():AddTimerWithCancel( leadInUnsmashableTime , "Smash", self )
	
	-- get the crate time passed to us from the manager
	local timer = self:GetVar("crateTime")
	
	-- if we have a time then start the timer to become unsmahable again
	if timer then
		GAMEOBJ:GetTimer():AddTimerWithCancel( timer - leadOutUnsmashableTime , "NoSmash", self )
	end
end

function onDie(self, msg)
    -- clear the timers
    GAMEOBJ:GetTimer():CancelAllTimers( self )
end

--------------------------------------------------------------
-- timers
--------------------------------------------------------------
function onTimerDone(self, msg)
    if msg.name == "NoSmash" then    
        -- dont allow the object to be smashed right before the switch
        self:SetFaction{faction = -1}
	elseif msg.name == "Smash" then    
        -- allow the object to be smashed again
        local savedFactions = self:GetVar("save_factions")
        
        -- set all the saved factions again
        for k,faction in ipairs(savedFactions) do      
			if k == 1 then
				self:SetFaction{faction = faction}
			else
				self:ModifyFaction{factionID = savedFaction, bAddFaction = true}
			end
        end
	end
end
