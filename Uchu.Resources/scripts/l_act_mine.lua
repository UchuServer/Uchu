------------------------------------------------------
--Script for the mines V.2
------------------------------------------------------
require('o_mis')
local interactRadius = 10
local maxWarnings = 3

function onStartup(self)
    --print("mine starting up")
    self:SetVar("RebuildComplete", false)
    self:SetProximityRadius { radius = interactRadius }
end

function onRebuildNotifyState(self, msg)
    if (msg.iState == 2) then
        --print("Rebuild complete")
        storeObjectByName(self, "Builder", msg.player)
        self:SetVar("RebuildComplete", true)
        self:SetVar("NumWarnings", 0 )
        self:AddObjectToGroup{group = "reset"}
    end
end

function onProximityUpdate(self, msg)
   
   if (msg.status == "ENTER") and (self:GetVar("RebuildComplete") == true) and (getObjectByName(self, "Builder"):IsEnemy{targetID = msg.objId}.enemy) then
        --print("Enemy detected")
        self:PlayFXEffect{name = "sirenlight_B", effectID = 242, effectType = "orange"}
        GAMEOBJ:GetTimer():AddTimerWithCancel( 0.25 , "Tick", self )
     
   end
end

onTimerDone = function(self, msg)
	if msg.name == "Tick" then
        if (self:GetVar("NumWarnings") >= maxWarnings) then
            --print("Mine exploded!")
	        self:CastSkill{skillID = 317 , optionalOriginatorID = getObjectByName(self, "Builder")  }
            GAMEOBJ:GetTimer():AddTimerWithCancel( 0.1 , "BlowedUp", self )
            --self:PlayFXEffect{name = "cannonbig", effectID = 71, effectType = "onfire_large"}
        	--self:Die{ killerID = self }
		else
			--print("Beep!")
            self:PlayFXEffect{name = "sirenlight_B", effectID = 242, effectType = "orange"}
            GAMEOBJ:GetTimer():AddTimerWithCancel( 0.25 , "Tick", self )
            self:SetVar("NumWarnings", (self:GetVar("NumWarnings") + 1))
		end
	end
    
    if msg.name == "BlowedUp" then
        self:Die{ killerID = self }
    end
end
