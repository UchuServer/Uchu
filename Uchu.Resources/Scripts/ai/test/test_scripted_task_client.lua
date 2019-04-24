--When a player clicks on me, send an update mission task message to the mission system
--This is attached to object 6944
--This triggers mission 446, the "eaten by sharks" mission

function onGetOverridePickType(self, msg)
    msg.ePickType = 14
	return msg
end

