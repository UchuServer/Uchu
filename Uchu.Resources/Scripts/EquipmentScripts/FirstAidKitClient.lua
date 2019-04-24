-----------------------------------------------
-- Server Side script for the Rank 1 Daredevil First Aid Kit 
-- Let's the server script know when the object was used
-----------------------------------------------

--[[
function onClientUse(self, msg)

    -- senderID should be the player
    self:FireEventServerSide{ senderID = msg.user}
    
end
--]]
--------------------------------------------------------------
-- override pick type to be interactive
--------------------------------------------------------------
function onGetOverridePickType(self, msg)

	msg.ePickType = 14	--interactive type
	return msg

end

function onCheckUseRequirements(self, msg)
    msg.bCanUse = false
    if msg.objIDUser:GetHealth{}.health < msg.objIDUser:GetMaxHealth{}.health then
        msg.bCanUse = true
    end
    return msg
end