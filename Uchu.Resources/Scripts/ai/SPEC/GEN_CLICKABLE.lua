
function onGetOverridePickType(self, msg) -- Get the Pick Type (cursor clicking options) for the script object, in preparation of changing it.

    msg.ePickType = 14 -- Set the Pick Type to 14
	return msg -- Send Pick Type 14 back to the script object
    
end

