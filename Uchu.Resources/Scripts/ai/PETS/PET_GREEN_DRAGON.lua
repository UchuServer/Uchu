--------------------------------------------------------------
-- Script on the FV Pet Green Dragons
-- sets the pets as untamable, and allows them to be tamed after spraying them with the watergun
-- 
-- created by Michael Edwards
-- updated Brandi... 2/4/10
-- Updated Medwards  3/3/10
-- Updated Medwards  8/25/10
-----------------------------------------------------------
 
function onStartup(self,msg)
    if self:IsPetWild().bIsPetWild == true then
		MakeUntameable(self)
    else 
         -- change faction
        self:SetFaction{faction = 99}
        -- clear threat list
        self:ClearThreatList{}
    end
end

-- When the player squirts the skunk with the water gun
function onSkillEventFired( self, msg )
    if msg.wsHandle == "waterspray" then
        -- Check if the pet has been tamed and, if so, don't do this
        if self:IsPetWild().bIsPetWild == false then
           return
        end
        -- Check if the skunk has been sprayed by seeing if it's on a non pet faction
        if not self:BelongsToFaction{factionID = 99}.bIsInFaction then
            -- change faction
            self:SetFaction{faction = 99}
            -- clear threat list
            self:ClearThreatList{}
            -- start a timer that will turn the skunk untamable and aggro  
            GAMEOBJ:GetTimer():AddTimerWithCancel( 30, "GoEvil", self )
            -- turn off stink fx
            self:StopFXEffect{name = "burning1"}
            self:StopFXEffect{name = "burning2"}
            -- Send a network valriable to the client script to change picktype.
            self:SetNetworkVar("bIAmTamable", true)
         end
    end
end

function onTimerDone(self, msg)
    --Did the timer to see if the skunk needs to go aggro again fire?
    if msg.name == "GoEvil" then
		MakeUntameable(self)
    end
end

--Checking the state of the pet taming minigame. If start, cancel timers. If quit, start short "go evil" timer
function onNotifyPetTamingMinigame(self, msg)  
     if msg.notifyType == "BEGIN" then
        GAMEOBJ:GetTimer():CancelAllTimers(self) 
     elseif msg.notifyType == "FAILED" or msg.notifyType == "QUIT" then
        GAMEOBJ:GetTimer():AddTimerWithCancel( 5, "GoEvil", self )
     end
end

-- used to remove taming faction and send a variable to the client script to make unpickable
function MakeUntameable(self)
	-- check the skunks taming state. 5 means the pet is currently being tamed. This checks if that is false
	if self:GetPetHasState{iStateType = 5}.bHasState == false then
		--make the pet non tamable
		-- change faction
		self:SetFaction{faction = 114}
		-- refill health
		self:SetHealth{health = 5}
		-- play the Burn effect
		self:PlayFXEffect{name = "burning1", effectID = 4059, effectType = "create"}
		self:PlayFXEffect{name = "burning2", effectID = 4060, effectType = "create"}
		self:SetNetworkVar("bIAmTamable", false)
		return
	end
end