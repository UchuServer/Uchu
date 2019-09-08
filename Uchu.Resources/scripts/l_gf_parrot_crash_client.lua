--------------------------------------------------------------
-- Script on the Parrots Crash in Gnarled Forest
-- keeps the player from getting to the treasure 
-- 
-- stolen from the spider cave entrance script in AG
-- updated Brandi... 1/28/10
-- updated Brandi... 2/26/10 added check for ninja cowl
-- updated abeechler ... 8/19/11 - removed Bob pop-up box
--------------------------------------------------------------

require('o_mis')

function onStartup(self,msg)

	self:SetVar("playerIn",false)

end

function onCollisionPhantom(self,msg)

	local player = GAMEOBJ:GetObjectByID(GAMEOBJ:GetLocalCharID())

	if (player:GetID() ~= msg.objectID:GetID()) then
		return
	end

	if (self:GetVar("playerIn") == false) then

		local item = player:GetEquippedItemInfo{ slot = "hair" }.lotID 

		--check to see if the player is wearing either the white or black ninja cowl, if they are, they can get through
		if item ~= 2641 and item ~= 2642 and item ~= 1889 then
		
		    self:SetVar("playerIn",true)

            --Notify parrots to play alarm animation
            local Parrots = self:GetObjectsInGroup{ group = "Parrots"}.objects

            for i = 1, table.maxn (Parrots) do
                Parrots[i]:PlayAnimation{animationID = "alarm"}
            end

            knockback(self)

        else
            -- Tell the server to cast our slow skill
            self:FireEventServerSide{senderID = player, args = "Slow"}   
        end

	end

end

function onOffCollisionPhantom(self,msg)

	local player = GAMEOBJ:GetObjectByID(GAMEOBJ:GetLocalCharID())
    local item = player:GetEquippedItemInfo{ slot = "hair" }.lotID 	
    --local playerID = msg.objectID:GetID()

	if (player:GetID() ~= msg.objectID:GetID()) then
		return
    else
        -- Removing item requirement in case you remove your cowl while in the volume
        --if item == 2641 or item == 2642 then
            -- Tell the server to cast our unslow skill
            self:FireEventServerSide{senderID = player, args = "Unslow"}  
        --end
    end

	self:SetVar("playerIn",false)
	GAMEOBJ:GetTimer():CancelTimer( "knockagain",self )

end

function knockback(self)

	if self:GetVar("playerIn") == false then return end

	GAMEOBJ:GetTimer():AddTimerWithCancel( 0.6, "playerEntered",self )

	local AnimObj = self:GetObjectsInGroup{ group = 'ParrotCrash', ignoreSpawners = true }.objects[1]

	AnimObj:StopFXEffect{name = "parrotwall"}
	AnimObj:PlayFXEffect{name = "parrotwall", effectID = 967, effectType = "pushy"}
	AnimObj:PlayAnimation{ animationID = 'parrot', bPlayImmediate = true }

end

function onTimerDone(self,msg)

	local player = GAMEOBJ:GetObjectByID(GAMEOBJ:GetLocalCharID())

	if (msg.name == "playerEntered") then

		if player then 

            local dir = self:GetObjectDirectionVectors().forward

			dir.y = 20
			dir.x = 5--dir.x * 50
			dir.z = -60--dir.z * 50		 
			player:PlayFXEffect{name = "birdcavepushback", effectID = 1537, effectType = "create"}--effectID = 1378, effectType = "push-back"}
			player:PlayAnimation{ animationID = "knockback-recovery" }
			player:Knockback { vector = dir }

			GAMEOBJ:GetTimer():AddTimerWithCancel( 0.5, "knockagain",self )

		end

	end

	if (msg.name == "knockagain") then

		knockback(self)

	end

end
