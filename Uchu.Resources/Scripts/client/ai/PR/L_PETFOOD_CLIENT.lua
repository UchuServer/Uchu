--Client side script allows item to be clicked. Works with L_PET_PETFOOD.LUA

----------------------------------------------------------------------
-- Item can be clicked
----------------------------------------------------------------------
--[[
function onGetOverridePickType(self, msg) -- Get the Pick Type (cursor clicking options) for the script object, in preparation of changing it.
    --print ("inside onGetOverridePickType function (client)")
    msg.ePickType = 14 -- Set the Pick Type to 14
	return msg -- Send Pick Type 14 back to the script object
    
end
]]--
