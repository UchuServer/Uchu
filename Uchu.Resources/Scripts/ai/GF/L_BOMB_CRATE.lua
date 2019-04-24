------------------------------------------------
-- L_BOMB_CRATE.lua
-- Used to handle the maelstrom chests in GF when they are hit by a player so they explode and give mission credit
-- Last Edited: 8-4-10
-- Edited by: MEdwards
------------------------------------------------
function onStartup(self) 
    self:SetProximityRadius { radius = 20 }
    self:SetVar("playersNearChest", 0)
    self:SetProximityRadius { radius = 10 ,name = "crateHitters" }
end

function onOnHit(self, msg)
    local player = msg.attacker
    if not self:GetVar("bIsHit") then
    ------------------------------
    -- Used to make the smasher of the crate be killed if they are too close
        local foundObj = self:GetProximityObjects{ name = "crateHitters" }.objects
        for i = 1, table.maxn (foundObj) do  
                if foundObj[i]:GetID() == player:GetID() then
                    player:RequestDie()
                   --break 
                end
        end
    --------------------------------
        self:SetVar("bIsHit" , true)
        self:CastSkill{skillID = 147, optionalOriginatorID =  player} --self:GetSkills().skills[1] }  -- has skill 147 (aoe that deals 2 damage)
        self:PlayEmbeddedEffectOnAllClientsNearObject{ radius = 16.0, fromObjectID = self, effectName = "camshake" }
        self:Die()
  
        --update the mission related to the crates
        player:UpdateMissionTask {taskType = "complete", value = 333, value2 = 1, target = self} 

        --update the achievements related to the crates
        player:UpdateMissionTask{taskType = "complete", value = 430, value2 = 1, target = self}
        player:UpdateMissionTask{taskType = "complete", value = 431, value2 = 1, target = self}
        player:UpdateMissionTask{taskType = "complete", value = 432, value2 = 1, target = self}
        player:UpdateMissionTask{taskType = "complete", value = 454, value2 = 1, target = self}
        player:UpdateMissionTask{taskType = "complete", value = 455, value2 = 1, target = self}
        player:UpdateMissionTask{taskType = "complete", value = 456, value2 = 1, target = self}
        player:UpdateMissionTask{taskType = "complete", value = 457, value2 = 1, target = self}
        player:UpdateMissionTask{taskType = "complete", value = 458, value2 = 1, target = self}
    end
end

-- Plays a shake when a player is close
function onProximityUpdate(self, msg)
	if msg.objId:BelongsToFaction{factionID = 1}.bIsInFaction then
		if (msg.status == "ENTER") then
			self:PlayAnimation{ animationID = "bounce" }
			self:PlayFXEffect{ name = "bouncin", effectType = "anim" }
			self:SetVar("playersNearChest", (self:GetVar("playersNearChest") + 1 ))
		elseif (msg.status == "LEAVE") then
			self:SetVar("playersNearChest", (self:GetVar("playersNearChest") - 1 ))
			if self:GetVar("playersNearChest") < 1 then
				self:PlayAnimation{ animationID = "idle" }
				self:StopFXEffect{ name = "bouncin" }
				self:SetVar("playersNearChest", 0)
			end
		end
	end
end